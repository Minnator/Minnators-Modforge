using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Editor;

public static class DebugMaps
{
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
         BenchmarkBorderDrawing();
         sw.Stop();
         milisecons.Add(sw.ElapsedMilliseconds);
      }

      Debug.WriteLine($"Average time: {milisecons.Average()} ms");
      Debug.WriteLine($"Max time: {milisecons.Max()} ms");
      Debug.WriteLine($"Min time: {milisecons.Min()} ms");
      Debug.WriteLine("-----------------------------------------------------------");
      Debug.WriteLine("Total time: " + milisecons.Sum() + " ms");
   }

   public static void BenchmarkBorderDrawing()
   {
      //Data.Borders is a Point[][] array
      var borders = Data.Borders.Length;
      var allBordersJoined = Data.Borders.SelectMany(x => x).ToArray();
      var borderMap = new Bitmap(5632, 2048, PixelFormat.Format24bppRgb);
      //long totalTicks = 0;
      //var sw = new Stopwatch();
      //var sw2 = new Stopwatch();
      //sw2.Start();

      for (int i = 0; i < borders; i++)
      {
         //sw.Restart();
         borderMap = DrawBorder(Data.Borders[i], Color.Aquamarine, borderMap);
         //sw.Stop();
         //totalTicks += sw.ElapsedTicks;
      }

      //sw2.Stop();
      //borderMap.Save(@"C:\Users\david\Downloads\ProvBorders.bmp");
      
      //Debug.WriteLine($"Average ms per border: {totalTicks / borders} micro seconds");
      //Debug.WriteLine($"Total time: {sw2.ElapsedMilliseconds} ms");
      //Debug.WriteLine("-----------------------------------------------------------");

      //Draw all borders at once
      //sw.Restart();
      borderMap = DrawBorder(allBordersJoined, Color.Aquamarine, borderMap);
      //sw.Stop();
      //Debug.WriteLine($"Drawing all borders at once took {sw.ElapsedMilliseconds} ms");
      //borderMap.Save(@"C:\Users\david\Downloads\AllProvBorders.bmp");
   }

   public static Bitmap DrawBorder(Point[] points, Color color, Bitmap bmp)
   {
      var rect = GetBoundingBox(points);
      var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

      unsafe
      {
         try
         {
            var r = color.R;
            var g = color.G;
            var b = color.B;
            var scan0 = (byte*)bmpData.Scan0.ToPointer();

            foreach (var point in points)
            {
               var index = (point.Y - rect.Y) * bmpData.Stride + (point.X - rect.X) * 3;

               scan0[index] = b;       // Blue component
               scan0[index + 1] = g;   // Green component
               scan0[index + 2] = r;   // Red component
            }
         }
         finally
         {
            bmp.UnlockBits(bmpData);
         }
      }

      return bmp;
   }

   public static Rectangle GetBoundingBox(Point[] points)
   {
      int minX = points.Min(p => p.X);
      int minY = points.Min(p => p.Y);
      int maxX = points.Max(p => p.X);
      int maxY = points.Max(p => p.Y);

      return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
   }
}