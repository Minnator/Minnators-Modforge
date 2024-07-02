using System;
using System.Drawing;
using System.Windows.Forms;
using Editor.DataClasses;
using Editor.Helper;
using Editor.Interfaces;

namespace Editor.MapModes;

public class DevelopmentMapMode : MapMode
{
   private int Max = int.MinValue;
   private int Min = int.MaxValue;
   
   public DevelopmentMapMode()
   {
      // Subscribe to events to update the min and max values when a province's development changes
      ProvinceEventHandler.OnProvinceBaseManpowerChanged += UpdateMinMax;
      ProvinceEventHandler.OnProvinceBaseTaxChanged += UpdateMinMax;
      ProvinceEventHandler.OnProvinceBaseProductionChanged += UpdateMinMax;
   }

   private void UpdateMinMax(object sender, ProvinceEventHandler.ProvinceDataChangedEventArgs e)
   {
      if (e.Value is not int newDev)
         return;
      var modifiedMinMax = false;

      if (newDev > Max)
      {
         Max = newDev;
         modifiedMinMax = true;
      }
      if (newDev < Min)
      {
         Min = newDev;
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
         if (province.GetTotalDevelopment() > Max)
            Max = province.GetTotalDevelopment();
         if (province.GetTotalDevelopment() < Min)
            Min = province.GetTotalDevelopment();
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
      return Globals.ColorProvider.GetColorOnGreenRedShade(Min, Max, totalDev);
   }


   public override string ToString()
   {
      return "Development";
   }
}