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
            if (cr.Provinces.Contains(id))
               return cr.Color.ToArgb();
         return Color.DimGray.ToArgb();
      }

      public override string GetMapModeName()
      {
         return MapModeType.ColonialRegions.ToString();
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         foreach (var cr in Globals.ColonialRegions.Values)
            if (cr.Provinces.Contains(provinceId))
               return $"Colonial region: {cr.Name} ({cr.GetLocalisation()})";
         return "Colonial region: [Unknown]";
      }
   }
}