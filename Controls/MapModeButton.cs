using System.Diagnostics;
using Editor.DataClasses.MapModes;
using Editor.Forms;

namespace Editor.Controls
{
   public class MapModeButton : Button
   {
      private MapModeType MapModeName = MapModeType.None;
      private char Hotkey;
      
      public MapModeButton(char hotkey)
      {
         Hotkey = hotkey;
         Text = $"(&{hotkey})";
         UseMnemonic = true;
         Dock = DockStyle.Fill;
         Font = new("Arial", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
         Margin = new(1,0,1,0);

         Click += OnButtonClick;
         MouseDown += OnMouseCLick;
      }

      public void SetMapMode(MapModeType mapMode)
      {
         MapModeName = mapMode;
         Text = $"{mapMode} (&{Hotkey})";
      }

      public override string ToString()
      {
         return MapModeName.ToString();
      }

      public void OnButtonClick(object? sender, EventArgs e)
      {
         if (MapModeName != MapModeType.None) 
            Globals.MapModeManager.SetCurrentMapMode(MapModeName);
      }

      public void OnMouseCLick(object? sender, MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Right)
         {
            // Open the MapModeSelection form at the current mouse position
            var form = new MapModeSelection(this);
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new (MousePosition.X, MousePosition.Y);
            form.ShowDialog();

         }
      }
   }
}