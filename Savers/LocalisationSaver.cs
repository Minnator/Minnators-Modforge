using System.Diagnostics;
using System.Text;
using Editor.Helper;

namespace Editor.Savers
{
   public enum LocSaveType
   {
      Vanilla,
      Mod,
      Replace,
      New
   }

   public static class LocalisationSaver
   {
      private static List<string> _vanillaLocalisationFileNames = [];
      private static List<string> _modLocalisationFileNames = [];
      private static List<string> _replaceLocalisationFileNames = [];
      private static UTF8Encoding _encoding = null!;

      public static void Initialize(List<string> vanillaFiles, List<string> modFiles, List<string> replaceFiles)
      {
         _vanillaLocalisationFileNames = vanillaFiles;
         _modLocalisationFileNames = modFiles;
         _replaceLocalisationFileNames = replaceFiles;
         _encoding = new (true);

         // Create the localisation folders if they don't exist
         if (!Directory.Exists(FilesHelper.CreateModPath("localisation", "replace")))
            Directory.CreateDirectory(FilesHelper.CreateModPath("localisation", "replace"));
      }

      public static bool SaveLocalisation(List<KeyValuePair<string, string>> locs, string language, bool replace)
      {
         CategorizeLocStrings(locs, 
            out var fromVanilla, 
            out var fromMod, 
            out var fromReplace, 
            out var newLoc);



         return true;
      }

      private static void SaveLocsToFile(List<KeyValuePair<string, string>> locs, string filePath, LocSaveType type)
      {
         var fileContent = GetFileContentDictionary(filePath, type);
         foreach (var loc in locs) 
            fileContent[loc.Key] = loc.Value;

         var streamWriter = new StreamWriter(filePath, false, _encoding);
         AddFileHeaderToStreamWriter(ref streamWriter);
         foreach (var loc in fileContent)
            streamWriter.WriteLine($" {loc.Key}:0 \"{loc.Value}\"");

         streamWriter.Close();
      }

      private static Dictionary<string, string> GetFileContentDictionary(string filePath, LocSaveType type)
      {
         return type switch
         {
            LocSaveType.Mod => Globals.ModLocalisation[filePath],
            LocSaveType.Vanilla => Globals.VanillaLocalisation[filePath],
            LocSaveType.Replace => Globals.ReplaceLocalisation[filePath],
            LocSaveType.New => [],
            _ => []
         };
      }

      private static void AddFileHeaderToStreamWriter(ref StreamWriter sw)
      {
         sw.WriteLine($"l_{Globals.Language.ToString().ToLower()}:");
      }

      private static void CategorizeLocStrings(List<KeyValuePair<string, string>> locStrings,
         out List<KeyValuePair<string, string>> fromVanilla, 
         out List<KeyValuePair<string, string>> fromMod,
         out List<KeyValuePair<string, string>> fromReplace,
         out List<KeyValuePair<string, string>> newLoc)
      {
         fromVanilla = [];
         fromMod = [];
         fromReplace = [];
         newLoc = [];

         foreach (var loc in locStrings)
         {
            if (!Localisation.GetFileForKey(loc.Key, out var fileName))
            {
               newLoc.Add(loc);
               continue;
            }

            switch (GetLocSaveType(loc.Key))
            {
               case LocSaveType.Vanilla:
                  fromVanilla.Add(loc);
                  break;
               case LocSaveType.Mod:
                  fromMod.Add(loc);
                  break;
               case LocSaveType.Replace:
                  fromReplace.Add(loc);
                  break;
               case LocSaveType.New:
                  newLoc.Add(loc);
                  break;
            }
         }
      }


      private static LocSaveType GetLocSaveType(string key)
      {
         foreach (var loc in Globals.ReplaceLocalisation)
         {
            if (loc.Value.ContainsKey(key))
               return LocSaveType.Replace;
         }

         foreach (var loc in Globals.ModLocalisation)
         {
            if (loc.Value.ContainsKey(key))
               return LocSaveType.Mod;
         }

         foreach (var loc in Globals.VanillaLocalisation)
         {
            if (loc.Value.ContainsKey(key))
               return LocSaveType.Vanilla;
         }
         return LocSaveType.New;
      }

   }
}