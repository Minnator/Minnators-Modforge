using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class ProsperityMapMode : MapMode
   {
      public override bool IsLandOnly => true;

      public ProsperityMapMode()
      {
         // Subscribe to events to update the min and max values when a province's development changes
         ProvinceEventHandler.OnProvinceProsperityChanged += UpdateProvince;
      }

      public override string GetMapModeName()
      {
         return "Prosperity";
      }

      public override string GetSpecificToolTip(int provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return $"Prosperity: {province.Prosperity}";
         return "Prosperity: -";
      }

      public override Color GetProvinceColor(int id)
      {
         if (Globals.SeaProvinces.Contains(id))
            return Globals.Provinces[id].Color;
         return Globals.ColorProvider.GetColorOnGreenRedShade(0, 100, Globals.Provinces[id].Prosperity);
      }
   }
}