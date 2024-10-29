using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Helper;

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

      public override MapModeType GetMapModeName()
      {
         return MapModeType.Country;
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
         {
            if (province.Owner == Tag.Empty)
               return "Country: [Unknown]";
            if (Globals.Countries.TryGetValue(province.Owner, out var country))
               return $"Country: {country.Tag} ({country.GetLocalisation()})";
         }
         return "Country: [Unknown]";
      }

      public override void RenderMapMode(Func<Province, int> method)
      {
         base.RenderMapMode(method);
         MapDrawing.DrawAllCapitals(Color.Yellow.ToArgb());
      }
   }
}