using System.Diagnostics;
using Editor.DataClasses;
using Editor.Helper;

namespace Editor.Loading;

public static class LoadingManager
{
   public static void LoadGameAndModDataToApplication(ModProject project, MapWindow mw)
   {
      TradeGoodsLoading.Load(project);
      TradeNodeLoading.Load(project);
      LoadDefinitionAndMap(project); //TODO SLOW
      DefaultMapLoading.Load(project.VanillaPath);
      AreaLoading.Load(project.VanillaPath, project.ColorProvider);
      RegionLoading.Load(project.VanillaPath, project.ColorProvider);
      SuperRegionLoading.Load(project.VanillaPath, project.ColorProvider);
      ContinentLoading.Load(project.VanillaPath, project.ColorProvider);
      LocalisationLoading.Load(project.ModPath, project.VanillaPath, project.Language);
      ProvinceParser.ParseAllUniqueProvinces(project.ModPath, project.VanillaPath); //TODO SLOW
      CultureLoading.LoadCultures(project);
      CountryLoading.LoadCountries(project); //TODO SLOW

      DebugPrints.PrintCountriesBasic();

      // MUST BE LAST in the loading sequence
      InitMapModes(mw);
      
      GC.Collect();
   }

   public static void InitializeComponents(MapWindow mw)
   {
      //HistoryResourceHelper.Initialize(this);

      Globals.HistoryManager.UndoDepthChanged += mw.UpdateUndoDepth;
      Globals.HistoryManager.RedoDepthChanged += mw.UpdateRedoDepth;

   }


   private static void InitMapModes(MapWindow mw)
   {
      var sw = Stopwatch.StartNew();
      Globals.MapModeManager = new(mw.MapPictureBox); // Initialize the MapModeManager
      Globals.MapModeManager.InitializeAllMapModes(); // Initialize all map modes
      mw.MapModeComboBox.Items.Clear();
      mw.MapModeComboBox.Items.AddRange([.. Globals.MapModeManager.GetMapModeNames()]);
      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Initializing MapModes", sw.ElapsedMilliseconds);
      sw.Restart();
      Globals.MapModeManager.SetCurrentMapMode("Provinces"); // Default map mode
      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Setting default map mode", sw.ElapsedMilliseconds);
   }


   private static void LoadDefinitionAndMap(ModProject project)
   {
      var provinces = DefinitionLoading.LoadDefinition([.. File.ReadAllLines(Path.Combine(project.VanillaPath, "map", "definition.csv"))]);
      Globals.MapPath = Path.Combine(project.VanillaPath, "map", "provinces.bmp");
      var (colorToProvId, colorToBorder, adjacency) = MapLoading.LoadMap(Globals.MapPath);

      Optimizer.OptimizeProvinces(provinces, colorToProvId, colorToBorder, project.MapSize.Width * project.MapSize.Height);

      Optimizer.OptimizeAdjacencies(adjacency);
   }
}