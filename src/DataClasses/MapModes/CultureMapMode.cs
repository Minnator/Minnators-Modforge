using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Helper;

namespace Editor.DataClasses.MapModes;

public class CultureMapMode : MapMode
{
   public CultureMapMode()
   {
      ProvinceEventHandler.OnProvinceCultureChanged += UpdateProvince;
   }

   public override bool IsLandOnly => true;

   public override int GetProvinceColor(Province id)
   {
      if (Globals.Cultures.TryGetValue(id.Culture, out var culture))
         return culture.Color.ToArgb();
      return Color.DimGray.ToArgb();
   }

   public override MapModeType MapModeType => MapModeType.Culture;

   public override string GetSpecificToolTip(Province id)
   {
      if (Globals.Cultures.TryGetValue(id.Culture, out var culture))
         return $"Culture: {culture.Name} ({Localisation.GetLoc(culture.Name)})";
      return "Culture: [Unknown]";
   }

   public override bool ShouldProvincesMerge(Province p1, Province p2)
   {
      return p1.Culture == p2.Culture;
   }
}