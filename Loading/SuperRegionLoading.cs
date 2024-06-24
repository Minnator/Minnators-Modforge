using System.Collections.Generic;
using Editor.Helper;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Editor.DataClasses;

namespace Editor.Loading;

public static class SuperRegionLoading
{
   private const string MAIN_PATTER = @"(?<name>.*)\s*=\s*{\s*(?<regions>[\w\s]+)\s*\s*}";


   public static void Load(string folder, ref Log log)
   {
      var sw = Stopwatch.StartNew();
      var path = Path.Combine(folder, "map", "superregion.txt");
      var newContent = IO.ReadAllLinesInUTF8(path);
      var superRegionDictionary = new Dictionary<string, SuperRegion>();
      var sb = new StringBuilder();

      foreach (var line in newContent)
         sb.Append(Parsing.RemoveCommentFromLine(line));

      var content = sb.ToString();
      var matches = Regex.Matches(content, MAIN_PATTER, RegexOptions.Multiline);
      foreach (Match match in matches)
      {
         var superRegionName = match.Groups["name"].Value;
         var regions = Parsing.GetStringList(match.Groups["regions"].Value);

         superRegionDictionary.Add(superRegionName, new SuperRegion(superRegionName, regions));

         foreach (var region in regions)
         {
            if (Data.Regions.TryGetValue(region, out var reg))
               reg.SuperRegion = region;
         }
      }


      Data.SuperRegions = superRegionDictionary;

      sw.Stop();
      log.WriteTimeStamp("Parsing Super Regions", sw.ElapsedMilliseconds);
   }

}