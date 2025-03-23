using System.Diagnostics;
using System.Reflection;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;

namespace Editor.DataClasses.MapModes;

public enum MapModeType
{
   None = 0,
#if DEBUG
   DEBUG = 999,
#endif
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
   BaseTax,
   BaseProduction,
   BaseManpower,
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
   Climate,
   Weather,
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
   public static Dictionary<Province, int> ColorCache = new(Globals.Provinces.Count);

   private static List<MapMode> MapModes { get; } = [];
   public static MapMode CurrentMapMode { get; set; } = null!;
   public static ProvinceIdMapMode IdMapMode { get; set; } = null!;
   public static bool PreviousLandOnly { get; set; }

   public static EventHandler<MapMode> MapModeChanged = delegate { };

   private static readonly Dictionary<string, MapModeType[]> PropertyRouting = new()
   {
      { nameof(Province.IsCity), [MapModeType.City] },
      { nameof(Province.CenterOfTrade), [MapModeType.CenterOfTrade] },
      { nameof(Province.Religion), [MapModeType.Religion] },
      { nameof(Province.Culture), [MapModeType.Culture] },
      { nameof(Province.Culture.CultureGroup), [MapModeType.CultureGroup] },
      { nameof(Province.Owner), [MapModeType.Country] },
      { nameof(Province.Controller), [MapModeType.Country] },
      { nameof(Province.TradeGood), [MapModeType.TradeGoods] },
      //{ nameof(Province.), MapModeType.TradeNode },
      { nameof(Province.TradeCompany), [MapModeType.TradeCompany] },
      //{ nameof(Province.ColonialRegion), MapModeType.ColonialRegions },
      { nameof(Province.Area), [MapModeType.Area] },
      //{ nameof(Province.Region), MapModeType.Regions },
      //{ nameof(Province.SuperRegion), MapModeType.SuperRegion },
      { nameof(Province.Continent), [MapModeType.Continent] },
      { nameof(Province.TotalDevelopment), [MapModeType.Development] },
      { nameof(Province.BaseTax), [MapModeType.Development, MapModeType.BaseTax] },
      { nameof(Province.BaseProduction), [MapModeType.Development, MapModeType.BaseProduction] },
      { nameof(Province.BaseManpower), [MapModeType.Development, MapModeType.BaseManpower] },
      { nameof(Province.LocalAutonomy), [MapModeType.Autonomy] },
      //{ nameof(Province.), MapModeType.Fort },
      { nameof(Province.Devastation), [MapModeType.Devastation] },
      { nameof(Province.Prosperity), [MapModeType.Prosperity] },
      { nameof(Province.IsHre), [MapModeType.Hre] },
      { nameof(Province.IsSeatInParliament), [MapModeType.ParliamentSeat] },
      { nameof(Province.Capital), [MapModeType.HasCapital] },
      //{ nameof(Province.Diplomatic), MapModeType.Diplomatic },
      { nameof(Province.Terrain), [MapModeType.Terrain] },
      { nameof(Province.AutoTerrain), [MapModeType.TerrainOverrides] },
      { nameof(CommonCountry.Color), [MapModeType.Country]},
      //{ nameof(Province.Climate), MapModeType.Climate },
      //{ nameof(Province.Weather), MapModeType.Weather },
      { nameof(Province.Buildings), [MapModeType.Fort]},
   };

   static MapModeManager()
   {
      InitializeAllMapModes();
   }

   public static void InitializeAllMapModes()
   {
#if DEBUG
      MapModes.Add(new DebugMapMode());
#endif
      MapModes.Add(new ProvinceMapMode());
      MapModes.Add(new AreaMapMode());
      MapModes.Add(new RegionsMapMode());
      MapModes.Add(new SuperRegionMapMode());
      MapModes.Add(new ContinentMapMode());
      MapModes.Add(new DevelopmentMapMode());
      MapModes.Add(new BaseTaxMapMode());
      MapModes.Add(new BaseProductionMapMode());
      MapModes.Add(new BaseManpowerMapMode());
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
      MapModes.Add(new ClimateMapMode());
      MapModes.Add(new WeatherMapMode());
      MapModes.Add(new GameOfLiveMapMode());
      MapModes.Add(new RGBMapMode());
      //MapModes.Add(new MemeMode());

      IdMapMode = new ()
      {
         Bitmap = new(Globals.MapPath)
      };

      foreach (var province in Globals.Provinces) 
         ColorCache.Add(province, 0);
   }

   public static void UpdateCache(Province p, int color)
   {
      ColorCache[p] = color;
   }

   public static void UpdateMapMode(MapModeType type)
   {
      if (CurrentMapMode.MapModeType != type)
         return;
      RenderCurrent();
   }

   public static void RenderCurrent()
   {
      ConstructCache();
      CurrentMapMode.RenderMapMode();
   }


   public static void ConstructClearCache(ICollection<Province> provinces, MapMode mapMode, Dictionary<Province, int> cache)
   {
      cache.Clear();
      ConstructCache(provinces, mapMode, cache);
   }
   public static void ConstructCache(ICollection<Province> provinces, MapMode mapMode, Dictionary<Province, int> cache)
   {
      foreach (var province in provinces)
         cache[province] = mapMode.GetProvinceColor(province);
   }

   public static void ConstructCache()
   {
      if (CurrentMapMode.IsLandOnly)
      {
         foreach (var landProvince in Globals.LandProvinces) 
            ColorCache[landProvince] = CurrentMapMode.GetProvinceColor(landProvince);

         foreach (var seaProvince in Globals.SeaProvinces) 
            ColorCache[seaProvince] = CurrentMapMode.GetSeaProvinceColor(seaProvince);

         foreach (var lake in Globals.LakeProvinces)
            ColorCache[lake] = CurrentMapMode.GetSeaProvinceColor(lake);
      }
      else
         foreach (var province in Globals.Provinces)
            ColorCache[province] = CurrentMapMode.GetProvinceColor(province);
   }

   public static void RenderMapMode(PropertyInfo propInfo)
   {
      if (PropertyRouting.TryGetValue(propInfo.Name, out var type))
         foreach (var mapModeType in type)
            if (CurrentMapMode.MapModeType == mapModeType)
               UpdateMapMode(mapModeType);
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