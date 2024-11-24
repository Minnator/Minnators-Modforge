using System.ComponentModel;
using System.Runtime.CompilerServices;
using Editor.Helper;

namespace Editor.DataClasses.Settings
{
   public class GuiSettings : PropertyEquals, INotifyPropertyChanged
   {
      private bool _showCountryFlagInCe = true;
      private bool _jumpToSelectedProvinceCollection = true;
      public event PropertyChangedEventHandler? PropertyChanged;

      protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

      protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
      {
         if (EqualityComparer<T>.Default.Equals(field, value)) return false;
         field = value;
         OnPropertyChanged(propertyName);
         return true;
      }

      [Description("Determines if the country flag should be shown in the country editor.")]
      [CompareInEquals]
      public bool ShowCountryFlagInCE
      {
         get => _showCountryFlagInCe;
         set => SetField(ref _showCountryFlagInCe, value);
      }

      [Description("Determines if the map should focus on a newly selected province collection.")]
      [CompareInEquals]
      public bool JumpToSelectedProvinceCollection
      {
         get => _jumpToSelectedProvinceCollection;
         set => SetField(ref _jumpToSelectedProvinceCollection, value);
      }
   }
}