using System.ComponentModel;

namespace Editor.DataClasses.Settings;

public class Settings
{
   [Description("Contains all settings regarding the saving of files.")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   public SavingSettings SavingSettings = new();
   [Description("Contains all generalized settings regarding mapmodes")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   public MapModeSettings MapModeSettings = new();
}

public class SavingSettings
{
   [Description("<true> Asks for a filename or location beofre creating a new file\n<false> creates files with default names")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   public bool AlwaysAskBeforeCreatingFiles = true;
}


public class MapModeSettings
{
   public bool ShowCountryCapitals { get; set; } = false;
}