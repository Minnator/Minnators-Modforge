using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using Editor.Loading;
using Editor.Helper;
using Windows.Storage;

namespace Editor.DataClasses.Settings;

[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
public class Settings
{
   [Description("Contains all settings regarding the misc settings.")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   public MiscSettings MiscSettings { get; set; } = new();

   [Description("Contains all settings regarding the saving of files.")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   public SavingSettings SavingSettings { get; set; } = new();

   [Description("Contains all settings regarding the rendering of the map.")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   public RenderingSettings RenderingSettings { get; set; } = new();

   [Description("Contains all settings regarding the map tooltip.")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   public ToolTipSettings ToolTipSettings { get; set; } = new();

   public override bool Equals(object? obj)
   {
      if (obj is not Settings settings)
         return false;

      return MiscSettings.Equals(settings.MiscSettings) && SavingSettings.Equals(settings.SavingSettings) && RenderingSettings.Equals(settings.RenderingSettings) && ToolTipSettings.Equals(settings.ToolTipSettings);
   }

   public override int GetHashCode()
   {
      return HashCode.Combine(MiscSettings, SavingSettings, RenderingSettings, ToolTipSettings);
   }
}

[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
public class MiscSettings
{
   private Language _language { get; set; } = Language.english;
   [Description("The language in which the localisation will be shown")]
   public Language Language
   {
      get => _language;
      set
      {
         _language = value;
         LocalisationLoading.Load();
      }
   }

   [Description("The path to the last opened mod")]
   public string LastModPath { get; set; } = string.Empty;

   [Description("The last used Vanilla location")]
   public string LastVanillaPath { get; set; } = string.Empty;

   public override bool Equals(object? obj)
   {
      if (obj is not MiscSettings settings)
         return false;

      return Language == settings.Language && LastModPath == settings.LastModPath && LastVanillaPath == settings.LastVanillaPath;
   }

   public override int GetHashCode()
   {
      return HashCode.Combine(Language, LastModPath, LastVanillaPath);
   }
}

[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
public class ToolTipSettings
{
   [Description("The text that will be shown in the tooltip")]
   public string ToolTipText { get; set; } = $"$MAPMODE_SPECIFIC$\n------------------\nId:   $id$\nName: $name$\nOwner: $owner$ ($owner%L$)\nArea: $area$ ($area%L$)";

   [Description("If the tooltip will be shown")]
   public bool ShowToolTip { get; set; } = true;

   public override bool Equals(object? obj)
   {
      if (obj is not ToolTipSettings settings)
         return false;

      return ToolTipText == settings.ToolTipText && ShowToolTip == settings.ShowToolTip;
   }

   public override int GetHashCode()
   {
      return HashCode.Combine(ToolTipText, ShowToolTip);
   }
}


[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
public class RenderingSettings
{
   [Description("The direction of occupation stripes on the map")]
   public StripesDirection StripesDirection { get; set; } = StripesDirection.DiagonalLbRt;

   public override bool Equals(object? obj)
   {
      if (obj is not RenderingSettings settings)
         return false;

      return StripesDirection == settings.StripesDirection;
   }

   public override int GetHashCode()
   {
      return HashCode.Combine(StripesDirection);
   }
}

[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
public class SavingSettings
{
   [Description("<true> Asks for a filename or location beofre creating a new file\n<false> creates files with default names")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   public bool AlwaysAskBeforeCreatingFiles { get; set; } = true;

   [Description("Define how often and if the Modforge should ask where to save edited objects.")]
   public FileSavingMode FileSavingMode { get; set; } = FileSavingMode.AskOnce;

   [Description("The location where the loading log will be saved")]
   public string LoadingLogLocation { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

   [Description("The location where the error log will be saved")]
   public string ErrorLogLocation { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

   public override bool Equals(object? obj)
   {
      if (obj is not SavingSettings settings)
         return false;

      return GetHashCode().Equals(settings.GetHashCode());
   }

   public override int GetHashCode()
   {
      return HashCode.Combine(AlwaysAskBeforeCreatingFiles, FileSavingMode);
   }
}


// ---------------------------------------------------------------------------------------------------------------------
//                                           SETTINGS SAVING AND LOADING
// ---------------------------------------------------------------------------------------------------------------------

public static class SettingsSaver
{
   public const string SETTINGS_FILE_NAME = "modforge_settings.json";
   public static readonly string? ExecutableFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

   public static bool Save(Settings settings)
   {
      var settingsJSON = JsonSerializer.Serialize(settings, options: new() { WriteIndented = true });
      return IO.WriteToFile(Path.Combine(ExecutableFolder ?? Globals.DownloadsFolder, SETTINGS_FILE_NAME), settingsJSON, false);
   }

}

public static class SettingsLoader
{
   public static readonly string? ExecutableFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

   public static Settings Load()
   {
      var settingsPath = Path.Combine(ExecutableFolder ?? Globals.DownloadsFolder, SettingsSaver.SETTINGS_FILE_NAME);
      if (!File.Exists(settingsPath))
         return new ();

      var settingsJSON = IO.ReadAllLinesInUTF8(settingsPath);
      return JsonSerializer.Deserialize<Settings>(string.Join('\n', settingsJSON)) ?? new Settings();
   }
}