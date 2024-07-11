using System.Windows.Forms;

namespace Editor.Controls;

public static class ControlFactory
{

   public static PannablePictureBox GetPannablePictureBox(ref Panel panel, MapWindow mapWindow)
   {
      return new PannablePictureBox(ref panel, mapWindow);
   }

   public static ToolStripMenuItem GetToolStripMenuItem(string text, EventHandler onClick, Image image = null!)
   {
      return new(text, image, onClick);
   }

   public static ToolStripMenuItem GetToolStripMenuItem(string text, EventHandler onClick, Keys shortcut, Image image = null!)
   {
      var item = new ToolStripMenuItem(text, image, onClick);
      item.ShortcutKeys = shortcut;
      return item;
   }

   public static ToolStripMenuItem GetDisabledToolStripMenuItem (string text, Image image = null!)
   {
      var item = new ToolStripMenuItem(text, image);
      item.Enabled = false;
      return item;
   }
}