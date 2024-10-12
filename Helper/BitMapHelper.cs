using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Editor.DataClasses.GameDataClasses;
using Editor.Interfaces;

namespace Editor.Helper;

public static class BitMapHelper
{

   /// <summary>
   /// Iterates over all provinces and writes the result of the method on the province
   /// </summary>
   /// <param name="method"></param>
   /// <param name="bmp"></param>
   public static void WriteOnProvince(Func<int, string> method, Bitmap bmp)
   {
      var font = new Font("Arial", 6);
      var brush = new SolidBrush(Color.Black);
      var graphics = Graphics.FromImage(bmp);

      foreach (var province in Globals.Provinces)
      {
         var text = method(province.Id);
         if (string.IsNullOrEmpty(text))
            continue;
         var textWidth = (int)graphics.MeasureString(text, font).Width;
         var textHeight = (int)graphics.MeasureString(text, font).Height;

         graphics.DrawString(text, font, brush, province.Center.X - textWidth / 2, province.Center.Y - textHeight / 2);
      }

   }


   public static void ModifyByProvinceCollection(Bitmap bmp, ICollection<Province> provinces, Func<Province, Color> method)
   {
      //var sw = Stopwatch.StartNew();
      var width = bmp.Width;
      var height = bmp.Height;

      // Lock the bits in memory
      var bitmapData = bmp.LockBits(new (0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);

      // Calculate stride (bytes per row)
      var stride = bitmapData.Stride;
      var bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;

      var parallelOptions = new ParallelOptions
      {
         MaxDegreeOfParallelism = Environment.ProcessorCount
      };

      unsafe
      {
         Parallel.ForEach(provinces, parallelOptions ,province =>
         {
            var points = new Point[province.PixelCnt];
            Array.Copy(Globals.Pixels, province.PixelPtr, points, 0, province.PixelCnt);

            var color = method(province);

            var ptr = (byte*)bitmapData.Scan0;
            foreach (var point in points)
            {
               var index = point.Y * stride + point.X * bytesPerPixel;

               ptr[index + 2] = color.R;
               ptr[index + 1] = color.G;
               ptr[index] = color.B;
            }
         });
      }

      bmp.UnlockBits(bitmapData);
      //sw.Stop();
      //Debug.WriteLine($"Modifying Bitmap took {sw.ElapsedMilliseconds} ms");
   }

   /// <summary>
   /// The Bitmap needs to be Disposed after use
   /// </summary>
   /// <param name="method"></param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   public static Bitmap GenerateBitmapFromProvinces(Func<Province, int> method)
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

         Parallel.ForEach(Globals.Provinces, province => 
         {
            var points = new Point[province.PixelCnt];
            Array.Copy(Globals.Pixels, province.PixelPtr, points, 0, province.PixelCnt);

            var color = Color.FromArgb(method(province));

            var ptr = (byte*)bitmapData.Scan0;
            foreach (var point in points)
            {
               var index = point.Y * stride + point.X * bytesPerPixel;

               ptr[index + 2] = color.R;
               ptr[index + 1] = color.G;
               ptr[index] = color.B;
            }
         });
      }

      bmp.UnlockBits(bitmapData);
      sw.Stop();
      //Debug.WriteLine($"Generating Bitmap took {sw.ElapsedMilliseconds}ms");
      return bmp;
   }

}