using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
      if (!FilesHelper.GetModOrVanillaPath(out var path, "map", "region.txt"))
      {
         Globals.ErrorLog.Write("Error: region.txt not found!");
         return;
      }
      
      Parsing.RemoveCommentFromMultilineString(IO.ReadAllInUTF8(path), out var content);
      ParseRegion(Regex.Matches(content, _pattern, RegexOptions.Multiline));

      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Parsing regions", sw.ElapsedMilliseconds);
   }

   private static void ParseRegion(MatchCollection matches)
   {
      Dictionary<string, Region> regionDictionary = [];
      foreach (Match match in matches)
      {
         var regionName = match.Groups["regionName"].Value;
         var areas = Parsing.GetStringList(match.Groups["areas"].Value);
         var monsoons = new List<Monsoon>();

         foreach (Capture monsoon in match.Groups["monsoons"].Captures)
         {
            var monsoonMatches = MonsoonRegex().Matches(monsoon.Value);

            foreach (Match monsoonMatch in monsoonMatches) 
               monsoons.Add(new (monsoonMatch.Groups["start"].Value, monsoonMatch.Groups["end"].Value));
         }
         var region = new Region(regionName, areas, monsoons)
         {
            Color = Globals.ColorProvider.GetRandomColor()
         };
         region.CalculateBounds();
         regionDictionary.Add(regionName, region);

         foreach (var area in areas)
         {
            if (Globals.Areas.TryGetValue(area, out var ar))
               ar.Region = regionName;
         }

      }
      Globals.Regions = regionDictionary;
   }

   [GeneratedRegex(@"monsoon\s*=\s*{\s*(?<start>[0-9.]+)\s*(?<end>[0-9.]+)\s*}")]
   private static partial Regex MonsoonRegex();
}