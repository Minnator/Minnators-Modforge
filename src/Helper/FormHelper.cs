using Editor.DataClasses.GameDataClasses;

namespace Editor.Helper
{
   public static class FormHelper
   {

      public static string[] GetCompletionSuggestion(ModifierValueType type)
      {
         return type switch
         {
            ModifierValueType.Bool => ["yes", "no"],
            ModifierValueType.Int => ["-5", "-4", "-3", "-2", "-1", "0", "1", "2", "3", "4", "5"],
            ModifierValueType.Float => ["-0.50", "-0.20", "-0.15", "-0.10", "-0.05", "0.00", "0.05", "0.10", "0.15", "0.20", "0.50"],
            ModifierValueType.Percentile => ["-0.04", "-0.03", "-0.02", "-0.01", "0.00", "0.01", "0.02", "0.03", "0.04"],
            _ => [""]
         };
      }
   }
}