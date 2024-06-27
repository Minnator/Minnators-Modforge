using System;
using System.Drawing;
using Editor.Helper;
using Editor.Loading;

namespace Editor.MapModes;

public class ProvinceMapMode : IMapMode
{
   public Bitmap Bitmap { get; set; } = null!;

   public ProvinceMapMode()
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
      return "Provinces";
   }

   private static Color GetProvinceColor(int provinceId)
   {
      return Globals.Provinces[provinceId].Color;
   }
}