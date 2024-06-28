using System;
using System.Collections.Generic;
using System.Drawing;
using Editor.Helper;

namespace Editor.Interfaces;

public abstract class MapMode
{
   public Bitmap Bitmap { get; set; } = null!;
   
   public virtual void RenderMapMode(Func<int, Color> method)
   {
      Bitmap?.Dispose();
      Bitmap = BitMapHelper.GenerateBitmapFromProvinces(GetProvinceColor);
      MapDrawHelper.DrawAllProvinceBorders(Bitmap, Color.Black);
   }

   public virtual string GetMapModeName()
   {
      return "MapMode";
   }

   public virtual Color GetProvinceColor(int id)
   {
      return Globals.Provinces[id].Color;
   }

   public virtual void Update(Rectangle rect)
   {
      Update(Geometry.GetProvinceIdsInRectangle(rect));
   }

   public virtual void Update(List<int> ids)
   {
      foreach (var id in ids)
         Update(id);
   }

   public virtual void Update(int id)
   {
      MapDrawHelper.DrawProvince(id, GetProvinceColor(id), Bitmap);
      MapDrawHelper.DrawProvinceBorder(id, Color.Black, Bitmap);
   }
}