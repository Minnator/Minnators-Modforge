using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.DataClasses;
using Editor.Helper;

namespace Editor.Loading;

public class LocalisationLoading
{
   public static void Load()
   {
      var sw = Stopwatch.StartNew();
      var modLocPath = Path.Combine(Globals.ModPath, "localisation");
      var vanillaLocPath = Path.Combine(Globals.VanillaPath, "localisation");
      var files = FilesHelper.GetAllFilesInFolder(vanillaLocPath, $"*_l_{Globals.Language.ToString().ToLower()}.yml");
      files.AddRange(FilesHelper.GetAllFilesInFolder(modLocPath, $"*_l_{Globals.Language.ToString().ToLower()}.yml"));

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
               lock (collisions)
               {
                  if (!collisions.TryAdd(key, existingValue))
                     collisions[key] = value;   
               }
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
                  lock (collisions)
                  {
                     if (!collisions.ContainsKey(kvp.Key))
                        collisions.Add(kvp.Key, kvp.Value);
                     collisions[kvp.Key] = kvp.Value;
                  }
               }
            }
         }
         threadDict.Clear();
      });

      Globals.Localisation.Clear();
      Globals.Localisation = loc;
      Globals.LocalisationCollisions.Clear();
      Globals.LocalisationCollisions = collisions;
      sw.Stop();
      if (Globals.State == State.Loading)
         Globals.LoadingLog.WriteTimeStamp($"Localisation loaded [{collisions.Count}] collisions", sw.ElapsedMilliseconds);
   }
   
}