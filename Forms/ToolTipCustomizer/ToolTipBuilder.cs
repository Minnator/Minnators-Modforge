﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Forms;

public static class ToolTipBuilder
{
   private const string TOOLTIP_ATTRIBUTE_REGEX = @"\$(?<att>(?<attrName>.*?))(?<useLoc>(?:%L)?\$)";

   private static readonly Regex TooltipAttributeRegex = new(TOOLTIP_ATTRIBUTE_REGEX, RegexOptions.Compiled);

   public static string BuildToolTip(string rawToolTip, Province provinceId)
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
               str = Globals.MapModeManager.CurrentMapMode.GetSpecificToolTip(provinceId);
            }
            else
            {
               bool useLoc = match.Groups["useLoc"].Value.Contains("%L");
               var value = provinceId.GetAttribute(match.Groups["attrName"].Value);
               if (string.IsNullOrEmpty(value.ToString()))
               {
                  lastMatch = match.Index + match.Length;
                  continue;
               }
               str = value.GetType() == typeof(ICollection<>)
                  ? string.Join(", ", (ICollection<string>)value)
                  : useLoc ? Localisation.GetLoc(value.ToString()!) : value.ToString();
            }
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