using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Editor.DataClasses;


namespace Editor.Helper;

public static class AdjacencyHelper
{

   public static void CalculateAdjacency()
   {
      var sw = new Stopwatch();
      sw.Start();
      Debug.WriteLine("Calculating Adjacencies");
      var provinceDict = Data.ColorToProvId;
      Dictionary<int, int[]> adjacencyList = [];
      var height = Data.MapHeight;
      var width = Data.MapWidth;

      using var bmp = new Bitmap(Data.MapPath);
      
      foreach (var province in Data.Provinces.Values)
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
         adjacencyList.Add(province.Id, [.. adjacencies]);
      }

      sw.Stop();
      Debug.WriteLine($"Adjacency calculation took {sw.ElapsedMilliseconds}ms");
      Data.AdjacentProvinces = adjacencyList;

      //DebugPrints.PrintAdjacencies();
   }

   // Faulty implementation
   public static void Calculate()
   {
      var sw = new Stopwatch();
      sw.Start();
      Debug.WriteLine("Calculating Adjacencies");

     using var bmp = new Bitmap(Data.MapPath);
      var provinces = Data.Provinces;
      Dictionary<int, HashSet<int>> adjacencyList = [];
      var intersectCount = 0;

      foreach (var province in provinces.Values)
      {
         var rect = GetFluffyRect(province.Bounds);

         foreach (var innerProv in provinces.Values)
         {
            if (MathHelper.RectanglesIntercept(rect, innerProv.Bounds))
            {
               intersectCount++;
               if (CheckBorders(province, innerProv, bmp))
               {
                  if (!adjacencyList.ContainsKey(province.Id))
                     adjacencyList.Add(province.Id, [innerProv.Id]);
                  else
                     adjacencyList[province.Id].Add(innerProv.Id);
               }
            }
         }
      }

      sw.Stop();
      Debug.WriteLine($"Adjacency calculation took {sw.ElapsedMilliseconds}ms");
      Debug.WriteLine($"Intersect count: {intersectCount}");

      var sb = new System.Text.StringBuilder();
      foreach (var kvp in adjacencyList)
      {
         sb.Append($"Prov [{kvp.Key,4}]: ");
         foreach (var adj in kvp.Value)
         {
            sb.Append($"{adj,4}, ");
         }
         sb.AppendLine();
      }
      File.WriteAllText(@"C:\Users\david\Downloads\adjacenciesDEBUG.txt", sb.ToString());

   }

   private static bool CheckBorders(Province province, Province innerProv, Bitmap bmp)
   {
      var borderPixels = new Point[province.BorderCnt];
      Array.Copy(Data.BorderPixels, province.BorderPtr, borderPixels, 0, province.BorderCnt);

      foreach (var pixel in borderPixels)
      {
         for (var i = -1; i <= 1; i++)
         {
            for (var j = -1; j <= 1; j++)
            {
               if ((i == 0 && j == 0) || (pixel.X + i < 0) || (pixel.X + i >= Data.MapWidth) || (pixel.Y + j < 0) || (pixel.Y + j >= Data.MapHeight))
               {
                  continue;
               }

               var color = bmp.GetPixel(pixel.X + i, pixel.Y + j);

               if (innerProv.Color == color)
               {
                  return true;
               }
            }
         }
      }
      return false;
   }

   private static Rectangle GetFluffyRect(Rectangle rect)
   {
      return new Rectangle(Math.Min(0, rect.X - 20), Math.Min(0, rect.Y - 20), Math.Min(Data.MapWidth - 1, rect.Width + 20), Math.Min(Data.MapHeight - 1, rect.Height + 20));
   }
}