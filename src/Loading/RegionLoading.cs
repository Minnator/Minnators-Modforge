using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Parsing = Editor.Parser.Parsing;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor.Loading;

public static partial class RegionLoading
{
   private static string _pattern =
      @"(?<regionName>[A-Za-z_]+)\s*=\s*{\s*areas\s*=\s*{\s*(?<areas>(?:\s*[A-Za-z_]+\s*)+)\s*}\s*(?<monsoons>(?:monsoon\s*=\s*{\s*(?:\s*[0-9.]+\s*)+\s*}\s*)*)}";

   public static void Load()
   {
      var sw = new Stopwatch();
      sw.Start();

      if (!FilesHelper.GetModOrVanillaPath(out var path, out var isModPath, "map", "region.txt"))
      {
         Globals.ErrorLog.Write("Error: region.txt not found!");
         return;
      }
      
      var pathObj = PathObj.FromPath(path, isModPath);
      Parsing.RemoveCommentFromMultilineString(IO.ReadAllInUTF8(path), out var content);
      ParseRegion(Regex.Matches(content, _pattern, RegexOptions.Multiline), ref pathObj);

      FileManager.AddRangeToDictionary(pathObj, Globals.Regions.Values);

      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Parsing regions", sw.ElapsedMilliseconds);
   }

   private static void ParseRegion(MatchCollection matches, ref PathObj pathObj)
   {
      Dictionary<string, Region> regionDictionary = [];
      foreach (Match match in matches)
      {
         var regionName = match.Groups["regionName"].Value;
         var areasStrings = Parsing.GetStringList(match.Groups["areas"].Value);
         List<Area> areas = [];

         foreach (var areaStr in areasStrings)
         {
            if (!Globals.Areas.TryGetValue(areaStr, out var area))
               Globals.ErrorLog.Write($"Error: Area {areaStr} not found in region {regionName}");
            else
               areas.Add(area);
         }

         var monsoons = new List<Monsoon>();

         foreach (Capture monsoon in match.Groups["monsoons"].Captures)
         {
            var monsoonMatches = MonsoonRegex().Matches(monsoon.Value);

            foreach (Match monsoonMatch in monsoonMatches) 
               monsoons.Add(new (monsoonMatch.Groups["start"].Value, monsoonMatch.Groups["end"].Value));
         }

         var region = new Region(regionName, Globals.ColorProvider.GetRandomColor(), areas) { Monsoon = monsoons };
         region.SetBounds();
         region.SetPath(ref pathObj);
         regionDictionary.Add(regionName, region);

      }
      Globals.Regions = regionDictionary;
   }

   [GeneratedRegex(@"monsoon\s*=\s*{\s*(?<start>[0-9.]+)\s*(?<end>[0-9.]+)\s*}")]
   private static partial Regex MonsoonRegex();
}