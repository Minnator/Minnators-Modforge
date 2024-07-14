using System.Text;
using Editor;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Loading
{
   public static class TradeNodeLoading
   {
      public static void Load(ModProject project)
      {

         var files = FilesHelper.GetFilesFromModAndVanillaUniquely(project.ModPath, project.VanillaPath, "common", "tradenodes");
         foreach (var file in files)
         {
            ParseTradeNodesFromString(IO.ReadAllInUTF8(file));
         }

         SetIncoming();
      }

      private static void SetIncoming()
      {
         foreach (var node in Globals.TradeNodes.Values)
         {
            foreach (var outgoing in node.Outgoing)
            {
               if (Globals.TradeNodes.TryGetValue(outgoing, out var outgoingNode))
               {
                  outgoingNode.Incoming.Add(node.Name);
               }
            }
         }
      }

      private static void ParseTradeNodesFromString(string fileContent)
      {
         var elements = Parsing.GetElements(0, fileContent);
         foreach (var element in elements)
         {
            if (element is not Block block)
            {
               Globals.ErrorLog.Write($"Forbidden content in tradenodes: {((Content)element)}");
               continue;
            }

            // initialize trade node by location and name
            var values = Parsing.GetKeyValueList(block.GetContent);
            var node = TradeNode.Empty;
            foreach (var value in values)
            {
               if (value.Key.Equals("location"))
               {
                  if (!int.TryParse(value.Value, out var location))
                  {
                     Globals.ErrorLog.Write($"Invalid location value in tradenode: {value.Value}");
                     break;
                  }
                  node = new (block.Name, location);
               }
               else if (value.Key.Equals("inland"))
                  node.IsInland = Parsing.YesNo(value.Value);
            }

            foreach (var subBlock in block.GetBlockElements)
            {
               ParseTradeNodeBlocks(ref node, subBlock);
            }

            if (node.Color == Color.Empty)
               node.Color = Globals.ColorProvider.GetRandomColor();

            Globals.TradeNodes.Add(node.Name, node);
         }
      }

      private static void ParseTradeNodeBlocks(ref TradeNode node, Block block)
      {
         switch (block.Name)
         {
            case "color":
               node.Color = Parsing.ParseColor(block.GetContent);
               break;
            case "members":
               node.Members = [..Parsing.GetIntListFromString(block.GetContent)];
               break;
            case "incoming":
               var kvp = Parsing.GetKeyValueList(block.GetContent);
               for (var index = 0; index < kvp.Count; index++)
               {
                  if (kvp[index].Key.Equals("name"))
                     node.Incoming.Add(kvp[index].Value);
               }
               break;
            case "outgoing":
               var kvp2 = Parsing.GetKeyValueList(block.GetContent);
               for (var index = 0; index < kvp2.Count; index++)
               {
                  if (kvp2[index].Key.Equals("name"))
                     node.Outgoing.Add(kvp2[index].Value);
               }
               break;
            default:
               Globals.ErrorLog.Write($"Forbidden block in tradenode: {block.Name}");
               break;
         }
      }
   }
}

public static class TradeNodeHelper
{
   public static List<Tag> GetAllCountriesInNode(string nodeName)
   {
      var node = Globals.TradeNodes[nodeName];
      var countries = new List<Tag>();
      foreach (var member in node.Members)
      {
         if (Globals.Provinces.TryGetValue(member, out var province))
            if (province.Owner != Tag.Empty)
               countries.Add(province.Owner);
      }
      return countries;
   }

   public static TradeNode GetTradeNodeByProvince(int provinceId)
   {
      foreach (var node in Globals.TradeNodes.Values)
      {
         if (node.Members.Contains(provinceId))
            return node;
      }
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
         foreach (var member in node.Members)
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