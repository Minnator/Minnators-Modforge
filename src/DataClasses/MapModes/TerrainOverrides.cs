using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class TerrainOverrides : MapMode
   {
      public override bool IsLandOnly => true;

      public TerrainOverrides()
      {
         Terrain.ItemsModified += UpdateProvinceCollection;
      }

      public override MapModeType MapModeType => MapModeType.TerrainOverrides;

      public override int GetProvinceColor(Province id)
      {
         return id.Terrain.Color.ToArgb();
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         var terrain = provinceId.Terrain;
         if (terrain == Terrain.Empty)
            return "No Terrain Override";
         return $"{terrain.Name} ({Localisation.GetLoc(terrain.Name)})";
      }

   }
}