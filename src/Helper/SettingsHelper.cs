using System.ComponentModel;
using System.Runtime.InteropServices.JavaScript;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Settings;
using Editor.ErrorHandling;
using Editor.Forms.Feature;
using Editor.Loading;

namespace Editor.Helper
{
   public static class SettingsHelper
   {
      public static void LoadSettingsToComponents()
      {
         LogManager.ChangeVerbosity(Globals.Settings.Logging.LoggingVerbosity);
      }


      public static void InitializeEvent()
      {
         Globals.Settings.Rendering.PropertyChanged += OnRenderSettingsChanged;
         Globals.Settings.Misc.PropertyChanged += OnMiscSettingsChanged;
         Globals.Settings.Logging.PropertyChanged += OnLoggingSettingsChanged;
         Globals.Settings.Gui.PropertyChanged += OnGuiSettingsChanged;
      }

      private static void OnLoggingSettingsChanged(object? sender, PropertyChangedEventArgs args)
      {
         switch (args.PropertyName)
         {
            case nameof(Settings.Logging.LoggingVerbosity):
               if (FormsHelper.GetOpenForm(out ErrorLogExplorer exp))
                  exp.LoadLogType(Globals.Settings.Logging.LoggingVerbosity);
               break;
         }
      }

      private static void OnGuiSettingsChanged(object? sender, PropertyChangedEventArgs args)
      {
         switch (args.PropertyName)
         {
            case nameof(GuiSettings.ShowCountryFlagInCE):
               Globals.MapWindow.CountryFlagLabel.Visible = Globals.Settings.Gui.ShowCountryFlagInCE;
               break;
            case nameof(GuiSettings.MapModes):
               Globals.MapWindow.UpdateMapModeButtons(false);
               break;
         }
      }

      private static void OnMiscSettingsChanged(object? sender, PropertyChangedEventArgs args)
      {
         switch (args.PropertyName)
         {
            case nameof(Settings.Misc.Language):
               LocalisationLoading.Load();
               break;
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
            case nameof(Settings.Rendering.GameOfLiveSurvivalRules):
               GameOfLive.Rules = Globals.Settings.Rendering.GameOfLiveSurvivalRules;
               GameOfLive.RunGameOfLive(Globals.Settings.Rendering.GameOfLiveGenerations);
               break;
            case nameof(Settings.Rendering.GameOfLiveGenerations):
               GameOfLive.RunGameOfLive(Globals.Settings.Rendering.GameOfLiveGenerations);
               break;
            case nameof(Settings.Rendering.AllowAnimatedMapModes):
               if (Globals.Settings.Rendering.AllowAnimatedMapModes)
                  MapModeManager.CurrentMapMode.SetActive();
               else
                  MapModeManager.CurrentMapMode.SetInactive();
               break;
         }
         Globals.ZoomControl.Invalidate();
      }
   }
}