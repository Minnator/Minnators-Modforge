using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Helper;

namespace Editor.DataClasses.MapModes;

public class CultureMapMode : MapMode
{
   public CultureMapMode()
   {
      // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceCultureChanged += UpdateProvince;
   }

   public override bool IsLandOnly => true;

   public override int GetProvinceColor(Province id)
   {
      return id.Culture.Color.ToArgb();
   }

   public override MapModeType MapModeType => MapModeType.Culture;

   public override string GetSpecificToolTip(Province id)
   {
      return $"Culture: {id.Culture.Name} ({Localisation.GetLoc(id.Culture.Name)})";
   }

}