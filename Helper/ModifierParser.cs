using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Helper
{
   public static class ModifierParser
   {
      private const string PROVINCE_MODIFIER_REGEX = "name\\s*=\\s*(.*)\\s*duration\\s*=\\s*(.*)";
      private static readonly Regex ProvinceModifierRegex = new (PROVINCE_MODIFIER_REGEX, RegexOptions.Compiled);

      public static bool ParseProvinceModifier(string str, out Modifier mod)
      {
         mod = default!;

         var match = ProvinceModifierRegex.Match(str);
         if (!match.Success)
         {
            Globals.ErrorLog.Write($"Could not parse province modifier from string: {str}");
         }

         var name = match.Groups[1].Value;
         var duration = int.Parse(match.Groups[2].Value);

         mod = new (name, duration);
         return true;
      }

      public static bool ParseTradeModifier(string str, out TradeModifier tradeModifier)
      {
         tradeModifier = default!;

         var kvps = str.Split('=', '\n');
         if (kvps.Length % 2 != 0)
         {
            Globals.ErrorLog.Write($"Could not parse trade power modifier from string: {str}");
            return false;
         }

         tradeModifier = new ();

         for (var i = 0; i < kvps.Length; i += 2)
         {
            switch (kvps[i].Trim())
            {
               case "who":
                  tradeModifier.Who = kvps[i + 1].Trim();
                  break;
               case "duration":
                  if (!int.TryParse(kvps[i + 1].Trim(), out var duration))
                  {
                     Globals.ErrorLog.Write($"Could not parse duration from string: {kvps[i + 1]} in trade_power_modifier");
                     return false;
                  }
                  tradeModifier.Duration = duration;
                  break;
               case "power":
                  if (!int.TryParse(kvps[i + 1].Trim(), out var power))
                  {
                     Globals.ErrorLog.Write($"Could not parse power from string: {kvps[i + 1]} in trade_power_modifier");
                     return false;
                  }
                  tradeModifier.Power = power;
                  break;
               case "key":
                  tradeModifier.Key = kvps[i + 1].Trim();
                  break;
            }
         }

         return true;
      }


   }
}