using System.Xml.Linq;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced;
using Editor.Parser;
using Editor.Saving;

namespace Editor.Loading
{
   public static class ProvinceGroupsLoading
   {
      public static void Load()
      {
         PathManager.GetFilePathUniquely(out var path, "map", "province_groups.txt");
         
         LoadProvinceGroupsFromFile(path);
      }

      private static void LoadProvinceGroupsFromFile(string file)
      {
         var pObject = PathObj.FromPath(file);
         Dictionary<string, ProvinceGroup> provinceGroups = [];

         var ( blocks, contents) = pObject.GetElements();
         if (blocks.Count == 0)
            return; // No province groups, they are optional, so no error
         
         if (contents.Count > 0)
            _ = new LoadingError(pObject, "No content allowed in 'province_groups.txt'");

         foreach (var block in blocks)
         {
            var provinces = EnhancedParsing.GetProvincesFromContent(block.ContentElements, pObject);
            provinceGroups.Add(block.Name, new (block.Name, Globals.ColorProvider.GetRandomColor(), ref pObject, provinces));
         }
         
         SaveMaster.AddRangeToDictionary(pObject, provinceGroups.Values);
         Globals.ProvinceGroups = provinceGroups;
      }
   }
}