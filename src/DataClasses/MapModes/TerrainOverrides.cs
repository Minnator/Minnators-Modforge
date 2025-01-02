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
         foreach (var terrain in Globals.Terrains)
         {
            if (terrain.SubCollection.Contains(id))
               return terrain.Color.ToArgb();
         }

         return Color.DimGray.ToArgb();
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         foreach (var terrain in Globals.Terrains)
         {
            if (terrain.SubCollection.Contains(provinceId))
               return $"{Localisation.GetLoc(terrain.Name)} ({terrain.Name})";
         }

         return "No Terrain Override";
      }

   }
}