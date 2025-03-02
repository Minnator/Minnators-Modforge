using Windows.Devices.Spi;
using Editor.ErrorHandling;
using Editor.Forms.Loadingscreen;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Scribbel
{

   public class Trigger
   {
      public static Trigger Empty { get; } = new();
      public virtual bool Evaluate(ITarget target) => true;

      public virtual bool ParseWithReplacement(ScriptedTriggerSource parent, EnhancedBlock block, PathObj po)
      {
         throw new NotImplementedException();
      }

      public virtual bool ParseWithReplacement(ScriptedTriggerSource parent, LineKvp<string, string> command, PathObj po)
      {
         throw new NotImplementedException();
      }

      // TODO scope?
      public virtual bool Parse(EnhancedBlock block, PathObj po)
      {
         throw new NotImplementedException();
      }

      public virtual bool Parse(LineKvp<string, string> command, PathObj po)
      {
         throw new NotImplementedException();
      }
   }

   public enum PCFL_Type
   {
      Int,
      Float,
      String,
      Bool,
   }
   public static class PCFL_TypeExtensions
   {
      public static Type ToSystemType(this PCFL_Type type)
      {
         return type switch
         {
            PCFL_Type.Int => typeof(int),
            PCFL_Type.Float => typeof(float),
            PCFL_Type.String => typeof(string),
            PCFL_Type.Bool => typeof(bool),
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unknown type: {type}")
         };
      }

      public static PCFL_Type ToPCFL_Type(this Type type)
      {
         return type switch
         {
            not null when type == typeof(int) => PCFL_Type.Int,
            not null when type == typeof(float) => PCFL_Type.Float,
            not null when type == typeof(string) => PCFL_Type.String,
            not null when type == typeof(bool) => PCFL_Type.Bool,
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unknown type: {type}")
         };
      }

      public static object DefaultValue(this PCFL_Type type)
      {
         return type switch
         {
            PCFL_Type.Int => 0,
            PCFL_Type.Float => 0f,
            PCFL_Type.String => string.Empty,
            PCFL_Type.Bool => false,
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unknown type: {type}")
         };
      }

      private static void CopyValue(Value from, Value to)
      {
         if (to.Type != from.Type)
         {
            switch (to.Type)
            {
               case PCFL_Type.Int:
                  ((Value<int>)to).Val = (int)to.Type.DefaultValue();
                  break;
               case PCFL_Type.Float:
                  ((Value<float>)to).Val = (float)to.Type.DefaultValue();
                  break;
               case PCFL_Type.String:
                  ((Value<string>)to).Val = (string)to.Type.DefaultValue();
                  break;
               case PCFL_Type.Bool:
                  ((Value<bool>)to).Val = (bool)to.Type.DefaultValue();
                  break;
            }
         }
         else
         {
            switch (to.Type)
            {
               case PCFL_Type.Int:
                  ((Value<int>)to).Val = ((Value<int>)from).Val;
                  break;
               case PCFL_Type.Float:
                  ((Value<float>)to).Val = ((Value<float>)from).Val;
                  break;
               case PCFL_Type.String:
                  ((Value<string>)to).Val = ((Value<string>)from).Val;
                  break;
               case PCFL_Type.Bool:
                  ((Value<bool>)to).Val = ((Value<bool>)from).Val;
                  break;
            }
         }
      }

      public static void CopyTo(this Value from, Value to)
      {
         CopyValue(from, to);
      }

      public static void CopyFrom(this Value to, Value from)
      {
         CopyValue(from, to);
      }
   }

   public abstract class Value(PCFL_Type type)
   {
      public PCFL_Type Type { get; } = type;
   }

   
   public class Value<T>(T value) : Value(typeof(T).ToPCFL_Type())
   {
      public T Val = value;
      public static Value<T> operator <<(Value<T> a, Value<T> b)
      {
         a.Val = b.Val;
         return b;
      }
      public static Value<T> operator >>(Value<T> b, Value<T> a)
      {
         a.Val = b.Val;
         return a;
      }

   }
   
   public class BaseTriggerBool : Trigger
   {
      private Value<bool> _boolValue = null!;

      public override bool Parse(LineKvp<string, string> command, PathObj po)
      {
         if (!PCFL_TriggerParser.ParseTriggerOfValue(command.Value, out bool parsedBool)
            .Then(o => o.ConvertToLoadingError(po, "Failed parsing scripted Trigger", command.Line)))
            return false;
         _boolValue = new(parsedBool);
         return true;
      }

      public override bool ParseWithReplacement(ScriptedTriggerSource parent, LineKvp<string, string> command, PathObj po)
      {
         if (!PCFL_TriggerParser.ParseTriggerOfValueReplace(command.Value, out var isReplace, out bool parsedBool, out var parsedReplace)
            .Then(o => o.ConvertToLoadingError(po, "Failed parsing scripted Trigger", command.Line)))
            return false;

         if (!isReplace)
         {
            _boolValue = new(parsedBool);
            return true;
         }
         // in case of replace
         if (parent.replacements.TryGetValue(parsedReplace!, out var value))
         {
            if (value.Type != PCFL_Type.Bool)
            {
               _ = new LoadingError(po, "msg", command.Line, addToManager: false);
               return false;
            }
            _boolValue = (Value<bool>)value;
         }
         else
         {
            _boolValue = new(false); // default value as we do not have one yet
            parent.replacements.Add(parsedReplace!, _boolValue);
         }

         return true;
      }

      public override bool Evaluate(ITarget target)
      {
         return false;
      }

   }

   public class CalledTrigger(ScriptedTriggerSource triggerSource, Dictionary<string, Value> values) : Trigger
   {
      public Dictionary<string, Value> _values = values;
      public void Activate() {
         foreach (var kv in _values)
         {
            kv.Value.CopyTo(triggerSource.replacements[kv.Key]);
         }
      }

      public override bool Evaluate(ITarget target)
      {
         return false;
      }

   }

   public class BooleanOperation(Operation op, List<Trigger> triggers) : Trigger// AND, OR, NOT
   {
      public Operation Operation { get; init; } = op;

      public List<Trigger> Triggers = triggers;

      public override bool Evaluate(ITarget target)
      {
         return Operation switch
         {
            Operation.AND => Triggers.All(t => t.Evaluate(target)),
            Operation.OR => Triggers.Any(t => t.Evaluate(target)),
            Operation.NOT => !Triggers.All(t => t.Evaluate(target)),
            _ => throw new EvilActions("WTF is this operation? We don't do quantum stuff yet!")
         };
      }


      public static Trigger Parse(EnhancedBlock block, PCFL_Scope scope, PathObj po, Operation op)
      {
         List<Trigger> triggers = [];
         if (!block.ParseTriggerBlock(scope, po, triggers))
            return Empty;

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
      Trigger trigger;

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

      public Trigger CreateCallInstance(EnhancedBlock block, PathObj po)
      {
         Dictionary<string, Value> values = new(replacements);
         var content = block.GetContentElements(true, po)[0]; //maybe recover from multiple contents
         foreach (var line in content.GetLineKvpEnumerator(po))
         {
            if (!replacements.TryGetValue(line.Key, out var value))
            {
               _ = new LoadingError(po, $"Unknown attribute definition in scripted trigger call '{block.Name}' valid are: <{string.Join(',', replacements.Values)}>", type:ErrorType.PCFL_TriggerValidationError);
               continue;
            }
            if (PCFL_TriggerParser.ParseTriggerValues(line.Value, value.Type, out var outValue)
               .Then(o => o.ConvertToLoadingError(po, $"Failed to parse argument '{line.Key}' of '{block.Name}. Should be '{value.Type}'.", line.Line, type: ErrorType.PCFL_TriggerValidationError)))
            {
               continue;
            }
            
            if (!values.ContainsKey(line.Key)) // Done??? >>> Maybe not use key from line but instead from dictionary then Value needs to save the string
            {
               _ = new LoadingError(po, $"Multiple definitions of '{line.Key}' in scripted trigger call '{block.Name}'", type:ErrorType.PCFL_TriggerValidationError, level:LogType.Warning);
               // multiple same keys
               continue;
            }
            values[line.Key] = outValue;
         }
         if (values.Count != replacements.Count)
         {
            // missing parameters
            _ = new LoadingError(po, $"Missing parameters for scripted effect '{block.Name}'. Expected <{replacements.Count}> but got <{values.Count}>!", type:ErrorType.PCFL_TriggerValidationError);
            return null!;
         }
         return new CalledTrigger(this, values);
      }
      
      public Trigger CreateInstance(LineKvp<string, string> command, PathObj po)
      {
         if (replacements.Count != 0)
         {
            _ = new LoadingError(po, $"Scripted trigger '{command.Key}' called without attributes but expected '{replacements.Count}'", type:ErrorType.PCFL_TriggerValidationError);
            // TODO how to recover
            return null!;
         }
         if (!PCFL_TriggerParser.ParseTriggerOfValue(command.Value, out bool parsedBool)
                               .Then(o => o.ConvertToLoadingError(po, $"Failed to create instance of scripted Trigger '{command.Key}'.", command.Line, type:ErrorType.PCFL_TriggerValidationError)))
            return null!;

         if (!parsedBool)
         {
            return new BooleanOperation(Operation.NOT, [trigger]);
         }
         return trigger;
      }

      

   }

}