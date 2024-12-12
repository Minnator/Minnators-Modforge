using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class TerrainMapMode : MapMode
   {
      public TerrainMapMode()
      {
         Terrain.OnTerrainChanged += UpdateComposite<Province>;
         Terrain.ItemsModified += UpdateProvinceCollection;
      }

      public override MapModeType GetMapModeName()
      {
         return MapModeType.Terrain;
      }

      public override int GetProvinceColor(Province id)
      {
         if (id.Terrain == Terrain.Empty)
            if(Globals.Settings.Rendering.ShowOceansAsGreyInTerrain && id.AutoTerrain.Name.ToLower().Equals("ocean"))
               return Color.DimGray.ToArgb();
            else
               return id.AutoTerrain.Color.ToArgb();
         return id.Terrain.Color.ToArgb();
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (provinceId.Terrain == Terrain.Empty)
            return $"Terrain (Auto): {Localisation.GetLoc(provinceId.AutoTerrain.Name)} ({provinceId.AutoTerrain.Name})";
         return $"Terrain: {Localisation.GetLoc(provinceId.Terrain.Name)} ({provinceId.Terrain.Name})";
      }
   }
}