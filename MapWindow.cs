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
using Editor.Controls;
using Editor.DataClasses;
using Editor.Loading;

namespace Editor
{
   public partial class MapWindow : Form
   {
      public readonly Log LoadingLog = new (@"C:\Users\david\Downloads", "Loading");
      public readonly Log ErrorLog = new (@"C:\Users\david\Downloads", "Error");
      
      public PannablePictureBox MapPictureBox = null!;

      public ModProject Project = new ()
      {
         Name = "Vanilla",
         ModPath = Consts.MOD_PATH,
         VanillaPath = Consts.VANILLA_PATH
      };

      public MapWindow()
      {
         InitGui();
         
         var (colorToProvId, colorToBorder) = MapLoading.LoadMap(
            ref LoadingLog, Path.Combine(Project.VanillaPath, "map", "provinces.bmp"));
         var provinces = DefinitionLoading.LoadDefinition([.. File.ReadAllLines(@"S:\SteamLibrary\steamapps\common\Europa Universalis IV\map\definition.csv")], ref LoadingLog);

         Optimizer.OptimizeProvinces(provinces, colorToProvId, colorToBorder, Project.MapSize.Width * Project.MapSize.Height);

      }

      public void InitGui()
      {
         InitializeComponent();
         MapPictureBox = ControlFactory.GetPannablePictureBox(@"S:\SteamLibrary\steamapps\common\Europa Universalis IV\map\provinces.bmp", ref MapPanel);
         MapPanel.Controls.Add(MapPictureBox);
      }
   }
}
