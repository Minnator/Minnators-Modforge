using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class ClimateMapMode : MapMode
   {
      public override MapModeType MapModeType { get; } = MapModeType.Climate;
      public override bool IsLandOnly { get; } = true;

      public override int GetProvinceColor(Province id)
      {
         foreach (var climate in Globals.Climates)
         {
            if (climate.Value.SubCollection.Contains(id))
               return climate.Value.Color.ToArgb();
         }
         return Color.FromArgb(92, 90, 64).ToArgb();
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         foreach (var climate in Globals.Climates)
         {
            if (climate.Value.SubCollection.Contains(provinceId))
               return $"Climate: {climate.Value.Name} ({Localisation.GetLoc(climate.Value.Name)})";
         }
         return "Climate: Temperate";
      }

      public override bool ShouldProvincesMerge(Province p1, Province p2)
      {
         foreach (var climate in Globals.Climates)
            if (climate.Value.SubCollection.Contains(p1) && climate.Value.SubCollection.Contains(p2))
               return true;
         return false;
      }
   }
}