using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;

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
      var x = Math.Max(r1.X, r2.X);
      var y = Math.Max(r1.Y, r2.Y);
      var width = Math.Min(r1.X + r1.Width, r2.X + r2.Width) - x;
      var height = Math.Min(r1.Y + r1.Height, r2.Y + r2.Height) - y;
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
   public static T Clamp<T> (T value, T min, T max) where T : IComparable<T>
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

   public static List<KeyValuePair<List<int>, Rectangle>> GetProvinceConnectedGroups(List<int> ids)
   {
      //Bfs to get all connected provinces
      List<KeyValuePair<List<int>, Rectangle>> groups = [];
      var visited = new HashSet<int>();

      foreach (var id in ids)
      {
         if (visited.Contains(id))
            continue;

         var group = new List<int>();
         var bounds = Rectangle.Empty;
         var queue = new Queue<int>();

         queue.Enqueue(id);
         visited.Add(id);

         while (queue.Count > 0)
         {
            var currentId = queue.Dequeue();
            var province = Globals.Provinces[currentId];

            group.Add(currentId);
            bounds = bounds == Rectangle.Empty ? province.Bounds : GetBounds([bounds, province.Bounds]);

            for (var index = 0; index < Globals.AdjacentProvinces[currentId].Length; index++)
            {
               var neighborId = Globals.AdjacentProvinces[currentId][index];
               if (ids.Contains(neighborId) && !visited.Contains(neighborId))
               {
                  queue.Enqueue(neighborId);
                  visited.Add(neighborId);
               }
            }
         }

         groups.Add(new(group, bounds));
      }

      return groups;
   }

   public static Point GetBfsCenterPoint(Province prov)
   {
      var bestPoint = new Point(-1, -1);
      var lowestCost = int.MinValue;

      var pixels = new Point[prov.PixelCnt];
      Array.Copy(Globals.Pixels, prov.PixelPtr, pixels, 0, prov.PixelCnt);
      var borderPixels = new HashSet<Point>();
      var temp = new Point[prov.BorderCnt];
      Array.Copy(Globals.BorderPixels, prov.BorderPtr, temp, 0, prov.BorderCnt);

      foreach (var point in temp)
      {
         borderPixels.Add(point);
      }

      foreach (var point in pixels)
      {
         var distances = GetBorderPoints(point, prov, ref borderPixels);
         var cost = GetCostPerPixel(distances);
         if (cost > lowestCost)
         {
            lowestCost = cost;
            bestPoint = point;
         }
      }

      return bestPoint;
   }

   public static int GetCostPerPixel(int[] distances)
   {
      int sum = 1;
      int diffs = 0;

      for (int i = 0; i < distances.Length; i++)
      {
         if (distances[i] < 0)
            return int.MinValue;
         sum *= distances[i];
         for (int j = i + 1; j < distances.Length; j++)
         {
            diffs += Math.Abs(distances[i] - distances[j]);
         }
      }
      return 2 * sum - diffs;
   }

   //TODO does weird stuff idk
   public static int[] GetBorderPoints(Point p, Province province, ref HashSet<Point> bps)
   {
      // goes west, east, north, south until it finds a border pixel
      var points = new int[4];

      if (bps.Contains(p))
         return [-1,-1,-1,-1];
      
      var y = p.Y;
      var x = p.X;

      while (y < province.Bounds.Bottom) // N
      {
         if (bps.Contains(new Point(x, y)))
         {
            points[0] = y - p.Y;
            break;
         }
         y++;
      }

      y = p.Y;
      while (y > 0) // S
      {
         if (bps.Contains(new Point(x, y)))
         {
            points[1] = p.Y - y;
            break;
         }

         y--;
      }

      y = p.Y;

      while (x < province.Bounds.Right) // E
      {
         if (bps.Contains(new Point(x, y)))
         {
            points[2] = x - p.X;
            break;
         }

         x++;
      }

      x = p.X;
      while (x > 0) // W
      {
         if (bps.Contains(new Point(x, y)))
         {
            points[3] = p.X - x;
            break;
         }
         x--;
      }

      return points;
   }



   public static bool GetIfHasStripePixels(Province province, bool onlyRebels, out Point[] stripe)
   {
      if (onlyRebels)
      {
         if (province.Controller != "REB")
         {
            stripe = [];
            return false;
         }
         stripe = GetStripeArray(province);
         return true;
      }

      if (province is { IsNonRebelOccupied: false} && province.Controller != "REB")
      {
         stripe = [];
         return false;
      }
      stripe = GetStripeArray(province);
      return true;
   }

   private static Point[] GetStripeArray(Province province)
   {
      List<Point> stripeList = [];
      var ptr = province.PixelPtr;
      for (var i = 0; i < province.PixelCnt; i++)
      {
         var pixel = Globals.Pixels[ptr + i];
         if (((pixel.X + pixel.Y) % 8) > 2)
            stripeList.Add(pixel);
      }

      return [.. stripeList];
   }

   // Method to find a center point within the area
   public static Point FindCenterPoint(Province province)
   {
      var areaPoints = new Point[province.PixelCnt];
      Array.Copy(Globals.Pixels, province.PixelPtr, areaPoints, 0, province.PixelCnt);

      // Calculate the centroid of the area points
      double xSum = 0;
      double ySum = 0;

      foreach (var point in areaPoints)
      {
         xSum += point.X;
         ySum += point.Y;
      }

      Point centroid = new Point((int)(xSum / areaPoints.Length), (int)(ySum / areaPoints.Length));

      // If the centroid is within the area, return it
      if (IsPointInProvince(centroid, ref areaPoints))
      {
         if (province.Center != centroid)
            Debug.WriteLine($"Found a better province center {province.Id}");
         province.Center = centroid;
         return centroid;
      }

      // If not, find the nearest point within the area
      Point nearestPoint = FindNearestPointWithinArea(centroid, ref areaPoints);

      province.Center = nearestPoint;
      return nearestPoint;
   }


   public static bool IsPointInProvince(Point point, ref Point[] pixels)
   {
      var isInBounds = point.X >= 0 && point.X < Globals.MapWidth && point.Y >= 0 && point.Y < Globals.MapHeight;
      return isInBounds && pixels.Contains(point);
   }

   // Method to find the nearest point within the area
   public static Point FindNearestPointWithinArea(Point point, ref Point[] areaPoints)
   {
      Point nearestPoint = areaPoints[0];
      double minDistance = double.MaxValue;

      foreach (var areaPoint in areaPoints)
      {
         double distance = Math.Sqrt(Math.Pow(areaPoint.X - point.X, 2) + Math.Pow(areaPoint.Y - point.Y, 2));
         if (distance < minDistance)
         {
            minDistance = distance;
            nearestPoint = areaPoint;
         }
      }

      return nearestPoint;
   }

}