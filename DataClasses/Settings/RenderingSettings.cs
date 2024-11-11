using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Editor.DataClasses.Settings
{
   [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public class RenderingSettings : INotifyPropertyChanged
   {
      private StripesDirection _stripesDirection = StripesDirection.DiagonalLbRt;
      private bool _showMapBorder = true;
      private Color _mapBorderColor = Color.Black;
      private int _mapBorderWidth = 2;

      [Description("The direction of occupation stripes on the map")]
      [CompareInEquals]
      public StripesDirection StripesDirection
      {
         get => _stripesDirection;
         set => SetField(ref _stripesDirection, value);
      }

      [Description("If the map border will be shown")]
      [CompareInEquals]
      public bool ShowMapBorder
      {
         get => _showMapBorder;
         set => SetField(ref _showMapBorder, value);
      }

      [Description("The color of the map border")]
      [CompareInEquals]
      public System.Drawing.Color MapBorderColor
      {
         get => _mapBorderColor;
         set => SetField(ref _mapBorderColor, value);
      }

      [Description("The width of the map border")]
      [CompareInEquals]
      public int MapBorderWidth
      {
         get => _mapBorderWidth;
         set => SetField(ref _mapBorderWidth, value);
      }


      public override bool Equals(object? obj)
      {
         if (obj is not RenderingSettings settings)
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