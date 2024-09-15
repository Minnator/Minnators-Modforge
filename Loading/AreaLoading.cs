using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.DataClasses;
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

   public static void LoadNew(ColorProviderRgb provider)
   {
      var sw = Stopwatch.StartNew();
      var path = FilesHelper.GetModOrVanillaPath("map", "area.txt");
      IO.ReadAllInANSI(path, out var newContent);

      var areaDictionary = new Dictionary<string, Area>();

      Parsing.RemoveCommentFromMultilineString(ref newContent, out var content);

      var elements = Parsing.GetElements(0, ref content);

      foreach (var element in elements)
      {
         if (element is not Block block)
         {
            Globals.ErrorLog.Write($"Error in area.txt: Invalid content: {element}");
            continue;
         }

         var areaName = block.Name;
         var contentElements = block.GetContentElements;
         List<int> ids = [];
         if (contentElements.Count > 0)
         {
            for (var i = 0; i < contentElements.Count; i++) 
               ids.AddRange(Parsing.GetIntListFromString(contentElements[i].Value));
         }
         areaDictionary.Add(areaName, new Area(areaName, [.. ids], provider.GetRandomColor()));
      }

      Globals.Areas = areaDictionary;

      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Parsing Areas", sw.ElapsedMilliseconds);
   }
}