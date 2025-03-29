using System.ComponentModel;
using System.Reflection;

namespace Editor.Helper
{
   public static class Sorting
   {

      public static bool TopologicalSortWithCycle(Dictionary<string, List<string>> objects, out List<string> result, bool bottomUp = false)
      {
         var inDegree = new Dictionary<string, int>();
         var adjList = new Dictionary<string, List<string>>();

         foreach (var (effect, contents) in objects)
         {
            inDegree.TryAdd(effect, 0);
            adjList[effect] = new List<string>();

            foreach (var subEffect in contents.Where(objects.ContainsKey))
            {
               adjList[effect].Add(subEffect);
               inDegree[subEffect] = inDegree.GetValueOrDefault(subEffect, 0) + 1;
            }
         }

         var queue = new Queue<string>(inDegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));
         var sorted = new List<string>();

         while (queue.Count > 0)
         {
            var effect = queue.Dequeue();
            sorted.Add(effect);

            foreach (var dep in adjList.GetValueOrDefault(effect, new List<string>()))
            {
               inDegree[dep]--;
               if (inDegree[dep] == 0)
                  queue.Enqueue(dep);
            }
         }

         if (sorted.Count != objects.Count)
         {
            var visited = new Dictionary<string, int>();  // 0 = unvisited, 1 = visiting, 2 = visited
            var stack = new Stack<string>();
            List<string> cycle = null;

            // Run DFS from every unvisited node
            foreach (var node in objects.Keys)
            {
               if (!visited.TryGetValue(node, out var value) || value == 0)
               {
                  if (Dfs(node, visited, ref cycle, stack, adjList))
                  {
                     result = cycle;
                     return false;
                  }
               }
            }

            result = []; 
            return false;
         }

         result = bottomUp ? sorted.AsEnumerable().Reverse().ToList() : sorted;
         return true;
      }

      private static bool Dfs(string node, Dictionary<string, int> visited, ref List<string> cycle, Stack<string> stack, Dictionary<string, List<string>> adjList)
      {
         visited.TryAdd(node, 0);
         if (visited[node] == 1) // Cycle detected
         {
            cycle = ExtractCycle(stack, node);
            return true;
         }
         if (visited[node] == 2) 
            return false; // Already processed

         visited[node] = 1;
         stack.Push(node);

         foreach (var neighbor in adjList[node])
         {
            if (Dfs(neighbor, visited, ref cycle, stack, adjList)) 
               return true;
         }

         stack.Pop();
         visited[node] = 2;
         return false;
      }

      private static List<string> ExtractCycle(Stack<string> stack, string cycleStart)
      {
         var cycleList = new List<string>();
         var collecting = false;

         foreach (var node in stack.Reverse())
         {
            if (node == cycleStart) collecting = true;
            if (collecting) cycleList.Add(node);
         }

         cycleList.Add(cycleStart); // Close the loop
         cycleList.Reverse(); // To maintain correct cycle order
         return cycleList;
      }




      public static bool TopologicalSort(Dictionary<string, List<string>> objects, out List<string> result, bool bottomUp = false)
      {
         var inDegree = new Dictionary<string, int>();
         var adjList = new Dictionary<string, List<string>>();

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

         var queue = new Queue<string>(inDegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));
         var sorted = new List<string>();
         var visited = new HashSet<string>();

         while (queue.Count > 0)
         {
            var effect = queue.Dequeue();
            sorted.Add(effect);
            visited.Add(effect);

            foreach (var dep in adjList.GetValueOrDefault(effect, []))
            {
               inDegree[dep]--;
               if (inDegree[dep] == 0)
                  queue.Enqueue(dep);
            }
         }

         if (sorted.Count != objects.Count)
         {
            // Cycle detected, find cycle members
            var cycleNodes = new HashSet<string>(objects.Keys.Except(visited));
            var cycle = new List<string>();

            foreach (var node in cycleNodes)
            {
               if (cycleNodes.Any(n => adjList.GetValueOrDefault(n, []).Contains(node)))
                  if (adjList[node].Count != 0)
                     cycle.Add(node);
            }

            result = cycle;
            return false;
         }

         result = bottomUp ? sorted.AsEnumerable().Reverse().ToList() : sorted;
         return true;
      }

   }
}