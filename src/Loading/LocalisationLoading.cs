using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Policy;
using System.Text.RegularExpressions;
using Editor.DataClasses;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Loading;

public partial class LocalisationLoading
{
   private static readonly Regex Regex = LocalisationRegex();
   public static void Load()
   {
      var sw = Stopwatch.StartNew();
      
      //var files = FilesHelper.GetFilesFromModAndVanillaUniquely($"*_l_{Globals.Language.ToString().ToLower()}.yml", "localisation");
      var replaceFiles = FilesHelper.GetFilesFromModAndVanillaUniquely($"*_l_{Globals.Settings.MiscSettings.Language.ToString().ToLower()}.yml", "localisation", "replace");

      var vanillaFiles = FilesHelper.GetAllFilesInFolder( Path.Combine(Globals.VanillaPath, "localisation"), $"*_l_{Globals.Settings.MiscSettings.Language.ToString().ToLower()}.yml");
      var modFiles = FilesHelper.GetFilesInFolder( Path.Combine(Globals.ModPath, "localisation"), $"*_l_{Globals.Settings.MiscSettings.Language.ToString().ToLower()}.yml");
      
      HashSet<LocObject> vanillaLocalisation = [];
      HashSet<LocObject> modLocalisation = [];
      HashSet<LocObject> replaceLoc = [];
      HashSet<LocObject> collisions = [];


      LoadLocFiles(replaceFiles, replaceLoc, collisions, true);
      LoadLocFiles(modFiles, modLocalisation, collisions, true);
      LoadLocFiles(vanillaFiles, vanillaLocalisation, collisions, false);

      CombineWithCollisions(replaceLoc, modLocalisation, collisions);
      CombineWithCollisions(replaceLoc, vanillaLocalisation, collisions);

      Globals.Localisation.Clear();
      Globals.Localisation = replaceLoc;
      Globals.LocalisationCollisions.Clear();
      Globals.LocalisationCollisions = collisions;
      sw.Stop();
      if (Globals.State == State.Loading)
         Globals.LoadingLog.WriteTimeStamp($"Localisation loaded [{collisions.Count}] collisions", sw.ElapsedMilliseconds);
   }

   private static void CombineWithCollisions(HashSet<LocObject> locObjects1, HashSet<LocObject> locObject2, HashSet<LocObject> collisions)
   {
      foreach (var loc in locObject2)
         if (!locObjects1.Add(loc))
            collisions.Add(loc);
   }



   private static void LoadLocFiles(List<string> files, HashSet<LocObject> loc, HashSet<LocObject> collisions, bool isMod)
   {
      Parallel.ForEach(files,
          () => new HashSet<LocObject>(),

          (fileName, state, localHashSet) =>
          {
             IO.ReadAllLinesANSI(fileName, out var lines);
             var pathObj = PathObj.FromPath(fileName, isMod);

             foreach (var line in lines)
             {
                var match = Regex.Match(line);
                if (!match.Success)
                   continue;

                var value = match.Groups["value"].Value;

                var locObj = new LocObject(match.Groups["key"].Value, value, ObjEditingStatus.Unchanged) ;
                locObj.SetPath(ref pathObj);

                if (localHashSet.TryGetValue(locObj, out var existingValue))
                {
                   if (existingValue.Value == value)
                      continue;
                   lock (collisions)
                   {
                      collisions.Add(locObj);
                   }
                }
                else
                {
                   localHashSet.Add(locObj);
                }
             }
             if (isMod)
                FileManager.AddRangeToDictionary(pathObj, localHashSet);

             return localHashSet;
          },

          localHashSet =>
          {
             lock (loc)
             {
                loc.UnionWith(localHashSet);
             }
          }
      );
   }

   [GeneratedRegex(@"\s*(?<key>.*):\d*\s+""(?<value>.*)""", RegexOptions.Compiled)]
   private static partial Regex LocalisationRegex();
}