using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Loading.Enhanced;
using Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;
using Editor.Parser;
using Editor.Saving;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   
   public static class BuildingsLoading
   {
      public static void Load()
      {
         var files = PathManager.GetFilesFromModAndVanillaUniquely("*.txt", "common", "buildings");
         List<Building> buildings = [];

         foreach (var file in files)
            ParseBuildingsFile(file, ref buildings);
         
         RuntimeEffects.GenerateBuildingEffects();
      }

      private static void ParseBuildingsFile(string file, ref List<Building> buildings)
      {
         var po = PathObj.FromPath(file);

         var (blocks, _) = po.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly);

         foreach (var block in blocks)
            Globals.Buildings.Add(new (block.Name));
      }
   }
}