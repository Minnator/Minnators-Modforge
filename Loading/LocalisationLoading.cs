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

      var loc = new Dictionary<string, string>();
      var collisions = new Dictionary<string, string>();

      //TODO Concurrent crash
      Parallel.ForEach(files, fileName =>
      {
         var lines = IO.ReadAllLinesInUTF8(fileName);
         var threadDict = new Dictionary<string, string>();

         foreach (var line in lines)
         {
            var match = regex.Match(line);
            if (match.Success)
            {
               var key = match.Groups["key"].Value;
               var value = match.Groups["value"].Value;

               if (loc.TryGetValue(key, out var existingValue))
               {
                  if (existingValue != value)
                  {
                     lock (collisions)
                     {
                        if (!collisions.ContainsKey(key))
                           collisions.Add(key, existingValue);
                        collisions[key] = value;
                     }
                  }
               }
               else
               {
                  threadDict.Add(key, value);
               }
            }
         }

         lock (loc)
         {
            foreach (var kvp in threadDict)
               loc.Add(kvp.Key, kvp.Value);
         }
      });

      Globals.Localisation = loc;
      Globals.LocalisationCollisions = collisions;
      sw.Stop();
      log.WriteTimeStamp($"Localisation loaded [{collisions.Count}] collisions", sw.ElapsedMilliseconds);
   }
}