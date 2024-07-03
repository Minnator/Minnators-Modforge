﻿using System;
using System.Diagnostics;
using System.IO;
using Editor.DataClasses;
using Editor.Helper;

namespace Editor.Loading;

public static class LoadingManager
{
   public static void LoadGameAndModDataToApplication(ModProject project, ref Log loadingLog, MapWindow mw)
   {

      LoadDefinitionAndMap(ref loadingLog, project);
      DefaultMapLoading.Load(project.VanillaPath, ref loadingLog);
      AreaLoading.Load(project.VanillaPath, project.ColorProvider, ref loadingLog);
      RegionLoading.Load(project.VanillaPath, project.ColorProvider, ref loadingLog);
      SuperRegionLoading.Load(project.VanillaPath, project.ColorProvider, ref loadingLog);
      ContinentLoading.Load(project.VanillaPath, project.ColorProvider, ref loadingLog);
      LocalisationLoading.Load(project.ModPath, project.VanillaPath, project.Language, ref loadingLog);
      ProvinceParser.ParseAllProvinces(project.ModPath, project.VanillaPath, ref loadingLog);

      // MUST BE LAST in the loading sequence
      InitMapModes(ref loadingLog, mw);
      
      GC.Collect();
      loadingLog.Close();
      loadingLog = null!;
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
      Globals.MapModeManager.InitializeAllMapModes(); // Initialize all map modes
      Globals.MapModeManager.SetCurrentMapMode("Provinces"); // Default map mode

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