using Editor.DataClasses.GameDataClasses;
using Editor.ParadoxLanguage.Scope;

namespace Editor.ParadoxLanguage.Trigger
{
   public class IntAttributeTrigger(ProvAttrGet attribute, Func<int, bool> condition) : Trigger
   {
      public ProvAttrGet Attribute { get; } = attribute;
      public Func<int, bool> Condition { get; } = condition;

      public override bool Evaluate(IScope scope)
      {
         if (scope is Province province)
         {
            var value = Convert.ToInt32(province.GetAttribute(Attribute));
            return Condition(value);
         }
         return false;
      }
   }

   public class FloatAttributeTrigger(ProvAttrGet attribute, Func<float, bool> condition) : Trigger
   {
      public ProvAttrGet Attribute { get; } = attribute;
      public Func<float, bool> Condition { get; } = condition;

      public override bool Evaluate(IScope scope)
      {
         if (scope is Province province)
         {
            var value = Convert.ToSingle(province.GetAttribute(Attribute));
            return Condition(value);
         }
         return false;
      }
   }

   public class StringAttributeTrigger(ProvAttrGet attribute, Func<string, bool> condition) : Trigger
   {
      public ProvAttrGet Attribute { get; } = attribute;
      public Func<string, bool> Condition { get; } = condition;

      public override bool Evaluate(IScope scope)
      {
         if (scope is Province province)
         {
            var value = province.GetAttribute(Attribute).ToString();
            if (value == null)
               return false;
            return Condition(value);
         }
         return false;
      }
   }

}