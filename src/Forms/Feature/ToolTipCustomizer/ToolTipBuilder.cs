using System.Text.RegularExpressions;
using System.Reflection;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.Helper;

namespace Editor.Forms.Feature;

public static class ToolTipBuilder
{
   private const string TOOLTIP_ATTRIBUTE_REGEX = @"\$(?<att>(?<attrName>.*?))(?<useLoc>(?:%L)?\$)";

   public static readonly PropertyInfo[] propertyInfo;

   static ToolTipBuilder()
   {
      propertyInfo = typeof(Province).GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(ToolTippable))).ToArray();
   }
   private static readonly Regex TooltipAttributeRegex = new(TOOLTIP_ATTRIBUTE_REGEX, RegexOptions.Compiled);

   public static string BuildToolTip(string rawToolTip, Province province)
   {
      // TODO improve and adept to new system of propNames
      
      var match = TooltipAttributeRegex.Match(rawToolTip);
      
      while (match.Success)
      {
         var str = string.Empty;
         if (match.Groups["attrName"].Value == "MAPMODE_SPECIFIC")
         {
            str = MapModeManager.CurrentMapMode.GetSpecificToolTip(province);
         }
         else
         {
            bool useLoc = match.Groups["useLoc"].Value.Contains("%L");
            var value = province.GetPropertyInfo(match.Groups["attrName"].Value)?.GetValue(province); // TODO fix
            if (value == null! || string.IsNullOrEmpty(value.ToString()))
            {
               continue;
            }
            str = value.GetType() == typeof(ICollection<>)
               ? string.Join(", ", (ICollection<string>)value)
               : useLoc ? Localisation.GetLoc(value.ToString()!) : value.ToString();
         }
         // Replace the match with the value
         rawToolTip = rawToolTip.Remove(match.Index, match.Length).Insert(match.Index, str!);
         match = match.NextMatch();
      }
      return rawToolTip;
   }

}

[AttributeUsage(AttributeTargets.Property)]
public class ToolTippable(bool isList = false) : Attribute
{
}