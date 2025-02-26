
using System.Text.Json;

namespace Editor.Helper
{
   public static class JSONWrapper
   {
      private static readonly JsonSerializerOptions _options = new()
      {
         WriteIndented = true,
         Converters = { new ColorJsonConverter() },
      };

      public static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, _options);
      public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _options)!;
      public static T Clone<T>(T obj) => Deserialize<T>(Serialize(obj));
      public static void Save<T>(T obj, string path) => IO.WriteToFile(path, Serialize(obj), false);
      public static T Load<T>(string path) => Deserialize<T>(IO.ReadAllInUTF8(path));
   }


   public class ColorJsonConverter : System.Text.Json.Serialization.JsonConverter<Color>
   {
      public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      {
         return ColorTranslator.FromHtml(reader.GetString()!); // Converts from "#RRGGBB" or named colors
      }

      public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
      {
         writer.WriteStringValue(ColorTranslator.ToHtml(value)); // Converts to "#RRGGBB" or named colors
      }
   }
}