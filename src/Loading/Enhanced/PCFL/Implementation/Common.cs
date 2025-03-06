using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Implementation;

public enum PCFL_Type
{
   Int,
   Float,
   String,
   Bool,
}

public interface ITarget;

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


public abstract class PCFL_Object{

};

public interface IPCFLObject 
{
   public bool ParseWithReplacement(ScriptedTriggerSource parent, EnhancedBlock block, PathObj po) => throw new NotImplementedException();
   public bool ParseWithReplacement(ScriptedTriggerSource parent, LineKvp<string, string> command, PathObj po) => throw new NotImplementedException();
   // Scope?
   public bool Parse(EnhancedBlock block, PathObj po) => throw new NotImplementedException();
   public bool Parse(LineKvp<string, string> command, PathObj po) => throw new NotImplementedException();
};

public interface IToken: IPCFLObject
{
   public void Activate(ITarget target);
}

public interface ITrigger: IPCFLObject
{
   public static ITrigger Empty { get; }= AlwaysTrigger.TrueTrigger;
   public bool Evaluate(ITarget target);
}

public class AlwaysTrigger : ITrigger
{

   private bool _value;

   private AlwaysTrigger(bool value)
   {
      _value = value;
   }

   public static readonly AlwaysTrigger TrueTrigger = new (true);
   public static readonly AlwaysTrigger FalseTrigger = new (false);
   
   public bool Evaluate(ITarget target)
   {
      return _value;
   }
}