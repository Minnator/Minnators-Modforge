using Editor.ErrorHandling;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Implementation;

public enum Operation
{
   AND,
   OR,
   NOT
}


public class CalledTrigger(ScriptedTriggerSource triggerSource, Dictionary<string, Value> values) : ITrigger
{
   public Dictionary<string, Value> _values = values;
   public void Activate()
   {
      foreach (var kv in _values)
      {
         kv.Value.CopyTo(triggerSource.replacements[kv.Key]);
      }
   }

   public bool Evaluate(ITarget target)
   {
      return false;
   }

}

public class BooleanOperation(Operation op, List<ITrigger> triggers) : ITrigger// AND, OR, NOT
{
   public Operation Operation { get; init; } = op;

   public List<ITrigger> Triggers = triggers;

   public bool Evaluate(ITarget target)
   {
      return Operation switch
      {
         Operation.AND => Triggers.All(t => t.Evaluate(target)),
         Operation.OR => Triggers.Any(t => t.Evaluate(target)),
         Operation.NOT => !Triggers.All(t => t.Evaluate(target)),
         _ => throw new EvilActions("WTF is this operation? We don't do quantum stuff yet!")
      };
   }


   public static ITrigger Parse(EnhancedBlock block, ParsingContext context, PathObj po, Operation op)
   {
      List<ITrigger> triggers = [];
      if (!block.ParseTriggerBlock(context, po, triggers))
         return ITrigger.Empty;

      // if there is one element anyway just simplify
      if (triggers.Count == 1 && op != Operation.NOT)
      {
         _ = new LoadingError(po, "Omitted an unnecessary 'AND' or 'OR' statement", line: block.StartLine, level: LogType.Information);
         return triggers[0];
      }

      return new BooleanOperation(op, triggers);
   }
}


// ex (TAX) <- Scripted
// hehe (hello) -> ex(hello) <- CalledTrigger with parameter
// ^^^ Scripted
// Todo optimize dicts with arrays / lists
public class ScriptedTriggerSource
{
   public Dictionary<string, Value> replacements = [];
   ITrigger trigger;

   public ScriptedTriggerSource(EnhancedBlock block, PathObj po)
   {
      trigger = null!;
      //TODO call default Parse Trigger but with unknown scope

      InferScopeFromUsage();
   }

   public void InferScopeFromUsage()
   {
      // TODO
   }

   public ITrigger CreateCallInstance(EnhancedBlock block, PathObj po)
   {
      Dictionary<string, Value> values = new(replacements);
      var content = block.GetContentElements(true, po)[0]; //maybe recover from multiple contents
      foreach (var line in content.GetLineKvpEnumerator(po))
      {
         if (!replacements.TryGetValue(line.Key, out var value))
         {
            _ = new LoadingError(po, $"Unknown attribute definition in scripted trigger call '{block.Name}' valid are: <{string.Join(',', replacements.Values)}>", type: ErrorType.PCFL_TriggerValidationError);
            continue;
         }
         /*
         if (GeneralFileParser.ParseSingleTriggerVal(ref value.Type,  line, out var outValue))
         {
            continue;
         } TODO
         */

         if (!values.ContainsKey(line.Key)) // Done??? >>> Maybe not use key from line but instead from dictionary then Value needs to save the string
         {
            _ = new LoadingError(po, $"Multiple definitions of '{line.Key}' in scripted trigger call '{block.Name}'", type: ErrorType.PCFL_TriggerValidationError, level: LogType.Warning);
            // multiple same keys
            continue;
         }
         //values[line.Key] = outValue;
      }
      if (values.Count != replacements.Count)
      {
         // missing parameters
         _ = new LoadingError(po, $"Missing parameters for scripted effect '{block.Name}'. Expected <{replacements.Count}> but got <{values.Count}>!", type: ErrorType.PCFL_TriggerValidationError);
         return null!;
      }
      return new CalledTrigger(this, values);
   }

   public ITrigger CreateInstance(LineKvp<string, string> command, PathObj po)
   {
      if (replacements.Count != 0)
      {
         _ = new LoadingError(po, $"Scripted trigger '{command.Key}' called without attributes but expected '{replacements.Count}'", type: ErrorType.PCFL_TriggerValidationError);
         // TODO how to recover
         return null!;
      }
      if (!TriggerParser.ParseTriggerOfValue(command.Value, out bool parsedBool)
                       .Then(o => o.ConvertToLoadingError(po, $"Failed to create instance of scripted Trigger '{command.Key}'.", command.Line, type: ErrorType.PCFL_TriggerValidationError)))
         return null!;

      if (!parsedBool)
      {
         return new BooleanOperation(Operation.NOT, [trigger]);
      }
      return trigger;
   }

}