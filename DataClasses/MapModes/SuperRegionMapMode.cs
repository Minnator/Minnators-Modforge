using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Helper;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor.DataClasses.MapModes;

public sealed class SuperRegionMapMode : MapMode
{
   public SuperRegionMapMode()
   {
      ProvinceEventHandler.OnSuperRegionRegionChanged += UpdateProvince!;
   }

   public override MapModeType GetMapModeName()
   {
      return MapModeType.SuperRegion;
   }

   public override int GetProvinceColor(Province id)
   {
      if (Globals.Provinces.TryGetValue(id, out var province))
         if (province.Area != Area.Empty)
            if (province.Area.Region != Region.Empty)
               return province.Area.Region.SuperRegion.Color.ToArgb();
      return Color.DarkGray.ToArgb();
   }

   public override string GetSpecificToolTip(Province province)
   {
         if (province.Area != Area.Empty)
            if (province.Area.Region != Region.Empty)
               return $"Super Region: {province.Area.Region.SuperRegion.Name} ({Localisation.GetLoc(province.Area.Region.SuperRegion.Name)})";
      return "Super Region: [Unknown]";
   }
}