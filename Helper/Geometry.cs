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
      foreach (var province in Data.Provinces.Values)
      {
         if (RectanglesIntercept(province.Bounds, rect))
            provinces.Add(province.Id);
      }
      return provinces;
   }

   public static List<Province> GetProvincesInRectangle(Rectangle rect)
   {
      var provinces = new List<Province>();
      foreach (var province in Data.Provinces.Values)
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
             (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
         {
            isInside = !isInside;
         }
      }
      return isInside;
   }

	public static List<int> GetPolygonDiff(List<int> p1, List<int> p2)
	{
		var diff = new List<int>();
		var p2Set = new HashSet<int>(p2);
      
		foreach (var id in p1)
		{
			if (!p2Set.Contains(id))
				diff.Add(id);
		}

		return diff;
	}

   public static List<int> GetPolygonDiffLasPoint(List<int> polygon)
   {
      return [polygon[0], polygon[polygon.Count - 1], polygon[polygon.Count - 2]];
   }
}