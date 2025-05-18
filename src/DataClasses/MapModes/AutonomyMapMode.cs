using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Events;

namespace Editor.DataClasses.MapModes;

public class AutonomyMapMode : MapMode
{
   public override bool IsLandOnly => true;
   public override string IconFileName => "mapmode_local_autonomy.dds";

   public AutonomyMapMode()
   {
      // TODO FIX MAP MODE UPDATES  ProvinceEventHandler.OnProvinceLocalAutonomyChanged += UpdateProvince;
   }

   public override int GetProvinceColor(Province id)
   {
      if (Globals.SeaProvinces.Contains(id))
         return id.Color.ToArgb();
      return Globals.ColorProvider.GetColorOnGreenRedShade(100, 0, id.Autonomy).ToArgb();
   }

   public override MapModeType MapModeType => MapModeType.Autonomy;

   public override string GetSpecificToolTip(Province provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         return $"Autonomy: [{province.Autonomy}%]";
      return "Autonomy: [Unknown]";
   }

}