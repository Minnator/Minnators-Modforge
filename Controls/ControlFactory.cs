using System.Windows.Forms;

namespace Editor.Controls;

public static class ControlFactory
{

   public static PannablePictureBox GetPannablePictureBox(string path, ref Panel panel, MapWindow mapWindow)
   {
      return new PannablePictureBox(path, ref panel, mapWindow);
   }

}