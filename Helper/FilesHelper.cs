using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Editor.Helper;

public static class FilesHelper
{
   // Gets all files in a folder with a specific file ending in all subfolders of the folder
   public static List<string> GetAllFilesInFolder(string folderPath, string searchPattern)
   {
      List<string> files = [];

      if (!Directory.Exists(folderPath))
         return files;

      foreach (var file in Directory.GetFiles(folderPath, searchPattern, SearchOption.AllDirectories)) 
         files.Add(file);

      return files;
   }

   // Gets all files in a folder with a specific file ending in the folder
   public static List<string> GetFilesInFolder(string folderPath, string searchPattern)
   {
      List<string> files = [];

      if (!Directory.Exists(folderPath))
         return files;

      foreach (var file in Directory.GetFiles(folderPath, searchPattern)) 
         files.Add(file);

      return files;
   }

   // Gets all files in a folder with a specific file ending in the folder but only in TopDirectoryOnly and not in subfolders
   public static List<string> GetFilesFromModAndVanillaUniquely(string modPathIn, string vanillaPathIn, params string[] path)
   {
      var internalPath = Path.Combine(path);
      var modPath = Path.Combine(modPathIn, internalPath);
      var vanillaPath = Path.Combine(vanillaPathIn, internalPath);
      HashSet<string> fileSet = [];

      if (Directory.Exists(modPath)) 
         fileSet.UnionWith(Directory.GetFiles(modPath, "*.txt", SearchOption.TopDirectoryOnly));

      if (Directory.Exists(vanillaPath))
         foreach (var file in Directory.GetFiles(vanillaPath, "*.txt", SearchOption.TopDirectoryOnly)) 
            fileSet.Add(file);

      return [..fileSet];
   }
}