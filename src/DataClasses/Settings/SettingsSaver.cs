using System.Text.Json;
using System.Text.Json.Serialization;
using Editor.Helper;

namespace Editor.DataClasses.Settings
{
   public static class SettingsSaver
   {
      public const string SETTINGS_FILE_NAME = "modforge_settings.json";
      
      public static void Save(Settings settings)
      {
         JSONWrapper.Save(settings, Path.Combine(Globals.AppDirectory, SETTINGS_FILE_NAME));
      }
   }

}