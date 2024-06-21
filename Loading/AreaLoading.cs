using Editor.DataClasses;
using Editor.Helper;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Editor.Loading;

public static class AreaLoading
{
   public static void Load(string folder, ref Log log)
   {
      var sw = Stopwatch.StartNew();
      var path = Path.Combine(folder, "map", "area.txt");
      var newContent = IO.ReadAllLinesInUTF8(path);

      var areaDictionary = new Dictionary<string, Area>();

      // Define the regex pattern to match area definitions
      var provinceRegex = new Regex(@"^(?<name>[A-Za-z_]*)\s*=\s*{(?:\s*(?!#|color)[A-Za-z_]+\s*)+}", RegexOptions.Multiline);

      foreach (var line in newContent)
      {
         // Skip empty lines and lines starting with '#' or containing 'color'
         if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.Contains("color"))
            continue;

         var match = provinceRegex.Match(line);
         if (!match.Success)
            continue;

         var areaName = match.Groups["name"].Value;

         areaDictionary.Add(areaName, new Area(areaName, [.. Parsing.GetProvincesList(match.Value)]));

         foreach (var provinceId in areaDictionary[areaName].Provinces)
            if (Data.Provinces.TryGetValue(provinceId, out var province)) 
               province.Area = areaName;
      }

      Data.Areas = areaDictionary;

      sw.Stop();
      log.WriteTimeStamp("Parsing Areas", sw.ElapsedMilliseconds);
   }

}