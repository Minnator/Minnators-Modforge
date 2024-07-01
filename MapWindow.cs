using Editor.Controls;
using Editor.DataClasses;
using Editor.Helper;
using Editor.Loading;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

#nullable enable

namespace Editor
{
   public partial class MapWindow : Form
   {
      public readonly Log LoadingLog = new (@"C:\Users\david\Downloads", "Loading");
      public readonly Log ErrorLog = new (@"C:\Users\david\Downloads", "Error");

      public readonly string? ApplicationName;

      public PannablePictureBox MapPictureBox = null!;

      public ModProject Project = new ()
      {
         Name = "Vanilla",
         ModPath = Consts.MOD_PATH,
         VanillaPath = Consts.VANILLA_PATH
      };

      public MapWindow()
      {
         ApplicationName = Process.GetCurrentProcess().ProcessName;

         InitGui();


         LoadDefinitionAndMap(ref LoadingLog);
         DefaultMapLoading.Load(Project.VanillaPath, ref LoadingLog);
         AreaLoading.Load(Project.VanillaPath, Project.ColorProvider, ref LoadingLog);
         RegionLoading.Load(Project.VanillaPath, Project.ColorProvider, ref LoadingLog);
         SuperRegionLoading.Load(Project.VanillaPath, Project.ColorProvider, ref LoadingLog);
         ContinentLoading.Load(Project.VanillaPath, ref LoadingLog);
         LocalisationLoading.Load(Project.ModPath, Project.VanillaPath, Project.Language, ref LoadingLog);


         // MUST BE LAST in the loading sequence
         InitMapModes(ref LoadingLog);
         GC.Collect();
         LoadingLog.Close();
         LoadingLog = null!;

         ResourceUsageHelper.Initialize(this);
         //HistoryResourceHelper.Initialize(this);

         Globals.HistoryManager.UndoDepthChanged += UpdateUndoDepth;
         Globals.HistoryManager.RedoDepthChanged += UpdateRedoDepth;

         ProvinceParser.ParseAllProvinces(Project.ModPath, Project.VanillaPath);
      }

      private void InitMapModes(ref Log log)
      {

         var sw = Stopwatch.StartNew();
         Globals.MapModeManager = new(MapPictureBox); // Initialize the MapModeManager
         //Globals.MapModeManager.SetCurrentMapMode("Provinces"); // Default map mode
         foreach (var mode in Globals.MapModeManager.GetMapModes())
         {
            MapModeComboBox.Items.Add(mode.GetMapModeName());
         }
         MapModeComboBox.SelectedIndex = 0;
         sw.Stop();
         log.WriteTimeStamp("Initializing MapModes", sw.ElapsedMilliseconds);
      }


      // Loads the map into the created PannablePictureBox
      private void InitGui()
      {
         InitializeComponent();
         MapPictureBox = ControlFactory.GetPannablePictureBox(ref MapPanel, this);
         MapPanel.Controls.Add(MapPictureBox);
      }

      private void LoadDefinitionAndMap(ref Log loadingLog)
      {
         var provinces = DefinitionLoading.LoadDefinition([.. File.ReadAllLines(Path.Combine(Project.VanillaPath, "map", "definition.csv"))], ref loadingLog);
         Globals.MapPath = Path.Combine(Project.VanillaPath, "map", "provinces.bmp");
         var (colorToProvId, colorToBorder, adjacency) = MapLoading.LoadMap(ref loadingLog, Globals.MapPath);

         Optimizer.OptimizeProvinces(provinces, colorToProvId, colorToBorder, Project.MapSize.Width * Project.MapSize.Height, ref loadingLog);

         Optimizer.OptimizeAdjacencies(adjacency, ref loadingLog);
      }


      #region ToolStrip update methods
      public void SetSelectedProvinceSum(int sum)
      {
         SelectedProvinceSum.Text = $"ProvSum: [{sum}]";
      }

      #region Resource Updater MethodInvokation

      public void UpdateMemoryUsage(float memoryUsage)
      {
         if (InvokeRequired) Invoke(new MethodInvoker(delegate { RamUsageStrip.Text = $"RAM: [{Math.Round(memoryUsage)} MB]"; }));
      }

      public void UpdateCpuUsage(float cpuUsage)
      {
         if (InvokeRequired) Invoke(new MethodInvoker(delegate { CpuUsageStrip.Text = $"CPU: [{Math.Round(cpuUsage, 2)}%]"; }));
      }

      #endregion

      #region HistoryManager Event Handlers
      private void UpdateRedoDepth(object sender, int e) => RedoDepthLabel.Text = $"Redos [{e}]";
      private void UpdateUndoDepth(object sender, int e) => UndoDepthLabel.Text = $"Undos [{e}]";
      #endregion
      #endregion

      private void MapWindow_FormClosing(object sender, FormClosingEventArgs e)
      {
         ResourceUsageHelper.Dispose();
      }

      #region History interface interactions

      private void RevertInSelectionHistory(object sender, EventArgs e)
      {
         var historyTreeView = new HistoryTree(Globals.HistoryManager.RevertTo);
         historyTreeView.VisualizeFull(Globals.HistoryManager.GetRoot());
         historyTreeView.ShowDialog();
      }

      private void DeleteHistoryToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Globals.HistoryManager.Clear();
      }

      #endregion

      private  void debugToolStripMenuItem_Click(object sender, EventArgs e)
      {
         DebugPrints.PrintAllAttributes(Globals.ParsingProvinces);
      }

      private void MapModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         Globals.MapModeManager.SetCurrentMapMode(MapModeComboBox.SelectedItem.ToString());
         GC.Collect(); // We force the garbage collector to collect the old bitmap
      }

      private void gCToolStripMenuItem_Click(object sender, EventArgs e)
      {
         GC.Collect();
      }
   }
}
