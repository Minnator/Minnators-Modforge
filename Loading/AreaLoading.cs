using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Loading;

public static class AreaLoading
{
   public static void LoadNew()
   {
      var sw = Stopwatch.StartNew();
      if (!FilesHelper.GetModOrVanillaPath(out var path, "map", "area.txt"))
      {
         Globals.ErrorLog.Write("Error: area.txt not found!");
         return;
      }
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
            for (var i = 0; i < contentElements.Count; i++) 
               ids.AddRange(Parsing.GetIntListFromString(contentElements[i].Value));

         areaDictionary.Add(areaName, new (areaName, [.. ids], Globals.MapWindow.Project.ColorProvider.GetRandomColor()));

         foreach (var provinceId in areaDictionary[areaName].Provinces)
            if (Globals.Provinces.TryGetValue(provinceId, out var province)) province.Area = areaName;
      }

      Globals.Areas = areaDictionary;

      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Parsing Areas", sw.ElapsedMilliseconds);
   }
}