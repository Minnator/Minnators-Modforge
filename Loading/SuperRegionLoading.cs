using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor.Loading;

public static class SuperRegionLoading
{
   private const string MAIN_PATTER = @"(?<name>[a-zA-Z_]*)\s*=\s*{\s*(?<regions>[\w\s]+)\s*\s*}";
   // TODO fix false read in if not formatted correctly

   public static void Load()
   {
      var sw = Stopwatch.StartNew();
      if (!FilesHelper.GetModOrVanillaPath(out var path, out var isModPath, "map", "superregion.txt"))
      {
         Globals.ErrorLog.Write("Error: superregion.txt not found!");
         return;
      }

      var pathObj = PathObj.FromPath(path, isModPath);
      var newContent = IO.ReadAllLinesInUTF8(path);
      var sb = new StringBuilder();

      foreach (var line in newContent)
         sb.Append(Parsing.RemoveCommentFromLine(line));

      var matches = Regex.Matches(sb.ToString(), MAIN_PATTER, RegexOptions.Multiline);
      foreach (Match match in matches)
      {
         var superRegionName = match.Groups["name"].Value;
         var regionStrings = Parsing.GetStringList(match.Groups["regions"].Value);
         List<Region> regions = [];
         foreach (var region in regionStrings)
         {
            if (Globals.Regions.TryGetValue(region, out var reg))
               regions.Add(reg);
         }

         var sRegion = new SuperRegion(superRegionName, Globals.ColorProvider.GetRandomColor(), regions);
         sRegion.SetPath(ref pathObj);
      }

      FileManager.AddRangeToDictionary(pathObj, Globals.SuperRegions.Values);

      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Parsing Super Regions", sw.ElapsedMilliseconds);
   }
}
