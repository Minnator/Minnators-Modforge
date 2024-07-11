using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class CountryMapMode : MapMode
   {
      public CountryMapMode()
      {
         ProvinceEventHandler.OnProvinceOwnerChanged += UpdateProvince;
      }

      public override bool IsLandOnly => true;

      public override Color GetProvinceColor(int id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
         {
            if (province.Owner == Tag.Empty)
               return Color.DimGray;
            if (Globals.Countries.TryGetValue(province.Owner, out var country))
               return country.Color;
         }
         return Color.DimGray;
      }

      public override string GetMapModeName()
      {
         return "Country";
      }

      public override string GetSpecificToolTip(int provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
         {
            if (province.Owner == Tag.Empty)
               return "Country: [Unknown]";
            if (Globals.Countries.TryGetValue(province.Owner, out var country))
               return $"Country: [{Localisation.GetLoc(country.Tag)} | {country.Tag}]";
         }
         return "Country: [Unknown]";
      }
   }
}