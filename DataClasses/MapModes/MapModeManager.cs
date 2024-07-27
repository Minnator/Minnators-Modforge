using System.Drawing.Imaging;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.MapModes;

namespace Editor.DataClasses.MapModes;

public class MapModeManager(PannablePictureBox pictureBox)
{
   private List<MapMode> MapModes { get; } = [];
   public MapMode CurrentMapMode { get; set; } = null!;
   private ProvinceIdMapMode IdMapMode { get; set; } = null!;
   public PannablePictureBox PictureBox { get; set; } = pictureBox;
   public bool PreviousLandOnly { get; set; }
   public bool RequireFullRedraw { get; set; } = true;
   public Bitmap ShareLiveBitmap { get; set; } = new(Globals.MapWidth, Globals.MapHeight, PixelFormat.Format24bppRgb);

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
      MapModes.Add(new TradeGoodsMapMode());
      MapModes.Add(new TradeNodeMapMode());
      MapModes.Add(new ReligionMapMode());
      MapModes.Add(new DevastationMapMode());
      MapModes.Add(new ProsperityMapMode());



      // We set the default map mode to retrieve province colors

      IdMapMode = new ();

      var modPath = Path.Combine(Globals.MapWindow.Project.ModPath, "map", "provinces.bmp");
      var vanillaPath = Path.Combine(Globals.MapWindow.Project.VanillaPath, "map", "provinces.bmp");
      if (File.Exists(modPath))
         IdMapMode.Bitmap = new(modPath);
      else if (File.Exists(vanillaPath))
         IdMapMode.Bitmap = new(vanillaPath);
      else
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
      return MapModes.Find(mode => mode.GetMapModeName() == name) ?? throw new InvalidOperationException($"Can not find mapmode {name}");
   }

   public List<string> GetMapModeNames()
   {
      var names = new List<string>();
      foreach (var mode in MapModes) 
         names.Add(mode.GetMapModeName());
      return names;
   }

   public void SetCurrentMapMode(string name)
   {
      if (CurrentMapMode?.GetMapModeName() == name) 
         return; // no need to change map mode if it is already the same
      CurrentMapMode = GetMapMode(name); 
      CurrentMapMode.RenderMapMode(CurrentMapMode.GetProvinceColor);
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