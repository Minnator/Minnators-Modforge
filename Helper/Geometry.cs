using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Editor.DataClasses;

namespace Editor.Helper;

public static class Geometry
{
   // Returns true if the two rectangles intersect
   public static bool RectanglesIntercept (Rectangle r1, Rectangle r2)
   {
      return r1.X < r2.X + r2.Width && r1.X + r1.Width > r2.X && r1.Y < r2.Y + r2.Height && r1.Y + r1.Height > r2.Y;
   }

   public static Rectangle GetIntersection (Rectangle r1, Rectangle r2)
   {
      var x = System.Math.Max(r1.X, r2.X);
      var y = System.Math.Max(r1.Y, r2.Y);
      var width = System.Math.Min(r1.X + r1.Width, r2.X + r2.Width) - x;
      var height = System.Math.Min(r1.Y + r1.Height, r2.Y + r2.Height) - y;
      return new Rectangle(x, y, width, height);
   }

   // Returns the center of the rectangle
   public static Point GetCenter (Rectangle rect)
   {
      return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
   }

   // Returns the points of the rectangle Border in clockwise order
   public static Point[] GetRectanglePoints(Rectangle rect)
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

   // Returns the bounding rectangle of the given points
   public static Rectangle GetBounds (Point[] points)
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

   public static Rectangle GetBounds(List<Rectangle> rects)
   {
      if (rects.Count == 0)
         return Rectangle.Empty;
      var minX = rects[0].X;
      var minY = rects[0].Y;
      var maxX = rects[0].X + rects[0].Width;
      var maxY = rects[0].Y + rects[0].Height;

      for (var i = 1; i < rects.Count; i++)
      {
         if (rects[i].X < minX)
            minX = rects[i].X;
         if (rects[i].X + rects[i].Width > maxX)
            maxX = rects[i].X + rects[i].Width;
         if (rects[i].Y < minY)
            minY = rects[i].Y;
         if (rects[i].Y + rects[i].Height > maxY)
            maxY = rects[i].Y + rects[i].Height;
      }
      return new Rectangle(minX, minY, maxX - minX, maxY - minY);
   }

   // Clamps the given value between the given min and max
   public static T Clamp<T> (T value, T min, T max) where T : System.IComparable<T>
   {
      if (value.CompareTo(min) < 0)
         return min;
      if (value.CompareTo(max) > 0)
         return max;
      return value;
   }

   public static List<int> GetProvinceIdsInRectangle (Rectangle rect)
   {
      var provinces = new List<int>();
      foreach (var province in Globals.Provinces.Values)
      {
         if (RectanglesIntercept(province.Bounds, rect))
            provinces.Add(province.Id);
      }
      return provinces;
   }

   public static List<Province> GetProvincesInRectangle(Rectangle rect)
   {
      var provinces = new List<Province>();
      foreach (var province in Globals.Provinces.Values)
      {
         if (RectanglesIntercept(province.Bounds, rect))
            provinces.Add(province);
      }
      return provinces;
   }

   public static List<int> GetProvincesInPolygon(List<Point> polygon)
   {
      var provinces = new List<int>();
      var polyBounds = GetBounds([.. polygon]);
      foreach (var province in GetProvincesInRectangle(polyBounds))
      {
         if (IsInPolygon(polygon, province.Center))
            provinces.Add(province.Id);
      }
      return provinces;
   }

   // Returns true if the given point is inside the polygon
   public static bool IsInPolygon(List<Point> polygon, Point point)
   {
      return IsPointInPolygon(point, polygon);
   }

   // Method to check if a point is within a polygon, if the point lies on the left for an uneven number of edges it is inside the polygon
   public static bool IsPointInPolygon(Point point, List<Point> polygon)
   {
      var isInside = false;
      var count = polygon.Count;
      for (int i = 0, j = count - 1; i < count; j = i++)
      {
         if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
             (point.X < ((polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (float)(polygon[j].Y - polygon[i].Y) + polygon[i].X)))
         {
            isInside = !isInside;
         }
      }
      return isInside;
   }

   public static List<Point> GetPolygonDiff(List<Point> polygon, List<Point> diff)
   {
      var diffPolygon = new List<Point>();
      foreach (var point in polygon)
      {
         if (!diff.Contains(point))
            diffPolygon.Add(point);
      }
      return diffPolygon;
   }

   public static List<Point> GetPolygonDiffLastPoint(List<Point> polygon)
   {
      return [polygon[0], polygon[polygon.Count - 1], polygon[polygon.Count - 2]];
   }

   public static Rectangle GetBounds(List<int> selection)
   {
      return GetBounds(selection.Select(id => Globals.Provinces[id].Bounds.Location).ToArray());
   }


   public static void GetAllPixelPoints(int[] provinceIds, out Point[] points)
   {
      var cnt = 0;
      foreach (var p in provinceIds)
      {
         cnt += Globals.Provinces[p].PixelCnt;
      }
      points = new Point[cnt];
      var index = 0;
      foreach (var p in provinceIds)
      {
         var prov = Globals.Provinces[p];
         Array.Copy(Globals.Pixels, prov.PixelPtr, points, index, prov.PixelCnt);
         index += prov.PixelCnt;
      }
   }

   public static void GetAllPixelPtrs(int[] ids, out int[,] ptrs)
   {
      ptrs = new int[ids.Length, 2];
      for (var i = 0; i < ids.Length; i++)
      {
         var prov = Globals.Provinces[ids[i]];
         ptrs[i, 0] = prov.PixelPtr;
         ptrs[i, 1] = prov.PixelCnt;
      }
   }

   public static Point[] GetAllBorderPoints(int[] provinceIds)
   {
      var cnt = 0;
      foreach (var p in provinceIds)
      {
         cnt += Globals.Provinces[p].BorderCnt;
      }
      var points = new Point[cnt];
      var index = 0;
      foreach (var p in provinceIds)
      {
         var prov = Globals.Provinces[p];
         Array.Copy(Globals.BorderPixels, prov.BorderPtr, points, index, prov.BorderCnt);
         index += prov.BorderCnt;
      }
      return points;
   }

}