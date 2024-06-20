using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Editor.Helper;

namespace Editor.DataClasses;

public static class Optimizer
{
   // Optimizes the provinces by copying the pixels and borders to one large array each and only saving pointers in the provinces
   // to where their points start and end. Also calculates the bounds of the provinces.
   // This allows for duplicate points in the BorderPixels array but increases performance.
   public static void OptimizeProvinces(Province[] provinces, ConcurrentDictionary<Color, List<Point>> colorToProvId, ConcurrentDictionary<Color, List<Point>> colorToBorder, int pixelCount, ref Log log)
   {
      var sw = new Stopwatch();
      sw.Start();
      var pixels = new Point[pixelCount];
      var borders = new Point[colorToBorder.Values.Sum(list => list.Count)];
      var dic = new Dictionary<Color, int>(provinces.Length);

      var pixelPtr = 0;
      var borderPtr = 0;
      var provincePtr = 0;
      
      foreach (var province in provinces)
      {
         var color = Color.FromArgb(province.Color.R, province.Color.G, province.Color.B);
         dic[color] = provincePtr;

         //set the province pointer for the province itself
         province.SelfPtr = provincePtr;
         provincePtr++;

         //copy the pixels of the province to the pixel array
         if (!colorToProvId.ContainsKey(color))
            continue;
         colorToProvId[color].CopyTo(pixels, pixelPtr);
         province.PixelPtr = pixelPtr;
         province.PixelCnt = colorToProvId[color].Count;
         pixelPtr += province.PixelCnt;

         //copy the borders of the province to the border array
         colorToBorder[color].CopyTo(borders, borderPtr);
         province.BorderPtr = borderPtr;
         province.BorderCnt = colorToBorder[color].Count;
         borderPtr += province.BorderCnt;

         //calculate the bounds of the provinces
         province.Bounds = MathHelper.GetBoundingRectangle(colorToBorder[color].ToArray());

      }

      sw.Stop();
      //Debug.WriteLine($"OptimizeProvinces took {sw.ElapsedMilliseconds}ms");
      log.WriteTimeStamp("OptimizeProvinces", sw.ElapsedMilliseconds);
      //var elapsed = sw.ElapsedMilliseconds;
      //Debug.WriteLine($"Per Province Cost: {elapsed / (float)provinces.Length * 1000} µs");

      // Set the optimized data to the Data class
      Data.BorderPixels = borders;
      Data.Pixels = pixels;
      Data.Provinces = provinces;
      Data.ColorToProvPtr = dic;

      // Free up memory from the ConcurrentDictionaries
      colorToBorder.Clear();
      colorToProvId.Clear();
   }


}