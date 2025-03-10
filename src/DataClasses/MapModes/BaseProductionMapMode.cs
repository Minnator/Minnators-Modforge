using Editor.DataClasses.Saveables;
using Editor.Saving;

namespace Editor.DataClasses.MapModes
{
   public class BaseProductionMapMode : MapMode
   {
      private int _max = int.MinValue;
      
      public override int GetProvinceColor(Province id)
      {
         if (!Globals.LandProvinces.Contains(id))
            return id.Color.ToArgb();

         return Globals.ColorProvider.GetColorOnGreenRedShade(0, _max, id.BaseProduction).ToArgb();
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return $"Base production: {province.BaseProduction}";
         return "nBaseTax: Unknown";
      }

      public override void SetActive()
      {
         BaseTaxMapMode.CalculateMinMax(ref _max, typeof(Province).GetProperty(nameof(Province.BaseProduction))!);
         base.SetActive();
      }
      public override MapModeType MapModeType { get; } = MapModeType.BaseProduction;

   }
}