using Editor.ErrorHandling;
using Editor.Forms.Loadingscreen;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Scribbel
{

   public class Trigger
   {
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

   public class BaseTrigger : Trigger
   {
      public virtual void ParseWithReplacement(ScriptedTrigger parent, EnhancedBlock block, PathObj po)
      {
         throw new NotImplementedException();
      }

      public virtual void ParseWithReplacement(ScriptedTrigger parent, LineKvp<string, string> command, PathObj po)
      {
         throw new NotImplementedException();
      }

      public virtual void Parse(EnhancedBlock block, PathObj po)
      {
         throw new NotImplementedException();
      }

      public virtual void Parse( LineKvp<string, string> command, PathObj po)
      {
         throw new NotImplementedException();
      }
   }
   
   public class BaseTriggerBool : BaseTrigger
   {
      private Value<bool> _boolValue = null!;

      public override void Parse(LineKvp<string, string> command, PathObj po)
      {
         if (!PCFL_TriggerParser.ParseTrigger(command.Value, out bool parsedBool)
            .Then(o => o.ConvertToLoadingError(po, "Failed parsing scripted Trigger", command.Line)))
            return;
         _boolValue = new(parsedBool);
      }

      public override void ParseWithReplacement(ScriptedTrigger parent, LineKvp<string, string> command, PathObj po)
      {
         if (!PCFL_TriggerParser.ParseTriggerReplace(command.Value, out var isReplace, out bool parsedBool, out var parsedReplace)
            .Then(o => o.ConvertToLoadingError(po, "Failed parsing scripted Trigger", command.Line)))
            return;

         if (!isReplace)
         {
            _boolValue = new(parsedBool);
            return;
         }
         // in case of replace
         if (parent.replacements.TryGetValue(parsedReplace!, out var value))
         {
            if (value.Type != PCFL_Type.Bool)
            {
               _ = new LoadingError(po, "msg", command.Line, addToManager: false);
               return;
            }
            _boolValue = (Value<bool>)value;
         }
         else
         {
            _boolValue = new(false); // default value as we do not have one yet
            parent.replacements.Add(parsedReplace!, _boolValue);
         }
      }
   }

   public class CalledTrigger(ScriptedTrigger trigger) : Trigger
   {
      public Dictionary<string, Value> values;
      public void Activate() {
         foreach (var kv in values)
         {
            kv.Value.CopyTo(trigger.replacements[kv.Key]);
         }
      }
   }


   // ex (TAX) <- Scripted
   // hehe (hello) -> ex(hello) <- CalledTrigger with parameter
   // ^^^ Scripted
   // Todo optimize dicts with arrays / lists
   public class ScriptedTrigger
   {
      public Dictionary<string, Value> replacements = [];
      Trigger trigger = new();


   }
}