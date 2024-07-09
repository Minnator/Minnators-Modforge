using System.Drawing;
using Editor.DataClasses.MapModes;
using Editor.Helper;

namespace Editor.MapModes;

public class AutonomyMapMode : MapMode
{
   public override bool IsLandOnly => true;

   public AutonomyMapMode()
   {
      // Subscribe to events to update the min and max values when a province's development changes
      ProvinceEventHandler.OnProvinceLocalAutonomyChanged += UpdateProvince;
   }

   public override Color GetProvinceColor(int id)
   {
      if (Globals.SeaProvinces.Contains(id))
         return Globals.Provinces[id].Color;
      return Globals.ColorProvider.GetColorOnGreenRedShade(100, 0, Globals.Provinces[id].LocalAutonomy);
   }

   public override string GetMapModeName()
   {
      return "Autonomy";
   }
}