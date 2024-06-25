using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Editor.Helper;

public class BitMapHelper
{
   /// <summary>
   /// The Bitmap needs to be Disposed after use
   /// </summary>
   /// <param name="method"></param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   public static Bitmap GenerateBitmap(Func<int, Color> method)
   {
      var sw = new Stopwatch();
      sw.Start();
      var width = Data.MapWidth;
      var height = Data.MapHeight;

      var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

      // Lock the bits in memory
      var bitmapData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);

      // Calculate stride (bytes per row)
      var stride = bitmapData.Stride;
      var bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
      
      unsafe
      {
         Parallel.ForEach(Data.Provinces.Values, province =>
         {
            var points = new Point[province.PixelCnt];
            Array.Copy(Data.Pixels, province.PixelPtr, points, 0, province.PixelCnt);

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
}