using System.Drawing.Imaging;
using Editor.Controls;
using Editor.DataClasses.MapModes;
using Editor.MapModes;

namespace Editor.DataClasses;

public class MapModeManager
{
   private List<MapMode> MapModes { get; } = [];
   public MapMode CurrentMapMode { get; set; } = null!;
   private ProvinceIdMapMode IdMapMode { get; set; } = null!;
   public PannablePictureBox PictureBox { get; set; }
   public bool PreviousLandOnly { get; set; }
   public bool RequireFullRedraw { get; set; } = true;
   public Bitmap ShareLiveBitmap { get; set; }

   public MapModeManager(PannablePictureBox pictureBox)
   {
      PictureBox = pictureBox;
      ShareLiveBitmap = new(Globals.MapWidth, Globals.MapHeight, PixelFormat.Format24bppRgb);
   }

   public void InitializeAllMapModes()
   {
      MapModes.Add(new ProvinceMapMode());
      MapModes.Add(new AreaMapMode());
      MapModes.Add(new RegionsMapMode());
      MapModes.Add(new SuperRegionMapMode());
      MapModes.Add(new ContinentMapMode());
      MapModes.Add(new DevelopmentMapMode());
      MapModes.Add(new CenterOfTradeMapMode());
      MapModes.Add(new AutonomyMapMode());
      MapModes.Add(new FortMapMode());
      MapModes.Add(new CultureGroupMapMode());
      MapModes.Add(new CultureMapMode());
      MapModes.Add(new CountryMapMode());


      // We set the default map mode to retrieve province colors

      IdMapMode = new ();
      IdMapMode.RenderMapMode(IdMapMode.GetProvinceColor); //TODO can be replaced by coping the bitmap from the modfolder if it exists

   }

   public void RenderCurrent()
   {
      CurrentMapMode.RenderMapMode(CurrentMapMode.GetProvinceColor);
   }
   public List<MapMode> GetMapModes()
   {
      return MapModes;
   }

   public MapMode GetMapMode(string name)
   {
      return MapModes.Find(mode => mode.GetMapModeName() == name);
   }

   public List<string> GetMapModeNames()
   {
      var names = new List<string>();
      foreach (var mode in MapModes)
      {
         names.Add(mode.GetMapModeName());
      }
      return names;
   }

   public void SetCurrentMapMode(string name)
   {
      CurrentMapMode = GetMapMode(name);
      if (Globals.MapModeRendering == MapModeRendering.Live)
      {
         CurrentMapMode.RenderMapMode(CurrentMapMode.GetProvinceColor);
      }
      else
         PictureBox.Image = CurrentMapMode.Bitmap; // We point the PictureBox to the new bitmap
      PictureBox.Invalidate(); // We force the PictureBox to redraw
   }

   public bool GetProvince(Point point, out Province province)
   {
      if (Globals.ColorToProvId.TryGetValue (IdMapMode.Bitmap.GetPixel(point.X, point.Y), out var provinceId))
      {
         province = Globals.Provinces[provinceId];
         return true;
      }
      province = null!;
      return false;
   }

}