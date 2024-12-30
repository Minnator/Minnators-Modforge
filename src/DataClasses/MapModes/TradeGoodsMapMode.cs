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

      public override int GetProvinceColor(Province province)
      {
         var tradeGood = TradeGoodHelper.StringToTradeGood(province.TradeGood);
         return tradeGood.Color.ToArgb();
      }

      public override MapModeType MapModeType => MapModeType.TradeGoods;

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
         {
            var tradeGood = TradeGoodHelper.StringToTradeGood(province.TradeGood);
            return $"Trade Good: {tradeGood.Name} ({Localisation.GetLoc(tradeGood.Name)})";
         }
         return $"Trade Good: Unknown";
      }

      public override bool ShouldProvincesMerge(Province p1, Province p2)
      {
         return p1.TradeGood == p2.TradeGood;
      }
   }
}