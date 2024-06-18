using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

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
}