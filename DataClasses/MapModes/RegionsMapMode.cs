using Editor.DataClasses.MapModes;
using Editor.Helper;

namespace Editor.MapModes;

public sealed class RegionsMapMode : MapMode
{
   public RegionsMapMode()
   {
      
   }

   public override string GetMapModeName()
   {
      return "Regions";
   }

   public override Color GetProvinceColor(int id)
   {
      if (Globals.Provinces.TryGetValue(id, out var province))
         if (Globals.Areas.TryGetValue(province.Area, out var areas))
            if (Globals.Regions.TryGetValue(areas.Region, out var region))
               return region.Color;
      return Color.DarkGray;
   }

   public override string GetSpecificToolTip(int provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         if (Globals.Areas.TryGetValue(province.Area, out var areas))
            if (Globals.Regions.TryGetValue(areas.Region, out var region))
               return $"Region: {region.Name} ({Localisation.GetLoc(region.Name)})";
      return "Region: [Unknown]";
   }
}