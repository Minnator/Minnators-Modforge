using Editor.Controls;
using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Settings;
using Editor.Events;
using Editor.Forms;
using Editor.Forms.Feature;
using Editor.Forms.Feature.AdvancedSelections;
using Editor.Forms.PopUps;
using Editor.Helper;
using Editor.Loading;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor;

#region enums
public enum CommandHistoryType
{
   SimpleSelection,
   ComplexSelection,
   Action
}
public enum State
{
   Loading,
   Running,
   Initializing,
   LoadingHistory,
   Waiting
}
public enum Mana
{
   ADM,
   DIP,
   MIL,
   NONE
}
public enum EditingStatus
{
   LoadingInterface,
   Idle,
   Saving,
   Interrupted
}
public enum Language
{
   english,
   french,
   german,
   spanish,
}
public enum StripesDirection
{
   Horizontal,
   Vertical,
   DiagonalLbRt,
   DiagonalLtRb,
   Dotted,
   Pluses
}

public enum FileSavingMode
{
   AskOnce,
   AskAlways,
   Automatic,
}

// When several provinces are selected only attributes that are the same across all selected provinces are shown.
// Other attributes e.g. development will be increased per province or set per province: 
// All province's tax will be increased by 1, all province's manpower will be set to 100.
public enum ProvinceEditingStatus
{
   PreviewOnly, // Province is only previewed to the gui, no editing is allowed
   PreviewUntilSelection, // Province is previewed until a selection is made then the selected province(s) are previewed and editing is allowed
   Selection // Province is only previewd when selected and editing is allowed
}
#endregion

//contains all required and used data across the application and instances of forms.
public static class Globals
{
   public const string DISCORD_INVITATION_LINK = "https://discord.gg/22AhD5qkme";
   public const string GITHUB_LINK = "https://github.com/Minnator/Editor.git";

   public static string VanillaPath = string.Empty;
   public static string ModPath = string.Empty;
   
   #region LoadingScreen
   public static int LoadingStages = 0;
   #endregion
   
   public static Random Random = null!;

   public static ConsoleForm? ConsoleForm = null;
   public static Search? SearchForm = null;
   public static MapWindow MapWindow = null!;
   public static ZoomControl ZoomControl = null!;
   public static AdvancedSelectionsForm? AdvancedSelectionsForm = null;
   public static GuiDrawings? GuiDrawings = null;

   // SETTINGS
   public static Settings Settings = new();
   public static readonly ProvinceEditingStatus ProvinceEditingStatus = ProvinceEditingStatus.Selection;

   // Date of history
   public static DateTime Date
   { 
      get
      {
         return MapWindow.DateControl.Date;
      }
   }
   // Logs
   public static readonly string DownloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
   public static readonly Log LoadingLog = new(Settings.Saving.LoadingLogLocation, "Loading");
   public static readonly Log ErrorLog = new(Settings.Saving.ErrorLogLocation, "Error");

   // Contains the current state of the application
   public static State State = State.Loading;
   public static EditingStatus EditingStatus = EditingStatus.Idle;
   public static bool AllowEditing => EditingStatus == EditingStatus.Idle;

   public static DescriptorData DescriptorData = new("-1", "none", [], "-1");

   // History Manager
   public static readonly HistoryManager HistoryManager = new(new CInitial());

   // Contains the current map mode
   public static MapModeManager MapModeManager = null!;

   // Color Provider
   public static readonly ColorProviderRgb ColorProvider = new();

   // Contains the map image bounds and path
   public static int MapWidth;
   public static int MapHeight;
   public static string MapPath = null!;

   // Terrain
   public static List<Terrain> Terrains = [];
   public static TerrainDefinitions TerrainDefinitions = new();
   public static TreeDefinitions TreeDefinitions = new();

   // Rivers
   public static Dictionary<int, Point[]> Rivers = [];

   // Ideas
   public static List<Idea> Ideas = [];

