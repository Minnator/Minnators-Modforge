using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Editor.Helper;

namespace Editor.DataClasses.Settings
{
   [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public class RenderingSettings : PropertyEquals, INotifyPropertyChanged
   {
      private StripesDirection _stripesDirection = StripesDirection.DiagonalLbRt;
      private bool _showMapBorder = true;
      private Color _mapBorderColor = Color.Black;
      private int _mapBorderWidth = 2;
      private int _minVisiblePixels = 80;
      private bool _showOceansAsGreyInTerrain = true;

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

      [Description("The minimum number of pixels of provinces.bmp visible on the map. Capped at 10")]
      [CompareInEquals]
      public int MinVisiblePixels
      {
         get => _minVisiblePixels;
         set => SetField(ref _minVisiblePixels, value);
      }

      [Description("If the oceans will be shown as grey in the terrain map mode")]
      [CompareInEquals]
      public bool ShowOceansAsGreyInTerrain
      {
         get => _showOceansAsGreyInTerrain;
         set => SetField(ref _showOceansAsGreyInTerrain, value);
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