using Editor.DataClasses.GameDataClasses;

namespace Editor.Testing;

public static class DebugPrints
{
   public static bool VerifyTopologicalSort(List<TradeNode> sortedNodes)
   {
      var nodeIndex = new Dictionary<TradeNode, int>();

      for (var i = 0; i < sortedNodes.Count; i++) 
         nodeIndex[sortedNodes[i]] = i;

      // Verify that all nodes have incoming links from earlier nodes
      foreach (var node in sortedNodes)
      {
         foreach (var incomingLink in node.Incoming)
         {
            if (nodeIndex[Globals.TradeNodes[incomingLink]] >= nodeIndex[node])
            {
               System.Diagnostics.Debug.WriteLine($"Violation: {incomingLink} comes after {node.Name} (Incoming link check failed).");
               return false;
            }
         }

         // Check outgoing links: they should point to nodes later in the sorted list
         foreach (var outgoingLink in node.Outgoing)
         {
            var targetNode = sortedNodes.First(n => n.Name == outgoingLink.Target);

            if (nodeIndex[targetNode] <= nodeIndex[node])
            {
               System.Diagnostics.Debug.WriteLine($"Violation: {targetNode.Name} comes before {node.Name} (Outgoing link check failed).");
               return false;
            }
         }
      }

      System.Diagnostics.Debug.WriteLine("Topological sort is valid.");
      return true;
   }

