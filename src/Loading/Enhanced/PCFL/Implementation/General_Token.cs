using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Implementation
{

   public abstract class SimpleEffect<T>(T defaultValue) : IToken where T : notnull 
   {
      internal Value<T> _value = new(defaultValue); // Default value and type of T
      public virtual bool Parse(LineKvp<string, string> command, PathObj po, ParsingContext context) => GeneralFileParser.ParseSingleTriggerVal(ref _value, command, po, context);
      public abstract void Activate(ITarget target);
      public virtual void GetTokenString(int tabs, ref StringBuilder sb)
      {
         SavingUtil.AddValue(tabs, _value, GetTokenName(), ref sb);
         sb.Replace('\n', ' ');
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