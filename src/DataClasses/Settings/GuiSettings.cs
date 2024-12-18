using System.ComponentModel;
using Editor.DataClasses.MapModes;

namespace Editor.DataClasses.Settings
{
   public class GuiSettings : SubSettings
   {
      private bool _showCountryFlagInCe = true;
      private bool _jumpToSelectedProvinceCollection = true;
      private MapModeType[] _mapModes = new MapModeType[10];
      private bool _enableDisableHistoryEntryCreationGlobally;

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

      [Description("The mapmodes which are available in the hotkey buttons where inxex 0 ist the button on the left and ^1 is the one on the right.")]
      [CompareInEquals]
      public MapModeType[] MapModes
      {
         get => _mapModes;
         set => SetField(ref _mapModes, value);
      }

      [Description("Determines if the history entry creation should be enabled or disabled globally.")]
      [CompareInEquals]
      public bool EnableDisableHistoryEntryCreationGlobally
      {
         get => _enableDisableHistoryEntryCreationGlobally;
         set => SetField(ref _enableDisableHistoryEntryCreationGlobally, value);
      }
   }
}