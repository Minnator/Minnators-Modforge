using Editor.DataClasses.GameDataClasses;
using Editor.Events;

namespace Editor.DataClasses.MapModes
{
   public class DevastationMapMode : MapMode
   {
      public override bool IsLandOnly => true;

      public DevastationMapMode()
      {
         // Subscribe to events to update the min and max values when a province's development changes
         ProvinceEventHandler.OnProvinceDevastationChanged += UpdateProvince;
      }

      public override MapModeType MapModeType => MapModeType.Devastation;

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return $"Devastation: {province.Devastation}";
         return "Devastation: -";
      }

      public override int GetProvinceColor(Province id)
      {
         if (Globals.SeaProvinces.Contains(id))
            return id.Color.ToArgb();
         return Globals.ColorProvider.GetColorOnGreenRedShade(100, 0, id.Devastation).ToArgb();
      }
   }
}