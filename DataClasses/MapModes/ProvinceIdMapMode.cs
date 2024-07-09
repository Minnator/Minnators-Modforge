using System;
using System.Drawing;
using Editor.DataClasses.MapModes;
using Editor.Helper;

namespace Editor.MapModes;

public sealed class ProvinceIdMapMode : MapMode
{
   public override void RenderMapMode(Func<int, Color> method)
   {
      Bitmap?.Dispose();
      Bitmap = BitMapHelper.GenerateBitmapFromProvinces(GetProvinceColor);
      MapDrawHelper.DrawAllProvinceBorders(Bitmap, Color.Black);
   }

   public override string GetMapModeName()
   {
      return "Province Id";
   }

   public override Color GetProvinceColor(int id)
   {
      return Globals.Provinces[id].Color;
   }
   
}