using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Events;

namespace Editor.DataClasses.MapModes
{
   public class HreMapMode : MapMode
   {
      public override bool IsLandOnly => true;

      public HreMapMode()
      {
         // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceIsHreChanged += UpdateProvince!;
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

   }
}