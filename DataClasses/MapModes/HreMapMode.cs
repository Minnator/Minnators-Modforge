using Editor.DataClasses.GameDataClasses;
using Editor.Events;

namespace Editor.DataClasses.MapModes
{
   public class HreMapMode : MapMode
   {
      public override bool IsLandOnly => true;

      public HreMapMode()
      {
         ProvinceEventHandler.OnProvinceIsHreChanged += UpdateProvince!;
      }

      public override int GetProvinceColor(Province id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
            return province.IsHre ? Color.Green.ToArgb() : Color.DimGray.ToArgb();
         return Color.DimGray.ToArgb();
      }

      public override string GetMapModeName()
      {
         return MapModeType.Hre.ToString();
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return province.IsHre ? "HRE" : "Not HRE";
         return "No province";
      }
   }
}