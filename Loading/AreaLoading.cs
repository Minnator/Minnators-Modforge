using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Loading;

public static class AreaLoading
{
   public static void Load(ColorProviderRgb provider)
   {
      var sw = Stopwatch.StartNew();
      var path = FilesHelper.GetModOrVanillaPath("map", "area.txt");
      var newContent = IO.ReadAllLinesInUTF8(path);

      var areaDictionary = new Dictionary<string, Area>();

      // Define the regex pattern to match area definitions
      var areaRegex = new Regex(@"(?<name>[A-Za-z_]*)\s*=\s*{(?<ids>[0-9\s]*)}\s*", RegexOptions.Multiline);
      var sb = new System.Text.StringBuilder();

      foreach (var line in newContent)
      {
         sb.Append(Parsing.RemoveCommentFromLine(line));
      }

      foreach (Match match in areaRegex.Matches(Regex.Replace(sb.ToString(), @"(?<color>color\s*=\s*{\s*(?:[0-9]{1,3}\s*){3}})", "")))
      {
         var areaName = match.Groups["name"].Value;

         areaDictionary.Add(areaName, new Area(areaName, [.. Parsing.GetIntListFromString(match.Groups["ids"].Value)], provider.GetRandomColor()));

         foreach (var provinceId in areaDictionary[areaName].Provinces)
            if (Globals.Provinces.TryGetValue(provinceId, out var province)) province.Area = areaName;
      }
      Globals.Areas = areaDictionary;

      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Parsing Areas", sw.ElapsedMilliseconds);
   }

}