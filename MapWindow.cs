using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Editor.Controls;
using Editor.DataClasses;
using Editor.Helper;
using Editor.Loading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

         ResourceUsageHelper.Initialize(this);

         LoadDefinitionAndMap(ref LoadingLog);
         DrawProvinceBorder();

         GC.Collect();
         LoadingLog.Close();
         LoadingLog = null!;

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
         var (colorToProvId, colorToBorder) = MapLoading.LoadMap(
            ref loadingLog, Path.Combine(Project.VanillaPath, "map", "provinces.bmp"));
         var provinces = DefinitionLoading.LoadDefinition([.. File.ReadAllLines(@"S:\SteamLibrary\steamapps\common\Europa Universalis IV\map\definition.csv")], ref loadingLog);

         Optimizer.OptimizeProvinces(provinces, colorToProvId, colorToBorder, Project.MapSize.Width * Project.MapSize.Height, ref loadingLog);
      }

      private void DrawProvinceBorder()
      {
         var rect = new Rectangle(0, 0, Project.MapSize.Width, Project.MapSize.Height);
         using var g = Graphics.FromImage(MapPictureBox.Overlay);
         g.Clear(Color.Transparent);
         MapDrawHelper.DrawOnMap(rect, Data.BorderPixels, Color.Black, MapPictureBox.Image);
         MapPictureBox.Invalidate();
         MapPictureBox.Overlay.Save(@"C:\Users\david\Downloads\borderTest.bmp", ImageFormat.Png);

      }

      public void SetSelectedProvinceSum(int sum)
      {
         SelectedProvinceSum.Text = $"ProvSum: [{sum}]";
      }

      public void UpdateMemoryUsage(float memoryUsage)
      {
         if (InvokeRequired) Invoke(new MethodInvoker(delegate { RamUsageStrip.Text = $"RAM: [{Math.Round(memoryUsage)} MB]"; }));
      }

      public void UpdateCpuUsage (float cpuUsage)
      {
         if (InvokeRequired) Invoke(new MethodInvoker(delegate { CpuUsageStrip.Text = $"CPU: [{Math.Round(cpuUsage, 2)}%]"; }));
      }

      private void MapWindow_FormClosing(object sender, FormClosingEventArgs e)
      {
         ResourceUsageHelper.Dispose();
      }
   }
}
