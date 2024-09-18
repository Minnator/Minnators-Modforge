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

      public override string GetMapModeName()
      {
         return "Devastation";
      }

      public override string GetSpecificToolTip(int provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return $"Devastation: {province.Devastation}";
         return "Devastation: -";
      }

      public override Color GetProvinceColor(int id)
      {
         if (Globals.SeaProvinces.Contains(id))
            return Globals.Provinces[id].Color;
         return Globals.ColorProvider.GetColorOnGreenRedShade(100, 0, Globals.Provinces[id].Devastation);
      }
   }
}