using System.Diagnostics;
using Editor.DataClasses.MapModes;
using Editor.Events;

namespace Editor.MapModes;

public class FortMapMode : MapMode
{
   //TODO read min an max from defines
   public override bool IsLandOnly => true;

   public FortMapMode()
   {
      // Subscribe to events to update the min and max values when a province's development changes
      ProvinceEventHandler.OnProvinceBuildingsChanged += UpdateProvince;
   }

   public override int GetProvinceColor(int id)
   {
      if (Globals.SeaProvinces.Contains(id))
         return Globals.Provinces[id].Color.ToArgb();

      // TODO: Expand to better forts
      switch (GetFortLevel(id))
      {
         case 0:
            return Color.DimGray.ToArgb();
         case 1:
            return Color.FromArgb(0, 255, 0).ToArgb();
         case 2:
            return Color.BurlyWood.ToArgb();
         case 3:
            return Color.FromArgb(255, 255, 0).ToArgb();
         case 4:
            return Color.FromArgb(255, 165, 0).ToArgb();
         case 5:
            return Color.FromArgb(255, 0, 0).ToArgb();
         case 6:
            return Color.FromArgb(139, 0, 0).ToArgb();
         case 7:
            return Color.FromArgb(128, 0, 128).ToArgb();
         case 8:
            return Color.FromArgb(120, 0, 230).ToArgb();
         case 9:
            return Color.FromArgb(0, 0, 0).ToArgb();
         default:
            return Color.FloralWhite.ToArgb();
      }
   }

   private int GetFortLevel(int id)
   {
      var level = 0;
      if (Globals.Provinces[id].Buildings.Contains("fort_15th"))
         level += 2;
      if (Globals.Provinces[id].Buildings.Contains("fort_16th"))
         level += 4;
      if (Globals.Provinces[id].Buildings.Contains("fort_17th"))
         level += 6;
      if (Globals.Provinces[id].Buildings.Contains("fort_18th"))
         level += 8;

      if (Globals.Capitals.Contains(id))
         level += 1;
      return level;
   }

   public override string GetMapModeName()
   {
      return "Fort Level";
   }

   public override string GetSpecificToolTip(int provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         return $"Fort Level: {GetFortLevel(provinceId)}";
      return $"No fort in {Globals.Provinces[provinceId].GetLocalisation()}";
   }

}