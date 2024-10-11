using Editor.Events;

namespace Editor.DataClasses.MapModes
{
   public class HasCapitalMapMode : MapMode
   {
      public override bool IsLandOnly => true;

      public HasCapitalMapMode()
      {
         ProvinceEventHandler.OnProvinceCapitalChanged += UpdateProvince!;
      }

      public override int GetProvinceColor(int id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
            return province.Capital != string.Empty ? Color.Green.ToArgb() : Color.DimGray.ToArgb();
         return Color.DimGray.ToArgb();
      }

      public override string GetMapModeName()
      {
         return "Has Capital GenericName";
      }

      public override string GetSpecificToolTip(int provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return province.Capital != string.Empty ? province.Capital : "No Capital name";
         return string.Empty;
      }
   }
}