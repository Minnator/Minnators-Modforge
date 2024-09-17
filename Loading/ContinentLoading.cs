using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Loading;

public static class ContinentLoading
{
   private static readonly string _pattern = @"(?<name>[A-Za-z_]*)\s*=\s*{(?<ids>(?:\s*[0-9]+\s*)*)}";

   public static void Load()
   {
      var sw = Stopwatch.StartNew();
      var path = FilesHelper.GetModOrVanillaPath("map", "continent.txt");
      var newContent = IO.ReadAllLinesInUTF8(path);

      var continentDictionary = new Dictionary<string, Continent>();
      var sb = new StringBuilder();

      foreach (var line in newContent)
         sb.Append(Parsing.RemoveCommentFromLine(line));

      var matches = Regex.Matches(sb.ToString(), _pattern, RegexOptions.Multiline);
      foreach (Match match in matches)
      {
         var name = match.Groups["name"].Value;
         var provinces = Parsing.GetIntListFromString(match.ToString());
         continentDictionary.Add(name, new (name, provinces){Color = Globals.MapWindow.Project.ColorProvider.GetRandomColor()});

         foreach (var provinceId in continentDictionary[name].Provinces)
            if (Globals.Provinces.TryGetValue(provinceId, out var province))
               province.Continent = name;
      }

      Globals.Continents = continentDictionary;

      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Parsing Continents", sw.ElapsedMilliseconds);
   }
}