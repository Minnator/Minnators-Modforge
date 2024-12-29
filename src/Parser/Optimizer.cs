using System.Collections.Concurrent;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Parser;

public static class Optimizer
{
   // Optimizes the provinces by copying the pixels and borders to one large array each and only saving pointers in the provinces
   // to where their points start and end. Also calculates the bounds of the provinces.
   // This allows for duplicate points in the BorderPixels array but increases performance.
   public static void OptimizeProvinces(ref HashSet<Province> provinces, ConcurrentDictionary<int, List<Point>> colorToProvId, ConcurrentDictionary<int, List<Point>> colorToBorder, int pixelCount)
   {
      Memory<Point> pixels = new(new Point[pixelCount]);
      Memory<Point> borders = new(new Point[colorToBorder.Values.Sum(list => list.Count)]);
      var dic = new Dictionary<int, Province>(provinces.Count);
      var dictionary = new Dictionary<int, Province>(provinces.Count);
      var provs = new HashSet<Province>(provinces.Count);

      var pixelPtr = 0;
      var borderPtr = 0;
      
      foreach (var province in provinces.ToArray().AsSpan())
      {
         var color = Color.FromArgb(province.Color.R, province.Color.G, province.Color.B).ToArgb();
         dic[color] = province;

         dictionary.Add(province.Id, province);

         //copy the pixels of the province to the pixel array
         if (!colorToProvId.ContainsKey(color))
            continue;
         colorToProvId[color].ToArray().AsSpan().CopyTo(pixels.Slice(pixelPtr, colorToProvId[color].Count).Span);
         province.PixelPtr = pixelPtr;
         province.PixelCnt = colorToProvId[color].Count;
         pixelPtr += province.PixelCnt;
         colorToBorder[color].ToArray().AsSpan().CopyTo(borders.Slice(borderPtr, colorToBorder[color].Count).Span);
         province.BorderPtr = borderPtr;
         province.BorderCnt = colorToBorder[color].Count;
         borderPtr += province.BorderCnt;

         //calculate the bounds of the provinces and set the center
         province.Bounds = Geometry.GetBounds([.. colorToBorder[color]]);
         province.Center = new (province.Bounds.X + province.Bounds.Width / 2, province.Bounds.Y + province.Bounds.Height / 2);



         province.Pixels = pixels.Slice(province.PixelPtr, province.PixelCnt);
         province.Borders = borders.Slice(province.BorderPtr, province.BorderCnt);
         // add the province to the dictionary
         // add the province to the dictionary

         provs.Add(province);
      }

      // Set the optimized data to the Globals class
      Globals.Provinces = provs;
      Globals.ProvinceIdToProvince = dictionary;
      Globals.ColorToProvId = dic;


      HashSet<Point> borderSet = [.. borders.Span];
      Parallel.ForEach(Globals.Provinces, province => RemoveBorderPixelsFromPixels(province, ref borderSet));

      // Free up memory from the ConcurrentDictionaries
      colorToBorder.Clear();
      colorToProvId.Clear();
   }

   public static void RemoveBorderPixelsFromPixels(Province province, ref HashSet<Point> borderSet)
   {
      var pixels = province.Pixels.Span;
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
      var adjacencyList = new Dictionary<Province, Province[]>(Globals.Provinces.Count);

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

      Globals.AdjacentProvinces = adjacencyList;

      // Free up memory from the ConcurrentDictionary
      colorToAdj.Clear();
   }

}