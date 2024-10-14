using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.Events;
using Editor.Helper;

namespace Editor.MapModes;

public sealed class AreaMapMode : MapMode
{
   public AreaMapMode()
   {
      // TODO listen to the provinces areas
      ProvinceEventHandler.OnProvinceAreaChanged += UpdateProvince!;
   }

   public override string GetMapModeName()
   {
      return MapModeType.Area.ToString();
   }

   public override int GetProvinceColor(Province id)
   {
      if (Globals.Provinces.TryGetValue(id, out var province))
         if (Globals.Areas.TryGetValue(province.Area, out var area))
            return area.Color.ToArgb();
      return Color.DarkGray.ToArgb();
   }

   public override string GetSpecificToolTip(Province provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         if (Globals.Areas.TryGetValue(province.Area, out var area))
            return $"Area: {area.Name} ({Localisation.GetLoc(area.Name)})";
      return "Area: [Unknown]";
   }
}