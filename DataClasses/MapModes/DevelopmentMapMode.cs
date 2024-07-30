using System;
using System.Drawing;
using System.Windows.Forms;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.Helper;

namespace Editor.MapModes;

public class DevelopmentMapMode : MapMode
{
   private int _max = int.MinValue;
   private int _min = int.MaxValue;

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

   private void UpdateMinMax(object sender, ProvinceEventHandler.ProvinceDataChangedEventArgs e)
   {
      if (e.Value is not int newDev)
         return;
      var modifiedMinMax = false;

      if (newDev > _max)
      {
         _max = newDev;
         modifiedMinMax = true;
      }
      if (newDev < _min)
      {
         _min = newDev;
         modifiedMinMax = true;
      }

      if (Globals.MapModeManager.CurrentMapMode != this)
         return;

      if (modifiedMinMax)
      {
         RenderMapMode(GetProvinceColor);
      }
      else
      {
         if (sender is not int id)
            return;

         Update(id);
      }
   }

   public void CalculateMinMax()
   {
      foreach (var province in Globals.Provinces.Values)
      {
         if (province.GetTotalDevelopment() > _max)
            _max = province.GetTotalDevelopment();
         if (province.GetTotalDevelopment() < _min)
            _min = province.GetTotalDevelopment();
      }
   }

   public override void RenderMapMode(Func<int, Color> method)
   {
      CalculateMinMax();
      base.RenderMapMode(method);
   }

   public override Color GetProvinceColor(int id)
   {
      if (!Globals.LandProvinces.Contains(id))
         return Globals.Provinces[id].Color;

      var totalDev = Globals.Provinces[id].GetTotalDevelopment();
      return Globals.ColorProvider.GetColorOnGreenRedShade(_min, _max, totalDev);
   }


   public override string GetMapModeName()
   {
      return "Total Development";
   }

   public override string GetSpecificToolTip(int provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         return $"Total Development: {province.GetTotalDevelopment()}\nBaseTax: {province.BaseTax}\nBaseProduction: {province.BaseProduction}\nBaseManpower: {province.BaseManpower}";
      return "Total Development: Unknown";
   }
}