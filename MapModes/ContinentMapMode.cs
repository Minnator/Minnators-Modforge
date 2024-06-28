using System;
using System.Collections.Generic;
using System.Drawing;
using Editor.Helper;
using Editor.Loading;

namespace Editor.MapModes;

public class ContinentMapMode : IMapMode
{
   public Bitmap Bitmap { get; set; }
   public void RenderMapMode(Func<int, Color> method)
   {
      Bitmap?.Dispose();
      Bitmap = BitMapHelper.GenerateBitmapFromProvinces(GetProvinceColor);
      MapDrawHelper.DrawAllProvinceBorders(Bitmap, Color.Black);
   }

   public string GetMapModeName()
   {
      return "Continents";
   }

   public Color GetProvinceColor(int id)
   {
      if (Globals.Provinces.TryGetValue(id, out var province))
         if (Globals.Continents.TryGetValue(province.Continent, out var continent))
            return continent.Color;
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