using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor.DataClasses.MapModes;

public sealed class SuperRegionMapMode : MapMode
{
   public SuperRegionMapMode()
   {
      SuperRegion.ItemsModified += UpdateProvinceCollection;
      SuperRegion.ColorChanged += UpdateComposite<Province>;
   }

   public override MapModeType MapModeType => MapModeType.SuperRegion;

   public override int GetProvinceColor(Province id)
   {
      var sr = id.GetFirstParentOfType(SaveableType.SuperRegion);
      if (sr != ProvinceComposite.Empty)
         return sr.Color.ToArgb();
      return Color.DimGray.ToArgb();
   }

   public override string GetSpecificToolTip(Province province)
   {
      if (province.Area != Area.Empty)
         if (province.Area.Region != Region.Empty)
            return $"Super Region: {province.Area.Region.SuperRegion.Name} ({Localisation.GetLoc(province.Area.Region.SuperRegion.Name)})";
      return "Super Region: [Unknown]";
   }

}