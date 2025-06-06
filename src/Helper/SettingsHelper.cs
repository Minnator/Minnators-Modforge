using System.ComponentModel;
using System.Runtime.InteropServices.JavaScript;
using Editor.DataClasses.Commands;
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

         Globals.Settings.Rendering.Map.MapBorderColor = Color.FromArgb(Globals.Settings.Rendering.Map.MapBorderColor.R,
                                                                        Globals.Settings.Rendering.Map.MapBorderColor.G,
                                                                        Globals.Settings.Rendering.Map.MapBorderColor.B);
      }


      public static void InitializeEvent()
      {
         Globals.Settings.Rendering.PropertyChanged += OnRenderSettingsChanged;
         Globals.Settings.Rendering.Map.PropertyChanged += OnMapSettingsChanged;
         Globals.Settings.Rendering.Selection.PropertyChanged += OnSelectionSettingsChanged;
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
            case nameof(GuiSettings.SelectionDrawerAlwaysOnTop):
               if (FormsHelper.GetOpenForm<SelectionDrawerForm>(out var form))
                  form.TopMost = Globals.Settings.Gui.SelectionDrawerAlwaysOnTop;
               break;
            case nameof(GuiSettings.NumOfPreloadedMonarchNameElements):
               Globals.MapWindow._monarchNames.UpdateCache((int)typeof(GuiSettings).GetProperty(args.PropertyName)!.GetValue(Globals.Settings.Gui)!);
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
            case nameof(Settings.Misc.CompactingSettings.AutoCompactingStrategy):
               switch (Globals.Settings.Misc.CompactingSettings.AutoCompactingStrategy)
               {
                  case CompactingSettings.AutoCompStrategy.None:
                     break;
                  case CompactingSettings.AutoCompStrategy.AfterXSize:
                     HistoryManager.TriggerCompaction(null, HistoryManager.GetUndoDepth);
                     break;
                  case CompactingSettings.AutoCompStrategy.EveryXMinutes:
                     HistoryManager.InitializeTimers();
                     break;
                  default:
                     throw new ArgumentOutOfRangeException();
               }

               break;
         }
      }

      private static void OnMapSettingsChanged(object? sender, PropertyChangedEventArgs args)
      {
         switch (args.PropertyName)
         {
            case nameof(MapSettings.MergeBorders):
               MapModeManager.RenderCurrent();
               break;
            case nameof(MapSettings.MapBorderColor):
               Globals.ZoomControl.BorderColor = Globals.Settings.Rendering.Map.MapBorderColor;
               Globals.ZoomControl.Invalidate();
               break;
            case nameof(MapSettings.MapBorderWidth):
               Globals.ZoomControl.BorderWidth = Globals.Settings.Rendering.Map.MapBorderWidth;
               Globals.ZoomControl.Invalidate();
               break;
            case nameof(MapSettings.ShowMapBorder):
               Globals.ZoomControl.Border = Globals.Settings.Rendering.Map.ShowMapBorder;
               Globals.ZoomControl.Invalidate();
               break;
            case nameof(MapSettings.MinVisiblePixels):
               Globals.ZoomControl.MinVisiblePixels = Globals.Settings.Rendering.Map.MinVisiblePixels;
               Globals.ZoomControl.ZoomingControl_Resize(null!, null!);
               break;
         }
      }

      private static void OnSelectionSettingsChanged(object? sender, PropertyChangedEventArgs args)
      {
         switch (args.PropertyName)
         {
            case nameof(SelectionSettings.SelectionPreviewMerging):
            case nameof(SelectionSettings.SelectionMerging):
               Selection.RePaintSelection();
               Globals.ZoomControl.Invalidate();
               break;
         }
      }

      private static void OnRenderSettingsChanged(object? sender, PropertyChangedEventArgs args)
      {
         switch (args.PropertyName)
         {
            case nameof(Settings.Rendering.EasterEggs.GameOfLiveSurvivalRules):
               GameOfLive.Rules = Globals.Settings.Rendering.EasterEggs.GameOfLiveSurvivalRules;
               GameOfLive.RunGameOfLive(Globals.Settings.Rendering.EasterEggs.GameOfLiveGenerations);
               break;
            case nameof(Settings.Rendering.EasterEggs.GameOfLiveGenerations):
               GameOfLive.RunGameOfLive(Globals.Settings.Rendering.EasterEggs.GameOfLiveGenerations);
               break;
            case nameof(Settings.Rendering.EasterEggs.AllowAnimatedMapModes):
               if (Globals.Settings.Rendering.EasterEggs.AllowAnimatedMapModes)
                  MapModeManager.CurrentMapMode.SetActive();
               else
                  MapModeManager.CurrentMapMode.SetInactive();
               break;
            case nameof(Settings.Rendering.Icons.IconTransparencyPadding):
               GameIconDefinition.UpdatePaddings();
               break;
         }

         Globals.ZoomControl.Invalidate();
      }
   }
}