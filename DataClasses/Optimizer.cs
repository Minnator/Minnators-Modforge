using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Editor.DataClasses;

public static class Optimizer
{
   public static Dictionary<Color, Point[]> OptimizeDataStructures(ConcurrentDictionary<Color, List<Point>> old)
   {
      return old.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
   }

   public static Point[][] OptimizePixelStructures(ConcurrentDictionary<Color, List<Point>> input)
   {
      var output = new Point[input.Count][];
      var i = 0;

      foreach (var kvp in input)
      {
         output[i] = [.. kvp.Value];
         i++;
      }

      return output;
   }
}