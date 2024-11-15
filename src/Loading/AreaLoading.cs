using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;
using Editor.Saving;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading;

[Loading]
public static class AreaLoading
{
   public static void Load()
   {
      var sw = Stopwatch.StartNew();
      if (!FilesHelper.GetModOrVanillaPath(out var path, out var isModPath, "map", "area.txt"))
      {
         Globals.ErrorLog.Write("Error: area.txt not found!");
         return;
      }
      IO.ReadAllInANSI(path, out var newContent);

      var pathObj = NewPathObj.FromPath(path, isModPath);
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

         List<Province> provinces = [];
         foreach (var id in ids)
         {
            if (Globals.ProvinceIdToProvince.TryGetValue(id, out var province))
               provinces.Add(province);
         }

         Area newArea = new(areaName, provinces, Globals.ColorProvider.GetRandomColor());
         newArea.SetPath(ref pathObj);
         areaDictionary.Add(areaName, newArea);
      }
      SaveMaster.AddRangeToDictionary(pathObj, areaDictionary.Values);

      Globals.Areas = areaDictionary;

      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Parsing Areas", sw.ElapsedMilliseconds);
   }
}