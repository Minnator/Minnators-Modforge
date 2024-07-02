using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Editor.Forms;

public static class ToolTipBuilder
{
   private const string TOOLTIP_ATTRIBUTE_REGEX = @"(?<att>\$(?<attrName>.*?)\$)";

   private static readonly Regex TooltipAttributeRegex = new(TOOLTIP_ATTRIBUTE_REGEX, RegexOptions.Compiled);

   public static string BuildToolTip(string rawToolTip, int provinceId)
   {
      var toolTipString = rawToolTip;
      var lastMatch = 0;
      while (true)
      {
         if (TooltipAttributeRegex.IsMatch(rawToolTip, lastMatch))
         {
            var match = TooltipAttributeRegex.Match(rawToolTip);
            var value = Globals.Provinces[provinceId].GetAttribute(match.Groups["attrName"].Value);
            var str = string.Empty;
            str = value.GetType() == typeof(ICollection<>)
               ? string.Join(", ", (ICollection<string>)value)
               : value.ToString();
            toolTipString = TooltipAttributeRegex.Replace(toolTipString, str);
            lastMatch = match.Index + match.Length;
         }
         else
            break;
      }
      return toolTipString;
   }

}