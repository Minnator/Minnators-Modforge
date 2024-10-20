using Editor.Helper;

namespace Editor.Loading
{
   public static class LoadUnitTypes
   {
      public static void Load()
      {
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "common", "units");

         
      }

      private static void LoadUnitType(string path)
      {
         Parsing.RemoveCommentFromMultilineString(IO.ReadAllInUTF8(path), out var content);
         var kvps = Parsing.GetKeyValueList(ref content);
      }
   }
}