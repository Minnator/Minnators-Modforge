using System.Diagnostics;
using System.IO;
using System.Text;
using Editor.Commands;

namespace Editor;

public static class DebugPrints
{

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