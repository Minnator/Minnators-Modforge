using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class TradeNodeMapMode : CollectionMapMode
   {
      public override bool IsLandOnly => false;

      public TradeNodeMapMode()
      {
         TradeNode.ItemsModified += UpdateProvinceCollection;
         TradeNode.ColorChanged += UpdateComposite<Province>;

         Collections = [.. Globals.TradeNodes.Values];
      }

      public override MapModeType MapModeType => MapModeType.TradeNode;

      public override int GetProvinceColor(Province id)
      {
         var node = TradeNodeHelper.GetTradeNodeByProvince(id);
         if (Equals(node, TradeNode.Empty))
            return Color.DimGray.ToArgb();
         return node.Color.ToArgb();
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
               sb.Append($" {outgoing.Target},");
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

      public override void SetActive()
      {
      }

      public override void SetInactive()
      {
      }

   }
}