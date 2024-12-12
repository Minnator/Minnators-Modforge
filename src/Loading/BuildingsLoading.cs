using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   
   public static class BuildingsLoading
   {
      public static void Load()
      {
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "common", "buildings");
         List<Building> buildings = [];

         foreach (var file in files)
            if (File.Exists(file))
               ParseBuildingsFile(file, ref buildings);
         
         foreach (var building in buildings) 
            Globals.BuildingKeys.Add(building.Name);
         
         Globals.Buildings = buildings;
      }

      private static void ParseBuildingsFile(string file, ref List<Building> buildings)
      {
         var content = IO.ReadAllInUTF8(file);
         var elements = Parsing.GetElements(0, ref content);

         foreach (var element in elements)
            if (element is Block block) 
               buildings.Add(new (block.Name));
      }
   }
}