   // Maps the name of TradeGoods to the TradeGood object
   public static readonly Dictionary<string, TradeGood> TradeGoods = [];
   public static readonly Dictionary<string, TradeNode> TradeNodes = [];

   // Culture Groups and Cultures
   public static Dictionary<string, CultureGroup> CultureGroups = [];
   public static Dictionary<string, Culture> Cultures = [];

   // Unit Types and GFX
   public static List<string> GraphicalCultures = [];
   public static HashSet<Unit> Units = [];

   // Contains the provinces and options to access them
   public static HashSet<Province> Provinces = [];
   public static Dictionary<int, Province> ProvinceIdToProvince = [];
   public static Dictionary<int, Province> ColorToProvId = [];
   public static Dictionary<Province, Province[]> AdjacentProvinces = [];
   public static HashSet<Strait> Straits = [];

   // TechnologyGroups
   public static readonly Dictionary<string, TechnologyGroup> TechnologyGroups = [];

   // Religion Stuff
   public static List<ReligiousGroup> ReligionGroups = [];
   public static readonly Dictionary<string, Religion> Religions = [];

   // Revolutionary Colors
   public static readonly Dictionary<int, Color> RevolutionaryColors = [];
   public static readonly HashSet<Color> CustomCountryColors = [];

   // Government Reforms and Government Types
   public static Dictionary<string, Government> GovernmentTypes = [];
   public static Dictionary<string, GovernmentReform> GovernmentReforms = [];

   // Country Groups
   private static Dictionary<Tag, Country> _countries = [];
   public static Dictionary<Tag, Country> Countries
   {
      get => _countries;
      set
      {
         _countries = value;
         GlobalEventHandlers.RaiseCountryListChanged();
      } 
   }

   // Province Groups
   public static HashSet<Province> LandProvinces = [];
   public static HashSet<Province> SeaProvinces = [];
   public static HashSet<Province> LakeProvinces = [];
   public static HashSet<Province> CoastalProvinces = [];
   public static HashSet<Province> NonLandProvinces = [];
   public static Province[] LandProvinceIds = [];
   public static Dictionary<string, ColonialRegion> ColonialRegions = [];

   // Modifiers
   public static Dictionary<string, EventModifier> EventModifiers = [];
   public static Dictionary<string, Modifier> ProvinceTriggeredModifiers = [];
   public static Dictionary<string, Modifier> TriggeredModifiers = [];

   // Trade
   public static Dictionary<string, TradeCompanyInvestment> TradeCompanyInvestments = [];
   public static Dictionary<string, TradeCompany> TradeCompanies = [];

   public static Dictionary<string, ProvinceGroup> ProvinceGroups = [];
   // Ingame Groups
   public static Dictionary<string, Area> Areas = [];
   public static Dictionary<string, Region> Regions { get; set; } = [];

   public static readonly Dictionary<string, SuperRegion> SuperRegions = [];
   public static Dictionary<string, Continent> Continents { get; set; } = [];


   // ------------ Saving ------------ \\ 

   public static SaveableType SaveableType = new();



   // ------------ Localisation ------------ \\
   public static HashSet<LocObject> Localisation = [];


   public static Dictionary<string, Dictionary<string, string>> VanillaLocalisationOld { get; set; } = [];
   public static Dictionary<string, Dictionary<string, string>> ModLocalisationOld { get; set; } = [];
   public static Dictionary<string, Dictionary<string, string>> ReplaceLocalisationOld { get; set; } = [];

   public static HashSet<LocObject> LocalisationCollisions { get; set; } = [];
   
   public static HashSet<string> ScriptedEffectNames {get; set; } = [];
   public static List<Building> Buildings { get; set; } = [];
   public static HashSet<string> BuildingKeys { get; set; }= [];

   // Modifiers
   public static string[] ModifierKeys = new string[1];
   public static Dictionary<int, ModifierValueType> ModifierValueTypes = [];

