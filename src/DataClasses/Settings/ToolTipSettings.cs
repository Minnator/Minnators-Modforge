using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Editor.Helper;

namespace Editor.DataClasses.Settings
{
   [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public class ToolTipSettings : PropertyEquals, INotifyPropertyChanged
   {
      private string _toolTipText = $"$MAPMODE_SPECIFIC$\n------------------\nId:   $id$\nName: $name$\nOwner: $owner$ ($owner%L$)\nArea: $area$ ($area%L$)";
      private bool _showToolTip = true;

      [Description("The text that will be shown in the tooltip")]
      [CompareInEquals]
      public string ToolTipText
      {
         get => _toolTipText;
         set => SetField(ref _toolTipText, value);
      }

      [Description("If the tooltip will be shown")]
      [CompareInEquals]
      public bool ShowToolTip
      {
         get => _showToolTip;
         set => SetField(ref _showToolTip, value);
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