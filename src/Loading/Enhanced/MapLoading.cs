using System.Drawing.Imaging;
using Editor.ErrorHandling;

namespace Editor.Loading.Enhanced
{
   public class IllegalMapDefinitionException(string message) : Exception(message);

   public static class MapLoading
   {
      private const int ALPHA = 255 << 24;

      public static void Load()
      {
         if (!EnhancedParser.GetModOrVanillaPath(out var po, "map", "provinces.bmp"))
            return;
         Globals.MapPath = po.GetPath();
         DefinitionLoading.LoadDefinition();
         // colorToProvId, colorToBorder
         var (colorToProvId, colorToBorder) = LoadProvinces();
         OptimizeProvincePixels(colorToProvId);
         OptimizeBordersAndAdjacency(colorToBorder);
      }

      private static void OptimizeBordersAndAdjacency(Dictionary<int, Dictionary<int, List<Point>>> borders)
      {
         Globals.AdjacentProvinces = new (borders.Count);

         foreach (var province in Globals.Provinces)
         {
            var color = province.Color.ToArgb();
            if (!borders.TryGetValue(color, out var bDict))
            {
               Globals.AdjacentProvinces.Add(province, []);
               province.TempBorderFix();
               province.SetBounds();
               province.Center = new(province.Bounds.X + province.Bounds.Width / 2, province.Bounds.Y + province.Bounds.Height / 2);
               continue;
            }
            foreach (var (adjacent, borderPixels) in bDict)
            {
               if (!Globals.ColorToProvId.TryGetValue(adjacent, out var adjProv))
               {
                  // throw new IllegalMapDefinitionException("");
                  _ = new ErrorObject($"Province {Color.FromArgb(adjacent)} ({adjacent}) has no pixels on the map!", ErrorType.ProvinceDefinitionError, "A province has no pixels in provinces.bmp. Either remove it from definition.csv or give it pixels.");
                  continue;
               }
               province.ProvinceBorders.Add(adjProv, new(borderPixels.ToArray()));
            }
            Globals.AdjacentProvinces.Add(province, province.ProvinceBorders.Keys.ToArray());

            province.TempBorderFix();
            province.SetBounds();
            province.Center = new(province.Bounds.X + province.Bounds.Width / 2, province.Bounds.Y + province.Bounds.Height / 2);
         }
      }

      private static void OptimizeProvincePixels(Dictionary<int, List<Point>> pixels) // remove assigned pixels from the dictionary to see if there are any left overs
      {  
         foreach (var province in Globals.Provinces)//.ToArray().AsSpan())
         {
            var color = province.Color.ToArgb();
            if (pixels.TryGetValue(color, out var provPixels)) 
               province.Pixels = new(provPixels.ToArray());
            else
               _ = new LogEntry(LogType.Warning, $"Province {province.Id} has no pixels on the map");
         }
      }

