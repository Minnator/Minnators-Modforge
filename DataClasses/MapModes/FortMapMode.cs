using System.Drawing;
using Editor.DataClasses.MapModes;
using Editor.Helper;

namespace Editor.MapModes;

public class FortMapMode : MapMode
{
   //TODO read min an max from defines
   public override bool IsLandOnly => true;

   public FortMapMode()
   {
      // Subscribe to events to update the min and max values when a province's development changes
      ProvinceEventHandler.OnProvinceHasFort15thChanged += UpdateProvince;
   }

   public override Color GetProvinceColor(int id)
   {
      if (Globals.SeaProvinces.Contains(id))
         return Globals.Provinces[id].Color;

      // TODO: Expand to better forts
      switch (GetFortLevel(id))
      {
         case 0:
            return Color.DimGray;
         case 1:
            return Color.FromArgb(0, 255, 0);
         case 2:
            return Color.BurlyWood;
         default:
            return Color.DimGray;
      }
   }

   private int GetFortLevel(int id)
   {
      if (Globals.Provinces[id].HasFort15Th)
         return 2;
      return 0;
   }

   public override string GetMapModeName()
   {
      return "Fort Level";
   }

}