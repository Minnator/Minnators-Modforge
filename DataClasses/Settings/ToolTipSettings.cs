using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Editor.DataClasses.Settings
{
   [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public class ToolTipSettings : INotifyPropertyChanged
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

      public override bool Equals(object? obj)
      {
         if (obj is not ToolTipSettings settings)
            return false;

         var properties = GetType().GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(CompareInEquals)));

         foreach (var property in properties)
            if (!Equals(property.GetValue(this), property.GetValue(settings)))
               return false;

         return true;
      }

      public override int GetHashCode()
      {
         var properties = GetType().GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(CompareInEquals)));
         var hash = 17;

         foreach (var property in properties)
            hash = unchecked(hash * 31 + (property.GetValue(this)?.GetHashCode() ?? 0));

         return hash;
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