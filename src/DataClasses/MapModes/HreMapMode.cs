using Editor.DataClasses.GameDataClasses;
using Editor.Events;

namespace Editor.DataClasses.MapModes
{
   public class HreMapMode : MapMode
   {
      public override bool IsLandOnly => true;

      public HreMapMode()
      {
         ProvinceEventHandler.OnProvinceIsHreChanged += UpdateProvince!;
      }

      public override int GetProvinceColor(Province id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
         {
            if (Globals.Countries.TryGetValue(province.Owner, out var country))
               if (country.HistoryCountry.IsElector)
                  return Color.Purple.ToArgb();
            if (province.IsHre)
               return Color.Green.ToArgb();
         }

         return Color.DimGray.ToArgb();
      }

      public override MapModeType MapModeType => MapModeType.Hre;

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return province.IsHre ? "HRE" : "Not HRE";
         return "No province";
      }

      public override bool ShouldProvincesMerge(Province p1, Province p2)
      {
         return p1.IsHre == p2.IsHre;
      }
   }
}