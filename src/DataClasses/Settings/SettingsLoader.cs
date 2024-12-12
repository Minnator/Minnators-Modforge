using Editor.Helper;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Editor.DataClasses.Settings
{
   public static class SettingsLoader
   {
      public static Settings Load() 
      {
         var settingsPath = Path.Combine(Globals.AppDirectory, SettingsSaver.SETTINGS_FILE_NAME);
         if (!File.Exists(settingsPath))
            return new ();

         var settingsJSON = IO.ReadAllLinesInUTF8(settingsPath);
         return JsonSerializer.Deserialize<Settings>(string.Join('\n', settingsJSON)) ?? new Settings();
      }
   }

   
}