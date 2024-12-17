using Editor.DataClasses.MapModes;
using Editor.DataClasses.Settings;
using Editor.Forms.PopUps;

namespace Editor.Controls
{
   public sealed class MapModeButton : Button
   {
      private MapModeType MapModeName = MapModeType.None;
      private char Hotkey;
      private const float MAXFONTSIZE = 8;
      private const float MINFONTSIZE = 1;
      internal int MapModeIndex;
      public MapModeButton(char hotkey, int mapModeIndex)
      {
         MapModeIndex = mapModeIndex;
         Hotkey = hotkey;
         Text = $"(&{hotkey})";
         UseMnemonic = true;
         Dock = DockStyle.Fill;
         Font = new("Arial", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
         Margin = new(1, 0, 1, 0);
         Padding = new(0);

         Click += OnButtonClick;
         MouseDown += OnMouseCLick;

         if (mapModeIndex < Globals.Settings.Gui.MapModes.Length)
            SetMapMode(Globals.Settings.Gui.MapModes[mapModeIndex]);
      }

      public void SetMapMode(MapModeType mapMode, bool callInv = true)
      {
         MapModeName = mapMode;
         Text = $"{mapMode} (&{Hotkey})";
         AdaptToLength(Text);
         if (MapModeIndex < Globals.Settings.Gui.MapModes.Length)
         {
            Globals.Settings.Gui.MapModes[MapModeIndex] = mapMode;
            if (Globals.State != State.Running)
               return;
            if (callInv)
               Globals.Settings.Gui.Invalidate(nameof(GuiSettings.MapModes));
         }
      }

      public void UpdateMapMode(bool callInv = true)
      {
         SetMapMode(Globals.Settings.Gui.MapModes[MapModeIndex], callInv);
      }


      public void AdaptToLength(string str)
      {
         using var g = CreateGraphics();
         var baseSize = g.MeasureString(str, Font);
         var estimate = MAXFONTSIZE * Width / baseSize.Width;

         if (estimate > MAXFONTSIZE)
            return;
         else if (estimate < MINFONTSIZE)
            estimate = MINFONTSIZE;

         

         Font = new(Font.FontFamily, estimate, Font.Style, Font.Unit, Font.GdiCharSet);
         var estimatedSize = g.MeasureString(str, Font);
         while (estimatedSize.Width > Width)
         {
            if (estimate <= MINFONTSIZE)
               break;
            Font = new(Font.FontFamily, estimate, Font.Style, Font.Unit, Font.GdiCharSet);
            estimate -= 0.5f;
            estimatedSize = g.MeasureString(str, Font);
         }
      }

      public override string ToString()
      {
         return MapModeName.ToString();
      }

      public void OnButtonClick(object? sender, EventArgs e)
      {
         if (MapModeName != MapModeType.None) 
            MapModeManager.SetCurrentMapMode(MapModeName);
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
            if (form.SelectedMapMode != MapModeType.None)
               MapModeManager.SetCurrentMapMode(form.SelectedMapMode);

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