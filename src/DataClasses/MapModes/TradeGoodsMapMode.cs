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
         // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceTradeGoodChanged += UpdateProvince;
      }

      public override int GetProvinceColor(Province province)
      {
         return province.TradeGood.Color.ToArgb();
      }

      public override MapModeType MapModeType => MapModeType.TradeGoods;

      public override string GetSpecificToolTip(Province provinceId)
      {
         var tradeGood = provinceId.TradeGood;
         if (tradeGood != TradeGood.Empty)
            return $"Trade Good: {tradeGood} ({tradeGood.Name})";
         return $"Trade Good: Unknown";
      }

   }
}