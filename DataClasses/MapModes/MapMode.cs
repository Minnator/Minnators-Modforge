using System.Diagnostics;
using Editor.Helper;

namespace Editor.DataClasses.MapModes;

public abstract class MapMode
{
   public Bitmap Bitmap { get; set; } = null!;
   public virtual bool IsLandOnly => false;
   public virtual bool ShowOccupation => false;

   public virtual void RenderMapMode(Func<int, Color> method)
   {
      var sw = Stopwatch.StartNew();
      Globals.MapWindow.MapPictureBox.IsPainting = true;
      switch (Globals.MapModeRendering)
      {
         case MapModeRendering.LiveBackground:
         case MapModeRendering.Live:
            Bitmap?.Dispose();

            if (IsLandOnly)
            {
               if (Globals.MapModeManager.PreviousLandOnly)
               {
                  // only update the land provinces
                  BitMapHelper.ModifyByProvinceCollection(Globals.MapModeManager.ShareLiveBitmap, Globals.LandProvinceIds, GetProvinceColor);
               }
               else
               {
                  // update all provinces but separately for land and sea to use different method which allows for more optimized province color methods
                  BitMapHelper.ModifyByProvinceCollection(Globals.MapModeManager.ShareLiveBitmap, Globals.LandProvinceIds, GetProvinceColor);
                  BitMapHelper.ModifyByProvinceCollection(Globals.MapModeManager.ShareLiveBitmap, Globals.NonLandProvinceIds, GetSeaProvinceColor);
               }
            }
            else
            {
               Globals.MapModeManager.ShareLiveBitmap = BitMapHelper.GenerateBitmapFromProvinces(method);
            }
            // draw borders on top of the provinces is always needed
            if (ShowOccupation)
               MapDrawHelper.DrawOccupations(false, Globals.MapModeManager.ShareLiveBitmap);
            if (Globals.Settings.MapModeSettings.ShowCountryCapitals)
               MapDrawHelper.DrawCapitals(Globals.MapModeManager.ShareLiveBitmap);
            MapDrawHelper.DrawAllProvinceBorders(Globals.MapModeManager.ShareLiveBitmap, Color.Black);
            Globals.MapModeManager.PictureBox.Image = Globals.MapModeManager.ShareLiveBitmap;
            break;
         case MapModeRendering.Cached:
            Bitmap?.Dispose();
            Bitmap = BitMapHelper.GenerateBitmapFromProvinces(GetProvinceColor);
            if (ShowOccupation)
               MapDrawHelper.DrawOccupations(false, Bitmap);
            if (Globals.Settings.MapModeSettings.ShowCountryCapitals)
               MapDrawHelper.DrawCapitals(Bitmap);
            MapDrawHelper.DrawAllProvinceBorders(Bitmap, Color.Black);
            Globals.MapModeManager.PictureBox.Image = Bitmap;
            break;
         default:
            throw new ("Unknown Rendering mode for MapModes");
      }
      Globals.MapWindow.MapPictureBox.IsPainting = false;
      Globals.MapModeManager.PictureBox.Invalidate();
      Globals.MapModeManager.PreviousLandOnly = IsLandOnly;

      sw.Stop();
      Debug.WriteLine($"RenderMapMode {GetMapModeName()} took {sw.ElapsedMilliseconds}ms");
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
      if (Globals.MapModeManager.CurrentMapMode != this)
         return;
      if (sender is not int id || (IsLandOnly && !Globals.LandProvinces.Contains(id)))
         return;
      Update(id);
   }

   public virtual void Update(int id)
   {
      Globals.MapWindow.MapPictureBox.IsPainting = true;
      switch (Globals.MapModeRendering)
      {
         case MapModeRendering.Cached:
            MapDrawHelper.DrawProvince(id, GetProvinceColor(id), Bitmap);
            if (ShowOccupation)
               MapDrawHelper.DrawOccupations(false, Bitmap);
            Globals.MapModeManager.PictureBox.Invalidate(MapDrawHelper.DrawProvinceBorder(id, Color.Black, Bitmap));
            break;
         case MapModeRendering.Live:
         case MapModeRendering.LiveBackground:
            MapDrawHelper.DrawProvince(id, GetProvinceColor(id), Globals.MapModeManager.ShareLiveBitmap);
            if (ShowOccupation)
               MapDrawHelper.DrawOccupations(false, Globals.MapModeManager.ShareLiveBitmap);
            Globals.MapModeManager.PictureBox.Invalidate(MapDrawHelper.DrawProvinceBorder(id, Color.Black, Globals.MapModeManager.ShareLiveBitmap));
            break;
      }
      Globals.MapWindow.MapPictureBox.IsPainting = false;
   }

   public virtual string GetSpecificToolTip(int provinceId)
   {
      var str = string.Empty;
      str = GetMapModeName();
      return str;
   }
}