   // Is used in parsing province files as some keys differ from standard effect keys
   public static readonly HashSet<string> UniqueAttributeKeys = [
      "add_claim", "add_core", "add_local_autonomy", "add_nationalism", "base_manpower", "base_production", "base_tax", "capital", "center_of_trade", "controller", "culture", "discovered_by", "extra_cost", "hre", "city", "native_ferocity", "native_hostileness", "native_size", "owner", "religion", "seat_in_parliament", "trade_goods", "tribal_owner", "unrest", "shipyard", "revolt_risk", "is_city", "reformation_center", "citysize"
   ];

   public static List<string> ToolTippableAttributes = [ 
      "base_manpower", "base_tax", "base_production",  "total_development", "area", "continent", "claims", "cores", "controller", "owner", "tribal_owner", "center_of_trade", "extra_cost", "native_ferocity", "native_hostileness", "native_size", "revolt_risk", ">local_autonomy", "nationalism", "discovered_by", "capital", "culture", "religion", "has_fort_15th", "is_hre", "is_city", "is_seat_in_parliament", "trade_good", "history", "multiline_attributes", "id", "name", "has_revolt", "is_occupied"
   ];

   public static readonly HashSet<string> CountryAttributes = 
   [
      "cannot_form_from_collapse_nation", "right_to_bear_arms", "all_your_core_are_belong_to_us", "random_nation_extra_size", 
   ];
}

// TODO LIST Until Alpha 1.0
// - [x] Add a way to change the language of the application
// - [x] Rework the Rendering of the map to be more efficient and using GDIP32
// - [x] Add zooming to the map 0.5x - 16.0x
// - [x] Add a way to change the map mode via customizable Hotkeys
// - [x] Fix Lasso Selection Preview sometimes being incorrect
// - [X] Make Magic wand tool
// - [x] Add a modifier creation and selection menu
// - [x] Add saving for all Province Collections
// - [x] Add descriptions on how to customize tooltips, map modes, and other things
// - [x] Add basic country editing there is now EXTENSIVE country editing
// - [x] Fix province center calculation
// - [x] Find last concurrency bugs
// - [x] Pre load and layout the Province Collection Editing GUI to prevent LagSpike on first opening said tab
// - [x] MelonCoaster easter Egg in Loading Screen
// - [x] Add a help page that leads to the official Discord
// - [x] Improve selection modifiers and its GUI by adding more and only listing in context valid options
// - [x] Fix the Province Collection Editing gui so that all types work the same and edge cases are handled
// - [x] Localisation editing for provinces and modifiers
// - [x] Check if province is selected by color of the pixels instead of bounds or center
// - [ ] Trade fix   //UI
// - [x] River fixes //UI
// - [x] Province ADJ localization fix
// - [x] Language in quicksettings fix
// - [x] File saving popup for saving map mode
// - [-] Save all and verify all save options
// - [x] 3/2 letters prefix from mods
// - [-] Support deleting in saving
// - [x] Toolstip customizer tooltips and fix column width
// - [x] Logging and Crash Reporter ==> Map mode crashes catchen und reporten and user so that it restarts
// - [x] When searching select the results
// - [x] Settings window / saving
// - [x] Open LoadingScreen and MapWindow on the same screen
// - [x] Discord Rich Presence
// - [-] Country Collection Editing, Saving is broken
// - [x] Terrain Editing and map mode
// - [ ] Province Group parsing


// TODO LIST Until Alpha 1.1
// - [ ] Generalize Loading
// - [ ] Add Prefix ignoring in search boxes
// - [ ] Straits editing and creation
// - [ ] PDX language support
// - [ ] Item scaling on Graphics (Trade arrows, straits, capitals, text)
// - [ ] FPS count for map rendering
// - [ ] Add a way to create custom map modes
// - [ ] Add a tradegoods creation and editing menu
// - [ ] Ideas making via drag and drop
// - [ ] Province Creation and editing
// - [ ] File syncing

// TODO LIST Until Alpha 1.2
// - [ ] Fix tooltip preventing MouseWheel event
// Resync files with project
