using System.Linq;
using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Helper;

namespace Editor.DataClasses.MapModes;

public class CultureGroupMapMode : MapMode
{
   public CultureGroupMapMode()
   {
      // Subscribe to events to update the min and max values when a province's development changes
      // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceCultureChanged += UpdateProvince;
   }

   public override bool IsLandOnly => true;

   public override int GetProvinceColor(Province id)
   {
      return id.Culture.CultureGroup.Color.ToArgb();
   }

   public override MapModeType MapModeType => MapModeType.CultureGroup;

   public override string GetSpecificToolTip(Province provinceId)
   {
      return $"Culture Group: {provinceId.Culture.CultureGroup.Name} ({Localisation.GetLoc(provinceId.Culture.CultureGroup.Name)})";
   }

}