using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Editor.Helper;

namespace Editor.DataClasses.Settings;

[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
public sealed class Settings : PropertyEquals, INotifyPropertyChanged
{
   private ToolTipSettings _toolTipSettings = new();
   private RenderingSettings _renderingSettings = new();
   private SavingSettings _savingSettings = new();
   private MiscSettings _miscSettings = new();

   [Description("Contains all settings regarding the misc settings.")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [CompareInEquals]
   public MiscSettings MiscSettings
   {
      get => _miscSettings;
      set => SetField(ref _miscSettings, value);
   }

   [Description("Contains all settings regarding the saving of files.")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [CompareInEquals]
   public SavingSettings SavingSettings
   {
      get => _savingSettings;
      set => SetField(ref _savingSettings, value);
   }

   [Description("Contains all settings regarding the rendering of the map.")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [CompareInEquals]
   public RenderingSettings RenderingSettings
   {
      get => _renderingSettings;
      set => SetField(ref _renderingSettings, value);
   }

   [Description("Contains all settings regarding the map tooltip.")]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [CompareInEquals]
   public ToolTipSettings ToolTipSettings 
   {
      get => _toolTipSettings;
      set => SetField(ref _toolTipSettings, value);
   }


   public event PropertyChangedEventHandler? PropertyChanged;

   private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
   {
      PropertyChanged?.Invoke(this, new (propertyName));
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