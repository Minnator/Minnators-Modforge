using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;


namespace Editor.Helper;

public static class AdjacencyHelper
{

   public static void CalculateAdjacency()
   {
      var sw = new Stopwatch();
      sw.Start();
      Debug.WriteLine("Calculating Adjacencies");
      var provinceDict = Data.ColorToProvPtr;
      Dictionary<int, int[]> adjacencyList = [];
      var height = Data.MapHeight;
      var width = Data.MapWidth;

      using var bmp = new Bitmap(Data.MapPath);
      
      foreach (var province in Data.Provinces)
      {
         var borderPixels = new Point[province.BorderCnt];
         Array.Copy(Data.BorderPixels, province.BorderPtr, borderPixels, 0, province.BorderCnt);

         var adjacencies = new HashSet<int>();
         foreach (var pixel in borderPixels)
         {
            for (var i = -1; i <= 1; i++)
            {
               for (var j = -1; j <= 1; j++)
               {
                  if (i == 0 && j == 0)
                  {
                     continue;
                  }

                  var x = pixel.X + i;
                  var y = pixel.Y + j;

                  if (x < 0 || x >= width || y < 0 || y >= height)
                  {
                     continue;
                  }

                  var color = bmp.GetPixel(x, y);
                  if (provinceDict.TryGetValue(color, out var adjProv)) 
                     adjacencies.Add(adjProv);
               }
            }

         }
         adjacencyList.Add(province.SelfPtr, [.. adjacencies]);
      }

      sw.Stop();
      Debug.WriteLine($"Adjacency calculation took {sw.ElapsedMilliseconds}ms");
      Data.AdjacentProvinces = adjacencyList;

      //DebugPrints.PrintAdjacencies();
   }

}