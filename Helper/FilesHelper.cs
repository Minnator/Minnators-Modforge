using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Editor.Parser;

namespace Editor.Helper;

public static partial class FilesHelper
{
   private const string ID_FROM_FILE_NAME_PATTERN = @"^\d+";

   private static readonly Regex IdRegex = IdFromString();

   [GeneratedRegex(ID_FROM_FILE_NAME_PATTERN, RegexOptions.Compiled)]
   private static partial Regex IdFromString();
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
   /// <param name="searchPattern"></param>
   /// <param name="internalPath"></param>
   /// <returns></returns>
   public static List<string> GetFilesFromModAndVanillaUniquely(string searchPattern = "*.txt", params string[] internalPath)
   {
      var (modFiles, vanillaFiles) = GetFilesFromModAndVanillaUniquelySeparated(searchPattern, internalPath);
      return modFiles.Concat(vanillaFiles).ToList();
   }

   public static (List<string>, List<string>) GetFilesFromModAndVanillaUniquelySeparated(string searchPattern, params string[] internalPath)
   {
      var folderPath = Path.Combine(internalPath);
      var modPath = Path.Combine(Globals.ModPath, folderPath);
      var vanillaPath = Path.Combine(Globals.VanillaPath, folderPath);
      
      List<string> modFiles = [];
      List<string> vanillaFiles = [];
      HashSet<string> uniqueFiles = [];

      if (Directory.Exists(modPath))
      {
         foreach (var file in Directory.GetFiles(modPath, searchPattern, SearchOption.TopDirectoryOnly))
         {
            if (uniqueFiles.Add(Path.GetFileName(file)))
               modFiles.Add(file);
         }
      }

      if (Directory.Exists(vanillaPath))
      {
         foreach (var file in Directory.GetFiles(vanillaPath, searchPattern, SearchOption.TopDirectoryOnly))
         {
            if (uniqueFiles.Add(Path.GetFileName(file)))
               vanillaFiles.Add(file);
         }
      }

      return (modFiles, vanillaFiles);
   }

   public static List<string> GetProvinceFilesUniquely()
   {
      var modPath = Path.Combine(Globals.ModPath, "history", "provinces");
      var vanillaPath = Path.Combine(Globals.VanillaPath, "history", "provinces");
      List<string> fileSet = [];
      HashSet<int> provinceIds = [];

      if (Directory.Exists(modPath))
      {
         foreach (var file in Directory.GetFiles(modPath, "*.txt", SearchOption.TopDirectoryOnly))
         {
            var match = ProvinceParser.IdRegex.Match(Path.GetFileName(file));
            if (match.Success)
            {
               if (provinceIds.Add(int.Parse(match.Groups[1].Value)))
                  fileSet.Add(file);
            }
         }
      }

      if (Directory.Exists(vanillaPath))
      {
         foreach (var file in Directory.GetFiles(vanillaPath, "*.txt", SearchOption.TopDirectoryOnly))
         {
            var match = ProvinceParser.IdRegex.Match(Path.GetFileName(file));
            if (match.Success)
            {
               if (provinceIds.Add(int.Parse(match.Groups[1].Value)))
                  fileSet.Add(file);
            }
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
      var files = GetFilesFromModAndVanillaUniquely(internalPath: internalPath);
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

   public static string CreateModPath(params string[] internalPath)
   {
      return Path.Combine(Globals.ModPath, Path.Combine(internalPath));
   }

}