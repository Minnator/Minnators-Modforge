using Editor.DataClasses.GameDataClasses;
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

      public override int GetProvinceColor(Province id)
      {
         if (Globals.Religions.TryGetValue(id.Religion, out var religion))
            return religion.Color.ToArgb();
         return Color.DimGray.ToArgb();
      }

      public override MapModeType MapModeType => MapModeType.Religion;

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Religions.TryGetValue(provinceId.Religion, out var religion))
            return $"Religion: {religion.Name} ({Localisation.GetLoc(religion.Name)})";
         return "Religion: [Unknown]";
      }

      public override bool ShouldProvincesMerge(Province p1, Province p2)
      {
         return p1.Religion == p2.Religion;
      }
   }
}