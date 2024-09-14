using Editor.DataClasses.GameDataClasses;

namespace Editor.Parser
{
   public static class EffectParser
   {
      public static bool ParseScriptedEffect(string name, string input, out Effect effect)
      {
         if (input.Equals("yes"))
         {
            effect = new ScriptedEffect(name, input, EffectValueType.Complex);
         }
         else
         {
            var attr = input.Split('=');
            if (attr.Length % 2 != 0)
            {
               effect = default!;
               return false;
            }
            List<KeyValuePair<string, string>> effects = [];
            for (var i = 0; i < attr.Length; i += 2)
            {
               effects.Add(new(attr[i], attr[i + 1]));
            }
            effect = new ScriptedEffect(name, input, EffectValueType.Complex) { AttributesList = effects };
         }
         return true;
      }
   }
}