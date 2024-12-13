using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Editor.DataClasses.MapModes;
using Editor.Helper;

namespace Editor.DataClasses.Settings
{
   [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public class RenderingSettings : SubSettings
   {
      private StripesDirection _stripesDirection = StripesDirection.DiagonalLbRt;
      private bool _showMapBorder = true;
      private Color _mapBorderColor = Color.Black;
      private int _mapBorderWidth = 2;
      private int _minVisiblePixels = 80;
      private bool _showOceansAsGreyInTerrain = true;
      private int _msTimerIntervalMapModeTimer = 17;
      private RGBMapMode.RGBMapModeType _rgbMapModeType = RGBMapMode.RGBMapModeType.Rotating;
      private GameOfLive.SurvivalRules _gameOfLiveSurvivalRules = GameOfLive.SurvivalRules.PopulationDynamics;
      private int _gameOfLiveGenerations = 100;
      private bool _gameOfLiveUseRandomCellChanges = false;
      private bool _allowAnimatedMapModes = true;

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


      [Description("The interval in milliseconds for the map mode timer (60 FPS is the base value)")]
      [CompareInEquals]
      public int MsTimerIntervalMapModeTimer
      {
         get => _msTimerIntervalMapModeTimer;
         set => SetField(ref _msTimerIntervalMapModeTimer, value);
      }

      [Description("The type of RGB map mode rendering which will be used")]
      [CompareInEquals]
      public RGBMapMode.RGBMapModeType RGBMapModeType
      {
         get => _rgbMapModeType;
         set => SetField(ref _rgbMapModeType, value);
      }

      [Description("The survival rules for the Game of Live map mode")]
      [CompareInEquals]
      public GameOfLive.SurvivalRules GameOfLiveSurvivalRules
      {
         get => _gameOfLiveSurvivalRules;
         set => SetField(ref _gameOfLiveSurvivalRules, value);
      }

      [Description("The number of generations which will be simulated for the Game of Live map mode")]
      [CompareInEquals]
      public int GameOfLiveGenerations
      {
         get => _gameOfLiveGenerations;
         set => SetField(ref _gameOfLiveGenerations, value);
      }

      [Description("If the Game of Live map mode will use random cell changes")]
      [CompareInEquals]
      public bool GameOfLiveUseRandomCellChanges
      {
         get => _gameOfLiveUseRandomCellChanges;
         set => SetField(ref _gameOfLiveUseRandomCellChanges, value);
      }

      [Description("If animated map modes are allowed")]
      [CompareInEquals]
      public bool AllowAnimatedMapModes
      {
         get => _allowAnimatedMapModes;
         set => SetField(ref _allowAnimatedMapModes, value);
      }
   }
}