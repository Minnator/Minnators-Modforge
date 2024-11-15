using Editor.DataClasses.MapModes;
using Editor.Forms.PopUps;

namespace Editor.Controls
{
   public sealed class MapModeButton : Button
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
         Padding = new(0);

         Click += OnButtonClick;
         MouseDown += OnMouseCLick;
      }

      public void SetMapMode(MapModeType mapMode)
      {
         MapModeName = mapMode;
         Text = $"{mapMode} (&{Hotkey})";
         var mm = Globals.MapModeManager.GetMapMode(mapMode);
         return;
         if (mm.Icon == null)
         {
            Text = $"{mapMode} (&{Hotkey})";
            return;
         }
         Text = $"(&{Hotkey})";
         SetImage(mm.Icon);
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

      public void SetImage(Bitmap image)
      {
         Image = image;
         ImageAlign = ContentAlignment.MiddleCenter; 
         TextAlign = ContentAlignment.MiddleRight; 
      }
   }
}