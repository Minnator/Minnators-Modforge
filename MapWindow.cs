using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor
{
   public partial class MapWindow : Form
   {
      public MapWindow()
      {
         InitializeComponent();

         MapLoading.LoadMap();
      }
   }
}
