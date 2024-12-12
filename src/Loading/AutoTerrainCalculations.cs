using System.Diagnostics;
using System.Drawing.Imaging;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Loading
{
   public static class AutoTerrainCalculations
   {
      private static int[] WaterColors;

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

         // The map that contains all the int-colors of the terrain
         var colorMap = new int[Globals.MapWidth][];
         var width = Globals.MapWidth;
         var height = Globals.MapHeight;

         // Resize the trees.bmp to the map size as it is 8x6.978 times smaller
         using var ogTreemap = new Bitmap(trees);
         //using var treesBmp = BmpLoading.ResizeIndexedBitmap(trees, width, height);
         using var terrainBmp = new Bitmap(file);

         var treeBmpData = ogTreemap.LockBits(new(0, 0, ogTreemap.Width, ogTreemap.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
         var treeScan0 = treeBmpData.Scan0;
         var terrainBmpData = terrainBmp.LockBits(new(0, 0, terrainBmp.Width, terrainBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
         var terrainScan0 = terrainBmpData.Scan0;

         var stride = terrainBmpData.Stride;
         var sw = Stopwatch.StartNew();

         var treesPalette = ogTreemap.Palette.Entries;
         var terrainPalette = terrainBmp.Palette.Entries;

         var blackInt = Color.Black.ToArgb();


         HashSet<int> waterColors = [];
         foreach (var ter in Globals.Terrains)
         {
            if (ter.IsWater || ter.IsInlandSea)
               waterColors.Add(ter.Color.ToArgb());
         }
         foreach (var terDef in Globals.TerrainDefinitions.Definitions)
         {
            if (terDef.Type.IsWater || terDef.Type.IsInlandSea)
               waterColors.Add(terrainPalette[terDef.ColorIndex].ToArgb());
         }

         WaterColors = waterColors.ToArray();


         var heightFactor = ogTreemap.Height / (float)terrainBmp.Height;
         var widthFactor = ogTreemap.Width / (float)terrainBmp.Width;
         var sourceStride = treeBmpData.Stride;
         // Load the colors from both bitmaps
         Parallel.For(0, width, x =>
         {
            var column = new int[height];
            var sourceX = (int)(x * widthFactor);
            unsafe
            {
               for (var y = 0; y < height; y++)
               {
                  var tColor = treesPalette[*((byte*)(treeScan0 + sourceX + sourceStride * (int)(y * heightFactor)))].ToArgb();
                  if (tColor == blackInt)
                     tColor = terrainPalette[*((byte*)terrainScan0 + x + y * stride)].ToArgb();
                  column[y] = tColor;
               }
            }

            colorMap[x] = column;
         });

         // Remove the rivers from the terrain
         Parallel.ForEach(Globals.Rivers, kvp =>
         {
            for (var point = 0; point < kvp.Value.Length; point++)
               colorMap[kvp.Value[point].X][kvp.Value[point].Y] = blackInt;
         });

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


         //Average out the colors of each province and resolve back to indices
         var terrainIndexToTerrain = Globals.TerrainDefinitions.GetColorToTerrain();         
         var treeToIndex = Globals.TreeDefinitions.GetColorToIndexKvps();

         List<KeyValuePair<int, Terrain>> terrainPaletteToTerrain = [];
         List<KeyValuePair<int, Terrain>> treePaletteToIndex = [];

         foreach (var kvp in terrainIndexToTerrain)
         {
            if (kvp.Key is > 254 or < 0)
               continue;
            terrainPaletteToTerrain.Add(new(terrainPalette[kvp.Key].ToArgb(), kvp.Value));
         }

         foreach (var kvp in treeToIndex)
         {
            if (kvp.Key is > 254 or < 0)
               continue;
            treePaletteToIndex.Add(new(treesPalette[kvp.Key].ToArgb(), kvp.Value));
         }

         try
         {
            foreach (var province in Globals.Provinces)
            {
               Dictionary<int, int> colorCounts = [];

               foreach (var point in province.Pixels)
               {
                  var color = colorMap[point.X][point.Y];
                  if (!colorCounts.TryAdd(color, 1))
                     colorCounts[color]++;
               }

               // determine min, max
               var max = 0;
               var maxColor = 0;

               foreach (var kvp in colorCounts)
               {
                  var currentMax = kvp.Value;
                  if (currentMax > max)
                  {
                     if (Globals.LandProvinces.Contains(province) && WaterColors.Contains(kvp.Key))
                        continue;
                     max = currentMax;
                     maxColor = kvp.Key;
                  }
               }

               // resolve the colors to indexes and set the terrain
               var colorInt = blackInt; // Black

               foreach (var treeKvp in treePaletteToIndex)
               {
                  if (treeKvp.Key == maxColor)
                  {
                     province.AutoTerrain = treeKvp.Value;
                     colorInt = maxColor;
                     break;
                  }
               }


               if (colorInt == blackInt)
               {
                  foreach (var terrainKvp in terrainPaletteToTerrain)
                  {
                     if (terrainKvp.Key == maxColor)
                     {
                        province.AutoTerrain = terrainKvp.Value;
                        break;
                     }
                  }
               }
            }
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            throw;
         }

         sw.Stop();
         //Debug.WriteLine($"Loading terrain and trees took {sw.ElapsedMilliseconds}ms");
      }

   }
}