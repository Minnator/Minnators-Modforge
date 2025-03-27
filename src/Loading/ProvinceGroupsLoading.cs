using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Helper;
using Editor.Parser;
using Editor.Saving;

namespace Editor.Loading
{
   public static class ProvinceGroupsLoading
   {
      public static void Load()
      {
         var files = PathManager.GetFilesFromModAndVanillaUniquely("*.txt", "map", "province_groups");

         foreach (var file in files) 
            LoadProvinceGroupsFromFile(file);
      }

      private static void LoadProvinceGroupsFromFile(string file)
      {
         var pObject = PathObj.FromPath(file);
         Dictionary<string, ProvinceGroup> provinceGroups = [];

         var elements = Parsing.GetElements(0, file);
         if (elements.Count == 0)
            return; // No province groups, they are optional, so no error

         foreach (var element in elements)
         {
            if (element is not Block block)
            {
               Globals.ErrorLog.Write($"Error in {file}: Invalid content: {element}; {file} must not contain any content elements!");
               continue;
            }

            var groupName = block.Name;
            provinceGroups.Add(groupName, new (groupName, Globals.ColorProvider.GetRandomColor(), ref pObject, Parsing.GetProvincesFromString(block.GetContent)));
         }
         
         SaveMaster.AddRangeToDictionary(pObject, provinceGroups.Values);
         Globals.ProvinceGroups = provinceGroups;
      }
   }
}