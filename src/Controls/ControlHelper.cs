using System.Drawing.Drawing2D;

namespace Editor.Controls
{
   public static class ControlHelper
   {

      public static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
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