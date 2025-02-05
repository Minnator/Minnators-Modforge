using System.ComponentModel;

namespace Editor.DataClasses.Settings
{
   public class GeneratorSettings : SubSettings
   {
      private int _randomSeed = 1444;
      private int _numOfUnitsToSelect = 8;
      private DevGeneratingSettings _devGeneratingSettings = new();
      private DistanceSettings _distanceSettings = new();

      [Description("The seed that will be used for random generation")]
      [CompareInEquals]
      public int RandomSeed
      {
         get => _randomSeed;
         set => SetField(ref _randomSeed, value);
      }

      [Description("The amount of Units which will be put in HistoricalUnits")]
      [CompareInEquals]
      public int NumOfUnitsToSelect
      {
         get => _numOfUnitsToSelect;
         set => SetField(ref _numOfUnitsToSelect, value);
      }


      [CompareInEquals]
      [TypeConverter(typeof(ExpandableObjectConverter))]
      public DevGeneratingSettings DevGeneratingSettings
      {
         get => _devGeneratingSettings;
         set => SetField(ref _devGeneratingSettings, value);
      }

      [CompareInEquals]
      [TypeConverter(typeof(ExpandableObjectConverter))]
      public DistanceSettings DistanceSettings
      {
         get => _distanceSettings;
         set => SetField(ref _distanceSettings, value);
      }

   }


   public class DistanceSettings : PropertySettings
   {
      private int _maxProvinceDistanceForCountryWithSameSize = 5;
      private int _maxCountryDevDifferenceForCountryWithSameSize = 75;

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
   }

   public class DevGeneratingSettings : PropertySettings
   {
      private int _minDevelopmentInGeneration = 3;
      private int _maxDevelopmentInGeneration = 25;
      
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
   }
}