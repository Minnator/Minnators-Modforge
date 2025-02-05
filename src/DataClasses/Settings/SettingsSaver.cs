using System.Text.Json;
using System.Text.Json.Serialization;
using Editor.Helper;

namespace Editor.DataClasses.Settings
{
   public static class SettingsSaver
   {
      public const string SETTINGS_FILE_NAME = "modforge_settings.json";
      public static JsonSerializerOptions options;


      public static void Init()
      {
         options = new ()
         {
            WriteIndented = true,
            Converters = { new ColorJsonConverter() },
         };
      }

      public static bool Save(Settings settings)
      {
         var settingsJSON = JsonSerializer.Serialize(settings, options);
         return IO.WriteToFile(Path.Combine(Globals.AppDirectory, SETTINGS_FILE_NAME), settingsJSON, false);
      }

   }

   public class ColorJsonConverter : JsonConverter<Color>
   {
      public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      {
         string colorString = reader.GetString()!;
         return ColorTranslator.FromHtml(colorString); // Converts from "#RRGGBB" or named colors
      }

      public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
      {
         string colorString = ColorTranslator.ToHtml(value); // Converts to "#RRGGBB" format
         writer.WriteStringValue(colorString);
      }
   }
}