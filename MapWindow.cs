using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text;
using Editor.Controls;
using Editor.DataClasses;
using Editor.Forms;
using Editor.Helper;
using Editor.Loading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Editor
{
   public partial class MapWindow : Form
   {
      //public readonly Log LoadingLog = new(@"C:\Users\david\Downloads", "Loading");
      //public readonly Log ErrorLog = new(@"C:\Users\david\Downloads", "Error");

      public PannablePictureBox MapPictureBox = null!;

      public ModProject Project = new()
      {
         Name = "Vanilla",
         ModPath = Consts.MOD_PATH,
         VanillaPath = Consts.VANILLA_PATH
      };

      public MapWindow()
      {
         Globals.MapWindow = this;
         //pause gui updates
         SuspendLayout();
         InitGui();

         LoadingManager.LoadGameAndModDataToApplication(Project, this);
         LoadingManager.InitializeComponents(this);

         //resume gui updates
         ResumeLayout();
         // Enable the Application
         Globals.LoadingLog.Close();
         Globals.State = State.Running;
      }


      // Loads the map into the created PannablePictureBox
      private void InitGui()
      {
         InitializeComponent();
         MapPictureBox = ControlFactory.GetPannablePictureBox(ref MapPanel, this);
         MapPanel.Controls.Add(MapPictureBox); 
         Globals.Date = new(1444, 11, 11);
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

      public void UpdateRedoDepth(object sender, int e) => RedoDepthLabel.Text = $"Redos [{e}]";
      public void UpdateUndoDepth(object sender, int e) => UndoDepthLabel.Text = $"Undos [{e}]";
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

      private void debugToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Globals.Provinces[1].BaseManpower = 50;
         Globals.Provinces[2].BaseManpower = 50;
         Globals.Provinces[3].BaseManpower = 50;
         Globals.Provinces[4].BaseManpower = 50;
      }

      private void MapModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (MapPictureBox.IsPainting)
            return;
         Globals.MapModeManager.SetCurrentMapMode(MapModeComboBox.SelectedItem.ToString());
         GC.Collect(); // We force the garbage collector to collect the old bitmap
      }

      private void gCToolStripMenuItem_Click(object sender, EventArgs e)
      {
         GC.Collect();
      }

      private void SaveCurrentMapModeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\";
         MapPictureBox.Image.Save($@"{downloadFolder}{MapModeComboBox.SelectedItem}.png", ImageFormat.Png);
      }

      private void openCustomizerToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var toolTipCustomizer = new ToolTipCustomizer();
         // Show the ToolTipCustomizer as a popup
         toolTipCustomizer.Show();
      }

      private void ShowToolTipMenuItem_Click(object sender, EventArgs e)
      {
         MapPictureBox.ShowToolTip = ShowToolTipMenuItem.Checked;
      }

      private void testToolStripMenuItem_Click(object sender, EventArgs e)
      {
         //var content = File.ReadAllText("C:\\Users\\david\\Downloads\\NestedBLocks.txt");
         var content = File.ReadAllText("S:\\SteamLibrary\\steamapps\\common\\Europa Universalis IV\\common\\cultures\\00_cultures.txt");
         var sw = Stopwatch.StartNew();
         var blocks = Parsing.GetNestedElementsIterative(0, ref content);
         sw.Stop();
         Debug.WriteLine("Parsing cultures took: " + sw.ElapsedMilliseconds + "ms");

         var sb = new StringBuilder();
         foreach (var block in blocks)
         {
            DebugPrints.BuildBlockString(0, block, ref sb);
         }
         File.WriteAllText("C:\\Users\\david\\Downloads\\NestedBLocksOutput2.txt", sb.ToString());
      }

      private void telescopeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         DebugMaps.TelescopeImageBenchmark();
      }

      private void MapWindow_KeyDown(object sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.F1)
         {
            if (Globals.ConsoleForm == null || Globals.ConsoleForm.IsDisposed)
            {
               Globals.ConsoleForm = new();
               Globals.ConsoleForm.Show();
            }
            else
            {
               Globals.ConsoleForm.BringToFront();
            }
         }
      }

      private void refStackToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var groups = Geometry.GetProvinceConnectedGroups(Globals.Countries["TUR"].GetOwnedProvinces);
         DebugMaps.DrawProvinceGroups(groups);
      }
   }
}
