using System.Windows.Forms;

namespace Editor.Controls;

public static class ControlFactory
{

   public static PannablePictureBox GetPannablePictureBox(ref Panel panel, MapWindow mapWindow)
   {
      return new PannablePictureBox(ref panel, mapWindow);
   }

}