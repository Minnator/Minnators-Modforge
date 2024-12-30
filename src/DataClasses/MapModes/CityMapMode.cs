using Editor.DataClasses.GameDataClasses;
using Editor.Events;

namespace Editor.DataClasses.MapModes
{
   public class CityMapMode : MapMode
   {
      public override bool IsLandOnly => true;

      public CityMapMode()
      {
         ProvinceEventHandler.OnProvinceIsCityChanged += UpdateProvince!;
      }

      public override int GetProvinceColor(Province id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
            return province.IsCity ? Color.Green.ToArgb() : Color.DimGray.ToArgb();
         return Color.DimGray.ToArgb();
      }

      public override MapModeType MapModeType => MapModeType.City;

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return province.IsCity ? "Is City" : "colonial";
         return string.Empty;
      }

      public override bool ShouldProvincesMerge(Province p1, Province p2)
      {
         return p1.IsCity == p2.IsCity;
      }
   }
}