using System;
using System.Collections.Generic;
using System.Drawing;
using Editor.Helper;
using Editor.Loading;

namespace Editor.MapModes;

public class RegionsMapMode : IMapMode
{
   public Bitmap Bitmap { get; set; } = null!;

   public RegionsMapMode()
   {
      RenderMapMode(GetProvinceColor);
   }

   public void RenderMapMode(Func<int, Color> method)
   {
      Bitmap?.Dispose();
      Bitmap = BitMapHelper.GenerateBitmapFromProvinces(GetProvinceColor);
      MapDrawHelper.DrawAllProvinceBorders(Bitmap, Color.Black);
   }

   public string GetMapModeName()
   {
      return "Regions";
   }

   public Color GetProvinceColor(int id)
   {
      if (Globals.Provinces.TryGetValue(id, out var province))
         if (Globals.Areas.TryGetValue(province.Area, out var areas))
            if (Globals.Regions.TryGetValue(areas.Region, out var region))
               return region.Color;
      return Color.DarkGray;
   }

   public void Update(Rectangle rect)
   {
      Update(Geometry.GetProvinceIdsInRectangle(rect));
   }

   public void Update(List<int> ids)
   {
      foreach (var id in ids)
         Update(id);
   }

   public void Update(int id)
   {
      MapDrawHelper.DrawProvince(id, GetProvinceColor(id), Bitmap);
      MapDrawHelper.DrawProvinceBorder(id, Color.Black, Bitmap);
   }
}