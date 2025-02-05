using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Editor.DataClasses.MapModes;
using Editor.Helper;
using static Editor.DataClasses.MapModes.RGBMapMode;
using static Editor.DataClasses.Settings.RenderingSettings;

namespace Editor.DataClasses.Settings
{
   [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public class RenderingSettings : SubSettings
   {
      public enum BorderMergeType
      {
         None,
         Merge,
         MergeAndLight
      }

      private EasterEggsSettings _easterEggsSettings = new();
      private IconsSettings _iconsSettings = new();
      private MapSettings _mapSettings = new();
      private MapModeSettings _mapMode = new();
      private SelectionSettings _selection = new();


      [CompareInEquals]
      [TypeConverter(typeof(ExpandableObjectConverter))]
      public MapSettings Map
      {
         get => _mapSettings;
         set => SetField(ref _mapSettings, value);
      }

      [CompareInEquals]
      [TypeConverter(typeof(ExpandableObjectConverter))]
      public IconsSettings Icons
      {
         get => _iconsSettings;
         set => SetField(ref _iconsSettings, value);
      }

      [CompareInEquals]
      [TypeConverter(typeof(ExpandableObjectConverter))]
      public EasterEggsSettings EasterEggs
      {
         get => _easterEggsSettings;
         set => SetField(ref _easterEggsSettings, value);
      }

      [CompareInEquals]
      [TypeConverter(typeof(ExpandableObjectConverter))]
      public MapModeSettings MapMode
      {
         get => _mapMode;
         set => SetField(ref _mapMode, value);
      }

      [CompareInEquals]
      [TypeConverter(typeof(ExpandableObjectConverter))]
      public SelectionSettings Selection
      {
         get => _selection;
         set => SetField(ref _selection, value);
      }
   }

   public class MapModeSettings : PropertySettings
   {
      private StripesDirection _stripesDirection = StripesDirection.DiagonalLbRt;
      private bool _showOceansAsGreyInTerrain = true;

      [Description("The direction of occupation stripes on the map")]
      [CompareInEquals]
      public StripesDirection StripesDirection
      {
         get => _stripesDirection;
         set => SetField(ref _stripesDirection, value);
      }
      
      [Description("If the oceans will be shown as grey in the terrain map mode")]
      [CompareInEquals]
      public bool ShowOceansAsGreyInTerrain
      {
         get => _showOceansAsGreyInTerrain;
         set => SetField(ref _showOceansAsGreyInTerrain, value);
      }

   }

   public class SelectionSettings : PropertySettings
   {
      private BorderMergeType _selectionMerging = BorderMergeType.MergeAndLight;
      private BorderMergeType _selectionPreviewMerging = BorderMergeType.MergeAndLight;

      [Description("How borders of provinces will be merged when selecting provinces")]
      [CompareInEquals]
      public BorderMergeType SelectionMerging
      {
         get => _selectionMerging;
         set => SetField(ref _selectionMerging, value);
      }

      [Description("How borders of provinces will be merged when previewing the selection of provinces")]
      [CompareInEquals]
      public BorderMergeType SelectionPreviewMerging
      {
         get => _selectionPreviewMerging;
         set => SetField(ref _selectionPreviewMerging, value);
      }
   }

   public class MapSettings : PropertySettings
   {
      private bool _showMapBorder = true;
      private int _mapBorderWidth = 2;
      private int _minVisiblePixels = 80;
      private BorderMergeType _mergeBorders = BorderMergeType.Merge;
      private ColorWrapper _mapBorderColor = new (){Value = Color.Black};

      [Description("If the map border will be shown")]
      [CompareInEquals]
      public bool ShowMapBorder
      {
         get => _showMapBorder;
         set => SetField(ref _showMapBorder, value);
      }

      [Description("The color of the map border")]
      [CompareInEquals]
      [TypeConverter(typeof(ExpandableObjectConverter))]
      public ColorWrapper MapBorderColor
      {
         get => _mapBorderColor;
         set => SetField(ref _mapBorderColor, value);
      }

      [Description("If borders of provinces will be merged")]
      [CompareInEquals]
      public BorderMergeType MergeBorders
      {
         get => _mergeBorders;
         set => SetField(ref _mergeBorders, value);
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
   }

   public class IconsSettings : PropertySettings
   {
      private int _iconTransparencyPadding = 3;

      [Description("The padding in pixels for all icons to have. Only applied after restart")]
      [CompareInEquals]
      public int IconTransparencyPadding
      {
         get => _iconTransparencyPadding;
         set => SetField(ref _iconTransparencyPadding, value);
      }
   }

   public class EasterEggsSettings : PropertySettings
   {
      private int _msTimerIntervalMapModeTimer = 17;
      private RGBMapMode.RGBMapModeType _rgbMapModeType = RGBMapMode.RGBMapModeType.Rotating;
      private GameOfLive.SurvivalRules _gameOfLiveSurvivalRules = GameOfLive.SurvivalRules.PopulationDynamics;
      private int _gameOfLiveGenerations = 100;
      private bool _gameOfLiveUseRandomCellChanges = false;
      private bool _allowAnimatedMapModes = true;

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