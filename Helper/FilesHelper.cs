using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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

   /// <summary>
   /// Gets all file paths in a folder with a <c>.txt</c> file ending in the folder but only in TopDirectoryOnly
   /// </summary>
   /// <param name="internalPath"></param>
   /// <returns></returns>
   public static List<string> GetFilesFromModAndVanillaUniquely(params string[] internalPath)
   {
      var folderPath = Path.Combine(internalPath);
      var modPath = Path.Combine(Globals.ModPath, folderPath);
      var vanillaPath = Path.Combine(Globals.VanillaPath, folderPath);
      List<string> fileSet = [];
      HashSet<string> fineNames = [];

      if (Directory.Exists(modPath))
      {
         foreach (var file in Directory.GetFiles(modPath, "*.txt", SearchOption.TopDirectoryOnly))
         {
            if (fineNames.Add(Path.GetFileName(file)))
               fileSet.Add(file);
         }
      }

      if (Directory.Exists(vanillaPath))
      {
         foreach (var file in Directory.GetFiles(vanillaPath, "*.txt", SearchOption.TopDirectoryOnly))
         {
            if (fineNames.Add(Path.GetFileName(file)))
               fileSet.Add(file);
         }
      }

      return [..fileSet];
   }
   
   public static bool GetFileUniquely(out string content, params string[] internalPath)
   {
      var folderPath = Path.Combine(internalPath);
      var modPath = Path.Combine(Globals.ModPath, folderPath);

      if (File.Exists(modPath))
      {
         content = IO.ReadAllInUTF8(modPath);
         return true;
      }

      var vanillaPath = Path.Combine(Globals.VanillaPath, folderPath);
      if (File.Exists(vanillaPath))
      {
         content = IO.ReadAllInUTF8(vanillaPath);
         return true;
      }

      content = string.Empty;
      return false;
   }

   public static bool GetFilePathUniquely(out string path, params string[] internalPath)
   {
      var intPath = Path.Combine(internalPath);
      var modPath = Path.Combine(Globals.ModPath, intPath);

      if (File.Exists(modPath))
      {
         path = modPath;
         return true;
      }

      var vanillaPath = Path.Combine(Globals.VanillaPath, intPath);
      if (File.Exists(vanillaPath))
      {
         path = vanillaPath;
         return true;
      }

      path = string.Empty;
      return false;
   }

   public static bool GetFilesUniquelyAndCombineToOne(out string output, params string[] internalPath)
   {
      var sb = new StringBuilder();
      var files = GetFilesFromModAndVanillaUniquely(internalPath);
      foreach (var file in files)
      {
         sb.Append(File.ReadAllText(file)).Append('\n');
      }
      output = sb.ToString();
      return files.Count > 0;
   }

   public static bool GetModOrVanillaPath(out string filePath, params string[] internalPath)
   {
      var innerPath = Path.Combine(internalPath);
      var path = Path.Combine(Globals.ModPath, innerPath);
      if (File.Exists(path))
      {
         filePath = path;
         return true;
      }
      filePath = Path.Combine(Globals.VanillaPath, innerPath);
      return File.Exists(filePath);
   }
}