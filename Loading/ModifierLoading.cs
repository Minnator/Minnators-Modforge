using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Windows.System;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;

namespace Editor.Loading
{
   public static class ModifierLoading
   {
      private const string MODIFIER_PATTERN = "(?<name>[A-Za-z_0-9]+)\\s*=\\s*{\\s*(?<content>[\\s\\S]*?)\\s*}";
      private static readonly Regex ModifierRegex = new(MODIFIER_PATTERN, RegexOptions.Compiled);

      public static void Load()
      {
         var sw = Stopwatch.StartNew();

         FilesHelper.GetFilesUniquelyAndCombineToOne(out var rawContent, "common", "event_modifiers");
         Parsing.RemoveCommentFromMultilineString(ref rawContent, out var fileContent);
         var matches = ModifierRegex.Matches(fileContent);

         Dictionary<string, EventModifier> modifiers = new(matches.Count);

         foreach (Match element in matches)
         {
            var content = element.Groups["content"].Value;
            var modifier = new EventModifier(element.Groups["name"].Value);
            var kvps = Parsing.GetKeyValueList(content);

            for (var i = 0; i < kvps.Count; i++)
            {
               if (!ModifierParser.ParseModifierFromName(kvps[i].Key, kvps[i].Value, out var mod))
               {
                  //TODO check if it is a trigger if so add to trigger attr and move on
                  Globals.ErrorLog.Write($"Unknown Modifier in modifiers file: {kvps[i].Key}");
                  continue;
               }
               modifier.Modifiers.Add(mod);
            }

            if (!modifiers.TryAdd(modifier.Name, modifier)) 
               Globals.ErrorLog.Write($"Duplicate modifier found: {modifier.Name}");
         }

         Globals.Modifiers = modifiers;
         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Event Modifiers", sw.ElapsedMilliseconds);
      }
   }
}