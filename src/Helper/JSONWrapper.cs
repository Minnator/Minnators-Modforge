
using System.Text.Json;
using Windows.Storage.Provider;

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
      public static bool Deserialize<T>(string json, out T value)
      {
         value = default!;
         if (string.IsNullOrWhiteSpace(json))
            return false;
         value = JsonSerializer.Deserialize<T>(json, _options)!;
         return true;
      }

      public static void Save<T>(T obj, string path, bool append = false) => IO.WriteToFile(path, Serialize(obj), append);
      public static void SaveToModforgeData<T>(T obj, string internalPath) => Save(obj, Path.Combine(Globals.AppDataPath, internalPath));
      public static bool Load<T>(string path, out T readValue)
      {
         readValue = default!;
         var json = IO.ReadAllInUTF8(path);
         if (string.IsNullOrWhiteSpace(json))
            return false;
         return Deserialize(json, out readValue);
      }

      public static bool LoadFromModforgeData<T>(string internalPath, out T readValue) => Load(Path.Combine(Globals.AppDataPath, internalPath), out readValue);

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