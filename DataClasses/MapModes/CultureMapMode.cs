using System.Diagnostics;
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

   public override Color GetProvinceColor(int id)
   {
      if (Globals.Cultures.TryGetValue(Globals.Provinces[id].Culture, out var culture))
         return culture.Color;
      return Color.DimGray;
   }

   public override string GetMapModeName()
   {
      return "Culture";
   }

   public override string GetSpecificToolTip(int provinceId)
   {
      if (Globals.Cultures.TryGetValue(Globals.Provinces[provinceId].Culture, out var culture))
         return $"Culture: {culture.Name} ({Localisation.GetLoc(culture.Name)})";
      return "Culture: [Unknown]";
   }
}