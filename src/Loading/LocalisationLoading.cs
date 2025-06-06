using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Loading;

public partial class LocalisationLoading
{
   private static readonly Regex Regex = LocalisationRegex();

   public static void Load()
   {
      var replaceFiles =
         PathManager.GetFilesFromModAndVanillaUniquely($"*_l_{Globals.Settings.Misc.Language.ToString().ToLower()}.yml", "localisation", "replace");

      var files = PathManager.GetAllFilesInFolder($"*_l_{Globals.Settings.Misc.Language.ToString().ToLower()}.yml", ["localisation"]);

      HashSet<LocObject> loc= [];
      HashSet<LocObject> replaceLoc = [];
      HashSet<LocObject> collisions = [];


      LoadLocFiles(replaceFiles, replaceLoc, collisions);
      LoadLocFiles(files, loc, collisions);

      CombineWithCollisions(replaceLoc, loc, collisions);

      Globals.Localisation.Clear();
      Globals.Localisation = replaceLoc;
      Globals.LocalisationCollisions.Clear();
      Globals.LocalisationCollisions = collisions;
   }

   private static void CombineWithCollisions(HashSet<LocObject> locObjects1, HashSet<LocObject> locObject2, HashSet<LocObject> collisions)
   {
      foreach (var loc in locObject2)
         if (!locObjects1.Add(loc))
            collisions.Add(loc);
   }


   private static void LoadLocFiles(List<string> files, HashSet<LocObject> loc, HashSet<LocObject> collisions)
   {
      Parallel.ForEach(files,
                       () => new HashSet<LocObject>(),
                       (filePath, _, localHashSet) =>
                       {
                          IO.ReadAllLinesANSI(filePath, out var lines);
                          var pathObj = PathObj.FromPath(filePath);

                          foreach (var line in lines)
                          {
                             var match = Regex.Match(line);
                             if (!match.Success)
                                continue;

                             var value = match.Groups["value"].Value;

                             var locObj = new LocObject(match.Groups["key"].Value, value, ObjEditingStatus.Unchanged);
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

                          if (pathObj.IsModPath && localHashSet.Count > 0)
                             SaveMaster.AddRangeToDictionary(pathObj, localHashSet);

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

   private static readonly LocObject SearchLoc = new("", string.Empty, ObjEditingStatus.Immutable);

   internal static void FilterLocObjs()
   {
      HashSet<LocObject> filteredLoc = new(12000);

      FilterByKey(Globals.Countries.Values.Select(x => x.TitleKey), Globals.Localisation, filteredLoc);
      FilterByKey(Globals.Countries.Values.Select(x => x.AdjectiveKey), Globals.Localisation, filteredLoc);

      FilterByKey(Globals.Provinces.Select(x => x.TitleKey), Globals.Localisation, filteredLoc);
      FilterByKey(Globals.Provinces.Select(x => x.AdjectiveKey), Globals.Localisation, filteredLoc);

      FilterByKey(Globals.Areas.Keys, Globals.Localisation, filteredLoc);
      FilterByKey(Globals.Regions.Keys, Globals.Localisation, filteredLoc);
      FilterByKey(Globals.SuperRegions.Keys, Globals.Localisation, filteredLoc);
      FilterByKey(Globals.TradeNodes.Keys, Globals.Localisation, filteredLoc);
      FilterByKey(Globals.TradeCompanies.Keys, Globals.Localisation, filteredLoc);
      FilterByKey(Globals.CultureGroups.Keys, Globals.Localisation, filteredLoc);
      FilterByKey(Globals.ColonialRegions.Keys, Globals.Localisation, filteredLoc);

      FilterByKey(Globals.Cultures.Keys, Globals.Localisation, filteredLoc);
      FilterByKey(Globals.Religions.Keys, Globals.Localisation, filteredLoc);

      FilterByKey(Globals.Missions.Keys, Globals.Localisation, filteredLoc);
      FilterByKey(Globals.MissionSlots.Select(x => x.Name), Globals.Localisation, filteredLoc);

      FilterByKey(Globals.Bookmarks.Select(x => x.TitleKey), Globals.Localisation, filteredLoc);

      Globals.Localisation.Clear();
      Globals.Localisation = filteredLoc;
   }

   private static void FilterByKey(IEnumerable<string> keyProvider, HashSet<LocObject> source, HashSet<LocObject> target)
   {
      foreach (var key in keyProvider)
      {
         SearchLoc.Key = key;
         if (source.TryGetValue(SearchLoc, out var locObj))
            target.Add(locObj);
      }
   }


   [GeneratedRegex(@"\s*(?<key>.*):\d*\s+""(?<value>.*)""", RegexOptions.Compiled)]
   private static partial Regex LocalisationRegex();
}