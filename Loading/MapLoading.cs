using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Editor.Loading;

public static class MapLoading
{
   public static (ConcurrentDictionary<Color, List<Point>>, ConcurrentDictionary<Color, List<Point>>, ConcurrentDictionary<Color, HashSet<Color>>) LoadMap(
       ref Log loadingLog, string path)
   {
      using var bmp = new Bitmap(path);
      var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
      var width = bmp.Width;
      var height = bmp.Height;
      var stride = bmpData.Stride;
      var scan0 = bmpData.Scan0;
      var sw = new Stopwatch();

      ConcurrentDictionary<Color, List<Point>> colorToProvId = new();
      ConcurrentDictionary<Color, List<Point>> colorToBorder = new();
      ConcurrentDictionary<Color, HashSet<Color>> colorToAdj = new();

      sw.Start();
      Parallel.For(0, width, x =>
      {
         unsafe
         {
            // Create local dictionaries for each thread to avoid locks and merge them at the end
            var localColorToProvId = new Dictionary<Color, List<Point>>();
            var localColorToBorder = new Dictionary<Color, List<Point>>();
            var localColorToAdj = new Dictionary<Color, HashSet<Color>>();

            for (var y = 0; y < height; y++)
            {
               var row = (byte*)scan0 + y * stride;
               var currentPoint = new Point(x, y);
               var currentColor = Color.FromArgb(row[x * 3 + 2], row[x * 3 + 1], row[x * 3]);

               if (!localColorToProvId.TryGetValue(currentColor, out var provPoints))
               {
                  provPoints = [];
                  localColorToProvId[currentColor] = provPoints;
               }
               provPoints.Add(currentPoint);

               if (y > 0)
               {
                  var nRow = (byte*)scan0 + ((y - 1) * stride);
                  var colN = Color.FromArgb(nRow[x * 3 + 2], nRow[x * 3 + 1], nRow[x * 3]);
                  if (colN != currentColor) 
                     AddBorderAndAdj(colN);
               }

               if (x < width - 1)
               {
                  var colN = Color.FromArgb(row[(x + 1) * 3 + 2], row[(x + 1) * 3 + 1], row[(x + 1) * 3]);
                  if (colN != currentColor) 
                     AddBorderAndAdj(colN);
               }

               if (y < height - 1)
               {
                  var sRow = (byte*)scan0 + ((y + 1) * stride);
                  var colN = Color.FromArgb(sRow[x * 3 + 2], sRow[x * 3 + 1], sRow[x * 3]);
                  if (colN != currentColor) 
                     AddBorderAndAdj(colN);
               }

               if (x > 0)
               {
                  var colN = Color.FromArgb(row[(x - 1) * 3 + 2], row[(x - 1) * 3 + 1], row[(x - 1) * 3]);
                  if (colN != currentColor) 
                     AddBorderAndAdj(colN);
               }

               continue;

               void AddBorderAndAdj(Color neighborColor)
               {
                  if (!localColorToBorder.TryGetValue(currentColor, out var borderPoints))
                  {
                     borderPoints = [];
                     localColorToBorder[currentColor] = borderPoints;
                  }
                  borderPoints.Add(currentPoint);

                  if (!localColorToAdj.TryGetValue(currentColor, out var adjColors))
                  {
                     adjColors = [];
                     localColorToAdj[currentColor] = adjColors;
                  }
                  adjColors.Add(neighborColor);
               }
            }

            // Merge local dictionaries into global concurrent cross-thread ones
            foreach (var kvp in localColorToProvId)
            {
               colorToProvId.AddOrUpdate(kvp.Key, kvp.Value, (_, existing) =>
               {
                  lock (existing)
                  {
                     existing.AddRange(kvp.Value);
                  }
                  return existing;
               });
            }

            foreach (var kvp in localColorToBorder)
            {
               colorToBorder.AddOrUpdate(kvp.Key, kvp.Value, (_, existing) =>
               {
                  lock (existing)
                  {
                     existing.AddRange(kvp.Value);
                  }
                  return existing;
               });
            }

            foreach (var kvp in localColorToAdj)
            {
               colorToAdj.AddOrUpdate(kvp.Key, kvp.Value, (_, existing) =>
               {
                  lock (existing)
                  {
                     foreach (var color in kvp.Value)
                     {
                        existing.Add(color);
                     }
                  }
                  return existing;
               });
            }
         }
      });


      sw.Stop();
      Debug.WriteLine($"Map Loading took {sw.ElapsedMilliseconds}ms");
      loadingLog.WriteTimeStamp("Pixel Initialisation", sw.ElapsedMilliseconds);

      Globals.MapWidth = width;
      Globals.MapHeight = height;

      return (colorToProvId, colorToBorder, colorToAdj);
   }
}
