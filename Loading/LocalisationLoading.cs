using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Editor.Helper;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Editor.DataClasses;

namespace Editor.Loading;

public class LocalisationLoading
{
   public static void Load(string modFolder, string vanillaFolder, Language language)
   {
      var sw = Stopwatch.StartNew();
      var modLocPath = Path.Combine(modFolder, "localisation");
      var vanillaLocPath = Path.Combine(vanillaFolder, "localisation");
      var files = FilesHelper.GetAllFilesInFolder(vanillaLocPath, $"*_l_{language.ToString().ToLower()}.yml");
      files.AddRange(FilesHelper.GetAllFilesInFolder(modLocPath, $"*_l_{language}.yml"));

      var regex = new Regex(@"\s*(?<key>.*):\d*\s+""(?<value>.*)""", RegexOptions.Compiled);
      var loc = new Dictionary<string, string>();
      var collisions = new Dictionary<string, string>();
      
      Parallel.ForEach(files, fileName =>
      {
         IO.ReadAllLinesANSI(fileName, out var lines);
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
         
               if (!collisions.TryAdd(key, existingValue))
                  collisions[key] = value;   
            }
            else
            {
               threadDict.TryAdd(key, value);
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

      Globals.Localisation = loc;
      Globals.LocalisationCollisions = collisions;
      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp($"Localisation loaded [{collisions.Count}] collisions", sw.ElapsedMilliseconds);
   }

   private static void BulkLoading(string modFolder, string vanillaFolder, string language, ref Log loadingLog)
   {
      var sw = Stopwatch.StartNew();
      var modLocPath = Path.Combine(modFolder, "name");
      var vanillaLocPath = Path.Combine(vanillaFolder, "name");
      var files = FilesHelper.GetAllFilesInFolder(vanillaLocPath, $"*_l_{language.ToString().ToLower()}.yml");
      files.AddRange(FilesHelper.GetAllFilesInFolder(modLocPath, $"*_l_{language}.yml"));

      var regex = new Regex(@"\s*(?<key>.*):\d*\s+""(?<value>.*)""", RegexOptions.Compiled);
      var loc = new Dictionary<string, string>();
      var collisions = new Dictionary<string, string>();
      List<string> lines = new(130000); // avg num of loc lines
      
      var sw2 = Stopwatch.StartNew();
      Parallel.ForEach(files, fileName =>
      {
         IO.ReadAllLinesANSI(fileName, out var data);
         lock (lines)
         {
            lines.AddRange(data);
         }
      });
      sw2.Stop();
      Console.WriteLine($"Read all lines in {sw2.ElapsedMilliseconds} ms");
      loadingLog.WriteTimeStamp("Read all lines", sw2.ElapsedMilliseconds);

      var numOfLines = lines.Count;
      var batchSize = numOfLines / Environment.ProcessorCount;
      var partitioner = Partitioner.Create(0, numOfLines, batchSize);
      var missMatchCounter = 0;
      List<string> missMatches = new(2000);
      lines.TrimExcess();
      var content = lines.ToArray();

      List<Dictionary<string, string>> threadDicts = new(Environment.ProcessorCount);

      Parallel.ForEach(partitioner, range =>
      {
         Dictionary<string, string> threadDict = [];

         for (var i = range.Item1; i < range.Item2; i++)
         {
            var line = content[i];
            var match = regex.Match(line);

            if (!match.Success)
            {
               // Record mismatched lines
               lock (missMatches)
               {
                  missMatchCounter++;
                  missMatches.Add(line);
               }
               continue;
            }

            var key = match.Groups["key"].Value;
            var value = match.Groups["value"].Value;

            string existingValue;
            var foundInLoc = false;

            // Check loc dictionary outside of lock
            lock (loc)
            {
               if (loc.TryGetValue(key, out existingValue))
               {
                  foundInLoc = true;
               }
            }

            if (foundInLoc)
            {
               // Compare values and handle collisions
               if (existingValue != value)
               {
                  lock (collisions)
                  {
                     if (!collisions.ContainsKey(key))
                     {
                        collisions.Add(key, existingValue);
                     }
                     collisions[key] = value;
                  }
               }
            }
            else
            {
               // Add to thread-local dictionary
               threadDict[key] = value;
            }
         }

         // Merge thread-local dictionary into loc and handle collisions
         lock (loc)
         {
            foreach (var kvp in threadDict)
            {
               if (!loc.ContainsKey(kvp.Key))
               {
                  loc[kvp.Key] = kvp.Value;
               }
               else
               {
                  lock (collisions)
                  {
                     if (!collisions.ContainsKey(kvp.Key))
                     {
                        collisions.Add(kvp.Key, kvp.Value);
                     }
                     collisions[kvp.Key] = kvp.Value;
                  }
               }
            }
         }
      });

      sw.Stop();
      Console.WriteLine($"Localisation loaded [{collisions.Count}] collisions of [{loc.Count}/{numOfLines}] with {missMatchCounter} misses in {sw.ElapsedMilliseconds} ms");
      loadingLog.WriteTimeStamp($"Localisation loaded [{collisions.Count}] collisions", sw.ElapsedMilliseconds);

      List<string> clearedMissmatches = [];
      var strrr = $"l_{language.ToString().ToLower()}";
      foreach (var str in missMatches)
      {
         var newStr = str.Trim();
         if (string.IsNullOrWhiteSpace(newStr) || newStr.StartsWith("#") || newStr.StartsWith(strrr))
            continue;
         clearedMissmatches.Add(newStr);
      }

      File.WriteAllLines("C:\\Users\\david\\Downloads\\missmatches.txt", clearedMissmatches);
   }

}