using System.Drawing;
using Editor.Helper;

namespace Editor.MapModes;

public class CenterOfTradeMapMode : Interfaces.MapMode
{
   //TODO read min an max from defines

   public CenterOfTradeMapMode()
   {
      // Subscribe to events to update the min and max values when a province's development changes
      ProvinceEventHandler.OnProvinceCenterOfTradeLevelChanged += UpdateProvince;
   }

   public override Color GetProvinceColor(int id)
   {
      if (Globals.SeaProvinces.Contains(id) || Globals.LakeProvinces.Contains(id))
         return Globals.Provinces[id].Color;

      return Globals.Provinces[id].CenterOfTrade switch
      {
         0 => Color.DimGray,
         1 => Color.FromArgb(0, 0, 255),
         2 => Color.FromArgb(0, 255, 0),
         3 => Color.FromArgb(255, 0, 0),
         _ => Color.FromArgb(255, 255, 255)
      };
   }

   public override string GetMapModeName()
   {
      return "Center of Trade";
   }
}