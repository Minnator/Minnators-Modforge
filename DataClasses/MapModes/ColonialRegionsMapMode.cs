using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.MapModes
{
   public class ColonialRegionsMapMode : MapMode
   {
      public override bool IsLandOnly => true;
      public override bool ShowOccupation => false;

      public override int GetProvinceColor(Province id)
      {
         foreach (var cr in Globals.ColonialRegions.Values)
            if (cr.GetProvinces().Contains(id))
               return cr.Color.ToArgb();
         return Color.DimGray.ToArgb();
      }

      public override MapModeType GetMapModeName()
      {
         return MapModeType.ColonialRegions;
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         foreach (var cr in Globals.ColonialRegions.Values)
            if (cr.GetProvinces().Contains(provinceId))
               return $"Colonial region: {cr.Name} ({cr.GetTitleLocKey})";
         return "Colonial region: [Unknown]";
      }
   }
}