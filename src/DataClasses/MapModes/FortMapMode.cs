using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Events;
using Editor.Helper;

namespace Editor.DataClasses.MapModes;

public class FortMapMode : MapMode
{
   //TODO read min an max from defines
   public override bool IsLandOnly => true;

   public FortMapMode()
   {
      // Subscribe to events to update the min and max values when a province's development changes
      // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceBuildingsChanged += UpdateProvince!;
   }

   public override int GetProvinceColor(Province id)
   {
      var fortLevel = GetFortLevel(id);
      return fortLevel < ColorProviderRgb.PredefinedColors.Length 
         ? ColorProviderRgb.PredefinedColors[fortLevel].ToArgb() 
         : Color.Azure.ToArgb();
   }

   private int GetFortLevel(Province id)
   {
      var level = 0;
      if (id.Buildings.Any(x => x.Name.Equals("fort_15th")))
         level += 2;
      if (id.Buildings.Any(x => x.Name.Equals("fort_16th")))
         level += 4;
      if (id.Buildings.Any(x => x.Name.Equals("fort_17th")))
         level += 6;
      if (id.Buildings.Any(x => x.Name.Equals("fort_18th")))
         level += 8;

      if (Globals.Countries.TryGetValue(id.Owner, out var owner) && owner.HistoryCountry.Capital == id)
         level += 1;
      return level;
   }

   public override MapModeType MapModeType => MapModeType.Fort;

   public override string GetSpecificToolTip(Province provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         return $"Fort Level: {GetFortLevel(provinceId)}";
      return $"No fort in {provinceId.TitleLocalisation}";
   }

}