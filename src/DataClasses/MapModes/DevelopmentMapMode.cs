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

   private void UpdateMinMax(object? sender, EventArgs e)
   {
      if (!CalculateMinMax())
         return;

      if (MapModeManager.CurrentMapMode != this)
         return;
      RenderMapMode();
   }

   /// <summary>
   /// Returns if a new max was found
   /// </summary>
   /// <returns></returns>
   public bool CalculateMinMax() //TODO this is calculated twice
   {
      var newMaxFound = false;
      var newMax = int.MinValue;
      foreach (var province in Globals.Provinces)
      {
         var totalDev = province.TotalDevelopment;
         if (totalDev > newMax)
         {
            newMax = totalDev;
            if (newMax > _max)
            {
               _max = newMax;
               newMaxFound = true;
            }
         }
      }

      // If the new max is smaller than the current max, update the max
      if (newMax < _max)
      {
         _max = newMax;
         newMaxFound = true;
      }
      return newMaxFound;
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
      CalculateMinMax();
      base.SetActive();
   }
}