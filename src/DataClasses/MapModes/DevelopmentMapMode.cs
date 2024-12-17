using Editor.DataClasses.GameDataClasses;
using Editor.Events;

namespace Editor.DataClasses.MapModes;

public class DevelopmentMapMode : MapMode
{
   private int _max = int.MinValue;

   public override bool IsLandOnly => true;

   public DevelopmentMapMode()
   {
      // Subscribe to events to update the min and max values when a province's development changes
      ProvinceEventHandler.OnProvinceBaseManpowerChanged += UpdateMinMax;
      ProvinceEventHandler.OnProvinceBaseManpowerChanged += UpdateProvince;
      ProvinceEventHandler.OnProvinceBaseTaxChanged += UpdateMinMax;
      ProvinceEventHandler.OnProvinceBaseTaxChanged += UpdateProvince;
      ProvinceEventHandler.OnProvinceBaseProductionChanged += UpdateMinMax;
      ProvinceEventHandler.OnProvinceBaseProductionChanged += UpdateProvince;
   }

   private void UpdateMinMax(object? sender, ProvinceEventHandler.ProvinceDataChangedEventArgs e)
   {
      if (!CalculateMinMax())
         return;

      if (MapModeManager.CurrentMapMode != this)
         return;
      RenderMapMode(GetProvinceColor);
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
         var totalDev = province.GetTotalDevelopment();
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

   public override void RenderMapMode(Func<Province, int> method)
   {
      CalculateMinMax();
      base.RenderMapMode(method);
   }

   public override int GetProvinceColor(Province id)
   {
      if (!Globals.LandProvinces.Contains(id))
         return id.Color.ToArgb();

      var totalDev = id.GetTotalDevelopment();
      return Globals.ColorProvider.GetColorOnGreenRedShade(0, _max, totalDev).ToArgb();
   }


   public override MapModeType MapModeType => MapModeType.Development;

   public override string GetSpecificToolTip(Province provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         return $"Total Development: {province.GetTotalDevelopment()}\nBaseTax: {province.BaseTax}\nBaseProduction: {province.BaseProduction}\nBaseManpower: {province.BaseManpower}";
      return "Total Development: Unknown";
   }
}