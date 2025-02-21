using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Events;

namespace Editor.DataClasses.MapModes
{
   public class CountryMapMode : MapMode
   {
      public CountryMapMode()
      {
         // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceOwnerChanged += UpdateProvince;
         // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceControllerChanged += UpdateProvince;
         Country.ItemsModified += UpdateProvinceCollection;
         Country.ColorChanged += UpdateComposite<Province>;
      }

      public override bool IsLandOnly => true;
      public override bool ShowOccupation => true;

      public override int GetProvinceColor(Province id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
         {
            if (province.Owner == Country.Empty)
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
            if (province.Owner == Country.Empty)
               return "Country: [Unknown]";
            if (Globals.Countries.TryGetValue(province.Owner, out var country))
               return $"Country: {country.Tag} ({country.TitleLocalisation})";
         }
         return "Country: [Unknown]";
      }
   }
}