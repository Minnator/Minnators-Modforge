using System.Reflection;
using Editor.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Editor.DataClasses.Settings
{
   public static class SettingsLoader
   {
      public static readonly string? ExecutableFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

      public static Settings Load()
      {
         var settingsPath = Path.Combine(ExecutableFolder ?? Globals.DownloadsFolder, SettingsSaver.SETTINGS_FILE_NAME);
         if (!File.Exists(settingsPath))
            return new ();

         var settingsJSON = IO.ReadAllLinesInUTF8(settingsPath);
         return JsonSerializer.Deserialize<Settings>(string.Join('\n', settingsJSON)) ?? new Settings();

         
      }
   }

   
}