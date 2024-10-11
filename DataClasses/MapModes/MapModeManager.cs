using System.Diagnostics;
using System.Drawing.Imaging;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.MapModes;

namespace Editor.DataClasses.MapModes;

public class MapModeManager()
{
   private List<MapMode> MapModes { get; } = [];
   public MapMode CurrentMapMode { get; set; } = null!;
   private ProvinceIdMapMode IdMapMode { get; set; } = null!;
   public bool PreviousLandOnly { get; set; }
   public bool RequireFullRedraw { get; set; } = true;

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
      MapModes.Add(new TradeCompanyMapMode());
      MapModes.Add(new ColonialRegionsMapMode());
      MapModes.Add(new ReligionMapMode());
      MapModes.Add(new DevastationMapMode());
      MapModes.Add(new ProsperityMapMode());
      MapModes.Add(new HreMapMode());
      MapModes.Add(new ParliamentSeatMapMode());
      MapModes.Add(new CityMapMode());
      MapModes.Add(new HasCapitalMapMode());
      MapModes.Add(new DiplomaticMapMode());



      // We set the default map mode to retrieve province colors

      IdMapMode = new ()
      {
         Bitmap = new(Globals.MapPath)
      };
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
      return MapModes.Find(mode => mode.GetMapModeName() == name) ?? IdMapMode;
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
      // CAN be null
      if (CurrentMapMode?.GetMapModeName() == name) 
         return; // no need to change map mode if it is already the same
      CurrentMapMode = GetMapMode(name); 
      CurrentMapMode.RenderMapMode(CurrentMapMode.GetProvinceColor);
      GC.Collect(); // We need to collect the garbage to free up memory but this is not ideal
      Globals.MapWindow.MapModeComboBox.SelectedItem = name;
   }

   public bool GetProvince(Point point, out Province province)
   {
      province = null!;
      if (Globals.ColorToProvId.TryGetValue (IdMapMode.Bitmap.GetPixel(point.X, point.Y), out var provinceId))
      {
         if (Globals.Provinces.TryGetValue(provinceId, out province))
            return true;
      }
      return false;
   }

   public void DrawProvinceCollectionFromCurrentMapMode(Bitmap bmp, params int[] ids)
   {
      foreach (var id in ids)
         MapDrawHelper.DrawProvince(id, Color.FromArgb(CurrentMapMode.GetProvinceColor(id)), bmp);
   }
}