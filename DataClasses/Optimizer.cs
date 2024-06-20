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
   public static Dictionary<Color, Point[]> OptimizeDataStructures(ConcurrentDictionary<Color, List<Point>> old)
   {
      return old.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
   }

   public static Point[][] OptimizePixelStructures(ConcurrentDictionary<Color, List<Point>> input)
   {
      var output = new Point[input.Count][];
      var i = 0;

      foreach (var kvp in input)
      {
         output[i] = [.. kvp.Value];
         i++;
      }

      return output;
   }

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
         province.Bounds = MathHelper.GetBoundingBox(colorToBorder[color].ToArray());

      }

      sw.Stop();
      Debug.WriteLine($"OptimizeProvinces took {sw.ElapsedMilliseconds}ms");
      log.WriteTimeStamp("OptimizeProvinces", sw.ElapsedMilliseconds);
      var elapsed = sw.ElapsedMilliseconds;
      Debug.WriteLine($"Per Province Cost: {elapsed / (float)provinces.Length * 1000} µs");

      Data.BorderPixels = borders;
      Data.Pixels = pixels;
      Data.Provinces = provinces;
      Data.ColorToProvPtr = dic;

      colorToBorder.Clear();
      colorToProvId.Clear();
   }


}