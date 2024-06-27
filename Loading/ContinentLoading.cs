using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Editor.DataClasses;
using Editor.Helper;

namespace Editor.Loading;

public static class ContinentLoading
{
   private static readonly string _pattern = @"(?<name>[A-Za-z_]*)\s*=\s*{(?<ids>(?:\s*[0-9]+\s*)*)}";

   public static void Load(string folder, ref Log log)
   {
      var sw = Stopwatch.StartNew();
      var path = Path.Combine(folder, "map", "continent.txt");
      var newContent = IO.ReadAllLinesInUTF8(path);

      var continentDictionary = new Dictionary<string, Continent>();
      var sb = new StringBuilder();

      foreach (var line in newContent)
         sb.Append(Parsing.RemoveCommentFromLine(line));

      var matches = Regex.Matches(sb.ToString(), _pattern, RegexOptions.Multiline);
      foreach (Match match in matches)
      {
         var name = match.Groups["name"].Value;
         var provinces = Parsing.GetProvincesList(match.ToString());
         continentDictionary.Add(name, new Continent(name, provinces));

         foreach (var provinceId in continentDictionary[name].Provinces)
            if (Globals.Provinces.TryGetValue(provinceId, out var province))
               province.Continent = name;
      }

      Globals.Continents = continentDictionary;

      sw.Stop();
      log.WriteTimeStamp("Parsing Continents", sw.ElapsedMilliseconds);
   }
}