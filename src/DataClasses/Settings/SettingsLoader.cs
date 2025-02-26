using Editor.Helper;

namespace Editor.DataClasses.Settings
{
   public static class SettingsLoader
   {
      public static Settings Load()
      {
         return JSONWrapper.LoadFromModforgeData<Settings>(SettingsSaver.SETTINGS_FILE_NAME, out var settings) ? settings : new ();
      }
   }
}