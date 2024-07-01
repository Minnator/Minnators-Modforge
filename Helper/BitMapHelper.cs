using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Interfaces;

namespace Editor.Helper;

public class BitMapHelper
{

   public static void WriteOnProvince(Func<int, string> method, Bitmap bmp)
   {
      var font = new Font("Arial", 6);
      var brush = new SolidBrush(Color.Black);
      var graphics = Graphics.FromImage(bmp);

      foreach (var province in Globals.Provinces.Values)
      {
         var text = method(province.Id);
         var textWidth = (int)graphics.MeasureString(text, font).Width;
         var textHeight = (int)graphics.MeasureString(text, font).Height;

         graphics.DrawString(text, font, brush, province.Center.X - textWidth / 2, province.Center.Y - textHeight / 2);
      }

   }

   /// <summary>
   /// The Bitmap needs to be Disposed after use
   /// </summary>
   /// <param name="method"></param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   public static Bitmap GenerateBitmapFromProvinces(Func<int, Color> method)
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

         Parallel.ForEach(Globals.Provinces.Values, province =>
         {
            var points = new Point[province.PixelCnt];
            Array.Copy(Globals.Pixels, province.PixelPtr, points, 0, province.PixelCnt);

            var color = method(province.Id);

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

   /// <summary>
   /// The Bitmap needs to be Disposed after use
   /// </summary>
   /// <param name="collection"></param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   public static Bitmap GenerateBitmapFromProvinceCollection(List<IProvinceCollection> collection)
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

      var options = new ParallelOptions
      {
         MaxDegreeOfParallelism = 1
      };

      var pixels = new Point[Globals.Pixels.Length];

      unsafe
      {
         Parallel.ForEach(collection, options, collect =>
         {
            //MapDrawHelper.GetAllPixelPoints(collect.GetProvinceIds(), out var points);
            MapDrawHelper.GetAllPixelPtrs(collect.GetProvinceIds(), out var id);
            var color = collect.Color;

            var ptr = (byte*)bitmapData.Scan0;

            for (var i = 0; i < id.GetLength(0); i++)
            {
               for (var j = id[i, 0]; j < id[i, 1] + id[i, 0]; j++)
               {
                  var point = pixels[j];
                  var index = pixels[j].Y * stride + point.X * bytesPerPixel;

                  ptr[index + 2] = color.R;
                  ptr[index + 1] = color.G;
                  ptr[index] = color.B;
               }
            }
         });
      }

      bmp.UnlockBits(bitmapData);
      sw.Stop();
      //Debug.WriteLine($"Generating Bitmap took {sw.ElapsedMilliseconds}ms");
      return bmp;
   }
}