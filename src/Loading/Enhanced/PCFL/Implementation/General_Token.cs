using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Implementation
{

   public abstract class SimpleEffect<T>(T defaultValue) : IToken where T : notnull 
   {
      public Value<T> _value = new(defaultValue); // Default value and type of T
      public virtual bool Parse(LineKvp<string, string> command, PathObj po, ParsingContext context) => GeneralFileParser.ParseSingleTriggerVal(ref _value, command, po, context);
      public abstract void Activate(ITarget target);
      public virtual void GetTokenString(int tabs, ref StringBuilder sb)
      {
         SavingUtil.AddValue(tabs, _value, GetTokenName(), ref sb);
      }

      public override bool Equals(object? obj)
      {
         if (obj is null)
            return false;
         if (ReferenceEquals(this, obj))
            return true;

         if (obj is not SimpleEffect<T> other)
            return false;

         return _value.Equals(other._value);
      }

      public override int GetHashCode()
      {
         return _value.GetHashCode() ^ GetTokenName().GetHashCode();
      }

      public abstract string GetTokenName();
      public abstract string GetTokenDescription();
      public abstract string GetTokenExample();
      public override string ToString() => GetTokenName();
   }

   public abstract class SimpleEffectNotValue<T>(T defaultValue, T notValue) : SimpleEffect<T>(defaultValue) where T : notnull
   {
      public override void GetTokenString(int tabs, ref StringBuilder sb) => SavingUtil.AddValueIfNot(tabs, _value, notValue, GetTokenName(), ref sb);
   }

}