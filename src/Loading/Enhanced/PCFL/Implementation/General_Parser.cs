using System.Diagnostics;
using System.Runtime.InteropServices;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Implementation;

public static class GeneralFileParser
{
   public static List<IToken> ParseSomeFile(string content, ParsingContext context, PathObj po)
   {
      /*
       * how do we know what do to?
       * Determine if Trigger / Effect
       * if we are trigger => all sub elements are triggers
       * if we are effect we call us again bc we like recursion
       */

      return ParseElementsToTokens(EnhancedParser.LoadBaseOrder(content, po), context, po);
   }

   public static List<IToken> ParseElementsToTokens(IEnumerable<IEnhancedElement> elements, ParsingContext context, PathObj po)
   {
      var limitIfFlowControl = new IfFLowControl(ITrigger.Empty, []);

      List<IToken> program = [];

      foreach (var element in elements)
      {
         if (element.IsBlock)
         {
            var block = (EnhancedBlock)element;
            if (block.Name.Equals("limit"))
            {
               List<ITrigger> triggers = [];

               if (block.ParseTriggerBlock(context, po, triggers))
               {
                  if (limitIfFlowControl.Trigger == ITrigger.Empty)
                     limitIfFlowControl.Trigger = triggers[0];
                  else if (limitIfFlowControl.Trigger is BooleanOperation { Operation: Operation.AND } operation)
                     operation.Triggers.AddRange(triggers);
                  else
                     limitIfFlowControl.Trigger = new BooleanOperation(Operation.AND, [.. triggers, limitIfFlowControl.Trigger]);
               }
               else
                  _ = new ErrorObject(ErrorType.EmptyLimitBlock,
                                      $"Block '{block.Name}' has an empty 'limit' which can be removed as it is always true.",
                                      level: LogType.Information);
            }

            // 
            if (ParseToken(block, context, po, out var token))
               program.Add(token!);
            else
            {
               // Throw error
            }

         }
         else
         {
            foreach (var kvp in ((EnhancedContent)element).GetLineKvpEnumerator(po))
            {
               if (ParseToken(kvp, context, po, out var token))
                  program.Add(token!);
               // TODO : Parse Effects
            }
         }
      }
      if (limitIfFlowControl.Trigger != ITrigger.Empty) // a trigger in a scope is interpreted as a simple 'if'
      {
         limitIfFlowControl.SubTokens = program;
         return [limitIfFlowControl];
      }
      return program;
   }

   public static bool ParseToken(LineKvp<string, string> input, ParsingContext context, PathObj po, out IToken? token)
   {
      token = null!;
      if (!context.scope.IsValidEffect(input.Key, out var creator))
      {
         _ = new LoadingError(po, $"Invalid Token: {input.Key}", line: input.Line, type: ErrorType.PCFL_TokenValidationError);
         return false;
      }
      token = creator(null, input, context, po)!;
      return token is not null;
   }

   public static bool ParseToken(EnhancedBlock input, ParsingContext context, PathObj po, out IToken? token)
   {
      // Not finished needs the limit functionality and so on
      token = null!;
      if (!context.scope.IsValidEffect(input.Name, out var creator))
      {
         _ = new LoadingError(po, $"Invalid Token: {input.Name}", line: input.StartLine, type: ErrorType.PCFL_TokenValidationError); //TODO Error
         return false;
      }
      token = creator(input, null, context, po)!;
      return token is not null;
   }


   public static bool ParseTokenBlock(this EnhancedBlock block, ParsingContext context, PathObj po, List<IToken> token)
   {
      token.AddRange(ParseElementsToTokens(block.GetElements(), context, po));
      foreach (var triggerElement in block.GetElements())
      {
         if (triggerElement.IsBlock)
         {
            ParseToken((EnhancedBlock)triggerElement, context, po, out var newToken);
            token.Add(newToken);
         }
         else
         {
            foreach (var kvp in ((EnhancedContent)triggerElement).GetLineKvpEnumerator(po))
            {
               ParseToken(kvp, context, po, out var newToken);
               token.Add(newToken);
            }
         }
      }

      return token.Count > 0;
   }

   // used for limit, AND, NOT, OR, 
   public static bool ParseTriggerBlock(this EnhancedBlock block, ParsingContext context, PathObj po, List<ITrigger> trigger)
   {
      foreach (var triggerElement in block.GetElements())
      {
         if (triggerElement.IsBlock)
         {
            ParseTrigger((EnhancedBlock)triggerElement, context, po, out var newTrigger);
            trigger.Add(newTrigger);
         }
         else
         {
            foreach (var kvp in ((EnhancedContent)triggerElement).GetLineKvpEnumerator(po))
            {
               ParseTrigger(kvp, context, po, out var newTrigger);
               trigger.Add(newTrigger);
            }
         }
      }

      return trigger.Count > 0;
   }

   public static bool ParseTriggerBlockToAnd(this EnhancedBlock block, ParsingContext context, PathObj po, out ITrigger trigger)
   {
      List<ITrigger> triggers = [];
      if (!block.ParseTriggerBlock(context, po, triggers))
      {
         trigger = ITrigger.Empty;
         return false;
      }

      if (triggers.Count == 1)
      {
         trigger = triggers[0];
         return true;
      }

      trigger = new BooleanOperation(Operation.AND, triggers);
      return true;
   }


