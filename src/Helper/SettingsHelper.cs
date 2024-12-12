using System.ComponentModel;
using Editor.DataClasses.Settings;

namespace Editor.Helper
{
   public static class SettingsHelper
   {
      public static void InitializeEvent()
      {
         Globals.Settings.Rendering.PropertyChanged += OnRenderSettingsChanged;
         Globals.Settings.Misc.PropertyChanged += OnMiscSettingsChanged;
      }

      private static void OnMiscSettingsChanged(object? sender, PropertyChangedEventArgs args)
      {
         switch (args.PropertyName)
         {

         }
      }

      private static void OnRenderSettingsChanged(object? sender, PropertyChangedEventArgs args)
      {
         switch (args.PropertyName)
         {
            case nameof(Settings.Rendering.MapBorderColor):
               Globals.ZoomControl.BorderColor = Globals.Settings.Rendering.MapBorderColor;
               Globals.ZoomControl.Invalidate();
               break;
            case nameof(Settings.Rendering.MapBorderWidth):
               Globals.ZoomControl.BorderWidth = Globals.Settings.Rendering.MapBorderWidth;
               Globals.ZoomControl.Invalidate();
               break;
            case nameof(Settings.Rendering.ShowMapBorder):
               Globals.ZoomControl.Border = Globals.Settings.Rendering.ShowMapBorder;
               Globals.ZoomControl.Invalidate();
               break;
            case nameof(Settings.Rendering.MinVisiblePixels):
               Globals.ZoomControl.MinVisiblePixels = Globals.Settings.Rendering.MinVisiblePixels;
               Globals.ZoomControl.ZoomingControl_Resize(null!, null!);
               break;
         }
         Globals.ZoomControl.Invalidate();
      }
   }
}