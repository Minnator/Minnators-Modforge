using System.Drawing;
using Editor.DataClasses.MapModes;
using Editor.Helper;

namespace Editor.MapModes;

public class CultureGroupMapMode : MapMode
{
   public CultureGroupMapMode()
   {
      // Subscribe to events to update the min and max values when a province's development changes
      ProvinceEventHandler.OnProvinceCultureChanged += UpdateProvince;
   }

   public override bool IsLandOnly => true;

   public override Color GetProvinceColor(int id)
   {
      if (Globals.Cultures.TryGetValue(Globals.Provinces[id].Culture, out var culture))
         if (Globals.CultureGroups.TryGetValue(culture.CultureGroup, out var group))
            return group.Color;
      return Color.DimGray;
   }

   public override string GetMapModeName()
   {
      return "Culture Group";
   }

   public override string GetSpecificToolTip(int provinceId)
   {
      if (Globals.Cultures.TryGetValue(Globals.Provinces[provinceId].Culture, out var culture))
         if (Globals.CultureGroups.TryGetValue(culture.CultureGroup, out var group))
            return $"Culture Group: {group.Name} ({Localisation.GetLoc(group.Name)})";
      return "Culture Group: [Unknown]";
   }
}