   // Ragequit -> Escalates errors
   public static bool ParseTrigger(LineKvp<string, string> input, ParsingContext context, PathObj po, out ITrigger trigger)
   {
      trigger = null!;
      if (!context.scope.IsValidTrigger(input.Key, out var creator))
      {
         _ = new LoadingError(po, $"Invalid Trigger: {input.Key}", line: input.Line, type: ErrorType.PCFL_TriggerValidationError);
         return false;
      }
      trigger = creator(null, input, context, po)!;
      return trigger is not null;
   }

   // Mod -> Recovers from errors
   public static bool ParseTrigger(EnhancedBlock block, ParsingContext context, PathObj po, out ITrigger? trigger)
   {
      trigger = null!;
      switch (block.Name.ToUpper())
      {
         case "NOT":
            trigger = BooleanOperation.Parse(block, context, po, Operation.NOT);
            return true;
         case "AND":
            trigger = BooleanOperation.Parse(block, context, po, Operation.AND);
            return true;
         case "OR":
            trigger = BooleanOperation.Parse(block, context, po, Operation.OR);
            return true;
      }

      // parse trigger scopeSwitches
      /*
       * Check if block.Name is trigger-ScopeSwitch
       * if so:
       *    parse as one
       * if not
       *    what is it doing here
      */

      if (!context.scope.IsValidTrigger(block.Name, out var creator))
      {
         _ = new LoadingError(po, $"Invalid Trigger: {block.Name}", line: block.StartLine, type: ErrorType.PCFL_TriggerValidationError);
         return false;
      }
      trigger = creator(block, null, context, po)!;
      return trigger is not null;
   }

   public static bool ParseSingleTriggerValue(ref Value<int> inValue, LineKvp<string, string> command, PathObj po)
   {
      if (!TriggerParser.ParseTriggerOfValue(command.Value, out int parsedValue).Then(o => o.ConvertToLoadingError(po, $"Failed parsing {command.Key} Trigger", command.Line)))
         return false;
      inValue.Val = parsedValue;
      return true;
   }

   public static bool ParseSingleTriggerVal<T>(ref Value<T> value, LineKvp<string, string> line, PathObj po) where T : notnull
   {
      if (Converter.Convert(line.Value, out T obj).Then(o => o.ConvertToLoadingError(po, $"Failed parsing {line.Key} Trigger", line.Line)))
      {
         value.Val = obj;
         return true;
      }
      return false;
   }

   public static bool ParseSingleTriggerValue(ref Value<string> inValue, LineKvp<string, string> command, PathObj po, string triggerName)
   {
      ParseSingleTriggerVal(ref inValue, command, po);
      if (!TriggerParser.ParseTriggerOfValue(command.Value, out string parsedValue).Then(o => o.ConvertToLoadingError(po, $"Failed parsing {command.Key} Trigger", command.Line)))
         return false;
      inValue.Val = parsedValue;
      return true;
   }
   public static bool ParseSingleTriggerValue(ref Value<bool> inValue, LineKvp<string, string> command, PathObj po)
   {
      if (!TriggerParser.ParseTriggerOfValue(command.Value, out bool parsedValue).Then(o => o.ConvertToLoadingError(po, $"Failed parsing {command.Key} Trigger", command.Line)))
         return false;
      inValue.Val = parsedValue;
      return true;
   }
   public static bool ParseSingleTriggerValue(ref Value<float> inValue, LineKvp<string, string> command, PathObj po)
   {
      if (!TriggerParser.ParseTriggerOfValue(command.Value, out float parsedValue).Then(o => o.ConvertToLoadingError(po, $"Failed parsing {command.Key} Trigger", command.Line)))
         return false;
      inValue.Val = parsedValue;
      return true;
   }

   public static bool ParseSingleTriggerReplaceValue(ScriptedTriggerSource parent, ref Value<int> inValue, LineKvp<string, string> command, PathObj po, string triggerName)
   {
      var error = TriggerParser.ParseTriggerOfValueReplace(command.Value, out var isReplace, out int parsedValue, out var parsedReplace);
      if (!error.Ignore())
      {
         ((ErrorObject)error).ConvertToLoadingError(po, "Failed parsing Integer", command.Line);
         return false;
      }
      if (!ParseSingleTriggerValueGeneral(parent, isReplace, parsedReplace!, parsedValue, ref inValue))
      {
         _ = new LoadingError(po, $"Value of type 'int' is of wrong for trigger <{triggerName}>: expected type '{inValue.Type}'", type: ErrorType.PCFL_TriggerValidationError);
         return false;
      }
      return true;
   }

   public static bool ParseSingleTriggerValueGeneral<T>(ScriptedTriggerSource parent, bool isReplace, string parsedReplace, T parsedValue, ref Value<T> inValue)
   {
      if (!isReplace)
      {
         inValue.Val = parsedValue;
         return true;
      }
      // in case of replace
      if (parent.replacements.TryGetValue(parsedReplace!, out var value))
      {
         if (value.Type != inValue.Type)
            return false;
         inValue = (Value<T>)value;
      }
      else
         parent.replacements.Add(parsedReplace!, inValue);

      return true;
   }
}