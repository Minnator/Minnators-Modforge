using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Editor;

public static class MapLoading
{
   [SuppressMessage("ReSharper", "UseCollectionExpression")]
   public static void LoadMap()
   {
      var path = @"S:\SteamLibrary\steamapps\common\Europa Universalis IV\map\provinces.bmp";

      // loading the map to get the width and height
      using var bmp = new Bitmap(path);
      var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
      var width = bmp.Width;
      var height = bmp.Height;
      var stride = bmpData.Stride;
      var scan0 = bmpData.Scan0;
      var bpp = 24; // bits per pixel (24 is the default for bmp)

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
               var currentColor = Color.FromArgb(row[x * 3 + 2], row[x * 3 + 1], row[x * 3]);

               colorToProvId.AddOrUpdate(currentColor, new List<Point> { new Point(x, y) }, (_, value) =>
               {
                  lock (value)
                  {
                     value.Add(new Point(x, y));
                  }
                  return value;
               });

               // Check if the pixel north is in the bounds of the image and if the color is different
               if (y > 0)
               {
                  var nRow = (byte*)scan0 + ((y - 1) * stride);
                  var rowIndex = x * 3;
                  var nCol = Color.FromArgb(nRow[rowIndex + 2], nRow[rowIndex + 1], nRow[rowIndex]);
                  if (nCol != currentColor)
                     colorToBorder.AddOrUpdate(currentColor, new List<Point> { new Point(x, y) }, (_, value) =>
                     {
                        lock (value)
                        {
                           value.Add(new Point(x, y));
                        }
                        return value;
                     });
               }

               // Check if the pixel east is in the bounds of the image and if the color is different
               if (x < width - 1)
               {
                  var rowIndex = (x + 1) * 3;
                  var eCol = Color.FromArgb(row[rowIndex + 2], row[rowIndex + 1], row[rowIndex]);
                  if (eCol != currentColor)
                     colorToBorder.AddOrUpdate(currentColor, new List<Point> { new Point(x, y) }, (_, value) =>
                     {
                        lock (value)
                        {
                           value.Add(new Point(x, y));
                        }
                        return value;
                     });
               }

               // Check if the pixel south is in the bounds of the image and if the color is different
               if (y < height - 1)
               {
                  var sRow = (byte*)scan0 + ((y + 1) * stride);
                  var rowIndex = x * 3;
                  var sCol = Color.FromArgb(sRow[rowIndex + 2], sRow[rowIndex + 1], sRow[rowIndex]);
                  if (sCol != currentColor)
                     colorToBorder.AddOrUpdate(currentColor, new List<Point> { new Point(x, y) }, (_, value) =>
                     {
                        lock (value)
                        {
                           value.Add(new Point(x, y));
                        }
                        return value;
                     });
               }

               // Check if the pixel west is in the bounds of the image and if the color is different
               if (x > 0)
               {
                  var rowIndex = (x - 1) * 3;
                  var wCol = Color.FromArgb(row[rowIndex + 2], row[rowIndex + 1], row[rowIndex]);
                  if (wCol != currentColor)
                     colorToBorder.AddOrUpdate(currentColor, new List<Point> { new Point(x, y) }, (_, value) =>
                     {
                        lock (value)
                        {
                           value.Add(new Point(x, y));
                        }
                        return value;
                     });
               }
            }
         }
      });



      sw.Stop();
      Debug.WriteLine($"Initialization took {sw.ElapsedMilliseconds}ms");
      Debug.WriteLine($"Found {colorToProvId.Count} provinces and {colorToBorder.Count} borders");

      var totalPointsInColorToId = colorToProvId.Values.Sum(list => list.Count);
      Debug.WriteLine($"Total points in colorToProvId: {totalPointsInColorToId} of {width * height} pixels");

      sw.Restart();
      DebugMaps.DrawAllBorder(colorToBorder, new Size(width, height), @"C:\Users\david\Downloads\borders.bmp");
      sw.Stop();
      Debug.WriteLine($"Drawing Borders took {sw.ElapsedMilliseconds}ms");
      sw.Restart();
      DebugMaps.DrawAllBorder(colorToProvId, new Size(width, height), @"C:\Users\david\Downloads\provinces.bmp");
      sw.Stop();
      Debug.WriteLine($"Drawing Provinces took {sw.ElapsedMilliseconds}ms");
   }
}
