using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.DataClasses;
using Editor.Helper;
using Editor.Savers;

namespace Editor.Loading;

public partial class LocalisationLoading
{
   private static readonly Regex Regex = LocalisationRegex();
   public static void Load()
   {
      var sw = Stopwatch.StartNew();
      
      //var files = FilesHelper.GetFilesFromModAndVanillaUniquely($"*_l_{Globals.Language.ToString().ToLower()}.yml", "localisation");
      var replaceFiles = FilesHelper.GetFilesFromModAndVanillaUniquely($"*_l_{Globals.Language.ToString().ToLower()}.yml", "localisation", "replace");

      var vanillaFiles = FilesHelper.GetAllFilesInFolder( Path.Combine(Globals.VanillaPath, "localisation"), $"*_l_{Globals.Language.ToString().ToLower()}.yml");
      var modFiles = FilesHelper.GetAllFilesInFolder( Path.Combine(Globals.ModPath, "localisation"), $"*_l_{Globals.Language.ToString().ToLower()}.yml");
      
      LocalisationSaver.Initialize(vanillaFiles, modFiles, replaceFiles);

      Dictionary<string, Dictionary<string, string>> vanillaLocalisation = [];
      Dictionary<string, Dictionary<string, string>> modLocalisation = [];
      Dictionary<string, Dictionary<string, string>> replaceLoc = [];
      var collisions = new Dictionary<string, string>();
      
      LoadLocFiles(vanillaFiles, vanillaLocalisation, collisions);
      LoadLocFiles(modFiles, modLocalisation, collisions);
      LoadLocFiles(replaceFiles, replaceLoc, collisions);

      var totalLoc = 0;
      foreach (var loc in vanillaLocalisation.Values)
         totalLoc += loc.Count;
      Debug.WriteLine($"{totalLoc} Loc strings");
      totalLoc = 0;
      foreach (var loc in replaceLoc.Values)
         totalLoc += loc.Count;
      Debug.WriteLine($"{totalLoc} Replace Loc strings");

      Globals.VanillaLocalisation.Clear();
      Globals.ModLocalisation.Clear();
      Globals.LocalisationCollisions.Clear();
      Globals.ReplaceLocalisation.Clear();
      Globals.VanillaLocalisation = vanillaLocalisation;
      Globals.ModLocalisation = modLocalisation;
      Globals.ReplaceLocalisation = replaceLoc;
      Globals.LocalisationCollisions = collisions;
      sw.Stop();
      if (Globals.State == State.Loading)
         Globals.LoadingLog.WriteTimeStamp($"Localisation loaded [{collisions.Count}] collisions", sw.ElapsedMilliseconds);
   }

   private static void LoadLocFiles(List<string> files, Dictionary<string, Dictionary<string, string>> loc, Dictionary<string, string> collisions)
   {
      Parallel.ForEach(files, fileName =>
      {
         IO.ReadAllLinesANSI(fileName, out var lines);
         var threadDict = new Dictionary<string, string>();

         foreach (var line in lines)
         {
            var match = Regex.Match(line);
            if (!match.Success)
               continue;

            var key = match.Groups["key"].Value;
            var value = match.Groups["value"].Value;

            if (threadDict.TryGetValue(key, out var existingValue))
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
         lock (loc)
         {
            loc.Add(fileName, threadDict);
         }
      });
   }

   [GeneratedRegex(@"\s*(?<key>.*):\d*\s+""(?<value>.*)""", RegexOptions.Compiled)]
   private static partial Regex LocalisationRegex();
}