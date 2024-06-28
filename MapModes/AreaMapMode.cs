using System;
using System.Collections.Generic;
using System.Drawing;
using Editor.Helper;
using Editor.Loading;

namespace Editor.MapModes;

public class AreaMapMode : IMapMode
{
   public Bitmap Bitmap { get; set; } = null!;

   public AreaMapMode()
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
      return "Areas";
   }

   Color IMapMode.GetProvinceColor(int id)
   {
      return GetProvinceColor(id);
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

   // Update the province with the given id
   public void Update(int id)
   {
      MapDrawHelper.DrawProvince( id, GetProvinceColor(id), Bitmap);
      // Draw the border of the province as it is overwritten by the province pixels
      MapDrawHelper.DrawProvinceBorder(id, Color.Black, Bitmap);
   }

   private Color GetProvinceColor(int provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         if (Globals.Areas.TryGetValue(province.Area, out var area))
            return area.Color;
      return Color.DarkGray;
   }
}