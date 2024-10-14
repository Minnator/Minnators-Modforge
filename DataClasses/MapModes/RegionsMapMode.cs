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

   public override string GetMapModeName()
   {
      return MapModeType.Regions.ToString();
   }

   public override int GetProvinceColor(Province id)
   {
      if (Globals.Provinces.TryGetValue(id, out var province))
         if (Globals.Areas.TryGetValue(province.Area, out var areas))
            if (Globals.Regions.TryGetValue(areas.Region, out var region))
               return region.Color.ToArgb();
      return Color.DarkGray.ToArgb();
   }

   public override string GetSpecificToolTip(Province province)
   {
      if (Globals.Areas.TryGetValue(province.Area, out var areas))
         if (Globals.Regions.TryGetValue(areas.Region, out var region))
            return $"Region: {region.Name} ({Localisation.GetLoc(region.Name)})";
      return "Region: [Unknown]";
   }
}