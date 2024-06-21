using Editor.DataClasses;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Editor.Helper;

namespace Editor.Loading;

public static class AreaLoading
{
   public static void Load(string folder, ref Log log)
   {
      var sw = new Stopwatch();
      sw.Start();
      var path = Path.Combine(folder, "map", "area.txt");
      var newContent = IO.ReadAllLinesInUTF8(path);
      var stringBuilder = new StringBuilder();

      //Filtering Comments and optional that are not important to the areas themselves
      foreach (var line in newContent)
      {
         if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.Contains("color"))
            continue;
         stringBuilder.AppendLine(Parsing.RemoveCommentFromLine(line));
      }

      var content = stringBuilder.ToString();

      var areaDictionary = new Dictionary<string, Area>();
      var provinceRegex = new Regex(@"(?<name>[A-Za-z_]*)\s*=\s*{.*?(?<provinces>[^\}|^#]*)", RegexOptions.Singleline);
      var matches = provinceRegex.Matches(content);

      foreach (Match match in matches)
      {
         var area = new Area(match.Groups["name"].Value, Parsing.GetProvincesList(match.Groups["provinces"].Value).ToArray());
         areaDictionary.Add(area.Name, area);

         foreach (var provinceId in area.Provinces)
         {
            if (!Data.Provinces.TryGetValue(provinceId, out var province))
               continue;
            province.Area = area.Name;
         }
      }

      Data.Areas = areaDictionary;
      sw.Stop();
      log.WriteTimeStamp("Parsing provinces", sw.ElapsedMilliseconds);
   }
}