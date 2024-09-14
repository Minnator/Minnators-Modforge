using System.Diagnostics;
using Windows.Foundation.Metadata;
using Editor.DataClasses;
using Editor.Parser;
using Path = System.IO.Path;

namespace Editor.Loading;

public static class LoadingManager
{

   public static void InitializeComponents(MapWindow mw)
   {
      Globals.HistoryManager.UndoDepthChanged += mw.UpdateUndoDepth;
      Globals.HistoryManager.RedoDepthChanged += mw.UpdateRedoDepth;

   }


   public static void InitMapModes(MapWindow mw)
   {
      var sw = Stopwatch.StartNew();
      Globals.MapModeManager = new(mw.MapPictureBox); // Initialize the MapModeManager
      Globals.MapModeManager.InitializeAllMapModes(); // Initialize all map modes
      mw.MapModeComboBox.Items.Clear();
      mw.MapModeComboBox.Items.AddRange([.. Globals.MapModeManager.GetMapModeNames()]);
      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Initializing MapModes", sw.ElapsedMilliseconds);
   }


   internal static void LoadDefinitionAndMap(ModProject project)
   {
      var definition = Path.Combine(project.ModPath, "map", "definition.csv");
      if (!File.Exists(definition))
      {
         definition = Path.Combine(project.VanillaPath, "map", "definition.csv");
         if (!File.Exists(definition))
            throw new FileNotFoundException("Could not find definition.csv in mod or vanilla folder");
      }

      var provinces = DefinitionLoading.LoadDefinition([.. File.ReadAllLines(definition)]);
      var modMap = Path.Combine(project.ModPath, "map", "provinces.bmp");
      var vanillaMap = Path.Combine(project.VanillaPath, "map", "provinces.bmp");
      if (File.Exists(modMap))
         Globals.MapPath = modMap;
      else if (File.Exists(vanillaMap))
         Globals.MapPath = vanillaMap;
      else
         throw new FileNotFoundException("Could not find provinces.bmp in mod or vanilla folder");
      var (colorToProvId, colorToBorder, adjacency) = MapLoading.LoadMap(Globals.MapPath);

      Optimizer.OptimizeProvinces(provinces, colorToProvId, colorToBorder, project.MapSize.Width * project.MapSize.Height);

      Optimizer.OptimizeAdjacencies(adjacency);
   }
}