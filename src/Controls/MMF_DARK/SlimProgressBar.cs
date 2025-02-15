using System.Drawing.Drawing2D;
using Editor.Controls;

namespace Editor.src.Controls.MMF_DARK
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
            using var backgroundPath = ControlHelper.CreateRoundedRectanglePath(progressBarRect, CornerRadius);
            using var backBrush = new SolidBrush(BackColor);
            g.FillPath(backBrush, backgroundPath);

            // Draw progress fill (Rounded Rectangle)
            if (progressWidth > 0)
            {
               var progressRect = new Rectangle(0, 0, progressWidth - 1, rect.Height - 1);
               using var progressPath = ControlHelper.CreateRoundedRectanglePath(progressRect, CornerRadius);
               using var foreBrush = new SolidBrush(ForeColor);
               g.FillPath(foreBrush, progressPath);
            }
         }
      }

   }
}