      private static unsafe (Dictionary<int, List<Point>>, Dictionary<int, Dictionary<int, List<Point>>>) LoadProvinces()
      {
         using var bmp = new Bitmap(Globals.MapPath);
         var bmpData = bmp.LockBits(new(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
         var width = Globals.MapWidth = bmp.Width;
         var height = Globals.MapHeight = bmp.Height;
         var stride = bmpData.Stride;
         var scan0 = bmpData.Scan0;

         var numThreads = Math.Min(Environment.ProcessorCount, (height - 1));
         var heightPerThread = (height - 1) / numThreads;

         var colorToProvId = new Dictionary<int, List<Point>>[numThreads];
         // Ah, yes an Array of a Dictionary of a Dictionary of a List of a Struct of Ints
         var colorToBorder = new Dictionary<int, Dictionary<int, List<Point>>>[numThreads]; 

         var internalWidth = width - 1;

         Parallel.For(0, numThreads, threadIndex =>
         {
            // We crate the local dictionaries for each thread
            var localColorToProvId = colorToProvId[threadIndex] = new();
            var localColorToBorder = colorToBorder[threadIndex] = new();

            // We only iterate from 1x1 to width-1 x height-1 to avoid checking for the edges

            var startY = threadIndex * heightPerThread;
            var endY = threadIndex == numThreads - 1 ? height - 1 : startY + heightPerThread;

            // We iterate over the map and look at the pixels east and south of the current pixel
            for (var y = startY; y < endY; y++)
            {
               var row = (byte*)scan0 + y * stride;
               var nextRow = row + stride;
               var nextColor = CurrentColor(row, 0);
               var nextPixel = new Point(0, y);

               for (var x = 0; x < internalWidth; x++)
               {
                  var xTimesThree = x * 3;
                  var currentPixel = nextPixel;
                  var currentColor = nextColor;

                  // We add a pixel to its province's color
                  if (!localColorToProvId.TryGetValue(currentColor, out var provPoints))
                  {
                     provPoints = [];
                     localColorToProvId[currentColor] = provPoints;
                  }
                  provPoints.Add(currentPixel);

                  nextColor = CurrentColor(row, xTimesThree + 3);
                  nextPixel = new (x + 1, y);
                  var southColor = CurrentColor(nextRow, xTimesThree);
                  var southPixel = new Point(x, y + 1);

                  AddToBorder(currentColor, nextColor, ref currentPixel, ref nextPixel, localColorToBorder);
                  AddToBorder(currentColor, southColor, ref currentPixel, ref southPixel, localColorToBorder);
               }
            }
         });
         

         // We merge the local dictionaries into the global dictionaries
         var totalProvToId = new Dictionary<int, List<Point>>();
         var totalColorToBorder = new Dictionary<int, Dictionary<int, List<Point>>>();

         foreach (var localColorToProvId in colorToProvId)
         {
            foreach (var (color, points) in localColorToProvId)
            {
               if (!totalProvToId.TryGetValue(color, out var provPoints))
               {
                  provPoints = [];
                  totalProvToId[color] = provPoints;
               }
               provPoints.AddRange(points);
            }
         }

         foreach (var localColorToBorder in colorToBorder)
         {
            foreach (var (color, borders) in localColorToBorder)
            {
               if (!totalColorToBorder.TryGetValue(color, out var borderDict))
               {
                  borderDict = [];
                  totalColorToBorder[color] = borderDict;
               }
               foreach (var (neighbour, borderPixels) in borders)
               {
                  if (!borderDict.TryGetValue(neighbour, out var borderPoints))
                  {
                     borderPoints = [];
                     borderDict[neighbour] = borderPoints;
                  }
                  borderPoints.AddRange(borderPixels);
               }
            }
         }
         
         // Analyze the last row
         var lastRow = (byte*)scan0 + (height - 1) * stride;
         var lastRowColor = CurrentColor(lastRow, 0);
         var lastRowPixel = new Point(0, height - 1);
         for (var x = 0; x < internalWidth; x++)
         {
            var xTimesThree = x * 3;
            var currentPixel = lastRowPixel;
            var currentColor = lastRowColor;

            if (!totalProvToId.TryGetValue(currentColor, out var provPoints))
            {
               provPoints = [];
               totalProvToId[currentColor] = provPoints;
            }
            provPoints.Add(currentPixel);

            lastRowColor = CurrentColor(lastRow, xTimesThree + 3);
            lastRowPixel = new(x + 1, height - 1);
            AddToBorder(currentColor, lastRowColor, ref currentPixel, ref lastRowPixel, totalColorToBorder);
         }
         
         // We analyze the last column
         var nextRow = (byte*)scan0 + stride;
         var xTimesThreeN = (width - 1) * 3;
         var nextColor = CurrentColor(nextRow, xTimesThreeN);
         var nextPixel = new Point(width - 1, 0);
         for (var y = 0; y < height - 1; y++)
         {
            var currentPixel = nextPixel;
            var currentColor = nextColor;

            var rightPixel = new Point(0, y);
            var rightColor = CurrentColor(nextRow, 0);
            AddToBorder(currentColor, rightColor, ref currentPixel, ref rightPixel, totalColorToBorder);
            if (!totalProvToId.TryGetValue(currentColor, out var provPoints))
            {
               provPoints = [];
               totalProvToId[currentColor] = provPoints;
            }
            provPoints.Add(currentPixel);

            nextRow = (byte*)scan0 + (y + 1) * stride;
            nextColor = CurrentColor(nextRow, xTimesThreeN);
            nextPixel = new (width - 1, y + 1);
            AddToBorder(currentColor, nextColor, ref currentPixel, ref nextPixel, totalColorToBorder);
         }

         // last pixel at height - 1, width - 1
         var lastPixel = new Point(width - 1, height - 1);
         if (!totalProvToId.TryGetValue(nextColor, out var lastProvPoints))
         {
            lastProvPoints = [];
            totalProvToId[nextColor] = lastProvPoints;
         }
         lastProvPoints.Add(lastPixel); 
         var rightPixelL = new Point(0, height - 1);
         var rightColorL = CurrentColor(nextRow, 0);
         AddToBorder(nextColor, rightColorL, ref lastPixel, ref rightPixelL, totalColorToBorder);


         bmp.UnlockBits(bmpData);
         return (totalProvToId, totalColorToBorder);
      }

      private static bool AddToBorder(int current, int neighbour, ref Point pixel, ref Point neighbourPixel, Dictionary<int, Dictionary<int, List<Point>>> borderDict)
      {
         // We don't want to add a border to itself
         if (current == neighbour)
            return false;

         // We add the pixel to the border of the current province
         
         if (!borderDict.TryGetValue(current, out var borders))
         {
            borders = [];
            borderDict[current] = borders;
         }
         if (!borders.TryGetValue(neighbour, out var borderPixels))
         {
            borderPixels = [];
            borders[neighbour] = borderPixels;
         }
         borderPixels.Add(pixel);

         // We add the pixel to the border of the neighbour province

         if (!borderDict.TryGetValue(neighbour, out borders))
         {
            borders = [];
            borderDict[neighbour] = borders;
         }
         if (!borders.TryGetValue(current, out borderPixels))
         {
            borderPixels = [];
            borders[current] = borderPixels;
         }
         borderPixels.Add(neighbourPixel);
         return true;
      }

      private static unsafe int CurrentColor(byte* row, int xTimesThree)
      {
         return ALPHA                          // Alpha: Fully opaque
                | (row[xTimesThree + 2] << 16) // Red
                | (row[xTimesThree + 1] << 8)  // Green
                | row[xTimesThree];            // Blue
      }
   }

}