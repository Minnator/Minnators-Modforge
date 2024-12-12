using System.Text.Json;
using Editor.Helper;

namespace Editor.DataClasses.Settings
{
   public static class SettingsSaver
   {
      public const string SETTINGS_FILE_NAME = "modforge_settings.json";

      public static bool Save(Settings settings)
      {
         var settingsJSON = JsonSerializer.Serialize(settings, options: new() { WriteIndented = true });
         return IO.WriteToFile(Path.Combine(Globals.AppDirectory, SETTINGS_FILE_NAME), settingsJSON, false);
      }

   }
}