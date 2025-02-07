using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Editor;
using Editor.Forms.Feature;
using Editor.Helper;

namespace Editor.DataClasses.Settings;

[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
public sealed class Settings : PropertyEquals, INotifyPropertyChanged
{
   private ToolTipSettings _toolTipSettings = new();
   private RenderingSettings _renderingSettings = new();
   private SavingSettings _savingSettings = new();
   private MiscSettings _miscSettings = new();
   private GuiSettings _guiSettings = new();
   private PopUpSettings _popUpSettings = new();
   private MetricDummySetting _metrics = new();
   private LoggingSettings _logging = new();
   private GeneratorSettings _generator = new();
   private AchievementsSettings _achievements = new();


   public Settings()
   {
      PropertyChanged += (sender, args) =>
      {
         if (Form.ActiveForm?.GetType() != typeof(SettingsWindow))
         {
            if (Globals.State == State.Running)
               SettingsSaver.Save(Globals.Settings);
         }
      };
   }

   [Description("Contains all settings regarding the misc settings.")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [CompareInEquals]
   public MiscSettings Misc
   {
      get => _miscSettings;
      set => SetField(ref _miscSettings, value);
   }

   [Description("Contains all settings regarding the saving of files.")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [CompareInEquals]
   public SavingSettings Saving
   {
      get => _savingSettings;
      set => SetField(ref _savingSettings, value);
   }

   [Description("Contains all settings regarding the rendering of the map.")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [CompareInEquals]
   public RenderingSettings Rendering
   {
      get => _renderingSettings;
      set => SetField(ref _renderingSettings, value);
   }

   [Description("Contains all settings regarding the map tooltip.")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [CompareInEquals]
   public ToolTipSettings ToolTip
   {
      get => _toolTipSettings;
      set => SetField(ref _toolTipSettings, value);
   }

   [Description("Contains all settings regarding the GUI customisation")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [CompareInEquals]
   public GuiSettings Gui
   {
      get => _guiSettings;
      set => SetField(ref _guiSettings, value);
   }

   [Description("Contains all settings regarding Pop ups")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [CompareInEquals]
   public PopUpSettings PopUps
   {
      get => _popUpSettings;
      set => SetField(ref _popUpSettings, value);
   }

   [Description("Contains all settings regarding the start up metrics of the application")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [CompareInEquals]
   public MetricDummySetting Metrics
   {
      get => _metrics;
      set => SetField(ref _metrics, value);
   }

   [Description("Contains all settings regarding the logging of the application")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [CompareInEquals]
   public LoggingSettings Logging
   {
      get => _logging;
      set => SetField(ref _logging, value);
   }

   [Description("Contains all settings regarding the generation of data")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [CompareInEquals]
   public GeneratorSettings Generator
   {
      get => _generator;
      set => SetField(ref _generator, value);
   }

   [Description("Contains all settings regarding the achievements of the application")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [CompareInEquals]
   public AchievementsSettings Achievements
   {
      get => _achievements;
      set => SetField(ref _achievements, value);
   }

   public event PropertyChangedEventHandler? PropertyChanged;

   public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
   {
      PropertyChanged?.Invoke(this, new(propertyName));
   }

   private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
   {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
   }
}

[AttributeUsage(AttributeTargets.Property)]
public class CompareInEquals : Attribute;

public abstract class SubSettings : PropertySettings
{
   [Browsable(false)]
   public bool IsAvailable { get; set; } = true;
}

public abstract class PropertySettings : PropertyEquals, INotifyPropertyChanged
{
   public event PropertyChangedEventHandler? PropertyChanged;

   private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
   {
      if (Globals.State != State.Running)
         return;
      PropertyChanged?.Invoke(this, new(propertyName));
      Globals.Settings.OnPropertyChanged(propertyName);
   }

   public void Invalidate(string propertyName)
   {
      OnPropertyChanged(propertyName);
   }

   internal bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
   {
      if (EqualityComparer<T>.Default.Equals(field, value))
         return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
   }
}

public class CEmptyStringConverter : ExpandableObjectConverter
{
   public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
   {
      return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
   }

   public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
   {
      if (destinationType == typeof(string) && value is PropertySettings)
      {
         return string.Empty; 
      }
      return base.ConvertTo(context, culture, value, destinationType) ?? string.Empty;
   }
}