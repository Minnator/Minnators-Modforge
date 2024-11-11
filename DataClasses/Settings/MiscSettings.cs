using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Editor.DataClasses.Settings
{
   [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public sealed class MiscSettings : INotifyPropertyChanged
   {
      private Language _language = Language.english;
      private string _lastModPath = string.Empty;
      private string _lastVanillaPath = string.Empty;
      private int _minDevelopmentInGeneration = 3;
      private int _maxDevelopmentInGeneration = 25;
      private int _randomSeed = 1444;

      [Description("The language in which the localisation will be shown")]
      [CompareInEquals]
      public Language Language
      {
         get => _language;
         set => SetField(ref _language, value);
      }

      [Description("The path to the last opened mod")]
      [CompareInEquals]
      public string LastModPath
      {
         get => _lastModPath;
         set => SetField(ref _lastModPath, value);
      }

      [Description("The last used Vanilla location")]
      [CompareInEquals]
      public string LastVanillaPath
      {
         get => _lastVanillaPath;
         set => SetField(ref _lastVanillaPath, value);
      }

      [Description("The minimum development a province will have when development is spread randomly")]
      [CompareInEquals]
      public int MinDevelopmentInGeneration
      {
         get => _minDevelopmentInGeneration;
         set => SetField(ref _minDevelopmentInGeneration, value);
      }

      [Description("The maximum development a province will have when development is spread randomly")]
      [CompareInEquals]
      public int MaxDevelopmentInGeneration
      {
         get => _maxDevelopmentInGeneration;
         set => SetField(ref _maxDevelopmentInGeneration, value);
      }

      [Description("The seed that will be used for random generation")]
      [CompareInEquals]
      public int RandomSeed
      {
         get => _randomSeed;
         set => SetField(ref _randomSeed, value);
      }

      public override bool Equals(object? obj)
      {
         if (obj is not MiscSettings settings)
            return false;

         var properties = GetType().GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(CompareInEquals)));

         foreach (var property in properties)
            if (!Equals(property.GetValue(this), property.GetValue(settings)))
               return false;

         return true;
      }

      public override int GetHashCode()
      {
         var properties = GetType().GetProperties()
             .Where(prop => Attribute.IsDefined(prop, typeof(CompareInEquals)));
         var hash = 17;

         foreach (var property in properties)
            hash = unchecked(hash * 31 + (property.GetValue(this)?.GetHashCode() ?? 0));

         return hash;
      }

      public event PropertyChangedEventHandler? PropertyChanged;

      private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
      {
         PropertyChanged?.Invoke(this, new(propertyName));
      }

      private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
      {
         if (EqualityComparer<T>.Default.Equals(field, value)) 
            return false;
         field = value;
         OnPropertyChanged(propertyName);
         return true;
      }
   }
}