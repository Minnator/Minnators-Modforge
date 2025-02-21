using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Events;

namespace Editor.DataClasses.MapModes
{
   public class HasCapitalMapMode : MapMode
   {
      public override bool IsLandOnly => true;

      public HasCapitalMapMode()
      {
         // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceCapitalChanged += UpdateProvince!;
      }

      public override int GetProvinceColor(Province id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
            return province.Capital != string.Empty ? Color.Green.ToArgb() : Color.DimGray.ToArgb();
         return Color.DimGray.ToArgb();
      }

      public override MapModeType MapModeType => MapModeType.HasCapital;

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return province.Capital != string.Empty ? province.Capital : "No Capital name";
         return string.Empty;
      }

   }
}