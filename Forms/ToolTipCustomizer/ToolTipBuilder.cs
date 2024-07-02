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
      var lastMatch = 0;
      while (true)
      {
         var match = TooltipAttributeRegex.Match(rawToolTip, lastMatch);
         if (match.Success)
         {
            var value = Globals.Provinces[provinceId].GetAttribute(match.Groups["attrName"].Value);
            var str = value.GetType() == typeof(ICollection<>)
               ? string.Join(", ", (ICollection<string>)value)
               : value.ToString();
            // Replace the match with the value
            rawToolTip = rawToolTip.Remove(match.Index, match.Length).Insert(match.Index, str);
            lastMatch = match.Index + str.Length;
         }
         else
            break;
      }
      return rawToolTip;
   }

}