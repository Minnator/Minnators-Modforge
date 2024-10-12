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

   public virtual void RenderMapMode(Func<Province, int> method)
   {
      // TODO optimize this method
      var sw = Stopwatch.StartNew();
      if (IsLandOnly)
      {
         if (Globals.MapModeManager.PreviousLandOnly)
         {
            // only update the land provinces
            MapDrawing.DrawOnMap(Globals.LandProvinces, GetProvinceColor, PixelsOrBorders.Pixels);
         }
         else
         {
            MapDrawing.DrawOnMap(Globals.LandProvinces, GetProvinceColor, PixelsOrBorders.Pixels);
            MapDrawing.DrawOnMap(Globals.NonLandProvinces, GetSeaProvinceColor, PixelsOrBorders.Pixels);
         }
      }
      else
      {
         MapDrawing.DrawOnMap(Globals.Provinces, method, PixelsOrBorders.Pixels);
      }
      // draw borders on top of the provinces is always needed
      if (ShowOccupation)
         MapDrawing.DrawOccupations(false);
      if (Globals.Settings.MapModeSettings.ShowCountryCapitals || ShowCapitals)
         MapDrawing.DrawAllCapitals();
      MapDrawing.DrawAllBorders(Color.Black.ToArgb());
      Selection.RePaintSelection();
      Globals.ZoomControl.Invalidate();
      Globals.MapModeManager.PreviousLandOnly = IsLandOnly;

      sw.Stop();
      Debug.WriteLine($"RenderMapMode {GetMapModeName()} took {sw.ElapsedMilliseconds}ms");
   }

   public virtual string GetMapModeName()
   {
      return "REPLACE_ME";
   }

   public virtual int GetProvinceColor(Province id)
   {
      return id.Color.ToArgb();
   }

   public virtual int GetSeaProvinceColor(Province id)
   {
      return id.Color.ToArgb();
   }

   public virtual void Update(Rectangle rect)
   {
      Update(Geometry.GetProvincesInRectangle(rect));
   }

   public virtual void Update(List<Province> ids)
   {
      if (ids.Count == 0)
         return;
      BlockDrawingOfCapitals = true; // Don't draw the capitals per province, they are more optimized to draw all at once

      foreach (var id in ids)
      {
         Update(id, false);
         MapDrawing.DrawOnMap(id, Selection._borderColor, PixelsOrBorders.Borders);
      }
      BlockDrawingOfCapitals = false;

      // Drawing all capitals at once is more optimized
      if (Globals.Settings.MapModeSettings.ShowCountryCapitals || ShowCapitals)
         MapDrawing.DrawCapitals(ids);
      Globals.ZoomControl.Invalidate();
   }

   public virtual void UpdateProvince(object sender, ProvinceEventHandler.ProvinceDataChangedEventArgs e)
   {
      if (Globals.MapModeManager.CurrentMapMode != this)
         return;
      if (sender is not Province id || (IsLandOnly && !Globals.LandProvinces.Contains(id)))
         return;
      Update(id);
   }

   public virtual void UpdateProvinceCollection(object? sender, Events.ProvinceCollectionEventArgs e)
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

   public virtual void Update(Province province, bool invalidate = true)
   {
      MapDrawing.DrawOnMap(province, GetProvinceColor(province), PixelsOrBorders.Pixels);
      if (ShowOccupation)
         MapDrawing.DrawOccupations(false);
      if (Globals.Settings.MapModeSettings.ShowCountryCapitals || ShowCapitals)
         MapDrawing.DrawAllCapitals();
      if (invalidate)
      {
         MapDrawing.DrawOnMap(province, Selection._borderColor, PixelsOrBorders.Borders);
         Globals.ZoomControl.Invalidate();
      }
      //TODO fix false border drawing
   }

   public virtual string GetSpecificToolTip(Province provinceId)
   {
      return GetMapModeName();
   }
}