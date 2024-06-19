using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor
{
   public partial class MapWindow : Form
   {
      public readonly Log LoadingLog = new (@"C:\Users\david\Downloads", "Loading");
      public readonly Log ErrorLog = new (@"C:\Users\david\Downloads", "Error");

      public MapWindow()
      {
         InitializeComponent();

         MapLoading.LoadMap(ref LoadingLog);
         DefinitionLoading.LoadDefinition([.. File.ReadAllLines(@"S:\SteamLibrary\steamapps\common\Europa Universalis IV\map\definition.csv")], ref LoadingLog);
      }
   }
}
