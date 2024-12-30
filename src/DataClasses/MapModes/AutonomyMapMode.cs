using Editor.DataClasses.GameDataClasses;
using Editor.Events;

namespace Editor.DataClasses.MapModes;

public class AutonomyMapMode : MapMode
{
   public override bool IsLandOnly => true;
   public override string IconFileName => "mapmode_local_autonomy.dds";

   public AutonomyMapMode()
   {
      // Subscribe to events to update the min and max values when a province's development changes
      ProvinceEventHandler.OnProvinceLocalAutonomyChanged += UpdateProvince;
      base.CropAndSetIcon();
   }

   public override int GetProvinceColor(Province id)
   {
      if (Globals.SeaProvinces.Contains(id))
         return id.Color.ToArgb();
      return Globals.ColorProvider.GetColorOnGreenRedShade(100, 0, id.LocalAutonomy).ToArgb();
   }

   public override MapModeType MapModeType => MapModeType.Autonomy;

   public override string GetSpecificToolTip(Province provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         return $"Autonomy: [{province.LocalAutonomy}%]";
      return "Autonomy: [Unknown]";
   }

   public override bool ShouldProvincesMerge(Province p1, Province p2)
   {
      return Math.Abs(p1.LocalAutonomy - p2.LocalAutonomy) < 0.001;
   }
}