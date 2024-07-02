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
      switch (Globals.MapModeRendering)
      {
         case MapModeRendering.LiveBackground:
         case MapModeRendering.Live:
            Bitmap?.Dispose();
            Globals.MapModeManager.ShareLiveBitmap = BitMapHelper.GenerateBitmapFromProvinces(method);
            MapDrawHelper.DrawAllProvinceBorders(Globals.MapModeManager.ShareLiveBitmap, Color.Black);
            break;
         case MapModeRendering.Cached:
            Bitmap?.Dispose();
            Bitmap = BitMapHelper.GenerateBitmapFromProvinces(GetProvinceColor);
            MapDrawHelper.DrawAllProvinceBorders(Bitmap, Color.Black);
            break;
         default:
            throw new ArgumentOutOfRangeException();
      }
      Globals.MapModeManager.PictureBox.Image = Globals.MapModeManager.ShareLiveBitmap;
      Globals.MapModeManager.PictureBox.Invalidate();
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
      switch (Globals.MapModeRendering)
      {
         case MapModeRendering.Cached:
            MapDrawHelper.DrawProvince(id, GetProvinceColor(id), Bitmap);
            Globals.MapModeManager.PictureBox.Invalidate(MapDrawHelper.DrawProvinceBorder(id, Color.Black, Bitmap));
            break;
         case MapModeRendering.Live:
            MapDrawHelper.DrawProvince(id, GetProvinceColor(id), Globals.MapModeManager.ShareLiveBitmap);
            Globals.MapModeManager.PictureBox.Invalidate(MapDrawHelper.DrawProvinceBorder(id, Color.Black, Globals.MapModeManager.ShareLiveBitmap));
            break;
      }
   }
}