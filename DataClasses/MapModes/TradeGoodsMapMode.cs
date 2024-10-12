using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class TradeGoodsMapMode : MapMode
   {
      public override bool IsLandOnly => true;

      public TradeGoodsMapMode()
      {
         ProvinceEventHandler.OnProvinceTradeGoodChanged += UpdateProvince;
      }

      public override int GetProvinceColor(Province id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
         {
            var tradeGood = TradeGoodHelper.StringToTradeGood(province.TradeGood);
            return tradeGood.Color.ToArgb();
         }
         return Color.DimGray.ToArgb();
      }

      public override string GetMapModeName()
      {
         return "Trade Goods";
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
         {
            var tradeGood = TradeGoodHelper.StringToTradeGood(province.TradeGood);
            return $"Trade Good: {tradeGood.Name} ({Localisation.GetLoc(tradeGood.Name)})";
         }
         return $"Trade Good: Unknown";
      }
   }
}