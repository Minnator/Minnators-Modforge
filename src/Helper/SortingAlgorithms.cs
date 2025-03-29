using System.Reflection;

namespace Editor.Helper
{
   public static class Sorting
   {
      private static bool TopologicalSort(Dictionary<string, List<string>> objects, out List<string> result, bool bottomUp = false)
      {
         var inDegree = new Dictionary<string, int>();
         var adjList = new Dictionary<string, List<string>>();

         // Build adjacency list and in-degree counts
         foreach (var (effect, contents) in objects)
         {
            inDegree.TryAdd(effect, 0);
            adjList[effect] = [];

            foreach (var subEffect in contents.Where(objects.ContainsKey))
            {
               adjList[effect].Add(subEffect);
               inDegree[subEffect] = inDegree.GetValueOrDefault(subEffect, 0) + 1;
            }
         }

         // Find effects with no dependencies
         var queue = new Queue<string>(inDegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));
         var sorted = new List<string>();

         while (queue.Count > 0)
         {
            var effect = queue.Dequeue();
            sorted.Add(effect);

            foreach (var dep in adjList.GetValueOrDefault(effect, []))
            {
               inDegree[dep]--;
               if (inDegree[dep] == 0)
                  queue.Enqueue(dep);
            }
         }

         if (sorted.Count != objects.Count)
         {
            result = [];
            return false;
         }

         if (bottomUp)
            result = sorted.AsEnumerable().Reverse().ToList(); // Reverse order for bottom-up resolution
         result = sorted;
         return true;
      }
   }
}