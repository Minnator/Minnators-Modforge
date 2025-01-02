using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class ColonialRegionsMapMode : MapMode
   {
      public ColonialRegionsMapMode()
      {
         ColonialRegion.ItemsModified += UpdateProvinceCollection;
         ColonialRegion.ColorChanged += UpdateComposite<Province>;
      }

      public override bool IsLandOnly => true;
      public override bool ShowOccupation => false;

      public override int GetProvinceColor(Province id)
      {
         foreach (var cr in Globals.ColonialRegions.Values)
            if (cr.GetProvinces().Contains(id))
               return cr.Color.ToArgb();
         return Color.DimGray.ToArgb();
      }
      
      public override MapModeType MapModeType => MapModeType.ColonialRegions;

      public override string GetSpecificToolTip(Province provinceId)
      {
         foreach (var cr in Globals.ColonialRegions.Values)
            if (cr.GetProvinces().Contains(provinceId))
               return $"Colonial region: {cr.Name} ({Localisation.GetLoc(cr.Name)})";
         return "Colonial region: [Unknown]";
      }

   }
}