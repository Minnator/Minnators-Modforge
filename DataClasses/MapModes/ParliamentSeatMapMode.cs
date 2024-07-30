using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class ParliamentSeatMapMode : MapMode
   {
      public override bool IsLandOnly => true;

      public ParliamentSeatMapMode()
      {
         ProvinceEventHandler.OnProvinceIsSeatInParliamentChanged += UpdateProvince!;
      }

      public override Color GetProvinceColor(int id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
            return province.IsSeatInParliament ? Color.Green : Color.DimGray;
         return Color.DimGray;
      }

      public override string GetMapModeName()
      {
         return "Parliament Seats";
      }

      public override string GetSpecificToolTip(int provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return province.IsSeatInParliament ? "Seat in Parliament" : "No Seat in Parliament";
         return string.Empty;
      }
   }
}