   /*
   public static void PrintAllProvinceHistories()
   {
      var sb = new StringBuilder();
      var sb2 = new StringBuilder();
      var total = 0;
      var totalEffects = 0;
      var validEffects = 0;
      var invalidEffects = 0;
      var provDummy = new Province();
      List<Effect> effects = [];
      HashSet<string> effectNames = [];
      foreach (var province in Globals.Provinces.Values)
      {
         sb.AppendLine($"Province: {province.Id,4} with {province.History.Count,2} entries:");
         total += province.History.Count;
         foreach (var history in province.History)
         {
            sb.AppendLine($"\t{DateTime.Now.ToString("yyyy.MM.dd")} [{history.Effects.Count,1}]");
            totalEffects += history.Effects.Count;
            effects.AddRange(history.Effects);
         }
      }

      foreach (var effect in effects)
      {
         effectNames.Add(effect.GenericName);
         if (effect.ExecuteProvince(provDummy))
         {
            validEffects ++;
         }
         else
         {
            invalidEffects ++;
            sb2.AppendLine(effect.ToString());
         }
      }

      sb2.Append("\n------------------------------------------------\n");
      sb2.AppendLine($"{effectNames.Count} different effects: ");
      foreach (var eff in effectNames)
      {
         sb2.AppendLine(eff);
      }

      sb2.Insert(0, $"Working: {validEffects} | not working: {invalidEffects}\n------------------------------------------------\n");
      sb2.Insert(0, $"Total: {total} entries with {totalEffects} effects\n");
      
      File.WriteAllText(@"C:\Users\david\Downloads\provinceHistoriesEffectsDEBUG.txt", sb2.ToString());

      sb.Insert(0, $"{total} entries with {totalEffects} effects\n------------------------------------------------\n");
      File.WriteAllText(@"C:\Users\david\Downloads\provinceHistoriesDEBUG.txt", sb.ToString());
   }



   public static void BuildBlockString(int tabs, IElement element, ref StringBuilder sb)
   {
      if (element.IsBlock)
      {
         var block = (Block)element;
         sb.Append(GetTabs(tabs));
         sb.Append(block.GenericName);
         sb.Append(" : \n");
         foreach (var subBlock in block.Blocks)
         {
            BuildBlockString(tabs + 1, subBlock, ref sb);
         }
      }
      else
      {
         //sb.Append(GetTabs(tabs) + "\"" + ((Content)element).Value + "\"\n");
      }
   }

   private static string GetTabs(int tabs)
   {
      return new('\t', tabs);
   }
   
   public static void PrintProvinceHistories()
   {
      var sb = new StringBuilder();
      foreach (var province in Globals.Provinces.Values)
      {
         
      }
      File.WriteAllText(@"C:\Users\david\Downloads\provinceHistoriesDEBUG.txt", sb.ToString());
   }

   public static void TestHistory()
   {
      var history = new HistoryManager(new CInitial());
      history.AddCommand(new CInitial());
      history.AddCommand(new CInitial());
      history.AddCommand(new CInitial());
      history.Undo();
      history.Undo();
      history.AddCommand(new CInitial());
      history.AddCommand(new CInitial());
      history.Undo();
      history.AddCommand(new CInitial());
      history.Undo();
      history.AddCommand(new CInitial());
      history.AddCommand(new CInitial());

      // Pop up the history tree form
      var historyTree = new HistoryTree(history.RevertTo);
      historyTree.Visualize(history.GetRoot());
      historyTree.ShowDialog();
   }




   public static void PrintOptimizedProvinces()
   {
      var sb = new StringBuilder();
      sb.AppendLine("ID   .. Pixels .. PixelPrt .. Borders .. BorderPtr");
      foreach (var province in Globals.Provinces.Values)
      {
         sb.AppendLine($"ID: {province.Id,4} .. {province.PixelCnt,6} .. {province.PixelPtr,8} .. {province.BorderCnt,6} .. {province.BorderPtr,8}");
      }
      File.WriteAllText(@"C:\Users\david\Downloads\provincesDEBUG.txt", sb.ToString());
   }

   public static void PrintAdjacencies()
   {
      var sb = new StringBuilder();
      foreach (var kvp in Globals.AdjacentProvinces)
      {
         sb.Append($"ProvincePtr: [{kvp.Key,4}] is adjacent to: [");
         foreach (var adj in kvp.Value)
         {
            sb.Append($"{adj,4}, ");
         }
         if (kvp.Value.Length > 0)
            sb.Remove(sb.Length - 2, 2);
         sb.Append("]");
         sb.AppendLine();
      }
      File.WriteAllText(@"C:\Users\david\Downloads\adjacenciesDEBUG.txt", sb.ToString());
   }

   public static void PrintRegions()
   {
      var sb = new StringBuilder();
      foreach (var region in Globals.Regions.Values)
      {
         sb.AppendLine($"Region: {region.GenericName} = {{");
         foreach (var regionArea in region.Areas)
         {
            sb.AppendLine($"\t{regionArea}");
         }

         foreach (var monsoon in region.Monsoon)
         {
            sb.AppendLine($"\t{monsoon.Start} -- {monsoon.End}");
         }

         sb.AppendLine("}");

      }
      File.WriteAllText(@"C:\Users\david\Downloads\regionsDEBUG.txt", sb.ToString());
   }

   public static void PrintCultures(List<CultureGroup> groups)
   {
      var sb = new StringBuilder();
      foreach (var group in groups)
      {
         sb.AppendLine($"Group: {group.GenericName} with [{group.Cultures.Count}] and names [{group.TotalNameCount}]");
         foreach (var culture in group.Cultures)
         {
            sb.AppendLine($"\tName: {culture.GenericName} with Names: [{culture.TotalNameCount}] and Primaries: [{culture.Primaries.Count}]");
         }
      }

      File.WriteAllText(@"C:\Users\david\Downloads\culturesDEBUG.txt", sb.ToString());
   }

   public static string GetListAsString<T>(List<T> list) where T : notnull
   {
      var sb = new StringBuilder();
      foreach (var item in list)
      {
         sb.Append($"{item.ToString()}, ");
      }
      sb.Remove(sb.Length - 2, 2);
      return sb.ToString();
   }

   public static void PrintCountriesBasic()
   {
      var sb = new StringBuilder();
      foreach (var country in Globals.Countries.Values)
      {
         sb.AppendLine($"Country: {country.Tag} with score {country.HistoricalScore}");
         sb.AppendLine($"\tFemale Names: {country.LeaderNames.Count}");
         sb.AppendLine($"\tArmy Names: {country.ArmyNames.Count}");
         sb.AppendLine($"\tNavy Names: {country.ShipNames.Count}");
         sb.AppendLine($"\tMale Names: {country.FleeTNames.Count}");
         sb.AppendLine($"\tMonarch Names: {country.MonarchNames.Count}");
         sb.AppendLine($"\tHistorical Ideas: {country.HistoricalIdeas.Count}");
         sb.AppendLine($"\tHistorical Units: {country.HistoricalUnits.Count}");
      }
      File.WriteAllText(@"C:\Users\david\Downloads\countriesDEBUG.txt", sb.ToString());
   }

   public static void PrintTopLevelBlocks(List<IElement> elements)
   {
      var sb = new StringBuilder();
      foreach (var element in elements)
      {
         if (element.IsBlock)
         {
            var block = (Block)element;
            BuildBlockString(0, block, ref sb);
         }
      }

      File.WriteAllText(@"C:\Users\david\Downloads\topLevelBlocksDEBUG.txt", sb.ToString());
   }
   */
}