using System.Diagnostics;
using Editor.DataClasses;
using Editor.Helper;

namespace Editor.Loading;

public static class LoadingManager
{
   public static void LoadGameAndModDataToApplication(ModProject project, MapWindow mw)
   {
      TradeGoodsLoading.Load(project);
      Globals.LoadingStage += 1;
      TradeNodeLoading.Load(project);
      Globals.LoadingStage += 1;
      TechnologyGroupsLoading.Load(project);
      Globals.LoadingStage += 1;
      BuildingsLoading.Load(project);
      Globals.LoadingStage += 1;
      ReligionLoading.Load(project);
      Globals.LoadingStage += 1;
      LoadDefinitionAndMap(project); //TODO SLOW
      Globals.LoadingStage += 1;
      DefaultMapLoading.Load(project.VanillaPath);
      Globals.LoadingStage += 1;
      AreaLoading.Load(project.VanillaPath, project.ColorProvider);
      Globals.LoadingStage += 1;
      RegionLoading.Load(project.VanillaPath, project.ColorProvider);
      Globals.LoadingStage += 1;
      SuperRegionLoading.Load(project.VanillaPath, project.ColorProvider);
      Globals.LoadingStage += 1;
      ContinentLoading.Load(project.VanillaPath, project.ColorProvider);
      Globals.LoadingStage += 1;
      LocalisationLoading.Load(project.ModPath, project.VanillaPath, project.Language);
      Globals.LoadingStage += 1;
      ProvinceParser.ParseAllUniqueProvinces(project.ModPath, project.VanillaPath); //TODO SLOW
      Globals.LoadingStage += 1;
      CultureLoading.LoadCultures(project);
      Globals.LoadingStage += 1;
      CountryLoading.LoadCountries(project); //TODO SLOW
      Globals.LoadingStage += 1;
      DebugPrints.PrintCountriesBasic();


      // MUST BE LAST in the loading sequence
      InitMapModes(mw);
      Globals.LoadingStage += 1;
      
      GC.Collect();
   }

   public static void InitializeComponents(MapWindow mw)
   {
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