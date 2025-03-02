using System.Diagnostics;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Scribbel;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL;

public static class PCFL_TriggerParser
{
   #region ValuesParsing

   // Boolean Methods
   public static IErrorHandle ParseTriggerOfValueReplace(string value, out bool isReplace, out bool outType, out string? outReplace)
   {
      outReplace = null;
      isReplace = false;
      outType = false;

      var error = IsValueReplaceable(value, out isReplace, out outReplace);
      if (!error.Ignore())
         return error;

      return ParseTriggerOfValue(value, out outType);
   }

   public static IErrorHandle ParseTriggerOfValue(string value, out bool outType)
   {
      var handle = Converter.ParseBool(value, out var boolObj);
      outType = (bool)boolObj;
      return handle;
   }

   // Integer Methods
   public static IErrorHandle ParseTriggerOfValueReplace(string value, out bool isReplace, out int outType, out string? outReplace)
   {
      outReplace = null;
      isReplace = false;
      outType = 0;

      var error = IsValueReplaceable(value, out isReplace, out outReplace);
      if (!error.Ignore())
         return error;

      return ParseTriggerOfValue(value, out outType);
   }

   public static IErrorHandle ParseTriggerValues(string value, PCFL_Type type, out Value outValue)
   {
      IErrorHandle handle;
      switch (type)
      {
         case PCFL_Type.Int:
            handle = ParseTriggerOfValue(value, out int intObj);
            outValue = new Value<int>(intObj);
            break;
         case PCFL_Type.Float:
            handle = ParseTriggerOfValue(value, out float floatObj);
            outValue = new Value<float>(floatObj);
            break;
         case PCFL_Type.String:
            handle = ParseTriggerOfValue(value, out string stringObj);
            outValue = new Value<string>(stringObj);
            break;
         case PCFL_Type.Bool:
            handle = ParseTriggerOfValue(value, out bool boolObj);
            outValue = new Value<bool>(boolObj);
            break;
         default:
            throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }

      return handle;
   }
   
   public static IErrorHandle ParseTriggerOfValue(string value, out int outType)
   {
      var handle = Converter.ParseInt(value, out var intObj);
      outType = (int)intObj;
      return handle;
   }

   // Float Methods
   public static IErrorHandle ParseTriggerOfValueReplace(string value, out bool isReplace, out float outType, out string? outReplace)
   {
      outReplace = null;
      isReplace = false;
      outType = 0f;

      var error = IsValueReplaceable(value, out isReplace, out outReplace);
      if (!error.Ignore())
         return error;

      return ParseTriggerOfValue(value, out outType);
   }

   public static IErrorHandle ParseTriggerOfValue(string value, out float outType)
   {
      var handle = Converter.ParseFloat(value, out var floatObj);
      outType = (float)floatObj;
      return handle;
   }

   // String Methods
   public static IErrorHandle ParseTriggerOfValueReplace(string value, out bool isReplace, out string outType, out string? outReplace)
   {
      outReplace = null;
      isReplace = false;
      outType = string.Empty;

      var error = IsValueReplaceable(value, out isReplace, out outReplace);
      if (!error.Ignore())
         return error;

      return ParseTriggerOfValue(value, out outType);
   }

   public static IErrorHandle ParseTriggerOfValue(string value, out string outType)
   {
      return EnhancedParser.IsValidString(value, out outType);
   }


   // Helper Methods
   public static IErrorHandle IsValueReplaceable(string value, out bool isReplace, out string? outReplace)
   {
      outReplace = null;
      isReplace = false;

      if (value.StartsWith('$') && value.EndsWith('$'))
      {
         if (value.Length < 3)
            return new ErrorObject(ErrorType.TempParsingError, "Invalid replace value. Must not be empty", addToManager: false);
         outReplace = value[1..^1];
         isReplace = true;
      }

      return ErrorHandle.Success;
   }

   #endregion

