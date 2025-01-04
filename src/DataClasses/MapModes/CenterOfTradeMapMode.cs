using Editor.DataClasses.GameDataClasses;
using Editor.Events;

namespace Editor.DataClasses.MapModes;

public class CenterOfTradeMapMode : MapMode
{
   //TODO read min an max from defines

   public override bool IsLandOnly => true;

   public CenterOfTradeMapMode()
   {
      // Subscribe to events to update the min and max values when a province's development changes
      // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceCenterOfTradeLevelChanged += UpdateProvince;
   }

   public override int GetProvinceColor(Province id)
   {
      if (Globals.SeaProvinces.Contains(id) || Globals.LakeProvinces.Contains(id))
         return id.Color.ToArgb();

      return id.CenterOfTrade switch
      {
         0 => Color.DimGray.ToArgb(),
         1 => Color.FromArgb(0, 0, 255).ToArgb(),
         2 => Color.FromArgb(0, 255, 0).ToArgb(),
         3 => Color.FromArgb(255, 0, 0).ToArgb(),
         _ => Color.FromArgb(255, 255, 255).ToArgb()
      };
   }

   public override MapModeType MapModeType => MapModeType.CenterOfTrade;

   public override string GetSpecificToolTip(Province provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         return province.CenterOfTrade switch
         {
            0 => "No center of trade",
            1 => "Center of trade: [Level 1]",
            2 => "Center of trade: [Level 2]",
            3 => "Center of trade: [Level 3]",
            _ => "Center of trade: [Unknown]"
         };
      return "Center of Trade: [Unknown]";
   }
}