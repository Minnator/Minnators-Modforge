using System.Drawing.Drawing2D;

namespace Editor.Controls
{
   public class SlimProgressBar : ProgressBar
   {
      public bool UseRoundedCorners = true;
      public int CornerRadius = 4;


      public SlimProgressBar()
      {
         SetStyle(ControlStyles.UserPaint, true);
         SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
         SetStyle(ControlStyles.AllPaintingInWmPaint, true);
         SetStyle(ControlStyles.SupportsTransparentBackColor, true);
         SetStyle(ControlStyles.ResizeRedraw, true);
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         if (!UseRoundedCorners)
         {
            var g = e.Graphics;
            var rect = ClientRectangle;
            var progressBarRect = rect with { Width = rect.Width - 1, Height = rect.Height - 1 };
            var progressBarFillRect = rect with { Width = (int)(rect.Width * ((double)Value / Maximum)) - 1, Height = rect.Height - 1 };

            g.Clear(Parent!.BackColor);
            g.FillRectangle(new SolidBrush(BackColor), progressBarRect);
            g.FillRectangle(new SolidBrush(ForeColor), progressBarFillRect);
         }
         else
         {
            var g = e.Graphics;
            g.Clear(Parent!.BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias; // Enable smooth drawing

            var rect = ClientRectangle;
            var progressBarRect = new Rectangle(0, 0, rect.Width - 1, rect.Height - 1);
            var progressWidth = (int)(rect.Width * ((double)Value / Maximum));

            // Draw background (Rounded Rectangle)
            using var backgroundPath = CreateRoundedRectanglePath(progressBarRect, CornerRadius);
            using var backBrush = new SolidBrush(BackColor);
            g.FillPath(backBrush, backgroundPath);

            // Draw progress fill (Rounded Rectangle)
            if (progressWidth > 0)
            {
               var progressRect = new Rectangle(0, 0, progressWidth - 1, rect.Height - 1);
               using var progressPath = CreateRoundedRectanglePath(progressRect, CornerRadius);
               using var foreBrush = new SolidBrush(ForeColor);
               g.FillPath(foreBrush, progressPath);
            }
         }
      }

      private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
      {
         var diameter = radius * 2;
         var path = new GraphicsPath();

         if (diameter > rect.Width) diameter = rect.Width;
         if (diameter > rect.Height) diameter = rect.Height;

         var arcRect = rect with { Width = diameter, Height = diameter };

         // Top-left corner
         path.AddArc(arcRect, 180, 90);

         // Top-right corner
         arcRect.X = rect.Right - diameter;
         path.AddArc(arcRect, 270, 90);

         // Bottom-right corner
         arcRect.Y = rect.Bottom - diameter;
         path.AddArc(arcRect, 0, 90);

         // Bottom-left corner
         arcRect.X = rect.Left;
         path.AddArc(arcRect, 90, 90);

         path.CloseFigure();
         return path;
      }
   }
}