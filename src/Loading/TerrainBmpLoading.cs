
using System.Diagnostics;
using System.Drawing.Imaging;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Loading
{
   public static class TerrainBmpLoading
   {

      public static void Load()
      {
         if (!FilesHelper.GetModOrVanillaPath(out var file, out _, "map", "terrain.bmp"))
         {
            Globals.ErrorLog.Write("Could not find terrain.bmp");
            return;
         }

         if (!FilesHelper.GetModOrVanillaPath(out var trees, out _, "map", "trees.bmp"))
         {
            Globals.ErrorLog.Write("Could not find trees.bmp");
            return;
         }

         var colorMap = new int[Globals.MapWidth][];

         using var bmp = new Bitmap(trees);
         using var treesBmp = BmpLoading.ResizeIndexedBitmap(bmp, Globals.MapWidth, Globals.MapHeight);
         var bmpData = treesBmp.LockBits(new(0, 0, treesBmp.Width, treesBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
         var width = treesBmp.Width;
         var height = treesBmp.Height;
         var stride = bmpData.Stride;
         var scan0 = bmpData.Scan0;

         var sw = new Stopwatch();
         sw.Start();
         try
         {
            Parallel.For(0, width, x =>
            {
               var line = new int[height];
               Color[] palette;
               lock (treesBmp)
               {
                  palette = treesBmp.Palette.Entries;
               }
               unsafe
               {
                  for (var y = 0; y < height; y++)
                  {
                     line[y] = palette[*((byte*)scan0 + x + y * stride)].ToArgb();
                  }
               }
               colorMap[x] = line;
            });
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            throw;
         }
         treesBmp.UnlockBits(bmpData);

         sw.Stop();
         Debug.WriteLine($"Terrain map: {sw.ElapsedMilliseconds}");
         sw.Restart();
         using var bmp2 = new Bitmap(file);
         bmpData = bmp2.LockBits(new(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
         width = bmp2.Width;
         height = bmp2.Height;
         stride = bmpData.Stride;
         scan0 = bmpData.Scan0;

         
         var blackInt = Color.Black.ToArgb();
         try
         {
            Parallel.For(0, width, x =>
            {
               var line = colorMap[x];
               Color[] palette;
               lock (bmp2)
               {
                  palette = bmp2.Palette.Entries;
               }
               unsafe
               {
                  for (var y = 0; y < height; y++)
                     if (line[y] == blackInt)
                        line[y] = palette[*((byte*)scan0 + x + y * stride)].ToArgb();
               }
               colorMap[x] = line;
            });
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            throw;
         }

         bmp2.UnlockBits(bmpData);
         
         sw.Stop();
         Debug.WriteLine($"Terrain map combination: {sw.ElapsedMilliseconds}");

         var nonBlackInts = 0;
         for (var x = 0; x < Globals.MapWidth; x++)
         {
            for (var y = 0; y < Globals.MapHeight; y++)
            {
               if (colorMap[x][y] != blackInt)
                  nonBlackInts++;
            }
         }

         Debug.WriteLine($"Non-black ints [0x{blackInt:X}]: {nonBlackInts}");

         var resultBmp = new Bitmap(Globals.MapWidth, Globals.MapHeight);
         var resultData = resultBmp.LockBits(new(0, 0, Globals.MapWidth, Globals.MapHeight), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
         var resultStride = resultData.Stride;
         var resultScan0 = resultData.Scan0;

         try
         {
            Parallel.For(0, Globals.MapWidth, x =>
            {
               unsafe
               {
                  for (var y = 0; y < Globals.MapHeight; y++) 
                     *((int*)resultScan0 + x + y * resultStride / 4) = colorMap[x][y];
               }
            });
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            throw;
         }

         resultBmp.UnlockBits(resultData);
         resultBmp.Save("terrain.png", ImageFormat.Png);

         sw.Restart();
         // remove all rivers
         try
         {
            Parallel.ForEach(Globals.Rivers, kvp =>
            {
               for (var point = 0; point < kvp.Value.Length; point++) 
                  colorMap[kvp.Value[point].X][kvp.Value[point].Y] = blackInt;
            });
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            throw;
         }

         sw.Stop();
         Debug.WriteLine($"Removing rivers: {sw.ElapsedMilliseconds}");

         var resultBmp2 = new Bitmap(Globals.MapWidth, Globals.MapHeight);
         var resultData2 = resultBmp2.LockBits(new(0, 0, Globals.MapWidth, Globals.MapHeight), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
         var resultStride2 = resultData2.Stride;
         var resultScan02 = resultData2.Scan0;

         try
         {
            Parallel.For(0, Globals.MapWidth, x =>
            {
               unsafe
               {
                  for (var y = 0; y < Globals.MapHeight; y++) 
                     *((int*)resultScan02 + x + y * resultStride2 / 4) = colorMap[x][y];
               }
            });
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            throw;
         }

         resultBmp2.UnlockBits(resultData2);

         resultBmp2.Save("terrainWithoutRivers.png", ImageFormat.Png);

         sw.Restart();

         // Average out the colors of each province to determine the terrain type
         var provTerrainMap = new int[Globals.MapWidth][];
         var treePalette = treesBmp.Palette.Entries;
         var terrainPalette = bmp2.Palette.Entries;

         for (var x = 0; x < Globals.MapWidth; x++)
            provTerrainMap[x] = new int[Globals.MapHeight];

         foreach (var province in Globals.LandProvinces)
         {
            Dictionary<int, int> colorCounts = [];

            foreach (var point in province.Pixels)
            {
               var color = colorMap[point.X][point.Y];
               if (!colorCounts.TryAdd(color, 1))
                  colorCounts[color]++;
            }

            var maxCount = int.MinValue;
            var maxColor = 0;

            foreach (var col in colorCounts)
            {
               if (col.Value > maxCount && col.Value != blackInt)
               {
                  maxCount = col.Value;
                  maxColor = col.Key;
               }
            }

            var colorInt = blackInt; // Black
            for (var i = 0; i < treePalette.Length; i++)
            {
               if (treePalette[i].ToArgb() == maxColor)
               {
                  colorInt = treePalette[i].ToArgb();
                  province.AutoTerrain = Globals.TreeDefinitions.GetTerrainForIndex(i);
                  break;
               }
            }

            if (colorInt == blackInt)
            {
               for (var i = 0; i < terrainPalette.Length; i++)
               {
                  var color = terrainPalette[i].ToArgb();
                  if (color == maxColor)
                  {
                     colorInt = color;
                     province.AutoTerrain = Globals.TerrainDefinitions.GetTerrainForIndex(i);
                     break;
                  }
               }
            }
            
            foreach (var point in province.Pixels)
               provTerrainMap[point.X][point.Y] = colorInt;
         }

         sw.Stop();
         Debug.WriteLine($"Province terrain: {sw.ElapsedMilliseconds}");

         var resultBmp3 = new Bitmap(Globals.MapWidth, Globals.MapHeight);
         var resultData3 = resultBmp3.LockBits(new(0, 0, Globals.MapWidth, Globals.MapHeight), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
         var resultStride3 = resultData3.Stride;
         var resultScan03 = resultData3.Scan0;

         try
         {
            Parallel.For(0, Globals.MapWidth, x =>
            {
               unsafe
               {
                  for (var y = 0; y < Globals.MapHeight; y++) 
                     *((int*)resultScan03 + x + y * resultStride3 / 4) = provTerrainMap[x][y];
               }
            });
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            throw;
         }

         resultBmp3.UnlockBits(resultData3);
         resultBmp3.Save("provinceTerrain.png", ImageFormat.Png);

         // paint the terrain from provinces
         var resultBmp4 = new Bitmap(Globals.MapWidth, Globals.MapHeight);
         var resultData4 = resultBmp4.LockBits(new(0, 0, Globals.MapWidth, Globals.MapHeight), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
         var resultStride4 = resultData4.Stride;
         var resultScan04 = resultData4.Scan0;

         try
         {
            Parallel.ForEach(Globals.Provinces, province =>
            {
               unsafe
               {
                  foreach (var point in province.Pixels)
                     *((int*)resultScan04 + point.X + point.Y * resultStride4 / 4) = province.AutoTerrain.Color.ToArgb();
               }
            });
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            throw;
         }

         resultBmp4.UnlockBits(resultData4);
         resultBmp4.Save("terrainFromProvinces.png", ImageFormat.Png);

         // paint combined map

         var resultBmp5 = new Bitmap(Globals.MapWidth, Globals.MapHeight);
         var resultData5 = resultBmp5.LockBits(new(0, 0, Globals.MapWidth, Globals.MapHeight), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
         var resultStride5 = resultData5.Stride;
         var resultScan05 = resultData5.Scan0;

         try
         {
            Parallel.ForEach(Globals.Provinces, province =>
            {
               unsafe
               {
                  int color;
                  color = province.Terrain == Terrain.Empty ? province.AutoTerrain.Color.ToArgb() : province.Terrain.Color.ToArgb();

                  foreach (var point in province.Pixels) 
                     *((int*)resultScan05 + point.X + point.Y * resultStride5 / 4) = color;
               }
            });
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            throw;
         }

         resultBmp5.UnlockBits(resultData5);
         resultBmp5.Save("combinedTerrain.png", ImageFormat.Png);
      }
   }
}