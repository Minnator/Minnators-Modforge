﻿using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Interfaces;
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

         var (modFiles, vanillaFiles) = FilesHelper.GetFilesFromModAndVanillaUniquelySeparated("*.txt", "common", "event_modifiers");
         Dictionary<string, EventModifier> modifiers = new();
         
         foreach (var file in modFiles)
         {
            var po = PathObj.FromPath(file, true);
            // Add the file to the ObjectSourceFiles and get the index
            HashSet<EventModifier> newModifiers = [];
            LoadEventModifier(file, ref po, newModifiers);
            foreach (var mod in newModifiers)
            {
               if (!modifiers.TryAdd(mod.Name, mod))
                  Globals.ErrorLog.Write($"Duplicate modifier found: {mod.Name}");
               FileManager.AddToDictionary(ref po, mod);
            }
         }

         foreach (var file in vanillaFiles)
         {
            var po = PathObj.FromPath(file, false);
            // Add the file to the ObjectSourceFiles and get the index
            HashSet<EventModifier> newModifiers = [];
            LoadEventModifier(file, ref po, newModifiers);
            foreach (var mod in newModifiers)
            {
               if (!modifiers.TryAdd(mod.Name, mod))
                  Globals.ErrorLog.Write($"Duplicate modifier found: {mod.Name}");
               FileManager.AddToDictionary(ref po, mod);
            }
         }

         Globals.EventModifiers = modifiers;
         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Event Modifiers", sw.ElapsedMilliseconds);
      }

      private static void LoadEventModifier(string fullPath, ref PathObj filePath, HashSet<EventModifier> modifiers)
      {
         Parsing.RemoveCommentFromMultilineString(IO.ReadAllInUTF8(fullPath), out var fileContent);
         var matches = ModifierRegex.Matches(fileContent);

         foreach (Match element in matches)
         {
            var content = element.Groups["content"].Value;
            var modifier = new EventModifier(element.Groups["name"].Value.Trim(), ref filePath);
            var kvps = Parsing.GetKeyValueList(content);

            for (var i = 0; i < kvps.Count; i++)
            {
               if (!ModifierParser.ParseModifierFromName(kvps[i].Key.Trim(), kvps[i].Value.Trim(), out var mod))
               {
                  if (ModifierParser.IsCustomModifierTrigger(kvps[i].Key))
                  {
                     modifier.TriggerAttribute.Add(new(kvps[i].Key, "yes"));
                     continue;
                  }
                  if (kvps[i].Key == "picture")
                  {
                     kvps[i].Value.TrimQuotes(out var pic);
                     modifier.Picture = pic;
                     continue;
                  }

                  //TODO check if it is a trigger if so add to trigger attr and move on
                  Globals.ErrorLog.Write($"Unknown Modifier in modifiers file: {kvps[i].Key}");
                  continue;
               }
               modifier.Modifiers.Add(mod);
            }

            modifiers.Add(modifier);
         }
      }
   }

}