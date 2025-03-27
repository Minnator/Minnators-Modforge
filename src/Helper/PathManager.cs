using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Parser;
using Editor.Saving;

namespace Editor.Helper;

public static partial class PathManager
{
   private const string ID_FROM_FILE_NAME_PATTERN = @"^\d+";

   private static readonly Regex IdRegex = IdFromString();

   [GeneratedRegex(ID_FROM_FILE_NAME_PATTERN, RegexOptions.Compiled)]
   private static partial Regex IdFromString();
   
   public static List<string> GetAllFilesInFolder(string searchPattern = ".*txt", params string[] internalPath)
   {
      var folderPath = Path.Combine(internalPath);
      var modPath = Path.Combine(Globals.ModPath, folderPath);
      var vanillaPath = Path.Combine(Globals.VanillaPath, folderPath);
      
      HashSet<string> uniqueFiles = [];
      
      var modFiles = GetFilesInPath(modPath, searchPattern, ref uniqueFiles);
      
      if (IsPathReplaced(internalPath))
         return modFiles;
      
      return modFiles.Concat(GetFilesInPath(vanillaPath, searchPattern, ref uniqueFiles)).ToList();
   }
   
   // Gets all files in a folder with a specific file ending in all subfolders of the folder
   public static List<string> GetAllFilesInFolder(string folderPath, string searchPattern)
   {
      if (!Directory.Exists(folderPath))
         return [];

      return Directory.GetFiles(folderPath, searchPattern, SearchOption.AllDirectories).ToList();
   }

   // Gets all files in a folder with a specific file ending in the folder
   public static List<string> GetFilesInFolder(string folderPath, string searchPattern)
   {
      if (!Directory.Exists(folderPath))
         return [];

      return Directory.GetFiles(folderPath, searchPattern).ToList();
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

   public static Bitmap GetDefaultFlagPath()
   {
      return ImageReader.ReadImage(Path.Combine(Globals.VanillaPath, "gfx", "flags", "REB.tga"));
   }

   /// <summary>
   /// First list is the mod files the 2nd the vanilla files
   /// </summary>
   /// <param name="searchPattern"></param>
   /// <param name="internalPath"></param>
   /// <returns></returns>
   public static (List<string>, List<string>) GetFilesFromModAndVanillaUniquelySeparated(string searchPattern, params string[] internalPath)
   {
      var folderPath = Path.Combine(internalPath);
      var modPath = Path.Combine(Globals.ModPath, folderPath);
      var vanillaPath = Path.Combine(Globals.VanillaPath, folderPath);
      
      HashSet<string> uniqueFiles = [];
      
      var modFiles = GetFilesInPath(modPath, searchPattern, ref uniqueFiles);
      
      if (IsPathReplaced(internalPath))
         return (modFiles, []);
      
      return (modFiles, GetFilesInPath(vanillaPath, searchPattern, ref uniqueFiles));
   }

   private static bool IsPathReplaced(string[] internalPath)
   {
      var result = Globals.DescriptorData.ReplacePaths.Any(path => CheckIfPathInPath(path, internalPath));
#if DEBUG
      if (result)
         Debug.WriteLine($"Path {string.Join(Path.DirectorySeparatorChar, internalPath)} is replaced");
#endif
      return result;
      }

   public static bool CheckIfPathInPath(string[] path, string[] checkPath)
   {
      for (var i = 0; i < Math.Min(path.Length, checkPath.Length); i++)
         if (!path[i].Equals(checkPath[i]))
            return false;
      return true;
   }

   private static List<string> GetFilesInPath(string path, string searchPattern, ref HashSet<string> uniqueFiles, SearchOption searchOption = SearchOption.TopDirectoryOnly)
   {
      List<string> modFiles = [];
      if (Directory.Exists(path))
         foreach (var file in Directory.GetFiles(path, searchPattern, searchOption))
            if (uniqueFiles.Add(Path.GetFileName(file)))
               modFiles.Add(file);

      return modFiles;
   }


   public static bool GetFilePathUniquely(out string path, params string[] internalPath)
   {
      return GetModOrVanillaPath(out path, out _, internalPath);
   }

   public static bool GetFilesUniquelyAndCombineToOne(out string output, params string[] internalPath)
   {
      var sb = new StringBuilder();
      var files = GetFilesFromModAndVanillaUniquely(internalPath: internalPath);
      foreach (var file in files) 
         sb.Append(File.ReadAllText(file)).Append('\n');
      output = sb.ToString();
      return files.Count > 0;
   }

   public static bool GetModOrVanillaPath(out string filePath, out bool isModPath, params string[] internalPath)
   {
      var innerPath = Path.Combine(internalPath);
      var path = Path.Combine(Globals.ModPath, innerPath);
      if (File.Exists(path))
      {
         isModPath = true;
         filePath = path;
         return true;
      }

      if (IsPathReplaced(internalPath))
      {
         isModPath = false;
         filePath = string.Empty;
         return false;
      }

      filePath = Path.Combine(Globals.VanillaPath, innerPath);
      isModPath = false;
      return File.Exists(filePath);
   }
   
   public static bool GetVanillaPath(out string filePath, params string[] path)
   {
      filePath = Path.Combine(Globals.VanillaPath, Path.Combine(path));
      return File.Exists(filePath);
   }
}