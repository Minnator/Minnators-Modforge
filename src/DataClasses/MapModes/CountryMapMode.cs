using Editor.DataClasses.GameDataClasses;
using Editor.Events;

namespace Editor.DataClasses.MapModes
{
   public class CountryMapMode : MapMode
   {
      public CountryMapMode()
      {
         ProvinceEventHandler.OnProvinceOwnerChanged += UpdateProvince;
         ProvinceEventHandler.OnProvinceControllerChanged += UpdateProvince;
         Country.ItemsModified += UpdateProvinceCollection;
         Country.ColorChanged += UpdateComposite<Province>;
      }

      public override bool IsLandOnly => true;
      public override bool ShowOccupation => true;

      public override int GetProvinceColor(Province id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
         {
            if (province.Owner == Tag.Empty)
               return Color.DimGray.ToArgb();
            if (Globals.Countries.TryGetValue(province.Owner, out var country))
               return country.Color.ToArgb();
         }
         return Color.DimGray.ToArgb();
      }

      public override MapModeType MapModeType => MapModeType.Country;

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
         {
            if (province.Owner == Tag.Empty)
               return "Country: [Unknown]";
            if (Globals.Countries.TryGetValue(province.Owner, out var country))
               return $"Country: {country.Tag} ({country.TitleLocalisation})";
         }
         return "Country: [Unknown]";
      }

      public override bool ShouldProvincesMerge(Province p1, Province p2)
      {
         return p1.Owner == p2.Owner;
      }
   }
}