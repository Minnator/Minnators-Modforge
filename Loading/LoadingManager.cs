using System;
using System.Diagnostics;
using System.IO;
using Editor.DataClasses;
using Editor.Helper;

namespace Editor.Loading;

public static class LoadingManager
{
   public static void LoadGameAndModDataToApplication(ModProject project, ref Log LoadingLog, MapWindow mw)
   {

      LoadDefinitionAndMap(ref LoadingLog, project);
      DefaultMapLoading.Load(project.VanillaPath, ref LoadingLog);
      AreaLoading.Load(project.VanillaPath, project.ColorProvider, ref LoadingLog);
      RegionLoading.Load(project.VanillaPath, project.ColorProvider, ref LoadingLog);
      SuperRegionLoading.Load(project.VanillaPath, project.ColorProvider, ref LoadingLog);
      ContinentLoading.Load(project.VanillaPath, ref LoadingLog);
      LocalisationLoading.Load(project.ModPath, project.VanillaPath, project.Language, ref LoadingLog);
      ProvinceParser.ParseAllProvinces(project.ModPath, project.VanillaPath, ref LoadingLog);

      // MUST BE LAST in the loading sequence
      InitMapModes(ref LoadingLog, mw);
      
      GC.Collect();
      LoadingLog.Close();
      LoadingLog = null!;
   }

   public static void InitializeComponents(MapWindow mw)
   {
      ResourceUsageHelper.Initialize(mw);
      //HistoryResourceHelper.Initialize(this);

      Globals.HistoryManager.UndoDepthChanged += mw.UpdateUndoDepth;
      Globals.HistoryManager.RedoDepthChanged += mw.UpdateRedoDepth;

   }


   private static void InitMapModes(ref Log log, MapWindow mw)
   {

      var sw = Stopwatch.StartNew();
      Globals.MapModeManager = new(mw.MapPictureBox); // Initialize the MapModeManager
      //Globals.MapModeManager.SetCurrentMapMode("Provinces"); // Default map mode
      foreach (var mode in Globals.MapModeManager.GetMapModes())
      {
         mw.MapModeComboBox.Items.Add(mode.GetMapModeName());
      }
      mw.MapModeComboBox.SelectedIndex = 0;
      sw.Stop();
      log.WriteTimeStamp("Initializing MapModes", sw.ElapsedMilliseconds);
   }


   private static void LoadDefinitionAndMap(ref Log loadingLog, ModProject project)
   {
      var provinces = DefinitionLoading.LoadDefinition([.. File.ReadAllLines(Path.Combine(project.VanillaPath, "map", "definition.csv"))], ref loadingLog);
      Globals.MapPath = Path.Combine(project.VanillaPath, "map", "provinces.bmp");
      var (colorToProvId, colorToBorder, adjacency) = MapLoading.LoadMap(ref loadingLog, Globals.MapPath);

      Optimizer.OptimizeProvinces(provinces, colorToProvId, colorToBorder, project.MapSize.Width * project.MapSize.Height, ref loadingLog);

      Optimizer.OptimizeAdjacencies(adjacency, ref loadingLog);
   }
}