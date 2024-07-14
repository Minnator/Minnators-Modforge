using Editor.DataClasses.MapModes;
using Editor.Helper;

namespace Editor.MapModes;

public sealed class AreaMapMode : MapMode
{
   public AreaMapMode()
   {

   }

   public override string GetMapModeName()
   {
      return "Areas";
   }

   public override Color GetProvinceColor(int id)
   {
      if (Globals.Provinces.TryGetValue(id, out var province))
         if (Globals.Areas.TryGetValue(province.Area, out var area))
            return area.Color;
      return Color.DarkGray;
   }

   public override string GetSpecificToolTip(int provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         if (Globals.Areas.TryGetValue(province.Area, out var area))
            return $"Area: {area.Name} ({Localisation.GetLoc(area.Name)})";
      return "Area: [Unknown]";
   }
}