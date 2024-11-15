using System.Text;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Saving
{
   public static class SaveTradeNodes
   {


      public static void FormatTradeNode(TradeNode tn, ref StringBuilder sb)
      {
         var tabs = 0;
         sb.AppendLine($"{tn.Name} = {{");
         tabs++;
         SavingUtil.AddColor(tabs, tn.Color, ref sb);
         SavingUtil.AddInt(tabs, "location", tn.Location, ref sb);
         SavingUtil.AddBool(tabs, tn.IsInland, "inland", ref sb);
         foreach (var outgoing in tn.Outgoing)
         {
            FormatOutGoing(tabs, outgoing, ref sb);
         }

         List<int> members = [];
         foreach (var member in tn.GetProvinces())
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
         var control = outgoing.Control;
         if (control.Count > 2)
         {
            control.RemoveAt(0);
            control.RemoveAt(control.Count - 1);
         }

         SavingUtil.AddFormattedPointFList("control", control, tabs, true, ref sb);
         SavingUtil.AddTabs(tabs - 1, ref sb);
         sb.AppendLine("}");
      }
   }
}