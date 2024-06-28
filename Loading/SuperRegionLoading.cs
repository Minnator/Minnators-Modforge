using System.Collections.Generic;
using Editor.Helper;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Editor.DataClasses;

namespace Editor.Loading;

public static class SuperRegionLoading
{
   private const string MAIN_PATTER = @"(?<name>[a-zA-Z_]*)\s*=\s*{\s*(?<regions>[\w\s]+)\s*\s*}";


   public static void Load(string folder, ColorProviderRgb colorProvider, ref Log log)
   {
      var sw = Stopwatch.StartNew();
      var path = Path.Combine(folder, "map", "superregion.txt");
      var newContent = IO.ReadAllLinesInUTF8(path);
      var superRegionDictionary = new Dictionary<string, SuperRegion>();
      var sb = new StringBuilder();
      var sb1 = new StringBuilder();

      foreach (var line in newContent)
         sb.Append(Parsing.RemoveCommentFromLine(line));

      var matches = Regex.Matches(sb.ToString(), MAIN_PATTER, RegexOptions.Multiline);
      foreach (Match match in matches)
      {
         var superRegionName = match.Groups["name"].Value;
         var regions = Parsing.GetStringList(match.Groups["regions"].Value);

         var sRegion = new SuperRegion(superRegionName, regions)
         {
            Color = colorProvider.GetRandomColor()
         };
         superRegionDictionary.Add(superRegionName, sRegion);

         foreach (var region in regions)
         {
            if (Globals.Regions.TryGetValue(region, out var reg))
            {
               reg.SuperRegion = superRegionName;
            }
         }
         
         sb1.AppendLine($"Super Region: {sRegion.Name} | {sRegion.Color}");
         foreach (var region in sRegion.Regions)
            sb1.AppendLine($"\t - {region}");
      }

      sb1.Clear();
      foreach (var region in Globals.Regions)
         sb1.AppendLine($"Region: {region.Value.Name} | {region.Value.SuperRegion} | {region.Value.Color}");

      File.WriteAllText(Path.Combine("C:\\Users\\david\\Downloads", "Regions.txt"), sb1.ToString());

      File.WriteAllText(Path.Combine("C:\\Users\\david\\Downloads", "SuperRegions.txt"), sb1.ToString());

      Globals.SuperRegions = superRegionDictionary;

      sw.Stop();
      log.WriteTimeStamp("Parsing Super Regions", sw.ElapsedMilliseconds);
   }
}
