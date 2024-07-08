using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Editor.Commands;
using Editor.Forms;
using Editor.Helper;

namespace Editor;

public static class DebugPrints
{




   public static void BuildBlockString(int tabs, IElement element, ref StringBuilder sb)
   {
      if (element.IsBlock)
      {
         var block = (Block)element;
         sb.Append(GetTabs(tabs));
         sb.Append(block.Name);
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

   public static void PrintAllAttributes(List<ParsingProvince> provinces)
   {
      var keys = GetAllUniqueAttributeKeys(provinces).ToList();
      var sb = new StringBuilder();
      keys.Sort();
      foreach (var key in keys)
      {
         sb.AppendLine(key);
      }
      File.WriteAllText(@"C:\Users\david\Downloads\allAttributesDEBUG.txt", sb.ToString());
   }

   public static HashSet<string> GetAllUniqueAttributeKeys(List<ParsingProvince> provinces)
   {
      var keys = new HashSet<string>();

      foreach (var province in provinces)
      {
         foreach (var attribute in province.Attributes)
         {
            keys.Add(attribute.Key);
         }
      }
      return keys;
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
         sb.AppendLine($"Region: {region.Name} = {{");
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
}