using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Editor.Helper;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Editor.DataClasses;

namespace Editor.Loading;

public class LocalisationLoading
{
   public static void Load(string modFolder, string vanillaFolder, Language language, ref Log log)
   {
      var sw = Stopwatch.StartNew();
      var modLocPath = Path.Combine(modFolder, "localisation");
      var vanillaLocPath = Path.Combine(vanillaFolder, "localisation");
      var files = FilesHelper.GetAllFilesInFolder(vanillaLocPath, $"*_l_{language.ToString().ToLower()}.yml");
      files.AddRange(FilesHelper.GetAllFilesInFolder(modLocPath, $"*_l_{language}.yml"));

      var regex = new Regex(@"\s*(?<key>.*):\d\s+""(?<value>.*)""", RegexOptions.Compiled);
      var sw2 = Stopwatch.StartNew();
      var loc = new Dictionary<string, string>();
      var collisions = new Dictionary<string, string>();

      for (var i = 0; i < 100; i++)
      {
         loc = [];
         collisions = [];
         
         //TODO Concurrent crash
         Parallel.ForEach(files, fileName =>
         {
            var lines = IO.ReadAllLinesInUTF8(fileName);
            var threadDict = new Dictionary<string, string>();

            foreach (var line in lines)
            {
               var match = regex.Match(line);
               if (!match.Success) 
                  continue;

               var key = match.Groups["key"].Value;
               var value = match.Groups["value"].Value;

               if (loc.TryGetValue(key, out var existingValue))
               {
                  if (existingValue == value) 
                     continue;

                  if (!collisions.ContainsKey(key))
                     collisions.Add(key, existingValue);
                  collisions[key] = value;
               }
               else
               {
                  threadDict.Add(key, value);
               }
            }
            foreach (var kvp in threadDict)
            {
               lock (loc)
               {
                  if (!loc.ContainsKey(kvp.Key))
                     loc.Add(kvp.Key, kvp.Value);
                  else
                  {
                     if (!collisions.ContainsKey(kvp.Key))
                        collisions.Add(kvp.Key, kvp.Value);
                     collisions[kvp.Key] = kvp.Value;
                  }
               }
            }
            threadDict.Clear();
         });
      }
      sw2.Stop();
      log.WriteTimeStamp($"Loc loaded in [{sw2.ElapsedMilliseconds / 100}] ms per iteration", sw2.ElapsedMilliseconds / 100);

      Globals.Localisation = loc;
      Globals.LocalisationCollisions = collisions;
      sw.Stop();
      log.WriteTimeStamp($"Localisation loaded [{collisions.Count}] collisions", sw.ElapsedMilliseconds);
   }
}