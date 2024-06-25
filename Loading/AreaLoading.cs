using Editor.DataClasses;
using Editor.Helper;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
      var areaRegex = new Regex(@"(?<name>[A-Za-z_]*)\s*=\s*{(?<ids>[0-9\s]*)}\s*", RegexOptions.Multiline);
      var sb = new System.Text.StringBuilder();

      foreach (var line in newContent)
      {
         sb.Append(Parsing.RemoveCommentFromLine(line));
      }
      var areaString = sb.ToString();
      areaString = Regex.Replace(areaString, @"(?<color>color\s*=\s*{\s*(?:[0-9]{1,3}\s*){3}})", "");
      Clipboard.SetText(areaString);
      foreach (Match match in areaRegex.Matches(areaString))
      {
         var areaName = match.Groups["name"].Value;

         areaDictionary.Add(areaName, new Area(areaName, [.. Parsing.GetProvincesList(match.Groups["ids"].Value)]));

         foreach (var provinceId in areaDictionary[areaName].Provinces)
            if (Data.Provinces.TryGetValue(provinceId, out var province)) province.Area = areaName;
      }
      Data.Areas = areaDictionary;

      sw.Stop();
      log.WriteTimeStamp("Parsing Areas", sw.ElapsedMilliseconds);
   }

}