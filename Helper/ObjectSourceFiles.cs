using Editor.Interfaces;

namespace Editor.Helper
{
   public static class ObjectSourceFiles
   {
      // ------------ Loading ------------ \\
      // Contains each file's internal path which was in to backtrack what files were changed and need overwriting
      // Vanilla files have a positive index and mod files have a negative index to differentiate them
      private static Dictionary<string, int> VanillaFiles = [];
      private static Dictionary<int, string> IndexToVanillaFile = [];
      private static int VanillaIndex = 0;
      private static Dictionary<string, int> ModFiles = [];
      private static Dictionary<int, string> IndexToModFile = [];
      private static int ModIndex = 0;

      public static bool AddVanillaFile(string fileName, out int index)
      {
         if (VanillaFiles.TryGetValue(fileName, out index))
            return false;
         if (fileName.StartsWith('\\'))
            fileName = fileName[1..];
         index = ++VanillaIndex;
         VanillaFiles.Add(fileName, index);
         IndexToVanillaFile.Add(index, fileName);
         return true;
      }

      public static bool AddModFile(string fileName, out int index)
      {
         if (ModFiles.TryGetValue(fileName, out index))
            return false;
         while (fileName.StartsWith('\\'))
            fileName = fileName[1..];
         index = --ModIndex;
         ModFiles.Add(fileName, index);
         IndexToModFile.Add(index, fileName);
         return true;
      }

      public static bool GetFileFromIndex(int index, out string fileName)
      {
         if (index > 0)
            return IndexToVanillaFile.TryGetValue(index, out fileName!);
         return IndexToModFile.TryGetValue(index, out fileName!);
      }

      public static List<T> GetAllElementsOfFile<T> (int fileIndex, ICollection<ISaveable> allElements)
      {
         if (fileIndex > 0)
            return allElements.Where(x => x.FileIndex == fileIndex).Cast<T>().ToList();
         return allElements.Where(x => x.FileIndex == fileIndex).Cast<T>().ToList();
      }

   }
}