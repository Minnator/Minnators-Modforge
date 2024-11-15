using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing.Imaging;
using Editor.Helper;

namespace Editor.Loading
{
   
   public static class RiverLoading
   {
      public static void Load()
      {
         if (!FilesHelper.GetModOrVanillaPath(out var path, out var isModPath, "map", "rivers.bmp"))
         {
            Globals.ErrorLog.Write("Could not find rivers.bmp");
            return;
         }

         using var bmp = new Bitmap(path);
         var bmpData = bmp.LockBits(new (0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
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
                  //int currentColor = Color.FromArgb(row[xTimesThree + 2], row[xTimesThree + 1], row[xTimesThree]).ToArgb();

                  if (currentColor > 253)
                     continue;
                  
                  riverSizes.AddOrUpdate(currentColor, [new Point(x, y)],
                     (_, existingList) =>
                     {
                        lock (existingList)
                        {
                           existingList.Add(new (x, y));
                           return existingList;
                        }
                     }
                  );
               }
            }
         });

         bmp.UnlockBits(bmpData);
         var palette = bmp.Palette.Entries;
         foreach (var river in riverSizes)
         {
            var color = palette[river.Key].ToArgb();
            Globals.Rivers.Add(color, river.Value.ToArray());
         }
      }
   }
}