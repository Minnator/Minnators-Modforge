using System.ComponentModel;
using System.Text;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Helper
{
   public static class TradeNodeHelper
   {
      private static List<TradeNode> CurrentPath = [];
      private static HashSet<TradeNode> Visited = [];

      public static List<TradeNode> HasCycle(TradeNode node)
      {
         if (CurrentPath.Contains(node))
            return CurrentPath.GetRange(CurrentPath.IndexOf(node), CurrentPath.Count - CurrentPath.IndexOf(node));

         if (!Visited.Add(node))
            return [];

         CurrentPath.Add(node);

         foreach (var neighbor in node.Outgoing)
         {
            var cycle = HasCycle(Globals.TradeNodes[neighbor.Target]);
            if (cycle.Count > 0)
               return cycle; // If a cycle is found, return it
         }

         CurrentPath.Remove(node);

         // No cycle found in this path
         return []; 
      }

      // Method to check if any of the nodes in the network have a cycle and return it
      public static List<TradeNode> FindCycle(List<TradeNode> tradeNodes)
      {
         foreach (var node in tradeNodes)
         {
            if (!Visited.Contains(node))
            {
               var cycle = HasCycle(node);
               if (cycle.Count > 0)
                  return cycle; // Return the first detected cycle
            }
         }

         return []; // No cycle detected in the entire network
      }

      // Kahn's Algorithm for topological sorting
      public static List<TradeNode> TopologicalSort(List<TradeNode> tradeNodes)
      {
         var inDegree = new Dictionary<TradeNode, int>();
         var queue = new Queue<TradeNode>();
         var result = new List<TradeNode>();

         // Step 1: Calculate in-degree for each node
         foreach (var node in tradeNodes)
         {
            inDegree[node] = node.Incoming.Count;

            // If the node has no incoming links, add it to the queue
            if (inDegree[node] == 0) 
               queue.Enqueue(node);
         }

         // Step 2: Process the nodes in topological order
         while (queue.Count > 0)
         {
            var current = queue.Dequeue();
            result.Add(current); // Add node to result in topological order

            // Step 3: Reduce the in-degree of all neighbors (nodes with outgoing links from current)
            foreach (var outgoingLink in current.Outgoing)
            {
               var neighbor = tradeNodes.First(n => n.Name == outgoingLink.Target);
               inDegree[neighbor]--;

               // If in-degree of neighbor becomes 0, add it to the queue
               if (inDegree[neighbor] == 0) 
                  queue.Enqueue(neighbor);
            }
         }

         // If we haven't processed all nodes, there's a cycle in the graph
         if (result.Count != tradeNodes.Count)
         {
            MessageBox.Show("There is a cycle in the Tradenodes, please use the find Cycle tool to get the cycle",
               "Cycle in TradeNodes", MessageBoxButtons.OK);
            return [];
         }

         return result;
      }

      public static List<Tag> GetAllCountriesInNode(string nodeName)
      {
         var node = Globals.TradeNodes[nodeName];
         var countries = new List<Tag>();
         foreach (var province in node.GetProvinces())
               if (province.Owner != Tag.Empty)
                  countries.Add(province.Owner);
         return countries;
      }

      public static TradeNode GetTradeNodeByProvince(Province province)
      {
         foreach (var node in Globals.TradeNodes.Values)
            if (node.GetProvinces().Contains(province))
               return node;
         return TradeNode.Empty;
      }

      public static void DumpTradeNodes(string path)
      {
         var sb = new StringBuilder();

         foreach (var node in Globals.TradeNodes.Values)
         {
            sb.AppendLine(node.Name);
            sb.AppendLine($"\tLocation: {node.Location}");
            sb.AppendLine($"\tColor: {node.Color}");
            sb.AppendLine($"\tIsInland: {node.IsInland}");
            sb.Append("\tMembers:\n\t\t");
            foreach (var member in node.GetProvinceIds())
            {
               sb.Append($"{member}, ");
            }
            sb.AppendLine();
            foreach (var incoming in node.Incoming)
            {
               sb.AppendLine($"\tIncoming: {incoming}");
            }
            foreach (var outgoing in node.Outgoing)
            {
               sb.AppendLine($"\tOutgoing: {outgoing}");
            }
         }

         File.WriteAllText(Path.Combine(path, "tradenodes_dump.txt"), sb.ToString());
      }
   }
}