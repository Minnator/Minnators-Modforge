using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   public struct DescriptorData
   {
      public string Version;
      public string Name;
      public List<string[]> ReplacePaths;
      public List<string> Tags;
      public List<string> Dependencies;
      public string SupportedVersion;

      public DescriptorData(string version, string name, List<string[]> replacePaths, List<string> tags, List<string> dependencies, string supportedVersion)
      {
         Version = version;
         Name = name;
         ReplacePaths = replacePaths;
         Tags = tags;
         Dependencies = dependencies;
         SupportedVersion = supportedVersion;

         // Sort Paths by length to have major replacements first and speed up replacement detection
         OptimizePaths();
      }

      public void SortPaths()
      {
         ReplacePaths.Sort((a, b) => a.Length.CompareTo(b.Length));
      }

      private bool IsPathInList(List<string[]> paths, string[] path)
      {
         foreach (var optimizedPath in paths)
            if (FilesHelper.CheckIfPathInPath(path, optimizedPath))
               return true;
         return false;
      }

      public void OptimizePaths()
      {
         SortPaths();
         List<string[]> optimizedPaths = [];
         foreach (var path in ReplacePaths)
            if (!IsPathInList(optimizedPaths, path)) 
               optimizedPaths.Add(path);

         ReplacePaths = optimizedPaths;
      }
   }

   
   public static class DescriptorLoading
   {
      public static void Load()
      {
         var file = FilesHelper.GetAllFilesInFolder(Globals.ModPath, "*.mod");
         if (file.Count == 0)
         {
            Globals.ErrorLog.Write("Could not find Descriptor File!");
            return;
         }

         var firstFile = file[0];
         var data = IO.ReadAllInUTF8(firstFile);

         Parsing.RemoveCommentFromMultilineString(ref data, out var content);
         var element = Parsing.GetElements(0, ref content);

         var name = string.Empty;
         List<string> tags = [];
         List<string[]> paths = [];
         List<string> dependencies = [];
         var version = string.Empty;
         var supportedVersion = string.Empty;
         

         foreach (var item in element)
         {
            if (item is Block b)
            {
               switch (b.Name)
               {
                  case "tags":
                     tags = Parsing.GetQuotedStringList(b.GetContent);
                     break;
                  case "dependencies":
                     dependencies = Parsing.GetQuotedStringList(b.GetContent);
                     break;
                  default:
                     Globals.ErrorLog.Write($"Unknown Block in Descriptor File: {b.Name}");
                     break;
               }

            }
            else
            {
               var kvps = Parsing.GetKeyValueList(((Content)item).Value);
               if (kvps.Count == 0)
               {
                  Globals.ErrorLog.Write($"Unknown Content in Descriptor File: {((Content)item).Value}");
                  continue;
               }

               foreach (var kvp in kvps)
               {
                  switch (kvp.Key)
                  {
                     case "name":
                        name = kvp.Value.TrimQuotes();
                        break;
                     case "version":
                        version = kvp.Value.TrimQuotes();
                        break;
                     case "supported_version":
                        supportedVersion = kvp.Value.TrimQuotes();
                        break;
                     case "replace_path":
                        paths.Add(kvp.Value.TrimQuotes().Split('/'));
                        break;
                     case "picture":
                     case "remote_file_id":
                        break;
                     default:
                        Globals.ErrorLog.Write($"Unknown Key in Descriptor File: {kvp.Key}");
                        break;
                  }
               }
            }
         }

         Globals.DescriptorData = new (version, name, paths, tags, dependencies, supportedVersion);
      }

   }
}