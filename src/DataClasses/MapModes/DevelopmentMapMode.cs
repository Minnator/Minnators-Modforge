using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Events;

namespace Editor.DataClasses.MapModes;

public class DevelopmentMapMode : MapMode
{
   private int _max = int.MinValue;

   public override bool IsLandOnly => true;

   public DevelopmentMapMode()
   {
      // Subscribe to events to update the min and max values when a province's development changes
      // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceBaseManpowerChanged += UpdateMinMax;
      // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceBaseManpowerChanged += UpdateProvince;
      // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceBaseTaxChanged += UpdateMinMax;
      // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceBaseTaxChanged += UpdateProvince;
      // TODO FIX MAP MODE UPDATESProvinceEventHandler.OnProvinceBaseProductionChanged += UpdateMinMax;
      // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceBaseProductionChanged += UpdateProvince;
   }

   
   public override int GetProvinceColor(Province id)
   {
      if (!Globals.LandProvinces.Contains(id))
         return id.Color.ToArgb();

      var totalDev = id.TotalDevelopment;
      return Globals.ColorProvider.GetColorOnGreenRedShade(0, _max, totalDev).ToArgb();
   }


   public override MapModeType MapModeType => MapModeType.Development;

   public override string GetSpecificToolTip(Province provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         return $"Total Development: {province.TotalDevelopment}\nBaseTax: {province.BaseTax}\nBaseProduction: {province.BaseProduction}\nBaseManpower: {province.BaseManpower}";
      return "Total Development: Unknown";
   }

   public override void SetActive()
   {
      BaseTaxMapMode.CalculateMinMax(ref _max, typeof(Province).GetProperty(nameof(Province.TotalDevelopment))!);
      base.SetActive();
   }
}