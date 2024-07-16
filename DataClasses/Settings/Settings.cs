namespace Editor.DataClasses.Settings;

public class Settings
{
   public MapModeSettings MapModeSettings = new();
}

public class MapModeSettings
{
   public bool ShowCountryCapitals { get; set; } = false;
}