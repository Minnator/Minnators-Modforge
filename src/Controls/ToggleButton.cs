namespace Editor.Controls
{
   public sealed class ToggleButton : Button
   {
      public enum VisualMode
      {
         Text,
         Image,
         Default
      }

      private bool _state;
      public VisualMode VsMode { get; set; } = VisualMode.Image;
      public Image? ImageOn { get; set; } 
      public Image? ImageOff { get; set; }

      private int ImageIndexOn { get; set; } = -1;
      private int ImageIndexOff { get; set; } = -1;

      public ToggleButton()
      {
         // Handle Click to toggle state
         Click += (sender, args) => Toggle();
         TextAlign = ContentAlignment.MiddleCenter;
         ImageAlign = ContentAlignment.MiddleCenter;
         UpdateVisuals();
      }

      public ToggleButton(Image on, Image off, VisualMode mode, bool state = true) : this()
      {
         ImageOn = on;
         ImageOff = off;
         VsMode = mode;
         _state = state;
      }

      public bool State
      {
         get => _state;
         set
         {
            _state = value;
            UpdateVisuals();
         }
      }

      public void Toggle()
      {
         _state = !_state;
         UpdateVisuals();
         Invalidate();
      }

      private void UpdateVisuals()
      {
         switch (VsMode)
         {
            case VisualMode.Text:
               Image = null;
               Text = _state ? "On" : "Off";
               break;
            case VisualMode.Image:
               Text = "";
               BackColor = Color.Transparent;
               if (ImageOn != null && ImageOff != null)
                  Image = _state ? ImageOn : ImageOff;
               else
               {
                  VsMode = VisualMode.Default;
                  UpdateVisuals();
               }
               break;
            case VisualMode.Default:
               Image = null;
               Text = _state ? "On" : "Off";
               BackColor = _state ? Color.DarkGreen : Color.DarkRed;
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      // override the paint event
      protected override void OnPaint(PaintEventArgs e)
      {
         UpdateVisuals();
         switch (VsMode)
         {
            case VisualMode.Text:
               // Black outline or highlight outline if selected
               // Back color should fill the button except the border
               // Text should be centered
               e.Graphics.FillRectangle(new SolidBrush(BackColor), new (ClientRectangle.X + 2, ClientRectangle.Y + 2, ClientRectangle.Width - 4, ClientRectangle.Height - 4));
               e.Graphics.DrawRectangle(new (ForeColor, 2), new(ClientRectangle.X + 1, ClientRectangle.Y + 1, ClientRectangle.Width - 2, ClientRectangle.Height - 2));
               using (var brush = new SolidBrush(ForeColor))
               {
                  e.Graphics.DrawString(Text, Font, brush, ClientRectangle, new StringFormat
                  {
                     Alignment = StringAlignment.Center,
                     LineAlignment = StringAlignment.Center
                  });
               }
               break;
            case VisualMode.Image:
            case VisualMode.Default:
               base.OnPaint(e);
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

   }
}
