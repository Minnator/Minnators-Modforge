using System.Diagnostics;
using Editor.Helper;

namespace Editor.DataClasses.MapModes;

public abstract class MapMode
{
   public Bitmap Bitmap { get; set; } = null!;
   public virtual bool IsLandOnly => false;

   public virtual void RenderMapMode(Func<int, Color> method)
   {
      switch (Globals.MapModeRendering)
      {
         case MapModeRendering.LiveBackground:
         case MapModeRendering.Live:
            var sw = Stopwatch.StartNew();
            Bitmap?.Dispose();

            if (IsLandOnly)
            {
               if (Globals.MapModeManager.PreviousLandOnly)
               {
                  BitMapHelper.ModifyByProvinceCollection(Globals.MapModeManager.ShareLiveBitmap, Globals.LandProvinceIds, GetProvinceColor);
               }
               else
               {
                  BitMapHelper.ModifyByProvinceCollection(Globals.MapModeManager.ShareLiveBitmap, Globals.NonLandProvinceIds, GetSeaProvinceColor);
               }
            }
            else
            {
               Globals.MapModeManager.ShareLiveBitmap = BitMapHelper.GenerateBitmapFromProvinces(method);
               Globals.MapModeManager.RequireFullRedraw = false;
            }
            MapDrawHelper.DrawAllProvinceBorders(Globals.MapModeManager.ShareLiveBitmap, Color.Black);
            Globals.MapModeManager.PictureBox.Image = Globals.MapModeManager.ShareLiveBitmap;
            Globals.MapModeManager.PreviousLandOnly = IsLandOnly;
            sw.Stop();
            Debug.WriteLine($"RenderMapMode {GetMapModeName()} took {sw.ElapsedMilliseconds}ms");
            break;
         case MapModeRendering.Cached:
            Bitmap?.Dispose();
            Bitmap = BitMapHelper.GenerateBitmapFromProvinces(GetProvinceColor);
            MapDrawHelper.DrawAllProvinceBorders(Bitmap, Color.Black);
            Globals.MapModeManager.PictureBox.Image = Bitmap;
            break;
         default:
            throw new ArgumentOutOfRangeException();
      }
      Globals.MapModeManager.PictureBox.Invalidate();
   }

   public virtual string GetMapModeName()
   {
      return "REPLACE_ME";
   }

   public virtual Color GetProvinceColor(int id)
   {
      return Globals.Provinces[id].Color;
   }

   public virtual Color GetSeaProvinceColor(int id)
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

   public virtual void UpdateProvince(object sender, ProvinceEventHandler.ProvinceDataChangedEventArgs e)
   {
      if (sender is not int id)
         return;
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