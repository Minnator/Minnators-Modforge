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

   public static Rectangle GetBoundsFloat(ICollection<PointF> points)
   {
      if (points.Count == 0)
         return Rectangle.Empty;
      var minX = float.MaxValue;
      var minY = float.MaxValue;
      var maxX = 0f;
      var maxY = 0f;

      foreach (var point in points)
      {
         if (point.X < minX)
            minX = point.X;
         if (point.X > maxX)
            maxX = point.X;
         if (point.Y < minY)
            minY = point.Y;
         if (point.Y > maxY)
            maxY = point.Y;
      }

      return new ((int)MathF.Floor(minX), (int)MathF.Floor(minY), (int)MathF.Ceiling(maxX - minX + 1), (int)MathF.Ceiling(maxY - minY + 1));
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
   
   public static Rectangle GetBounds(ICollection<Province> provinces)
   {
      List<Rectangle> rects = [];
      foreach (var province in provinces) 
         rects.Add(province.Bounds);
      return GetBounds(rects);
   }

   public static List<Province> GetVisibleCapitals(HashSet<Province> provs)
   {
      List<Province> provinces = [];
      foreach (var p in provs)
         if (Globals.Countries.TryGetValue(p.Owner, out var country) && country.HistoryCountry.Capital == p)
            provinces.Add(p);
      return provinces;
   }

   public static bool IsPointInRectangle(Point point, Rectangle rect)
   {
      return IsPointInRectangle(point, ref rect);
   }

   public static bool IsPointInRectangle(Point point, ref Rectangle rect)
   {
      return point.X >= rect.X && point.X < rect.X + rect.Width && point.Y >= rect.Y && point.Y < rect.Y + rect.Height;
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
      foreach (var province in Globals.Provinces)
      {
         if (RectanglesIntercept(province.Bounds, rect))
            provinces.Add(province.Id);
      }
      return provinces;
   }

   public static ICollection<Province> GetProvincesInRectangle(Rectangle rect)
   {
      HashSet<Province> provinces = [];
      List<SuperRegion> srs = [];
      foreach (var sr in Globals.SuperRegions.Values)
      {
         if (RectanglesIntercept(rect, sr.Bounds))
            srs.Add(sr);
      }

      foreach (var sr in srs)
      {
         foreach (var province in sr.GetProvinces())
         {
            if (RectanglesIntercept(rect, province.Bounds))
               provinces.Add(province);
         }
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
      return [polygon[0], polygon[^1], polygon[^2]];
   }



   public static List<KeyValuePair<List<int>, Rectangle>> GetProvinceConnectedGroups(List<int> ids)
   {
      //Bfs to get all connected provinces
      List<KeyValuePair<List<int>, Rectangle>> groups = [];
   
      /*
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
      */
      return groups;
   }

   public static Point GetBfsCenterPoint(Province prov)
   {
      /*
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
      */
      return new Point();
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

   public static bool GetIfHasStripePixels(Province province, bool onlyRebels, out Point[] stripe)
   {
      if (onlyRebels)
      {
         if (province.Controller != "REB")
         {
            stripe = [];
            return false;
         }
         GetStripesArray(province, out stripe);
         return true;
      }

      if (province is { IsNonRebelOccupied: false} && province.Controller != "REB")
      {
         stripe = [];
         return false;
      }
      GetStripesArray(province, out stripe);
      return true;
   }

   public static void GetStripesArray(Province province, out Point[] stripes)
   {
      switch (Globals.Settings.Rendering.StripesDirection)
      {
         case StripesDirection.Horizontal:
            GetStripeArrayHorizontal(province, out stripes);
            break;
         case StripesDirection.Vertical:
            GetStripeArrayVertical(province, out stripes);
            break;
         case StripesDirection.DiagonalLbRt:
            GetStripeArrayLbRt(province, out stripes);
            break;
         case StripesDirection.DiagonalLtRb:
            GetStripeArrayRbLt(province, out stripes);
            break;
         case StripesDirection.Dotted:
            GetStripesArrayDotted(province, out stripes);
            break;
         case StripesDirection.Pluses:
            GetStripesArrayPluses(province, out stripes);
            break;
         default:
            throw new ();
      }
   }

   private static void GetStripesArrayPluses(Province province, out Point[] stripes)
   {
      List<Point> stripeList = [];
      foreach (var pixel in province.Pixels)
      {
         var moduloX = pixel.X % 8;
         var moduloY = pixel.Y % 8;

         if (moduloX is 7 or 0 || moduloY is 7 or 0 // boundary row
                               || moduloX < 3 && moduloY < 3 // bottom left
                               || moduloX < 3 && moduloY > 4 // top left
                               || moduloX > 4 && moduloY < 3 // bottom right
                               || moduloX > 4 && moduloY > 4) // top right
            continue;
         stripeList.Add(pixel);
      }

      stripes = [.. stripeList];
   }

   private static void GetStripesArrayDotted(Province province, out Point[] stripes)
   {
      List<Point> stripeList = [];

      foreach (var pixel in province.Pixels)
      {
         if ((pixel.X % 8) < 2 && (pixel.Y % 8) < 2)
            stripeList.Add(pixel);
      }

      stripes = [.. stripeList];
   }

   /// <summary>
   /// Returns an array of points that are in the left bottom to right top direction lines
   /// </summary>
   /// <param name="province"></param>
   /// <param name="stripes"></param>
   private static void GetStripeArrayLbRt(Province province, out Point[] stripes)
   {
      List<Point> stripeList = [];

      foreach (var pixel in province.Pixels)
      {
         if (((pixel.X + pixel.Y) % 8) < 2)
            stripeList.Add(pixel);
      }

      stripes = [.. stripeList];
   }

   private static void GetStripeArrayRbLt(Province province, out Point[] stripes)
   {
      List<Point> stripeList = [];

      foreach (var pixel in province.Pixels)
      {
         if (((pixel.X - pixel.Y) % 8) < 2)
            stripeList.Add(pixel);
      }

      stripes = [.. stripeList];
   }

   private static void GetStripeArrayHorizontal(Province province, out Point[] stripes)
   {
      List<Point> stripeList = [];

      foreach (var pixel in province.Pixels)
      {
         if ((pixel.Y % 8) < 2)
            stripeList.Add(pixel);
      }

      stripes = [.. stripeList];
   }

   private static void GetStripeArrayVertical(Province province, out Point[] stripes)
   {
      List<Point> stripeList = [];
      
      foreach (var pixel in province.Pixels)
      {
         if ((pixel.X % 8) < 2)
            stripeList.Add(pixel);
      }

      stripes = [.. stripeList];
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


   public static List<Province> GetAllNeighboringProvinces(List<Province> provinces)
   {
      var neighboringProvinces = new HashSet<Province>();
      var provinceGroup = new HashSet<Province>();

      foreach (var province in provinces) 
         provinceGroup.Add(province);

      foreach (var province in provinces)
      {
         foreach (var neighborId in Globals.AdjacentProvinces[province])
         {
            if (provinceGroup.Contains(neighborId))
               continue;
            neighboringProvinces.Add(neighborId);
         }
      }

      return neighboringProvinces.ToList();
   }

   public static List<Tag> GetAllNeighboringCountries(List<Province> provinces)
   {
      var neighboringCountries = new HashSet<Tag>();
      var provinceGroup = new HashSet<Province>();

      foreach (var province in provinces) 
         provinceGroup.Add(province);

      foreach (var province in provinces)
      {
         foreach (var neighborId in Globals.AdjacentProvinces[province])
         {
            if (provinceGroup.Contains(neighborId))
               continue;
            if (Globals.Countries.TryGetValue(neighborId.Controller, out var country))
               neighboringCountries.Add(country.Tag);
         }
      }

      return neighboringCountries.ToList();
   }

   public static List<Province> GetProvinceIdsInRectangles(ICollection<Rectangle> rects)
   {
      var provinces = new HashSet<Province>();
      foreach (var province in Globals.Provinces)
      {
         foreach (var rect in rects)
         {
            if (Geometry.RectanglesIntercept(province.Bounds, rect))
               provinces.Add(province);
         }
      }
      return provinces.ToList();
   }

   public static List<Province> GetProvincesInRectanglesWithPixelCheck(ICollection<Rectangle> rects)
   {
      HashSet<Province> provinces = [];
      foreach (var province in Globals.Provinces)
      {
         foreach (var rect in rects)
         {
            if (Geometry.RectanglesIntercept(province.Bounds, rect) && CheckIfHasPixelInRectangle(province, rect))
               provinces.Add(province);
         }
      }
      return provinces.ToList();
   }

   public static List<Province> GetProvincesInRectangleWithPixelCheck(Rectangle rect, List<Province> provinces)
   {
      List<Province> result = [];
      foreach (var province in provinces)
      {
         if (Geometry.RectanglesIntercept(province.Bounds, rect) && CheckIfHasPixelInRectangle(province, rect))
            result.Add(province);
      }
      return result;
   }

   /// <summary>
   /// Returns a list of provinces that are fully contained in the triangle and a list of provinces that intersect with the triangle.
   /// </summary>
   /// <param name="triangle"></param>
   /// <param name="provinces"></param>
   /// <returns></returns>
   public static (List<Province>, List<Province>) GetProvincesInTriangleWithPixelCheck(ref Triangle triangle, List<Province> provinces)
   {
      // 15 ms
      List<Province> fullyContains = [];
      List<Province> intersects = [];
      foreach (var province in provinces)
      {
         if (triangle.IntersectsRectangle(province.Bounds))
         {
            if (CheckIfHasPixelInTriangle(province, ref triangle))
               intersects.Add(province);
         }
         else if (triangle.FullyContains(province.Bounds))
            fullyContains.Add(province);
      }
      return (fullyContains, intersects);
   }

   public static List<int> GetProvinceIdsInRectanglesByCenter(ICollection<Rectangle> rects)
   {
      var provinces = new HashSet<int>();
      foreach (var province in Globals.Provinces)
      {
         foreach (var rect in rects)
         {
            if (rect.Contains(province.Center))
               provinces.Add(province.Id);
         }
      }
      return provinces.ToList();
   }

   public static (Rectangle, ICollection<Rectangle>, ICollection<Rectangle>) GetDeltaSelectionRectangle(Rectangle previous, Rectangle newRect)
   {
      var intersectionWidth = Math.Min(previous.Width, newRect.Width);
      var intersectionHeight = Math.Min(previous.Height, newRect.Height);

      var toAdd = new List<Rectangle>();
      var toRemove = new List<Rectangle>();
      Rectangle intersect;

      var previousLarger = previous.Width > newRect.Width;
      var newLarger = previous.Width < newRect.Width;

      if (previous.Location == newRect.Location)
      {
         intersect = new(previous.X, previous.Y, intersectionWidth, intersectionHeight);
         SetRectanglesTopLeftFixed(previous, newRect, previousLarger, toAdd, intersectionWidth);
         SetRectanglesTopLeftFixed(newRect, previous, newLarger, toRemove, intersectionWidth);
      }
      else if (previous.Bottom == newRect.Bottom && previous.Right == newRect.Right)
      {
         intersect = new(previous.X + previous.Width - intersectionWidth, previous.Y + previous.Height - intersectionHeight, intersectionWidth, intersectionHeight);
         SetRectanglesBottomRightFixed(previous, newRect, previousLarger, toAdd, intersectionWidth);
         SetRectanglesBottomRightFixed(newRect, previous, newLarger, toRemove, intersectionWidth);
      }
      else if (previous.Top == newRect.Top && previous.Right == newRect.Right)
      {
         intersect = new(previous.X + previous.Width - intersectionWidth, previous.Y, intersectionWidth, intersectionHeight);
         SetRectanglesTopRightFixed(previous, newRect, previousLarger, toAdd, intersectionWidth);
         SetRectanglesTopRightFixed(newRect, previous, newLarger, toRemove, intersectionWidth);
      }
      else if (previous.Bottom == newRect.Bottom && previous.Left == newRect.Left)
      {
         intersect = new(previous.X, previous.Y + previous.Height - intersectionHeight, intersectionWidth, intersectionHeight);
         SetRectanglesBottomLeftFixed(previous, newRect, previousLarger, toAdd, intersectionWidth);
         SetRectanglesBottomLeftFixed(newRect, previous, newLarger, toRemove, intersectionWidth);
      }
      else
      {
         return (Rectangle.Empty, [previous], [newRect]);
      }
      return (intersect, toAdd, toRemove);
   }

   private static void SetRectanglesTopLeftFixed(Rectangle rect1, Rectangle rect2, bool larger, List<Rectangle> list, int intersection_width)
   {
      if (larger)
         list.Add(new(rect1.X + rect2.Width, rect1.Y, rect1.Width - rect2.Width, rect1.Height));
      if (rect1.Height > rect2.Height)
      {
         var rect = new Rectangle(rect1.X, rect1.Y + rect2.Height, rect1.Width, rect1.Height - rect2.Height);
         if (larger)
            rect = rect with { Width = intersection_width };
         list.Add(rect);
      }
   }

   private static void SetRectanglesBottomRightFixed(Rectangle rect1, Rectangle rect2, bool larger, List<Rectangle> list, int intersection_width)
   {
      if (larger)
         list.Add(new(rect1.X, rect1.Y, rect1.Width - rect2.Width, rect1.Height));
      if (rect1.Height > rect2.Height)
      {
         var rect = new Rectangle(rect1.X, rect1.Y, rect1.Width, rect1.Height - rect2.Height);
         if (larger)
            rect = rect with { X = rect1.X + rect1.Width - intersection_width, Width = intersection_width };
         list.Add(rect);
      }
   }

   private static void SetRectanglesTopRightFixed(Rectangle rect1, Rectangle rect2, bool larger, List<Rectangle> list, int intersection_width)
   {
      if (larger)
         list.Add(new(rect1.X, rect1.Y, rect1.Width - rect2.Width, rect1.Height));
      if (rect1.Height > rect2.Height)
      {
         list.Add(new Rectangle(rect1.X + rect1.Width - intersection_width, rect1.Y + rect2.Height, intersection_width, rect1.Height - rect2.Height));
      }
   }

   private static void SetRectanglesBottomLeftFixed(Rectangle rect1, Rectangle rect2, bool larger, List<Rectangle> list,
      int intersection_width)
   {
      if (larger)
         list.Add(new(rect1.X + rect2.Width, rect1.Y, rect1.Width - rect2.Width, rect1.Height));
      if (rect1.Height > rect2.Height)
      {
         var rect = new Rectangle(rect1.X, rect1.Y, rect1.Width, rect1.Height - rect2.Height);
         if (larger)
            rect = rect with { Width = intersection_width };
         list.Add(rect);
      }


   }

   public static bool CheckIfHasPixelInTriangle(Province province, ref Triangle tri)
   {
      foreach (var pixel in province.Borders)
      {
         if (tri.Contains(pixel))
            return true;
      }
      return false;
   }

   public static bool CheckIfHasPixelInRectangle(Province province, Rectangle rect)
   {
      foreach (var pixel in province.Borders)
      {
         if (rect.Contains(pixel))
            return true;
      }
      return false;
   }



   /// <summary>
   /// 0 = on the line -1 = left side 1 = right side
   /// </summary>
   /// <param name="startPoint"></param>
   /// <param name="connectionVector"></param>
   /// <param name="checkPoint"></param>
   /// <returns></returns>
   public static int OnWhichSideOfLine(ref Point startPoint, ref Point connectionVector, ref Point checkPoint)
   {
      return (checkPoint.X - startPoint.X) * connectionVector.Y - (checkPoint.Y - startPoint.Y) * connectionVector.X;
   }

   public static int ComplexLineCalc(ref Point startPoint, ref Point connectionVector, ref Point checkPoint, int connectionMagSqrt, out bool inside)
   {
      ;
      var testVector = checkPoint.Subtract(ref startPoint);
      var res = testVector.Dot(ref connectionVector);
      inside = res >= 0 && res <= connectionMagSqrt;
      return testVector.X * connectionVector.Y - testVector.Y * connectionVector.X;
   }

   public static List<Province> testLineWithPixel(Point a, Point b, ICollection<Province> provinces)
   {
      List<Province> provs = [];
      foreach (var prov in provinces)
      {
         if (Test(prov, ref a, ref b))
            provs.Add(prov);

      }

      return provs;
   }

   public static bool Test(Province prov, ref Point a, ref Point b)
   {
      var connectionVector = new Point(b.X - a.X, b.Y - a.Y);
      var connectionMagSqrt = connectionVector.Dot(ref connectionVector);
      if (connectionMagSqrt < 1)
         return false;


      var rectA = prov.Bounds.Location;
      var rectB = new Point(prov.Bounds.Right, prov.Bounds.Bottom);
      var rectC = new Point(prov.Bounds.Right, prov.Bounds.Y);
      var rectD = new Point(prov.Bounds.X, prov.Bounds.Bottom);

      if (!prov.Bounds.Contains(a) && !prov.Bounds.Contains(b) && !DoLinesIntersect(rectA, rectB, a, b) && !DoLinesIntersect(rectB, rectC, a, b)
         && !DoLinesIntersect(rectC, rectD, a, b) && !DoLinesIntersect(rectD, rectA, a, b))
         return false;

      var i = 0;

      var side = false;

      for (; i < prov.Borders.Length; i++)
      {
         var res = ComplexLineCalc(ref a, ref connectionVector, ref prov.Borders[i], connectionMagSqrt, out var inside);
         if (inside)
         {
            if (res == 0)
               return true;
            side = res > 0;
            i++;
            break;
         }
      }
      if (i == prov.Borders.Length)
         return false;


      for (; i < prov.Borders.Length; i++)
      {
         var newside = ComplexLineCalc(ref a, ref connectionVector, ref prov.Borders[i], connectionMagSqrt, out var inside);
         if (inside && (newside == 0 || side != newside > 0))
            return true;
      }
      return false;
   }

   public static bool DoLinesIntersect(Point a, Point b, Point c, Point d)
   {
      // Get the direction of the two lines
      var abx = b.X - a.X;
      var aby = b.Y - a.Y;
      var cdx = d.X - c.X;
      var cdy = d.Y - c.Y;

      // Compute the determinant
      var det = (-cdx * aby + abx * cdy);
      if (det == 0)
         return false; // Parallel lines

      // Calculate the position of the intersection on each segment
      var s = (-aby * (a.X - c.X) + abx * (a.Y - c.Y)) * Math.Sign(det);
      var t = (cdx * (a.Y - c.Y) - cdy * (a.X - c.X)) * Math.Sign(det);

      // Check if intersection happens within the line segments
      return (s >= 0 && s <= Math.Abs(det) && t >= 0 && t <= Math.Abs(det));
   }

   public static bool IsRectangleInPolygon(List<Point> polygon, Rectangle rect)
   {
      var rectA = rect.Location;
      var rectB = new Point(rect.Right, rect.Bottom);
      var rectC = new Point(rect.Right, rect.Y);
      var rectD = new Point(rect.X, rect.Bottom);

      return Geometry.IsPointInPolygon(rectA, polygon) && Geometry.IsPointInPolygon(rectB, polygon) && Geometry.IsPointInPolygon(rectC, polygon) && Geometry.IsPointInPolygon(rectD, polygon);
   }

   public static bool IsPointInPolygon(List<Point> polygon, ICollection<Point> points)
   {
      foreach (var p in points)
      {
         if (!Geometry.IsPointInPolygon(p, polygon))
            return false;
      }
      return true;
   }

   public static List<Province> GetFullyContainedProvinces(ref Triangle triangle, ICollection<Province> provinces)
   {
      List<Province> provs = [];
      foreach (var prov in provinces)
      {
         if (triangle.FullyContains(prov.Bounds))
            provs.Add(prov);

      }

      return provs;
   }

}

public struct Triangle
{
   public Point A = Point.Empty;
   public Point B = Point.Empty;
   public Point C = Point.Empty;

   public Triangle(Point a, Point b, Point c)
   {
      A = a;
      B = b;
      C = c;
   }

   public void RollPoints(Point c)
   {
      B = C;
      C = c;
   }

   public bool Contains(Point p)
   {
      var s = A.Y * C.X - A.X * C.Y + (C.Y - A.Y) * p.X + (A.X - C.X) * p.Y;
      var t = A.X * B.Y - A.Y * B.X + (A.Y - B.Y) * p.X + (B.X - A.X) * p.Y;

      if ((s < 0) != (t < 0))
         return false;

      var a = -B.Y * C.X + A.Y * (C.X - B.X) + A.X * (B.Y - C.Y) + B.X * C.Y;
      return a < 0 ? (s <= 0 && s + t >= a) : (s >= 0 && s + t <= a);
   }

   public bool FullyContains(Rectangle rect)
   {
      var rectA = rect.Location;
      var rectB = new Point(rect.Right, rect.Bottom);
      var rectC = new Point(rect.Right, rect.Y);
      var rectD = new Point(rect.X, rect.Bottom);

      return Contains(rectA) && Contains(rectB) && Contains(rectC) && Contains(rectD);
   }

   public bool RectangleFullyContainsTriangle(Rectangle rect)
   {
      return rect.Contains(A) || rect.Contains(B) || rect.Contains(C);
   }

   public bool IntersectsRectangle(Rectangle rect)
   {
      var rectA = rect.Location;
      var rectB = new Point(rect.Right, rect.Bottom);
      var rectC = new Point(rect.Right, rect.Y);
      var rectD = new Point(rect.X, rect.Bottom);

      return IntersectsLine(rectA, rectB) || IntersectsLine(rectB, rectC) || IntersectsLine(rectC, rectD) || IntersectsLine(rectD, rectA);
   }

   public bool IntersectsLine(Point a, Point b)
   {
      return Geometry.DoLinesIntersect(A, B, a, b) || Geometry.DoLinesIntersect(A, C, a, b) || Geometry.DoLinesIntersect(B, C, a, b);
   }

   public static Triangle Empty = new(Point.Empty, Point.Empty, Point.Empty);

   // == operator for two triangles

}
public static class PointExtensions
{
   public static Point Add(this Point point, int x, int y)
   {
      return new Point(point.X + x, point.Y + y);
   }

   public static Point Subtract(this Point point, int x, int y)
   {
      return new Point(point.X - x, point.Y - y);
   }

   public static Point Multiply(this Point point, int x, int y)
   {
      return new Point(point.X * x, point.Y * y);
   }

   public static Point Divide(this Point point, int x, int y)
   {
      return new Point(point.X / x, point.Y / y);
   }

   public static Point Add(this Point point, ref Point other)
   {
      return new Point(point.X + other.X, point.Y + other.Y);
   }

   public static Point Subtract(this Point point, ref Point other)
   {
      return new Point(point.X - other.X, point.Y - other.Y);
   }

   public static Point Subtract(this Point point, Point other)
   {
      return new Point(point.X - other.X, point.Y - other.Y);
   }

   public static Point Multiply(this Point point, ref Point other)
   {
      return new Point(point.X * other.X, point.Y * other.Y);
   }

   public static Point Divide(this Point point, ref Point other)
   {
      return new Point(point.X / other.X, point.Y / other.Y);
   }

   public static int Dot(this Point point, ref Point other)
   {
      return point.X * other.X + point.Y * other.Y;
   }

}