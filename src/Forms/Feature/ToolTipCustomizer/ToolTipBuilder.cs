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
      Match match = TooltipAttributeRegex.Match(rawToolTip);
      while (match.Success)
      {
         string empty = string.Empty;
         string str;
         if (match.Groups["attrName"].Value == "MAPMODE_SPECIFIC")
         {
            str = MapModeManager.CurrentMapMode.GetSpecificToolTip(province);
         }
         else
         {
            bool flag = match.Groups["useLoc"].Value.Contains("%L");
            object values = province.GetPropertyInfo(match.Groups["attrName"].Value)?.GetValue((object)province);
            if (values == null || string.IsNullOrEmpty(values.ToString()))
            {
               match = match.NextMatch();
               continue;
            }
            str = values.GetType() == typeof(ICollection<>) ? string.Join(", ", (IEnumerable<string>)values) : (flag ? Localisation.GetLoc(values.ToString()) : values.ToString());
         }
         rawToolTip = rawToolTip.Remove(match.Index, match.Length).Insert(match.Index, str);
         match = match.NextMatch();
      }
      return rawToolTip;
   }

}

[AttributeUsage(AttributeTargets.Property)]
public class ToolTippable(bool isList = false) : Attribute
{
}