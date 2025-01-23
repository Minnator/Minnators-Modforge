using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Editor.DataClasses.Settings
{
   public enum PreferredEditor
   {
      VSCode,
      NotepadPlusPlus,
      Other
   }


[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public sealed class MiscSettings : SubSettings
   {
      private Language _language = Language.english;
      private string _lastModPath = string.Empty;
      private string _lastVanillaPath = string.Empty;
      private int _minDevelopmentInGeneration = 3;
      private int _maxDevelopmentInGeneration = 25;
      private int _randomSeed = 1444;
      private int _autoPropertiesCountBig = 8;
      private int _autoPropertiesCountSmall = 3;
      private int _maxProvinceDistanceForCountryWithSameSize = 5;
      private int _maxCountryDevDifferenceForCountryWithSameSize = 75;
      private int _historicRivalsFriendsGenerationAmount = 3;
      private bool _useEu4Cursor = true;
      private bool _useDiscordRichPresence = true;
      private bool _useDynamicProvinceNames = true;
      private PreferredEditor _preferredEditor = PreferredEditor.VSCode;
      private int _maxCompactingSize = 500;
      private int _minNumForCompacting = 5;

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

      [Description("The maximum number of provinces to search for a country with the same size")]
      [CompareInEquals]
      public int MaxProvinceDistanceForCountryWithSameSize
      {
         get => _maxProvinceDistanceForCountryWithSameSize;
         set => SetField(ref _maxProvinceDistanceForCountryWithSameSize, value);
      }

      [Description("The maximum development difference for a country to be considered of same size")]
      [CompareInEquals]
      public int MaxCountryDevDifferenceForCountryWithSameSize
      {
         get => _maxCountryDevDifferenceForCountryWithSameSize;
         set => SetField(ref _maxCountryDevDifferenceForCountryWithSameSize, value);
      }

      [Description("The amount of historic rivals and friends that will be generated")]
      [CompareInEquals]
      public int HistoricRivalsFriendsGenerationAmount
      {
         get => _historicRivalsFriendsGenerationAmount;
         set => SetField(ref _historicRivalsFriendsGenerationAmount, value);
      }

      [Description("Determines if the EU4 cursor should be used")]
      [CompareInEquals]
      public bool UseEu4Cursor   
      {
         get => _useEu4Cursor;
         set => SetField(ref _useEu4Cursor, value);
      }

      [Description("Determines if the Discord Rich Presence should be used. \n Changing this requires restarting the application.")]
      [CompareInEquals]
      public bool UseDiscordRichPresence
      {
         get => _useDiscordRichPresence;
         set => SetField(ref _useDiscordRichPresence, value);
      }

      [Description("Determines if dynamic province names should be used")]
      [CompareInEquals]
      public bool UseDynamicProvinceNames
      {
         get => _useDynamicProvinceNames;
         set => SetField(ref _useDynamicProvinceNames, value);
      }

      [Description("The preferred editor for opening files")]
      [CompareInEquals]
      public PreferredEditor PreferredEditor
      {
         get => _preferredEditor;
         set => SetField(ref _preferredEditor, value);
      }

      [Description("The maximum number of commands which will be compacted")]
      [CompareInEquals]
      public int MaxCompactingSize
      {
         get => _maxCompactingSize;
         set => SetField(ref _maxCompactingSize, value);
      }

      [Description("The minimum number of commands which will be compacted if applicable")]
      [CompareInEquals]
      public int MinNumForCompacting
      {
         get => _minNumForCompacting;
         set => SetField(ref _minNumForCompacting, value);
      }
   }
}