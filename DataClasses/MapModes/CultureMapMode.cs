using Editor.DataClasses.MapModes;
using Editor.Helper;

namespace Editor.MapModes;

public class CultureMapMode : MapMode
{
   public CultureMapMode()
   {
      ProvinceEventHandler.OnProvinceCultureChanged += UpdateProvince;
   }

   public override bool IsLandOnly => true;

   public override System.Drawing.Color GetProvinceColor(int id)
   {
      if (Globals.Cultures.TryGetValue(Globals.Provinces[id].Culture, out var culture))
         return culture.Color;
      return Color.DimGray;
   }

   public override string GetMapModeName()
   {
      return "Culture";
   }
}