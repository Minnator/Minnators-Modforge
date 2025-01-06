using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class ReligionMapMode : MapMode
   {
      public ReligionMapMode()
      {
         // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceReligionChanged += UpdateProvince;
      }

      public override bool IsLandOnly => true;

      public override int GetProvinceColor(Province id)
      {
         return id.Religion.Color.ToArgb();
      }

      public override MapModeType MapModeType => MapModeType.Religion;

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (provinceId.Religion == Religion.Empty)
            return "Religion: [Unknown]";
         
         return $"Religion: {provinceId.Religion.Name} ({Localisation.GetLoc(provinceId.Religion.Name)})";
      }

   }
}