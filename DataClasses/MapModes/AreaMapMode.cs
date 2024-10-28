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

   public override MapModeType GetMapModeName()
   {
      return MapModeType.Area;
   }

   public override int GetProvinceColor(Province id)
   {
      if (id.Area != Area.Empty )
         return id.Area.Color.ToArgb();
      return Color.DarkGray.ToArgb();
   }

   public override string GetSpecificToolTip(Province provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return $"Area: {province.Area.Name} ({Localisation.GetLoc(province.Area.Name)})";
      return "Area: [Unknown]";
   }
}