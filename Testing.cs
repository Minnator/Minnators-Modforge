using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Editor
{
   public class EnumHelpers<TTarget> : IDisposable
   {
      public EnumHelpers()
      {
         Dict = Enum.GetNames(typeof(TTarget)).ToDictionary(x =>
         {
            return x;
         }, x =>
         {
            return (TTarget)Enum.Parse(typeof(TTarget), x);
         }, StringComparer.OrdinalIgnoreCase);

      }

      private readonly Dictionary<string, TTarget> Dict;

      public bool TryConvert(string value, out TTarget enumValue)
      {
         return Dict.TryGetValue(value, out enumValue!);
      }

      public void Dispose()
      {
         Dict.Clear();
      }

   }


   public class DefaultValueIgnoringResolver : DefaultContractResolver
   {
      protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
      {
         var property = base.CreateProperty(member, memberSerialization);

         property.ShouldSerialize = instance =>
         {
            var propertyValue = property.ValueProvider.GetValue(instance);

            // Handle complex objects (classes/structs) recursively
            if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
               return HasNonDefaultValues(propertyValue);
            }

            // Handle primitive properties with DefaultValueAttribute
            var defaultValueAttribute = member.GetCustomAttribute<DefaultValueAttribute>();
            if (defaultValueAttribute != null)
            {
               return !Equals(propertyValue, defaultValueAttribute.Value);
            }

            // If no default value is specified, always serialize the property
            return true;
         };

         return property;
      }

      // Recursively checks if any properties in a complex object have non-default values
      private bool HasNonDefaultValues(object? obj)
      {
         if (obj == null)
         {
            return false;
         }

         // Get all properties of the object
         var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

         foreach (var property in properties)
         {
            var defaultValueAttribute = property.GetCustomAttribute<DefaultValueAttribute>();
            var propertyValue = property.GetValue(obj);

            // If it's a complex type, check its nested properties
            if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
               if (HasNonDefaultValues(propertyValue))
               {
                  return true;
               }
            }
            else
            {
               // If it's a primitive type, check if it's different from the default value
               if (defaultValueAttribute != null && !Equals(propertyValue, defaultValueAttribute.Value))
               {
                  return true;
               }
            }
         }

         // If none of the properties have non-default values, return false
         return false;
      }
   }

   public class Test_SubSettings
   {
      [DefaultValue(10)] 
      public int SubSetting1 { get; set; } = 10;
      [DefaultValue(true)] 
      public bool SubSetting2 { get; set; } = true;
   }

   public class Test_Settings
   {
      [DefaultValue("Default")]
      public string Setting1 { get; set; } = "Default";
      [DefaultValue(100)]
      public int Setting2 { get; set; } = 100;
      public Test_SubSettings SubSettings { get; set; } = new ();
   }

   public static class SettingsManager
   {
      private static readonly string SettingsFilePath = "settings.json";
      public static Test_Settings CurrentSettings { get; private set; } = new();

      // Load the settings from file or create a new instance with default values
      public static void LoadSettings()
      {
         if (File.Exists(SettingsFilePath))
         {
            // Load the settings from the file
            var json = File.ReadAllText(SettingsFilePath);
            CurrentSettings = JsonConvert.DeserializeObject<Test_Settings>(json);
         }
         else
         {
            // No settings file exists, so use the default settings
            CurrentSettings = new Test_Settings();
         }

         // Ensure that any null subsettings are replaced with default subsettings
         if (CurrentSettings.SubSettings == null)
         {
            CurrentSettings.SubSettings = new Test_SubSettings();
         }
      }

      // Save only the settings that are different from the default
      public static void SaveSettings()
      {
         var jsonSettings = new JsonSerializerSettings
         {
            // Ignore properties that have default values
            DefaultValueHandling = DefaultValueHandling.Ignore,
            // Don't serialize null values
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
             ContractResolver = new DefaultValueIgnoringResolver()
         };

         // Serialize the current settings with the given Json.NET settings
         var json = JsonConvert.SerializeObject(CurrentSettings, jsonSettings);
         File.WriteAllText(SettingsFilePath, json);
      }
   }
}