using System.Collections.Generic;
using System.IO;

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
}