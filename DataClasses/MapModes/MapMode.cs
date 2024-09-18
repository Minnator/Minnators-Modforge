using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Helper;

namespace Editor.DataClasses.MapModes;

public abstract class MapMode
{
   public Bitmap Bitmap { get; set; } = null!;
   public virtual bool IsLandOnly => false;
   public virtual bool ShowOccupation => false;
   public virtual bool IsProvinceMapMode => true;
   public virtual bool ShowCapitals => false;
   public virtual bool BlockDrawingOfCapitals { get; set; } = false;

   public virtual void RenderMapMode(Func<int, Color> method)
   {
      var sw = Stopwatch.StartNew();
      Globals.MapWindow.MapPictureBox.IsPainting = true;
      switch (Globals.MapModeRendering)
      {
         case MapModeRendering.LiveBackground:
         case MapModeRendering.Live:
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
            if (Globals.Settings.MapModeSettings.ShowCountryCapitals || ShowCapitals)
               MapDrawHelper.DrawAllCapitals(Globals.MapModeManager.ShareLiveBitmap);
            MapDrawHelper.DrawAllProvinceBorders(Globals.MapModeManager.ShareLiveBitmap, Color.Black);
            Globals.MapModeManager.PictureBox.Image = Globals.MapModeManager.ShareLiveBitmap;
            break;
         case MapModeRendering.Cached:
            Bitmap = BitMapHelper.GenerateBitmapFromProvinces(GetProvinceColor);
            if (ShowOccupation)
               MapDrawHelper.DrawOccupations(false, Bitmap);
            if (Globals.Settings.MapModeSettings.ShowCountryCapitals)
               MapDrawHelper.DrawAllCapitals(Bitmap);
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
      if (ids.Count == 0)
         return;
      List<Rectangle> rects = [];
      BlockDrawingOfCapitals = true; // Don't draw the capitals per province, they are more optimized to draw all at once

      foreach (var id in ids)
      {
         Update(id, false);
         switch (Globals.MapModeRendering)
         {
            case MapModeRendering.Live:
            case MapModeRendering.LiveBackground:
               rects.Add(MapDrawHelper.DrawProvinceBorder(id, Color.Black, Globals.MapModeManager.ShareLiveBitmap));
               break;
            case MapModeRendering.Cached:
               rects.Add(MapDrawHelper.DrawProvinceBorder(id, Color.Black, Bitmap));
               break;
         }
      }
      BlockDrawingOfCapitals = false;

      // Drawing all capitals at once is more optimized
      switch (Globals.MapModeRendering)
      {
         case MapModeRendering.Live:
         case MapModeRendering.LiveBackground:
            if (Globals.Settings.MapModeSettings.ShowCountryCapitals || ShowCapitals)
               MapDrawHelper.RedrawCapitals(Globals.MapModeManager.ShareLiveBitmap, ids);
            break;
         case MapModeRendering.Cached:
            break;
      }
      var totalRect = Geometry.GetBounds(rects);
      Globals.MapModeManager.PictureBox.Invalidate(totalRect);
   }

   public virtual void UpdateProvince(object sender, ProvinceEventHandler.ProvinceDataChangedEventArgs e)
   {
      if (Globals.MapModeManager.CurrentMapMode != this)
         return;
      if (sender is not int id || (IsLandOnly && !Globals.LandProvinces.Contains(id)))
         return;
      Update(id);
   }

   public virtual void UpdateProvinceCollection(object? sender, ProvinceCollectionEvents.ProvinceGroupEventArgs e)
   {
      if (Globals.MapModeManager.CurrentMapMode != this)
         return;
      foreach (var id in e.Ids)
      {
         if (IsLandOnly && !Globals.LandProvinces.Contains(id))
            continue;
         Update(id);
      }
   }

   public virtual void Update(int id, bool invalidate = true)
   {
      Globals.MapWindow.MapPictureBox.IsPainting = true;
      switch (Globals.MapModeRendering)
      {
         case MapModeRendering.Cached:
            MapDrawHelper.DrawProvince(id, GetProvinceColor(id), Bitmap);
            if (ShowOccupation)
               MapDrawHelper.DrawOccupations(false, Bitmap);
            if (invalidate)
               Globals.MapModeManager.PictureBox.Invalidate(MapDrawHelper.DrawProvinceBorder(id, Color.Black, Bitmap));
            break;
         case MapModeRendering.Live:
         case MapModeRendering.LiveBackground:
            MapDrawHelper.DrawProvince(id, GetProvinceColor(id), Globals.MapModeManager.ShareLiveBitmap);
            if (ShowOccupation)
               MapDrawHelper.DrawOccupations(false, Globals.MapModeManager.ShareLiveBitmap);
            if (!BlockDrawingOfCapitals)
               if (Globals.Settings.MapModeSettings.ShowCountryCapitals || ShowCapitals)
                  MapDrawHelper.RedrawCapitals(Globals.MapModeManager.ShareLiveBitmap, [id]);
            if (invalidate)
               Globals.MapModeManager.PictureBox.Invalidate(MapDrawHelper.DrawProvinceBorder(id, Color.Black, Globals.MapModeManager.ShareLiveBitmap));
            //TODO fix false border drawing
            break;
      }
      Globals.MapWindow.MapPictureBox.IsPainting = false;
   }

   public virtual string GetSpecificToolTip(int provinceId)
   {
      return GetMapModeName();
   }
}