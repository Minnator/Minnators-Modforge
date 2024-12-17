using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.Helper;

namespace Editor.Forms.Feature;

public static class ToolTipBuilder
{
   private const string TOOLTIP_ATTRIBUTE_REGEX = @"\$(?<att>(?<attrName>.*?))(?<useLoc>(?:%L)?\$)";

   private static readonly Regex TooltipAttributeRegex = new(TOOLTIP_ATTRIBUTE_REGEX, RegexOptions.Compiled);

   public static string BuildToolTip(string rawToolTip, Province province)
   {
      var lastMatch = 0;
      while (true)
      {
         var match = TooltipAttributeRegex.Match(rawToolTip, lastMatch);
         if (match.Success)
         {
            var str = string.Empty;
            if (match.Groups["attrName"].Value == "MAPMODE_SPECIFIC")
            {
               str = MapModeManager.CurrentMapMode.GetSpecificToolTip(province);
            }
            else
            {
               bool useLoc = match.Groups["useLoc"].Value.Contains("%L");
               var value = province.GetAttribute(match.Groups["attrName"].Value);
               if (value == null! || string.IsNullOrEmpty(value.ToString()))
               {
                  lastMatch = match.Index + match.Length;
                  continue;
               }
               str = value.GetType() == typeof(ICollection<>)
                  ? string.Join(", ", (ICollection<string>)value)
                  : useLoc ? Localisation.GetLoc(value.ToString()!) : value.ToString();
            }
            // Replace the match with the value
            rawToolTip = rawToolTip.Remove(match.Index, match.Length).Insert(match.Index, str!);
            lastMatch = match.Index + str!.Length;
         }
         else
            break;
      }
      return rawToolTip;
   }

}