   // Parses a file as an effect
   public static List<PCFL_Token> ParseSomeFile(string content, PCFL_Scope scope, PathObj po)
   {
      /*
       * how do we know what do to?
       * Determine if Trigger / Effect
       * if we are trigger => all sub elements are triggers
       * if we are effect we call us again bc we like recursion
       */

      var orderedElement = EnhancedParser.LoadBaseOrder(content, po);

      List<PCFL_Token> program = [];

      var limitIfFlowControl = new IfFLowControl(Trigger.Empty, []);

      foreach (var element in orderedElement)
      {
         if (element.IsBlock)
         {
            var block = (EnhancedBlock)element;
            if (block.Name.Equals("limit"))
            {
               List<Trigger> triggers = [];

               if (block.ParseTriggerBlock(scope, po, triggers))
               {
                  if (limitIfFlowControl.Trigger == Trigger.Empty) 
                     limitIfFlowControl.Trigger = triggers[0];
                  else if (limitIfFlowControl.Trigger is BooleanOperation { Operation: Operation.AND } operation)
                     operation.Triggers.AddRange(triggers);
                  else
                     limitIfFlowControl.Trigger = new BooleanOperation(Operation.AND, [..triggers, limitIfFlowControl.Trigger]);
               }
               else
                  _ = new ErrorObject(ErrorType.EmptyLimitBlock,
                                      $"Block '{block.Name}' has an empty 'limit' which can be removed as it is always true.",
                                      level: LogType.Information);
            }

         }
         else
         {
            foreach (var kvp in ((EnhancedContent)element).GetLineKvpEnumerator(po))
            {
               // TODO : Parse Effects
            }
         }
      }
      if (limitIfFlowControl.Trigger != Trigger.Empty) // a trigger in a scope is interpreted as a simple 'if'
      {
         limitIfFlowControl.SubTokens = program;
         return [limitIfFlowControl];
      }
      return program;
   }

   // used for limit, AND, NOT, OR, 
   public static bool ParseTriggerBlock(this EnhancedBlock block, PCFL_Scope scope, PathObj po, List<Trigger> trigger)
   {
      foreach (var triggerElement in block.GetElements())
      {
         if (triggerElement.IsBlock)
         {
            ParseTrigger((EnhancedBlock)triggerElement, scope, po, out var newTrigger);
            trigger.Add(newTrigger);
         }
         else
         {
            foreach (var kvp in ((EnhancedContent)triggerElement).GetLineKvpEnumerator(po))
            {
               ParseTrigger(kvp, scope, po, out var newTrigger);
               trigger.Add(newTrigger);
            }
         }
      }
      
      return trigger.Count > 0;
   }

   public static bool ParseTriggerBlockToAnd(this EnhancedBlock block, PCFL_Scope scope, PathObj po, out Trigger trigger)
   {
      List<Trigger> triggers = [];
      if (!block.ParseTriggerBlock(scope, po, triggers))
      {
         trigger = Trigger.Empty;
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
   public static bool ParseTrigger(LineKvp<string, string> input, PCFL_Scope scope, PathObj po, out Trigger trigger)
   {
      trigger = null!;
      if (!scope.IsValidTrigger(input.Key, out var creator))
      {
         _ = new LoadingError(po, $"Invalid Trigger: {input.Key}", line:input.Line, type: ErrorType.PCFL_TriggerValidationError);
         return false;
      }
      trigger = creator(null, input, scope, po)!;
      return trigger is not null;
   }

   // Mod -> Recovers from errors
   public static bool ParseTrigger(EnhancedBlock block, PCFL_Scope scope, PathObj po, out Trigger? trigger)
   {
      trigger = null!;
      switch (block.Name.ToUpper())
      {
         case "NOT":
            trigger = BooleanOperation.Parse(block, scope, po, Operation.NOT);
            return true;
         case "AND":
            trigger = BooleanOperation.Parse(block, scope, po, Operation.AND);
            return true;
         case "OR":
            trigger = BooleanOperation.Parse(block, scope, po, Operation.OR);
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

      if (!scope.IsValidTrigger(block.Name, out var creator))
      {
         _ = new LoadingError(po, $"Invalid Trigger: {block.Name}", line:block.StartLine, type: ErrorType.PCFL_TriggerValidationError);
         return false;
      }
      trigger = creator(block, null, scope, po)!;
      return trigger is not null;
   }

   public static bool ParseSingleTriggerValue(ref Value<int> inValue, LineKvp<string, string> command, PathObj po, string triggerName)
   {
      Debug.Assert(command.Key.Equals(triggerName), $"'{triggerName}' must be the trigger name of a {triggerName} trigger");
      if (!ParseTriggerOfValue(command.Value, out int parsedValue).Then(o => o.ConvertToLoadingError(po, $"Failed parsing {command.Key} Trigger", command.Line)))
         return false;
      inValue.Val = parsedValue;
      return true;
   }

   public static bool ParseSingleTriggerReplaceValue(ScriptedTriggerSource parent, ref Value<int> inValue, LineKvp<string, string> command, PathObj po, string triggerName)
   {
      Debug.Assert(command.Key.Equals(triggerName), $"'{triggerName}' must be the trigger name of a {triggerName} trigger");
      var error = ParseTriggerOfValueReplace(command.Value, out var isReplace, out int parsedValue, out var parsedReplace);
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

