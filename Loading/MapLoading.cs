using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Editor.DataClasses;

namespace Editor;

public static class MapLoading
{
   [SuppressMessage("ReSharper", "UseCollectionExpression")]
   public static void LoadMap(ref Log loadingLog)
   {
      var path = @"S:\SteamLibrary\steamapps\common\Europa Universalis IV\map\provinces.bmp";

      // loading the map to get the width and height
      using var bmp = new Bitmap(path);
      var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
      var width = bmp.Width;
      var height = bmp.Height;
      var stride = bmpData.Stride;
      var scan0 = bmpData.Scan0;

      ConcurrentDictionary<Color, List<Point>> colorToProvId = new ();
      ConcurrentDictionary<Color, List<Point>> colorToBorder = new ();

      
      var sw = new Stopwatch();
      sw.Start();

      Parallel.For(0, height, y =>
      {
         unsafe
         {
            var row = (byte*)scan0 + (y * stride);

            for (var x = 0; x < width; x++)
            {
               var currentPoint = new Point(x, y);
               var currentColor = Color.FromArgb(row[x * 3 + 2], row[x * 3 + 1], row[x * 3]);

               colorToProvId.AddOrUpdate(currentColor, new List<Point> { currentPoint }, (_, value) =>
               {
                  lock (value) // Is required to prevent race conditions in the List object
                  {
                     value.Add(currentPoint);
                  }
                  return value;
               });

               // Check if the pixel north is in the bounds of the image and if the color is different
               if (y > 0)
               {
                  var nRow = (byte*)scan0 + ((y - 1) * stride);
                  var rowIndex = x * 3;
                  if (Color.FromArgb(nRow[rowIndex + 2], nRow[rowIndex + 1], nRow[rowIndex]) != currentColor)
                     colorToBorder.AddOrUpdate(currentColor, new List<Point> { currentPoint }, (_, value) =>
                     {
                        lock (value)
                        {
                           value.Add(currentPoint);
                        }
                        return value;
                     });
               }

               // Check if the pixel east is in the bounds of the image and if the color is different
               if (x < width - 1)
               {
                  var rowIndex = (x + 1) * 3;
                  if (Color.FromArgb(row[rowIndex + 2], row[rowIndex + 1], row[rowIndex]) != currentColor)
                     colorToBorder.AddOrUpdate(currentColor, new List<Point> { currentPoint }, (_, value) =>
                     {
                        lock (value)
                        {
                           value.Add(currentPoint);
                        }
                        return value;
                     });
               }

               // Check if the pixel south is in the bounds of the image and if the color is different
               if (y < height - 1)
               {
                  var sRow = (byte*)scan0 + ((y + 1) * stride);
                  var rowIndex = x * 3;
                  if (Color.FromArgb(sRow[rowIndex + 2], sRow[rowIndex + 1], sRow[rowIndex]) != currentColor)
                     colorToBorder.AddOrUpdate(currentColor, new List<Point> { currentPoint }, (_, value) =>
                     {
                        lock (value)
                        {
                           value.Add(currentPoint);
                        }
                        return value;
                     });
               }

               // Check if the pixel west is in the bounds of the image and if the color is different
               if (x > 0)
               {
                  var rowIndex = (x - 1) * 3;
                  if (Color.FromArgb(row[rowIndex + 2], row[rowIndex + 1], row[rowIndex]) != currentColor)
                     colorToBorder.AddOrUpdate(currentColor, new List<Point> { currentPoint }, (_, value) =>
                     {
                        lock (value)
                        {
                           value.Add(currentPoint);
                        }
                        return value;
                     });
               }
            }
         }
      });



      sw.Stop();

      loadingLog.WriteTimeStamp("Pixel Initialisation", sw.ElapsedMilliseconds);

      Debug.WriteLine($"Initialization took {sw.ElapsedMilliseconds}ms");
      sw.Restart();
      DebugMaps.DrawAllBorder(colorToBorder, new Size(width, height), @"C:\Users\david\Downloads\borders.bmp");
      sw.Stop();
      Debug.WriteLine($"Drawing Borders took {sw.ElapsedMilliseconds}ms");
      loadingLog.WriteTimeStamp("Drawing Borders", sw.ElapsedMilliseconds);
      sw.Restart();
      DebugMaps.DrawAllBorder(colorToProvId, new Size(width, height), @"C:\Users\david\Downloads\provinces.bmp");
      sw.Stop();
      loadingLog.WriteTimeStamp("Drawing Provinces", sw.ElapsedMilliseconds);
      Debug.WriteLine($"Drawing Provinces took {sw.ElapsedMilliseconds}ms");
      sw.Restart();
      var optimizedProvId = Optimizer.OptimizePixelStructures(colorToProvId);
      var optimizedBorder = Optimizer.OptimizePixelStructures(colorToBorder);
      sw.Stop();
      loadingLog.WriteTimeStamp("Optimizing data structures", sw.ElapsedMilliseconds);
      Debug.WriteLine($"Optimizing data structures took {sw.ElapsedMilliseconds}ms");
   }

   
}
