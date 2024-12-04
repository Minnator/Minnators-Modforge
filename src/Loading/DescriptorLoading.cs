using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   public struct DescriptorData
   {
      public string Version;
      public string Name;
      public List<string> ReplacePaths;
      public string SupportedVersion;

      public DescriptorData(string version, string name, List<string> replacePaths, string supportedVersion)
      {
         Version = version;
         Name = name;
         ReplacePaths = replacePaths;
         SupportedVersion = supportedVersion;
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
         var version = string.Empty;
         var supportedVersion = string.Empty;
         

         foreach (var item in element)
         {
            if (item is Block b)
            {
               if (!b.Name.Equals("tags"))
               {
                  Globals.ErrorLog.Write($"Unknown Block in Descriptor File: {b.Name}");
                  continue;
               }

               tags = Parsing.GetQuotedStringList(b.GetContent);
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
                     default:
                        Globals.ErrorLog.Write($"Unknown Key in Descriptor File: {kvp.Key}");
                        break;
                  }
               }
            }
         }

         Globals.DescriptorData = new (version, name, tags, supportedVersion);
      }

   }
}