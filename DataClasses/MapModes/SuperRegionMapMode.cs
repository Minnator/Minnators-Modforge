using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.Events;
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

   public override MapModeType GetMapModeName()
   {
      return MapModeType.SuperRegion;
   }

   public override int GetProvinceColor(Province id)
   {
      var sr = id.GetFirstParentOfType(SaveableType.SuperRegion);
      if (sr != ProvinceComposite.Empty)
         return sr.Color.ToArgb();
      return Color.DimGray.ToArgb();
   }

   public override string GetSpecificToolTip(Province province)
   {
      if (province.GetArea() != Area.Empty)
         if (province.GetArea().Region != Region.Empty)
            return $"Super Region: {province.GetArea().Region.SuperRegion.Name} ({Localisation.GetLoc(province.GetArea().Region.SuperRegion.Name)})";
      return "Super Region: [Unknown]";
   }
}