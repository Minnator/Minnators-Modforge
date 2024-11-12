using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Editor.Helper;

namespace Editor.DataClasses.Settings
{
   [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public sealed class MiscSettings : PropertyEquals, INotifyPropertyChanged
   {
      private Language _language = Language.english;
      private string _lastModPath = string.Empty;
      private string _lastVanillaPath = string.Empty;
      private int _minDevelopmentInGeneration = 3;
      private int _maxDevelopmentInGeneration = 25;
      private int _randomSeed = 1444;
      private int _autoPropertiesCountBig = 8;
      private int _autoPropertiesCountSmall = 3;

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

      [Description("The amount of properties that will be generated automatically\nHistoricUnits, HistoricIdeas")]
      [CompareInEquals]
      public int AutoPropertiesCountBig
      {
         get => _autoPropertiesCountBig;
         set => SetField(ref _autoPropertiesCountBig, value);
      }

      [Description("The amount of properties that will be generated automatically\nHistoricRivals, HistoricFriends")]
      [CompareInEquals]
      public int AutoPropertiesCountSmall
      {
         get => _autoPropertiesCountSmall;
         set => SetField(ref _autoPropertiesCountSmall, value);
      }

      [Description("The seed that will be used for random generation")]
      [CompareInEquals]
      public int RandomSeed
      {
         get => _randomSeed;
         set => SetField(ref _randomSeed, value);
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