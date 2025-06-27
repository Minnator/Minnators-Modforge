using System.Runtime.CompilerServices;
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
         JSONWrapper.SaveToModforgeData(settings, SETTINGS_FILE_NAME);
      }
   }

}