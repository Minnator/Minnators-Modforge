using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Savers
{
   public static class SaveTradeNodes
   {

      public static void SaveAllTradeNodes(List<TradeNode> tns)
      {
         StringBuilder sb = new();
         foreach (var tradeNode in tns) 
            FormatTradeNode(tradeNode, ref sb);

         IO.WriteToFile(Path.Combine(Globals.ModPath, "common", "tradenodes", "00_tradenodes.txt"), sb.ToString(), false);
      }

      private static void FormatTradeNode(TradeNode tn, ref StringBuilder sb)
      {
         var tabs = 0;
         sb.AppendLine($"{tn.Name} = {{");
         tabs++;
         SavingUtil.AddColor(tabs, tn.Color, ref sb);
         SavingUtil.AddInt(tabs, tn.Location, "location", ref sb);
         SavingUtil.AddBool(tabs, tn.IsInland, "inland", ref sb);
         foreach (var outgoing in tn.Outgoing)
         {
            FormatOutGoing(tabs, outgoing, ref sb);
         }

         List<int> members = [];
         foreach (var member in tn.Members)
            members.Add(member.Id);
         SavingUtil.AddFormattedIntList("members", members, tabs, ref sb);
         sb.AppendLine("}");
      }

      private static void FormatOutGoing(int tabs, Outgoing outgoing, ref StringBuilder sb)
      {
         SavingUtil.AddTabs(tabs, ref sb);
         sb.AppendLine("outgoing = {");
         tabs++;
         SavingUtil.AddTabs(tabs, ref sb);
         sb.AppendLine($"name = \"{outgoing.Target}\"");
         SavingUtil.AddFormattedIntList("path", outgoing.Path, tabs, ref sb);
         SavingUtil.AddFormattedPointFList("control", outgoing.Control, tabs, ref sb);
         SavingUtil.AddTabs(tabs - 1, ref sb);
         sb.AppendLine("}");
      }
   }
}