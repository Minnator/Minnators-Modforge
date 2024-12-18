using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.MapModes;

public enum MapModeType
{
   None = 0,
   Area,
   Autonomy,
   CenterOfTrade,
   City,
   ColonialRegions,
   Continent,
   Country,
   Culture,
   CultureGroup,
   Development,
   Devastation,
   Diplomatic,
   Fort,
   HasCapital,
   Hre,
   ParliamentSeat,
   Prosperity,
   Province,
   ProvinceGroup,
   Regions,
   Religion,
   SuperRegion,
   Terrain,
   TerrainOverrides,
   TradeCompany,
   TradeGoods,
   TradeNode,
   GameOfLive,
   RGB,
   BadApple
}


public static class MapModeManager
{
   private static int _totalMapModeTime = 0;
   public static int MinMapModeTime = int.MaxValue;
   public static int MaxMapModeTime = int.MinValue;
   private static readonly List<int> MapModeTimes = new (100);
   private static readonly Stopwatch Stopwatch = new();
   public static int AverageMapModeTime => MapModeTimes.Count == 0 ? 0 : _totalMapModeTime / MapModeTimes.Count;
   public static int LasMapModeTime => MapModeTimes.Count == 0 ? 0 : MapModeTimes[^1];


   private static List<MapMode> MapModes { get; } = [];
   public static MapMode CurrentMapMode { get; set; } = null!;
   public static ProvinceIdMapMode IdMapMode { get; set; } = null!;
   public static bool PreviousLandOnly { get; set; }

   public static EventHandler<MapMode> MapModeChanged = delegate { };

   static MapModeManager()
   {
      InitializeAllMapModes();
   }

   public static void InitializeAllMapModes()
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
      MapModes.Add(new TerrainOverrides());
      MapModes.Add(new TerrainMapMode());
      MapModes.Add(new GameOfLiveMapMode());
      MapModes.Add(new RGBMapMode());
      //MapModes.Add(new MemeMode());

      IdMapMode = new ()
      {
         Bitmap = new(Globals.MapPath)
      };
   }
   

   public static void RenderCurrent()
   {
      CurrentMapMode.RenderMapMode(CurrentMapMode.GetProvinceColor);
   }

   public static MapMode GetMapMode(MapModeType type)
   {
      return MapModes.Find(mode => mode.MapModeType == type) ?? IdMapMode;
   }
   
   public static void SetCurrentMapMode(MapModeType type)
   {
      try
      {
         // This is null only on the first run when loading
         if (CurrentMapMode?.MapModeType == type)
            return;
         CurrentMapMode?.SetInactive();
         CurrentMapMode = GetMapMode(type);
         CurrentMapMode.SetActive();
         Stopwatch.Restart();
         RenderCurrent();
         Stopwatch.Stop();
         Globals.MapWindow.MapModeComboBox.SelectedItem = type.ToString();

         #region Metrics

         if (MapModeTimes.Count == 100)
         {
            _totalMapModeTime -= MapModeTimes[0];
            MapModeTimes.RemoveAt(0);
         }

         _totalMapModeTime += (int)Stopwatch.ElapsedMilliseconds;
         MapModeTimes.Add((int)Stopwatch.ElapsedMilliseconds);
         if (Stopwatch.ElapsedMilliseconds < MinMapModeTime)
            MinMapModeTime = (int)Stopwatch.ElapsedMilliseconds;
         if (Stopwatch.ElapsedMilliseconds > MaxMapModeTime)
            MaxMapModeTime = (int)Stopwatch.ElapsedMilliseconds;

         #endregion

         MapModeChanged(null, CurrentMapMode);
      }
      catch (Exception)
      {
         MessageBox.Show("Mapmode rendering failed.\nTry again and if the issue persists contact a developer",
            "Error in Rendering!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
         SetCurrentMapMode(MapModeType.Province);
      }
   }

   public static int GetMapModeColor(Province p)
   {
      return CurrentMapMode.GetProvinceColor(p);
   }

   public static int GetColorForMapMode(Province p, MapModeType mapMode)
   {
      var mpMode = GetMapMode(mapMode);
      if (mpMode.IsLandOnly && Globals.LandProvinces.Contains(p))
         return mpMode.GetProvinceColor(p);
      return mpMode.GetSeaProvinceColor(p);
   }

}