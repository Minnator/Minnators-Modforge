using Editor.Controls;
using Editor.DataClasses;
using Editor.Helper;
using Editor.Loading;
using System;
using System.Diagnostics;
using System.Drawing;
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
         ApplicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

         Debug.WriteLine($"APPLICATION NAME: {ApplicationName}");

         InitGui();


         LoadDefinitionAndMap(ref LoadingLog);
         DefaultMapLoading.Load(Project.VanillaPath, ref LoadingLog);
         AreaLoading.Load(Project.VanillaPath, ref LoadingLog);
         RegionLoading.Load(Project.VanillaPath, ref LoadingLog);
         SuperRegionLoading.Load(Project.VanillaPath, ref LoadingLog);
         ContinentLoading.Load(Project.VanillaPath, ref LoadingLog);
         DrawProvinceBorder();
         
         GC.Collect();
         LoadingLog.Close();
         LoadingLog = null!;

         ResourceUsageHelper.Initialize(this);
         //HistoryResourceHelper.Initialize(this);

         Data.HistoryManager.UndoDepthChanged += UpdateUndoDepth;
         Data.HistoryManager.RedoDepthChanged += UpdateRedoDepth;

      }

      // Loads the map into the created PannablePictureBox
      private void InitGui()
      {
         InitializeComponent();
         MapPictureBox = ControlFactory.GetPannablePictureBox(@"S:\SteamLibrary\steamapps\common\Europa Universalis IV\map\provinces.bmp", ref MapPanel, this);
         MapPanel.Controls.Add(MapPictureBox);
      }

      private void LoadDefinitionAndMap(ref Log loadingLog)
      {
         var provinces = DefinitionLoading.LoadDefinition([.. File.ReadAllLines(@"S:\SteamLibrary\steamapps\common\Europa Universalis IV\map\definition.csv")], ref loadingLog);
         //TODO Remove hardcoded path
         Data.MapPath = Path.Combine(Project.VanillaPath, "map", "provinces.bmp");
         var (colorToProvId, colorToBorder, adjacency) = MapLoading.LoadMap(
            ref loadingLog, Data.MapPath);

         Optimizer.OptimizeProvinces(provinces, colorToProvId, colorToBorder, Project.MapSize.Width * Project.MapSize.Height, ref loadingLog);

         Optimizer.OptimizeAdjacencies(adjacency, ref loadingLog);
      }

      private void DrawProvinceBorder()
      {
         var sw = Stopwatch.StartNew();
         var rect = new Rectangle(0, 0, Project.MapSize.Width, Project.MapSize.Height);
         MapDrawHelper.DrawOnMap(rect, Data.BorderPixels, Color.Black, MapPictureBox.Image);
         MapPictureBox.Invalidate(rect);
         sw.Stop();
         LoadingLog.WriteTimeStamp("Drawing Borders", sw.ElapsedMilliseconds);
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

      private void RevertInSelectionHistory(object sender, EventArgs e)
      {
         var historyTreeView = new HistoryTree(Data.HistoryManager.RevertTo);
         historyTreeView.VisualizeFull(Data.HistoryManager.GetRoot());
         historyTreeView.ShowDialog();
      }

      private void DeleteHistoryToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Data.HistoryManager.Clear();
      }
   }
}
