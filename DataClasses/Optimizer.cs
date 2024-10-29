using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Policy;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses;

public static class Optimizer
{
   // Optimizes the provinces by copying the pixels and borders to one large array each and only saving pointers in the provinces
   // to where their points start and end. Also calculates the bounds of the provinces.
   // This allows for duplicate points in the BorderPixels array but increases performance.
   public static void OptimizeProvinces(ref Province[] provinces, ConcurrentDictionary<int, List<Point>> colorToProvId, ConcurrentDictionary<int, List<Point>> colorToBorder, int pixelCount)
   {
      var sw = new Stopwatch();
      sw.Start();
      var pixels = new Point[pixelCount];
      var borders = new Point[colorToBorder.Values.Sum(list => list.Count)];
      var dic = new Dictionary<int, Province>(provinces.Length);
      var dictionary = new Dictionary<int, Province>(provinces.Length);
      var provs = new HashSet<Province>(provinces.Length);

      var pixelPtr = 0;
      var borderPtr = 0;
      
      foreach (var province in provinces)
      {
         var color = Color.FromArgb(province.Color.R, province.Color.G, province.Color.B).ToArgb();
         dic[color] = province;

         dictionary.Add(province.Id, province);

         //copy the pixels of the province to the pixel array
         if (!colorToProvId.ContainsKey(color))
            continue;
         colorToProvId[color].CopyTo(pixels, pixelPtr);
         province.PixelPtr = pixelPtr;
         province.PixelCnt = colorToProvId[color].Count;
         pixelPtr += province.PixelCnt;
         colorToBorder[color].CopyTo(borders, borderPtr);
         province.BorderPtr = borderPtr;
         province.BorderCnt = colorToBorder[color].Count;
         borderPtr += province.BorderCnt;

         //calculate the bounds of the provinces and set the center
         province.Bounds = Geometry.GetBounds([.. colorToBorder[color]]);
         province.Center = new Point(province.Bounds.X + province.Bounds.Width / 2, province.Bounds.Y + province.Bounds.Height / 2);

         province.Pixels = new Point[province.PixelCnt];
         Array.Copy(pixels, province.PixelPtr, province.Pixels, 0, province.PixelCnt);

         province.Borders = new Point[province.BorderCnt];
         Array.Copy(borders, province.BorderPtr, province.Borders, 0, province.BorderCnt);
         // add the province to the dictionary
         // add the province to the dictionary

         provs.Add(province);
      }

      // Set the optimized data to the Globals class
      Globals.Provinces = provs;
      Globals.ProvinceIdToProvince = dictionary;
      Globals.ColorToProvId = dic;


      HashSet<Point> borderSet = [.. borders];
      Parallel.ForEach(Globals.Provinces, province => RemoveBorderPixelsFromPixels(province, ref borderSet));

      sw.Stop();
      //Debug.WriteLine($"OptimizeProvinces took {sw.ElapsedMilliseconds}ms");
      Globals.LoadingLog.WriteTimeStamp("OptimizeProvinces", sw.ElapsedMilliseconds);
      //var elapsed = sw.ElapsedMilliseconds;
      //Debug.WriteLine($"Per Province Cost: {elapsed / (float)provinces.Length * 1000} µs");


      // Free up memory from the ConcurrentDictionaries
      colorToBorder.Clear();
      colorToProvId.Clear();
   }

   public static void RemoveBorderPixelsFromPixels(Province province, ref HashSet<Point> borderSet)
   {
      var pixels = province.Pixels;


      var newPixels = new Point[pixels.Length - province.BorderCnt];
      var cnt = 0;
      for (var i = 0; i < pixels.Length; i++)
      {
         if (borderSet.Contains(pixels[i]))
            continue;
         newPixels[cnt++] = pixels[i];
      }

      province.Pixels = newPixels;
   }

   public static void OptimizeAdjacencies(ConcurrentDictionary<int, HashSet<int>> colorToAdj)
   {
      var sw = new Stopwatch();
      sw.Start();
      var adjacencyList = new Dictionary<Province, Province[]>(Globals.Provinces.Count);

      /*foreach (var kvp in colorToAdj) 
      {
         adjacencyList.Add(Globals.ColorToProvId[kvp.Key], kvp.Value.Select(color =>
         {
            return Globals.ColorToProvId[color];
         }).ToArray());
      }*/

      foreach (var adjacent in colorToAdj)
      {
         var adjIds = new Province[adjacent.Value.Count];
         var cnt = 0;
         foreach (var color in adjacent.Value)
         {
            if (Globals.ColorToProvId.TryGetValue(color, out var value))
               adjIds[cnt++] = Globals.ProvinceIdToProvince[value];
            else
               Globals.ErrorLog.Write($"Could not find definition for color {color}!");
         }

         if (Globals.ColorToProvId.TryGetValue(adjacent.Key, out var key))
            adjacencyList[Globals.ProvinceIdToProvince[key]] = adjIds;
         else
            Globals.ErrorLog.Write($"Could not find province with color {adjacent.Key}");
      }

      sw.Stop();
      //Debug.WriteLine($"Adjacency calculation took {sw.ElapsedMilliseconds}ms");
      Globals.LoadingLog.WriteTimeStamp("Adjacency optimization", sw.ElapsedMilliseconds); 
      Globals.AdjacentProvinces = adjacencyList;

      // Free up memory from the ConcurrentDictionary
      colorToAdj.Clear();
   }

}