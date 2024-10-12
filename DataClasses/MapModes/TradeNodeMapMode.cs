using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class TradeNodeMapMode : MapMode
   {
      public override string GetMapModeName()
      {
         return "Trade Nodes";
      }

      public override int GetProvinceColor(Province id)
      {
         var node = TradeNodeHelper.GetTradeNodeByProvince(id);
         if (Equals(node, TradeNode.Empty))
            return Color.DimGray.ToArgb();
         return node.Color.ToArgb();
      }

      public override void RenderMapMode(Func<Province, int> method)
      {
         base.RenderMapMode(method);
         MapDrawing.WriteOnProvince(GetCoTLevel);
      }

      private string GetCoTLevel(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var prov) && prov.CenterOfTrade > 0)
            return prov.CenterOfTrade.ToString();
         return string.Empty;
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         var node = TradeNodeHelper.GetTradeNodeByProvince(provinceId);
         if (Equals(node, TradeNode.Empty))
            return "TradeNode: <undefined>";
         var sb = new StringBuilder();
         sb.AppendLine($"TradeNode: {node.Name} ({Localisation.GetLoc(node.Name)})");
         sb.AppendLine($"Inland: {node.IsInland}");
         if (Globals.Provinces.TryGetValue(provinceId, out var prov))
            if (prov.CenterOfTrade > 0)
               sb.AppendLine($"Center of Trade: {prov.CenterOfTrade}");
         sb.AppendLine($"Outgoing: ");
         sb.Append("\t");
         if (node.Outgoing.Count > 0)
            foreach (var outgoing in node.Outgoing)
               sb.Append($" {outgoing},");
         sb.Remove(sb.Length - 1, 1);
         sb.AppendLine();
         sb.AppendLine($"Incoming: ");
         sb.Append("\t");
         if (node.Incoming.Count > 0)
            foreach (var incoming in node.Incoming)
               sb.Append($" {incoming},");
         sb.Remove(sb.Length - 1, 1);
         return sb.ToString();
      }
   }
}