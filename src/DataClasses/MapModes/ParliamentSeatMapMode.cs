using Editor.DataClasses.GameDataClasses;
using Editor.Events;

namespace Editor.DataClasses.MapModes
{
   public class ParliamentSeatMapMode : MapMode
   {
      public override bool IsLandOnly => true;

      public ParliamentSeatMapMode()
      {
         ProvinceEventHandler.OnProvinceIsSeatInParliamentChanged += UpdateProvince!;
      }

      public override int GetProvinceColor(Province id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
            return province.IsSeatInParliament ? Color.Green.ToArgb() : Color.DimGray.ToArgb();
         return Color.DimGray.ToArgb();
      }

      public override MapModeType MapModeType => MapModeType.ParliamentSeat;

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return province.IsSeatInParliament ? "Seat in Parliament" : "No Seat in Parliament";
         return string.Empty;
      }

      public override bool ShouldProvincesMerge(Province p1, Province p2)
      {
         return p1.IsSeatInParliament == p2.IsSeatInParliament;
      }
   }
}