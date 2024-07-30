using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class CityMapMode : MapMode
   {
      public override bool IsLandOnly => true;

      public CityMapMode()
      {
         ProvinceEventHandler.OnProvinceIsCityChanged += UpdateProvince!;
      }

      public override Color GetProvinceColor(int id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
            return province.IsCity ? Color.Green : Color.DimGray;
         return Color.DimGray;
      }

      public override string GetMapModeName()
      {
         return "City";
      }

      public override string GetSpecificToolTip(int provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return province.IsCity ? "Is City" : "colonial";
         return string.Empty;
      }
   }
}