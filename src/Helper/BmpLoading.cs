using System.Collections.Concurrent;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Windows.UI.ViewManagement;

namespace Editor.Helper
{
   public static class BmpLoading
   {
      public static (ConcurrentDictionary<byte, List<Point>>, Color[]) LoadIndexedBitMap(string path)
      {

         using var bmp = new Bitmap(path);
         var bmpData = bmp.LockBits(new(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
         var width = bmp.Width;
         var height = bmp.Height;
         var stride = bmpData.Stride;
         var scan0 = bmpData.Scan0;

         ConcurrentDictionary<byte, List<Point>> riverSizes = [];
         Parallel.For(0, width, x =>
         {
            unsafe
            {
               for (var y = 0; y < height; y++)
               {
                  var row = (byte*)scan0 + y * stride;
                  var currentColor = row[x];

                  if (currentColor > 253)
                     continue;

                  riverSizes.AddOrUpdate(currentColor, [new Point(x, y)],
                     (_, existingList) =>
                     {
                        lock (existingList)
                        {
                           existingList.Add(new(x, y));
                           return existingList;
                        }
                     }
                  );
               }
            }
         });

         bmp.UnlockBits(bmpData);
         return (riverSizes, bmp.Palette.Entries);
      }

      public static Bitmap ResizeIndexedBitmap(Bitmap input, int width, int height)
      {
         var bpp24Map = new Bitmap(width, height, PixelFormat.Format24bppRgb);

         using var g = Graphics.FromImage(bpp24Map);
         g.InterpolationMode = InterpolationMode.NearestNeighbor;
         g.DrawImage(input, 0, 0, width, height);

         return ConvertToIndexedBitmap(ref bpp24Map, input.Palette);
      }

      public static Bitmap ConvertBitmap(Bitmap indexedBitmap, PixelFormat format)
      {
         if ((indexedBitmap.PixelFormat & PixelFormat.Indexed) == 0)
            throw new ArgumentException("The input bitmap must be in an indexed format.");

         var nonIndexedBitmap = new Bitmap(indexedBitmap.Width, indexedBitmap.Height, format);
         using var g = Graphics.FromImage(nonIndexedBitmap);
         g.DrawImage(indexedBitmap, 0, 0);

         return nonIndexedBitmap;
      }

      public static Bitmap ConvertToIndexedBitmap(ref Bitmap source, ColorPalette palette)
      {
         var indexedBitmap = new Bitmap(source.Width, source.Height, PixelFormat.Format8bppIndexed);
         indexedBitmap.Palette = palette;

         var sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
         var indexedData = indexedBitmap.LockBits(new Rectangle(0, 0, indexedBitmap.Width, indexedBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

         var indexedStride = indexedData.Stride;
         var sourceStride = sourceData.Stride;
         try
         {

            unsafe
            {
               var sourceScan0 = (byte*)sourceData.Scan0;
               var indexedScan0 = (byte*)indexedData.Scan0;

               for (var y = 0; y < source.Height; y++)
               {
                  var sourceRow = sourceScan0 + y * sourceStride;
                  var indexedRow = indexedScan0 + y * indexedStride;

                  for (var x = 0; x < source.Width; x++)
                  {
                     indexedRow[x] = FindClosestPaletteIndex(sourceRow[x * 3 + 2], sourceRow[x * 3 + 1], sourceRow[x * 3 + 0], palette);
                  }
               }
            }
         }
         finally
         {
            source.UnlockBits(sourceData);
            indexedBitmap.UnlockBits(indexedData);
         }

         return indexedBitmap;
      }

      private static byte FindClosestPaletteIndex(byte r, byte g, byte b, ColorPalette palette)
      {
         var bestMatchIndex = 0;
         var bestMatchDistance = int.MaxValue;

         for (var i = 0; i < palette.Entries.Length; i++)
         {
            var paletteColor = palette.Entries[i];
            var dr = r - paletteColor.R;
            var dg = g - paletteColor.G;
            var db = b - paletteColor.B;
            var distance = dr * dr + dg * dg + db * db;

            if (distance >= bestMatchDistance)
               continue;

            bestMatchDistance = distance;
            bestMatchIndex = i;

            if (distance == 0)
               break;
         }

         return (byte)bestMatchIndex;
      }

   }
}