using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.Events;
using Editor.Helper;

namespace Editor.MapModes;

public sealed class RegionsMapMode : MapMode
{
   public RegionsMapMode()
   {
      ProvinceEventHandler.OnProvinceRegionAreasChanged += UpdateProvince!;
   }

   public override MapModeType GetMapModeName()
   {
      return MapModeType.Regions;
   }

   public override int GetProvinceColor(Province id)
   {
      if (Globals.Provinces.TryGetValue(id, out var province))
         if (!Equals(province.Area, Area.Empty))
            return province.Area.Region.Color.ToArgb();
      return Color.DarkGray.ToArgb();
   }

   public override string GetSpecificToolTip(Province province)
   {
      if (!Equals(province.Area, Area.Empty))
            return $"Region: {province.Area.Region.Name} ({Localisation.GetLoc(province.Area.Region.Name)})";
      return "Region: [Unknown]";
   }
}