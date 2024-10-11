using Editor.Events;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class ReligionMapMode : MapMode
   {
      public ReligionMapMode()
      {
         ProvinceEventHandler.OnProvinceReligionChanged += UpdateProvince;
      }

      public override bool IsLandOnly => true;

      public override int GetProvinceColor(int id)
      {
         if (Globals.Religions.TryGetValue(Globals.Provinces[id].Religion, out var religion))
            return religion.Color.ToArgb();
         return Color.DimGray.ToArgb();
      }

      public override string GetMapModeName()
      {
         return "Religion";
      }

      public override string GetSpecificToolTip(int provinceId)
      {
         if (Globals.Religions.TryGetValue(Globals.Provinces[provinceId].Religion, out var religion))
            return $"Religion: {religion.Name} ({Localisation.GetLoc(religion.Name)})";
         return "Religion: [Unknown]";
      }
   }
}