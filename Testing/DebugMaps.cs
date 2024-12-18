namespace Editor.Testing;

public static class DebugMaps
{
   /*
   public static void YoloDefinition()
   {
      List<Province> europeanProvinces = [];

      foreach (var province in Globals.Provinces.Values)
      {
         if (province.Continent == "europe")
            europeanProvinces.Add(province);
      }

      Debug.WriteLine($"Found {europeanProvinces.Count} provinces in Europe");

      var sb = new StringBuilder();
      var cnt = 1;
      foreach (var province in europeanProvinces)
      {
         sb.Append(cnt);
         sb.Append(";");
         sb.Append(province.Color.R);
         sb.Append(";");
         sb.Append(province.Color.G);
         sb.Append(";");
         sb.Append(province.Color.B);
         sb.Append(";");
         sb.Append(province.GetTitleLocalisation());
         sb.Append(";");
         sb.Append("x");
         sb.Append(";\n");

         cnt ++;
      }

      File.WriteAllText("C:\\Users\\david\\Downloads\\europeanProvinces.csv", sb.ToString());
   }

   public static Bitmap GenerateBitmapFromProvinces()
   {
      var sw = new Stopwatch();
      sw.Start();
      var width = Globals.MapWidth;
      var height = Globals.MapHeight;

      var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

      // Lock the bits in memory
      var bitmapData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);

      // Calculate stride (bytes per row)
      var stride = bitmapData.Stride;
      var bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;

      unsafe
      {

         Parallel.ForEach(Globals.Provinces.Values, province => // TODO transfer the collection per method call
         {
            var borderPixels = new HashSet<Point>();
            var temp = new Point[province.BorderCnt];
            Array.Copy(Globals.BorderPixels, province.BorderPtr, temp, 0, province.BorderCnt);

            foreach (var point in temp) 
               borderPixels.Add(point);

            var points = new Point[province.PixelCnt];
            Array.Copy(Globals.Pixels, province.PixelPtr, points, 0, province.PixelCnt);

            int min = int.MaxValue;
            int max = int.MinValue;
            var ptr = (byte*)bitmapData.Scan0;
            var costs = new int[province.PixelCnt];

            for (var index = 0; index < points.Length; index++)
            {
               var point = points[index];
               var dist = Geometry.GetBorderPoints(point, province, ref borderPixels);
               var cost = Geometry.GetCostPerPixel(dist);
               costs[index] = cost;

               if (cost < min)
               {
                  if (cost != int.MinValue)
                     min = cost;
               }
               else if (cost > max)
               {
                  max = cost;
               }
            }
            

            for (var index = 0; index < costs.Length; index++)
            {
               var cost = costs[index];
               if (cost == int.MinValue)
               {
                  costs[index] = 0;
                  continue;
               }
               if (min == max)
                  costs[index] = 0;
               else
               {
                  costs[index] = (int)(255 * ((float)(costs[index] - min)) / (max - min));
                  if (costs[index] > 255)
                     costs[index] = 255;
                  else if (costs[index] < 0)
                     costs[index] = 0;
               }
            }

            var cnt = 0;
            foreach (var point in points)
            { 
               var index = point.Y * stride + point.X * bytesPerPixel;

               ptr[index + 2] = (byte)costs[cnt++];
               ptr[index + 1] = 0;
               ptr[index] = 0;
            }
         });
      }

      bmp.UnlockBits(bitmapData);
      sw.Stop();
      //Debug.WriteLine($"Generating Bitmap took {sw.ElapsedMilliseconds}ms");
      return bmp;
   }



   public static void TestCenterPoints()
   {
      var sw = Stopwatch.StartNew();
      var bmp1 = GenerateBitmapFromProvinces();
      MapDrawHelper.DrawAllProvinceBorders(bmp1, Color.LightBlue);
      bmp1.Save("C:\\Users\\david\\Downloads\\bestPoints.png", ImageFormat.Png);

      sw.Stop();
      Debug.WriteLine($"TestCenterPoints: {sw.ElapsedMilliseconds} ms");
      return;
      var bestPoints = new Point[Globals.LandProvinceIds.Length];
      var cnt = 0;
      foreach (var province in Globals.LandProvinceIds)
      {
         var prov = Globals.Provinces[province];
         //if (province == 1473)
           // Debugger.Break();
         bestPoints[cnt] = Geometry.GetBfsCenterPoint(prov);
         if (cnt % 100 == 0)
            Debug.WriteLine($"Province {cnt} / {Globals.LandProvinceIds.Length} has this center {bestPoints[cnt]} pixels");
         cnt++;
      }

      using var bmp = BitMapHelper.GenerateBitmapFromProvinces(GetProvinceMapColor);
      MapDrawHelper.DrawAllProvinceBorders(bmp, Color.Black);
      var g = Graphics.FromImage(bmp);
      foreach (var point in bestPoints)
      {
         g.FillRectangle(Brushes.Red, point.X - 1, point.Y - 1, 2, 2);
      }

      sw.Stop();
      Debug.WriteLine($"TestCenterPoints: {sw.ElapsedMilliseconds} ms");

      bmp.Save("C:\\Users\\david\\Downloads\\bestPoints.png", ImageFormat.Png);
   }

   public static Color GetProvinceMapColor(int id)
   {
      if (Globals.Provinces.TryGetValue(id, out var prov))
      {
         return prov.Color;
      }
      return Color.Black;
   }


   public static void DrawProvinceGroups(List<KeyValuePair<List<int>, Rectangle>> groups)
   {
      //using var adjMap = new Bitmap (Globals.MapWidth, Globals.MapHeight);
      //MapDrawHelper.DrawAllProvinceBorders(adjMap, Color.Black);
      //DrawAdjacencyNumbers(adjMap);
      using var bmp = new Bitmap(Globals.MapWidth, Globals.MapHeight);
      using var g = Graphics.FromImage(bmp);
      var rand = new Random();

      foreach (var group in groups)
      {
         var color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
         foreach (var id in group.Key)
         {
            if (Globals.Provinces.TryGetValue(id, out var prov))
            {
               MapDrawHelper.DrawProvince(prov.Id, color, bmp);
            }
         }
      }
      MapDrawHelper.DrawAllProvinceBorders(bmp, Color.Black);

      bmp.Save("C:\\Users\\david\\Downloads\\groups.png", ImageFormat.Png);
   }




   public static unsafe void TelescopeImageBenchmark()
   {
      var sw = Stopwatch.StartNew();
      var bmp = new Bitmap(9000, 6000, PixelFormat.Format24bppRgb);
      var width = bmp.Width;
      var height = bmp.Height;
      var bitmapData = bmp.LockBits(new(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
      var scan0 = (byte*)bitmapData.Scan0.ToPointer();
      for (var i = 0; i < 2000; i++)
      {
         DrawEntireMap(6000, 9000, scan0, ref bitmapData);
         //Console.WriteLine($"Rendering Bitmap {i,3}");
      }
      bmp.UnlockBits(bitmapData);
      sw.Stop();
      Debug.WriteLine($"TelescopeImageBenchmark: {(sw.ElapsedMilliseconds / 2000f)} ms total {sw.ElapsedMilliseconds}");
      bmp.Save("C:\\Users\\david\\Downloads\\telescope.png", ImageFormat.Png);
   }

   public static unsafe void DrawEntireMap(int height, int width, byte* scan0, ref BitmapData bitmapData)
   {

      var stride = bitmapData.Stride;

      var paralellOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount};

      // Parallelize the iteration over all pixels
      Parallel.For(0, height, paralellOptions, y =>
      {
         for (var x = 0; x < width; x++)
         {
            var index = y * stride + x * 3;
            scan0[index] = 128; // Blue component
         }
      });
   }

   public static void MapModeDrawing()
   {
      var sw = Stopwatch.StartNew();

      var bmp = new Bitmap(MapModeManager.GetMapMode("Provinces").Bitmap);
      BitMapHelper.WriteOnProvince(GetProvinceIdString, bmp);
      bmp.Save("C:\\Users\\david\\Downloads\\areas.png", ImageFormat.Png);
      sw.Stop();
      Debug.WriteLine($"MapModeDrawing: {sw.ElapsedMilliseconds} ms");
   }

   private static string GetProvinceIdString(int id)
   {
      return id.ToString();
   }

   public static void MapModeDrawing3()
   {
      List<IProvinceCollection> areas = [.. Globals.Areas.Values];
      var sw = Stopwatch.StartNew();

      var bmp = BitMapHelper.GenerateBitmapFromProvinceCollection(areas);

      MapDrawHelper.DrawAllProvinceBorders(bmp, Color.Black);
      bmp.Save("C:\\Users\\david\\Downloads\\areas12.png", ImageFormat.Png);
      sw.Stop();
      Debug.WriteLine($"MapModeDrawing: {sw.ElapsedMilliseconds} ms");
      DrawAreasOnMap();
      Debug.WriteLine($"----------------------------------------------");
   }



   public static void DrawAreasOnMap()
   {
      var sw = Stopwatch.StartNew();

      var bmp = BitMapHelper.GenerateBitmapFromProvinces(GetColorArea);
      
      MapDrawHelper.DrawAllProvinceBorders(bmp, Color.Black);
      bmp.Save("C:\\Users\\david\\Downloads\\areas.png", ImageFormat.Png);
      sw.Stop();
      Debug.WriteLine($"DrawAreasOnMap: {sw.ElapsedMilliseconds} ms");
   }
   public static void MapModeDrawing2()
   {
      var sw = Stopwatch.StartNew();

      var bmp = new Bitmap(Globals.MapWidth, Globals.MapHeight);
      foreach (var province in Globals.Provinces.Values)
      {
         MapDrawHelper.DrawProvince(province.Id, province.Color, bmp);
      }
      MapDrawHelper.DrawAllProvinceBorders(bmp, Color.Black);

      bmp.Save("C:\\Users\\david\\Downloads\\areas.png", ImageFormat.Png);
      sw.Stop();
      Debug.WriteLine($"MapModeDrawing2: {sw.ElapsedMilliseconds} ms");
   }

   public static Color GetColorArea(int id)
   {
      if (Globals.Provinces.TryGetValue(id, out var prov))
      {
         if (Globals.Areas.TryGetValue(prov.Area, out var area))
         {
            return area.Color;
         }
      }
      return Color.Black;
   }

   public static void Test()
   {
      var sw = Stopwatch.StartNew();
      var provincePixels = Globals.Provinces.Values.SelectMany(province =>
      {
         var points2 = new Point[province.PixelCnt];
         Array.Copy(Globals.Pixels, province.PixelPtr, points2, 0, province.PixelCnt);
         return points2;
      }).ToArray();
      
      sw.Stop();
      Debug.WriteLine($"Test: {sw.ElapsedMilliseconds} ms {provincePixels.Length}");
      var ids = Globals.Provinces.Values.Select(province => province.Id).ToArray();
      sw.Restart();
      Geometry.GetAllPixelPoints(ids, out var points);
      sw.Stop();
      Debug.WriteLine($"Test2: {sw.ElapsedMilliseconds} ms {points.Length}");
   }



   public static void DrawAreasOnMap2()
   {
      var sw = Stopwatch.StartNew();
      var bmp = new Bitmap(Globals.MapWidth, Globals.MapHeight, PixelFormat.Format32bppRgb);
      using var g = Graphics.FromImage(bmp);
      var rand = new Random();

      foreach (var area in Globals.Areas.Values)
      {
         var color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
         for (int i = 0; i < area.Provinces.Length; i++)
         {
            var prov = Globals.Provinces[area.Provinces[i]];
            var points = new Point[prov.BorderCnt];
            Array.Copy(Globals.BorderPixels, prov.BorderPtr, points, 0, prov.BorderCnt);
            g.FillPolygon(new SolidBrush(color), points);
         }
      }

      sw.Stop();
      Debug.WriteLine($"DrawAreasOnMap: {sw.ElapsedMilliseconds} ms");

      bmp.Save("C:\\Users\\david\\Downloads\\areas.png", ImageFormat.Png);
   }








   public static unsafe void DrawAllBorder(ConcurrentDictionary<Color, List<Point>> points, Size size, string saveTo)
   {
      // Create a new Bitmap with specified size
      using var bmp = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);

      // Lock the bitmap data
      BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);

      try
      {
         // Pointer to the start of bitmap data
         byte* scan0 = (byte*)bmpData.Scan0.ToPointer();

         // Parallel processing of points
         Parallel.ForEach(points, kvp =>
         {
            Color color = kvp.Key;
            List<Point> pointList = kvp.Value;

            foreach (var point in pointList)
            {
               int index = point.Y * bmpData.Stride + point.X * 3; // Calculate pixel index

               // Set pixel color at calculated index
               scan0[index + 2] = color.R;   // Red component
               scan0[index + 1] = color.G;   // Green component
               scan0[index] = color.B;       // Blue component
            }
         });
      }
      finally
      {
         // Unlock the bitmap data
         bmp.UnlockBits(bmpData);
      }

      // Save the bitmap
      bmp.Save(saveTo, ImageFormat.Bmp);
   }

   public static void Bench()
   {
      var sw = new Stopwatch();
      var milisecons = new List<long>();

      for (int i = 0; i < 100; i++)
      {
         sw.Restart();
         sw.Stop();
         milisecons.Add(sw.ElapsedMilliseconds);
      }

      Debug.WriteLine($"Average time: {milisecons.Average()} ms");
      Debug.WriteLine($"Max time: {milisecons.Max()} ms");
      Debug.WriteLine($"Min time: {milisecons.Min()} ms");
      Debug.WriteLine("-----------------------------------------------------------");
      Debug.WriteLine("Total time: " + milisecons.Sum() + " ms");
   }

   public static void DrawAdjacencyNumbers(Bitmap bmp1)
   {
      var bmp = new Bitmap(bmp1);
      byte col = 0;

      using var g = Graphics.FromImage(bmp);

      unsafe
      {

         // Draw the adjacency numbers on the provinces
         foreach (var prov in Globals.Provinces.Values)
         {
            if (Globals.AdjacentProvinces.TryGetValue(prov.Id, out var province))
            {
               var str = $"{province.Length}";
               var font = new Font("Arial", 8);
               var size = g.MeasureString(str, font);
               var pointS = new Point(prov.Center.X - (int)size.Width / 2, prov.Center.Y - (int)size.Height / 2);

               g.DrawString(str, font, Brushes.Black, pointS);
            }
            if (prov.BorderCnt < 4)
               continue;
            var points = new Point[prov.BorderCnt];
            Array.Copy(Globals.BorderPixels, prov.BorderPtr, points, 0, prov.BorderCnt);
            var bmpData = bmp.LockBits(prov.Bounds, ImageLockMode.ReadWrite, bmp.PixelFormat);
            var scan0 = (byte*)bmpData.Scan0.ToPointer();
            foreach (var point in points)
            {
               var index = (point.Y - prov.Bounds.Y) * bmpData.Stride + (point.X - prov.Bounds.X) * 4;

               scan0[index] = col;       // Blue component
               scan0[index + 1] = col;   // Green component
               scan0[index + 2] = col;   // Red component
            }
            bmp.UnlockBits(bmpData);
         }
      }

      bmp.Save("C:\\Users\\david\\Downloads\\adjacency.png", ImageFormat.Png);
      bmp.Dispose();
   }

   public static void PrintProvinceTypeMap()
   {
      var bmp = BitMapHelper.GenerateBitmapFromProvinces(id =>
      {
         if (Globals.Provinces.TryGetValue(id, out var prov))
         {
            if (Globals.LandProvinces.Contains(prov.Id))
               return Color.Green;
            if (Globals.SeaProvinces.Contains(prov.Id))
               return Color.Blue;
            if (Globals.LakeProvinces.Contains(prov.Id))
               return Color.LightBlue;
            if (Globals.CoastalProvinces.Contains(prov.Id))
               return Color.Yellow;
         }
         return Color.Black;
      });

      bmp.Save("C:\\Users\\david\\Downloads\\provinceTypeMap.png", ImageFormat.Png);
      bmp.Dispose();
   }

   public static void AreasToMap()
   {
      Dictionary<string, Color> color = [];
      var rand = new Random();
      
      foreach (var area in Globals.Areas.Values)
         color.Add(area.GenericName, Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256)));

      var bmp = BitMapHelper.GenerateBitmapFromProvinces(id =>
      {
         if (Globals.Provinces.TryGetValue(id, out var prov))
         {
            if (Globals.Areas.TryGetValue(prov.Area, out var area))
               return color[area.GenericName];
         }
         return Color.Black;
      });

      bmp.Save("C:\\Users\\david\\Downloads\\areas.png", ImageFormat.Png);
      bmp.Dispose();
   }

   public static void CentroidPoints()
   {
      var sw = Stopwatch.StartNew();
      var centroids = new Point[Globals.Provinces.Count];
      var cnt = 0;

      foreach (var province in Globals.Provinces.Values)
      {
         centroids[cnt] = Geometry.FindCenterPoint(province);
         cnt++;
      }

      using var bmp = BitMapHelper.GenerateBitmapFromProvinces(GetProvinceColor);
      var g = Graphics.FromImage(bmp);
      foreach (var point in centroids)
      {
         g.FillRectangle (Brushes.Red, point.X - 1, point.Y - 1, 2, 2);
      }

      bmp.Save("C:\\Users\\david\\Downloads\\centroids.png", ImageFormat.Png);
      sw.Stop();
      Debug.WriteLine($"CentroidPoints: {sw.ElapsedMilliseconds} ms");

      foreach (var province in Globals.Provinces.Values)
      {
         var points = new Point[province.PixelCnt];
         Array.Copy(Globals.Pixels, province.PixelPtr, points, 0, province.PixelCnt);
         if (!Geometry.IsPointInProvince(province.Center, ref points))
         {
            Debug.WriteLine($"Province {province.Id} is not in the center");
         }
      }
   }

   public static Color GetProvinceColor(int id)
   {
      if (Globals.Provinces.TryGetValue(id, out var prov))
      {
         if (Globals.Areas.TryGetValue(prov.Area, out var area))
         {
            return area.Color;
         }
      }
      return Color.Black;
   }

   public static void GridMap()
   {
      var bmp = new Bitmap(Globals.MapWidth, Globals.MapHeight, PixelFormat.Format24bppRgb);
      //var sw = Stopwatch.StartNew();
      var width = bmp.Width;
      var height = bmp.Height;

      // Lock the bits in memory
      var bitmapData = bmp.LockBits(new(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);

      // Calculate stride (bytes per row)
      var stride = bitmapData.Stride;
      var bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;

      var parallelOptions = new ParallelOptions
      {
         MaxDegreeOfParallelism = Environment.ProcessorCount
      };

      unsafe
      {
         Parallel.For(0, height, parallelOptions, y =>
         {
            var ptr = (byte*)bitmapData.Scan0;
            for (int x = 0; x < width; x++)
            {
               Color color = Color.DarkGray;
               if (y % 100 == 0)
                  color = Color.Black;
               else if (x % 100 == 0)
                  color = Color.Black;

               var index = y * stride + x * bytesPerPixel;
               
               ptr[index + 2] = color.R;
               ptr[index + 1] = color.G;
               ptr[index] = color.B;
            }
         });
      }

      bmp.UnlockBits(bitmapData);
      bmp.Save("C:\\Users\\david\\Downloads\\gridMap.png", ImageFormat.Png);
      //sw.Stop();
      //Debug.WriteLine($"Modifying Bitmap took {sw.ElapsedMilliseconds} ms");
   }
   */
}