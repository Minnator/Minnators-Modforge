﻿using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;
using Editor.Saving;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading;


public static class AreaLoading
{
   public static void Load()
   {
      if (!FilesHelper.GetModOrVanillaPath(out var path, out var isModPath, "map", "area.txt"))
      {
         Globals.ErrorLog.Write("Error: area.txt not found!");
         return;
      }
      IO.ReadAllInANSI(path, out var newContent);

      var pathObj = PathObj.FromPath(path, isModPath);
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

         Area newArea = new(areaName, Globals.ColorProvider.GetRandomColor(), ref pathObj, provinces);
         newArea.SetBounds();
         if (!areaDictionary.TryAdd(areaName, newArea))
            Globals.ErrorLog.Write($"Error: Area {areaName} already exists!");
      }
      SaveMaster.AddRangeToDictionary(pathObj, areaDictionary.Values);

      Globals.Areas = areaDictionary;
   }
}