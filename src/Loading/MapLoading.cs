using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing.Imaging;
using Editor.Helper;
using Editor.Parser;

namespace Editor.Loading;

public static class MapLoading
{
   internal static void Load()
   {
      if (!FilesHelper.GetFilePathUniquely(out var definitionPath, "map", "definition.csv"))
         throw new FileNotFoundException("Could not find definition.csv in mod or vanilla folder");

      if (!FilesHelper.GetFilePathUniquely(out var mapPath, "map", "provinces.bmp"))
         throw new FileNotFoundException("Could not find \"provinces.bmp\" in mod or vanilla folder");

      Globals.MapPath = mapPath;
      var provinces = DefinitionLoading.LoadDefinition([.. File.ReadAllLines(definitionPath)]);

      //Get the size of the image at Globals.MapPath
      using var stream = new FileStream(Globals.MapPath, FileMode.Open, FileAccess.Read);
      using var image = Image.FromStream(stream, useEmbeddedColorManagement: false, validateImageData: false);
      Globals.MapHeight = image.Size.Height;
      Globals.MapWidth = image.Size.Width;

      var (colorToProvId, colorToBorder, adjacency) = LoadMap(Globals.MapPath);

      Optimizer.OptimizeProvinces(ref provinces, colorToProvId, colorToBorder, image.Width * image.Height);

      Optimizer.OptimizeAdjacencies(adjacency); 
   }

   private static (ConcurrentDictionary<int, List<Point>>, ConcurrentDictionary<int, List<Point>>, ConcurrentDictionary<int, HashSet<int>>) LoadMap(string path)
   {
      using var bmp = new Bitmap(path);
      var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
      var width = bmp.Width;
      var height = bmp.Height;
      var stride = bmpData.Stride;
      var scan0 = bmpData.Scan0;

      ConcurrentDictionary<int, List<Point>> colorToProvId = new();
      ConcurrentDictionary<int, List<Point>> colorToBorder = new();
      ConcurrentDictionary<int, HashSet<int>> colorToAdj = new();

      var widthMinusOne = width - 1; // to avoid calculating height times width - 1
      var heightMinusOne = height - 1; // to avoid calculating width times height - 1

      // could further be optimized by writing 4 loops for the special cases on the edges: top, bottom, left, right and
      // one that does not need to check for the edges would save ~14.000.000 if checks but code would be less readable not to say horrible
      Parallel.For(0, width, x =>
      {
         unsafe
         {
            // Create local dictionaries for each thread to avoid locks and merge them at the end
            var localColorToProvId = new Dictionary<int, List<Point>>();
            var localColorToBorder = new Dictionary<int, List<Point>>();
            var localColorToAdj = new Dictionary<int, HashSet<int>>();
            var xTimesThree = x * 3;
            var westOffset = (x - 1) * 3;
            var eastOffset = (x + 1) * 3;

            for (var y = 0; y < height; y++)
            {
               var row = (byte*)scan0 + y * stride;
               var currentPoint = new Point(x, y);
               var currentColor = Color.FromArgb(row[xTimesThree + 2], row[xTimesThree + 1], row[xTimesThree]).ToArgb();

               if (!localColorToProvId.TryGetValue(currentColor, out var provPoints))
               {
                  provPoints = [];
                  localColorToProvId[currentColor] = provPoints;
               }
               provPoints.Add(currentPoint);

               // The following ifs could be removed if it was just for visuals but if some madlad would decide to put a 1 pixel wide province on 
               // the edge of the map it would break the adjacency calculation and would not be initialized correctly

               var found = false;
               // Check if the current pixel is on the edge of the map and if so skip the checks for the neighbors
               if (y > 0)
               {
                  var nRow = (byte*)scan0 + (y - 1) * stride;
                  var colN = Color.FromArgb(nRow[xTimesThree + 2], nRow[xTimesThree + 1], nRow[xTimesThree]).ToArgb();
                  if (colN != currentColor) 
                     AddBorderAndAdj(colN, ref found);
               }

               if (x < widthMinusOne)
               {
                  var colN = Color.FromArgb(row[eastOffset + 2], row[eastOffset + 1], row[eastOffset]).ToArgb();
                  if (colN != currentColor) 
                     AddBorderAndAdj(colN, ref found);
               }

               if (y < heightMinusOne)
               {
                  var sRow = (byte*)scan0 + (y + 1) * stride;
                  var colN = Color.FromArgb(sRow[xTimesThree + 2], sRow[xTimesThree + 1], sRow[xTimesThree]).ToArgb();
                  if (colN != currentColor) 
                     AddBorderAndAdj(colN, ref found);
               }

               if (x > 0)
               {
                  var colN = Color.FromArgb(row[westOffset + 2], row[westOffset + 1], row[westOffset]).ToArgb();
                  if (colN != currentColor) 
                     AddBorderAndAdj(colN, ref found);
               }

               continue;

               // Helper function to avoid duplicate code 
               // Adds the current point to the border of the current color and adds the neighbor color to the adjacency list
               void AddBorderAndAdj(int neighborColor, ref bool found)
               {
                  if (!found)
                  {
                     if (!localColorToBorder.TryGetValue(currentColor, out var borderPoints))
                     {
                        borderPoints = [];
                        localColorToBorder[currentColor] = borderPoints;
                     }
                     borderPoints.Add(currentPoint);
                     found = true;
                  }

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

      Globals.MapWidth = width;
      Globals.MapHeight = height;

      return (colorToProvId, colorToBorder, colorToAdj);
   }
}
