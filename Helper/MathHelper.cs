using System.Drawing;

namespace Editor.Helper;

public static class MathHelper
{
   public static bool RectanglesIntercept (Rectangle r1, Rectangle r2)
   {
      return r1.X < r2.X + r2.Width && r1.X + r1.Width > r2.X && r1.Y < r2.Y + r2.Height && r1.Y + r1.Height > r2.Y;
   }

   public static Point[] GetAllPointsOnRectangleBorder(Rectangle rect)
   {
      if (rect.Width < 2 || rect.Height < 2)
         return [];
      var points = new Point[2 * rect.Width + 2 * rect.Height - 4];
      var index = 0;
      for (var x = rect.X; x < rect.X + rect.Width; x++)
      {
         points[index++] = new Point(x, rect.Y);
         points[index++] = new Point(x, rect.Y + rect.Height - 1);
      }
      for (var y = rect.Y + 1; y < rect.Y + rect.Height - 1; y++)
      {
         points[index++] = new Point(rect.X, y);
         points[index++] = new Point(rect.X + rect.Width - 1, y);
      }
      return points;
   }

   public static Rectangle GetBoundingBox (Point[] points)
   {
      if (points.Length == 0)
         return Rectangle.Empty;
      var minX = points[0].X;
      var minY = points[0].Y;
      var maxX = points[0].X;
      var maxY = points[0].Y;

      for (var i = 1; i < points.Length; i++)
      {
         if (points[i].X < minX)
            minX = points[i].X;
         if (points[i].X > maxX)
            maxX = points[i].X;
         if (points[i].Y < minY)
            minY = points[i].Y;
         if (points[i].Y > maxY)
            maxY = points[i].Y;
      }

      return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
   }

   public static T Clamp<T> (T value, T min, T max) where T : System.IComparable<T>
   {
      if (value.CompareTo(min) < 0)
         return min;
      if (value.CompareTo(max) > 0)
         return max;
      return value;
   }
}