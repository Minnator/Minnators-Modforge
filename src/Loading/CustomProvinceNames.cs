﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Parser;
using Editor.Saving;

namespace Editor.Loading
{
   public static partial class CustomProvinceNames
   {
      private static readonly Regex RawLineRegex = RawLineRegexGenerate();
      private static readonly Regex CultLocLineRegex = CultLocRegexGenerate();

      [GeneratedRegex(@"(?<province>\d+)\s*=\s*(?<value>.*)", RegexOptions.Compiled)]
      private static partial Regex RawLineRegexGenerate();

      [GeneratedRegex(@"(?<value>""([^""]+)"")\s+((?<value>""([^""]*)"")|(?<capital>[^\s}]+))", RegexOptions.Compiled)]
      private static partial Regex CultLocRegexGenerate();

      public static void Load()
      {
         var files = FilesHelper.GetFilesFromModAndVanillaUniquelySeparated("*.txt", "common", "province_names");

         Dictionary<Province, HashSet<CultProvLocObject>> totalDict = [];
         Dictionary<Province, HashSet<CultProvLocObject>> collisionsDict = [];

         foreach (var file in files.Item1)
         {
            var results = LoadFile(file, true);
            UpdateTotalLocDict(ref totalDict, ref collisionsDict, ref results);
         }

         foreach (var file in files.Item2)
         {
            var results = LoadFile(file, false);
            UpdateTotalLocDict(ref totalDict, ref collisionsDict, ref results);
         }
         
         CreateAndSetContainers(ref totalDict);
      }

      private static void CreateAndSetContainers(ref Dictionary<Province, HashSet<CultProvLocObject>> input)
      {
         Dictionary<Province, CultProvLocContainer> containers = [];

         foreach (var (province, cultProvLocObjects) in input)
         {
            if (!containers.TryGetValue(province, out var container))
            {
               CultProvLocContainer cont = new();
               cont.AddList(cultProvLocObjects);
               containers[province] = cont;
            }
            else
               container.AddList(cultProvLocObjects);
         }

         containers.TrimExcess();
         Globals.CustomProvinceNames.Clear();
         Globals.CustomProvinceNames = containers;
      }

      private static void UpdateTotalLocDict(ref Dictionary<Province, HashSet<CultProvLocObject>> total, ref Dictionary<Province, HashSet<CultProvLocObject>> collisions, ref Dictionary<Province, HashSet<CultProvLocObject>> toAdd)
      {
         foreach (var result in toAdd)
            if (!total.TryGetValue(result.Key, out var existing))
               total[result.Key] = result.Value;
            else
               foreach (var cultProvLocObject in result.Value)
                  if (!existing.Add(cultProvLocObject))
                  {
                     if (!collisions.TryGetValue(result.Key, out var existingCollisions))
                        collisions[result.Key] = [cultProvLocObject];
                     else
                        existingCollisions.Add(cultProvLocObject);
                  }
      }

      private static Dictionary<Province, HashSet<CultProvLocObject>> LoadFile(string file, bool isMod)
      {
         IO.ReadAllInANSI(file, out var data);
         Parsing.RemoveCommentFromMultilineString(ref data, out var content);
         var lines = Parsing.GetLinesOfString(ref content);
         var (type, typeValue) = GetCustomProvLocType(file);
         Dictionary <Province, HashSet<CultProvLocObject>> results = [];
         var pathObj = PathObj.FromPath(file, isMod);

         foreach (var line in lines)
         {
            var kvp = RawLineRegex.Match(line);
            if (!kvp.Success)
            {
               _ = new FileRefLogEntry(LogType.Warning, $"Failed to parse province_names: {line} in file {file}", file);
               continue;
            }

            var id = kvp.Groups["province"].Value;
            if (!Globals.ProvinceIdToProvince.TryGetValue(int.Parse(id), out var province))
            {
               _ = new FileRefLogEntry(LogType.Warning, $"Province {id} is used but never defined as a province in province_names! {file}", file);
               continue;
            }

            var value = kvp.Groups["value"].Value;
            if (value.StartsWith('{'))
            {
               var match = CultLocLineRegex.Match(value);
               if (!match.Success)
               {
                  _ = new FileRefLogEntry(LogType.Warning, $"Failed to parse complex province_names: {value} in file {file}", file);
                  continue;
               }

               var provName = match.Groups["value"].Value;
               var capitalName = match.Groups["capital"].Value;
               if (!results.TryAdd(province, [new CultProvLocObject(type, typeValue, provName, capitalName, ObjEditingStatus.Unchanged)]))
                  _ = new FileRefLogEntry(LogType.Warning, $"Duplicate province_names definition for {province} in {file}", file);
               continue;
            }

            value = value.TrimQuotes();
            if (!results.TryAdd(province, [new CultProvLocObject(type, typeValue, value, ObjEditingStatus.Unchanged)]))
               _ = new FileRefLogEntry(LogType.Warning, $"Duplicate province_names definition for {province} in {file}", file);
         }

         if (isMod) //@MelonCoaster why do we only add when mod?
            foreach (var objList in results.Values)
            {
               foreach (var obj in objList)
                  obj.SetPath(ref pathObj);
               SaveMaster.AddRangeToDictionary(pathObj, objList);
            }

         return results;
      }

      private static (CustomProvLocType, string) GetCustomProvLocType(string path)
      {
         var fileName = Path.GetFileNameWithoutExtension(path);
         
         if (Tag.TryParse(fileName, out var tag))
            if (Globals.Countries.Contains(tag))
               return (CustomProvLocType.Tag, fileName);
            else
               _ = new LogEntry(LogType.Warning,
                  $"Country tag {tag} referenced in {fileName} (province_names) with no country attached to it!");

         if (Globals.Cultures.ContainsKey(fileName))
            return (CustomProvLocType.Culture, fileName);

         if (Globals.CultureGroups.ContainsKey(fileName))
            return (CustomProvLocType.CultureGroup, fileName);
         return (CustomProvLocType.None, fileName);
      }
   }
}