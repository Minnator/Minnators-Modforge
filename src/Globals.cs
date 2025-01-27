using System.ComponentModel;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Settings;
using Editor.Forms;
using Editor.Helper;
using Editor.Loading;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor;

#region enums

public enum Eu4CursorTypes
{
   Loading,
   Normal,
   Select,
}

public enum CommandHistoryType
{
   ComplexSelection,
   Action,
   Compacting
}

public enum State
{
   Loading,
   Running,
}

public enum Mana
{
   NONE = -1,
   ADM,
   DIP,
   MIL,
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

#endregion

//contains all required and used data across the application and instances of forms.
public static class Globals
{
   static Globals()
   {
#if DEBUG
      if (!Directory.Exists(DebugPath))
         Directory.CreateDirectory(DebugPath);
#endif
   }

   public const string TOOL_NAME = "Minnator's Modforge";

   public const string DISCORD_INVITATION_LINK = "https://discord.gg/22AhD5qkme";
   public const string GITHUB_LINK = "https://github.com/Minnator/Editor.git";

   public static string VanillaPath = string.Empty;
   public static string ModPath = string.Empty;
   public static readonly string AppDirectory = AppDomain.CurrentDomain.BaseDirectory;

#if DEBUG
   public static readonly string DebugPath = Path.Combine(AppDirectory, "Debug");
#endif

   #region LoadingScreen
   public static int LoadingStages = 0;
   #endregion
   
   public static Random Random = null!;

   public static MapWindow MapWindow = null!;
   public static ZoomControl ZoomControl = null!;
   public static int EquatorY = 0;

   public static int MaxGovRank = 3;

   // SETTINGS
   public static Settings Settings = new();

   // Logs
   //public static readonly string DownloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
   public static readonly Log LoadingLog = new(Settings.Saving.LogLocation, "Loading");
   public static readonly Log ErrorLog = new(Settings.Saving.LogLocation, "Error");

   // Contains the current state of the application
   public static State State = State.Loading;

   public static DescriptorData DescriptorData = new("-1", "none", [], [], [], "-1");

   // Color Provider
   public static readonly ColorProviderRgb ColorProvider = new();

   // Contains the map image bounds and path
   public static int MapWidth;
   public static int MapHeight;
   public static string MapPath = null!;

   // Terrain
   public static BindingDictionary<string, Terrain> Terrains = new(new(string.Empty, Terrain.Empty));
   public static TerrainDefinitions TerrainDefinitions = new();
   public static TreeDefinitions TreeDefinitions = new();

   // Rivers
   public static Dictionary<int, Point[]> Rivers = [];

   // Ideas
   public static List<Idea> Ideas = [];

   // Maps the name of TradeGoods to the TradeGood object
   public static readonly BindingDictionary<string, TradeGood> TradeGoods = new(new (string.Empty, TradeGood.Empty));
   public static readonly Dictionary<string, TradeNode> TradeNodes = [];

   // Culture Groups and Cultures
   public static Dictionary<string, CultureGroup> CultureGroups = [];
   public static BindingDictionary<string, Culture> Cultures = new (new (string.Empty, Culture.Empty));

   // Unit Types and GFX
   public static BindingList<string> GraphicalCultures = [];
   public static BindingDictionary<string, Unit> Units = new(new(string.Empty, Unit.Empty));

   // Bookmarks
   public static List<Bookmark> Bookmarks = [];

   // Contains the provinces and options to access them
   public static HashSet<Province> Provinces = [];
   public static Dictionary<int, Province> ProvinceIdToProvince = [];
   public static Dictionary<int, Province> ColorToProvId = [];
   public static Dictionary<Province, Province[]> AdjacentProvinces = [];
   public static HashSet<Strait> Straits = [];

   // TechnologyGroups
   public static readonly BindingDictionary<string, TechnologyGroup> TechnologyGroups = new(new(string.Empty, TechnologyGroup.Empty));

   // Religion Stuff
   public static List<ReligiousGroup> ReligionGroups = [];
   public static readonly BindingDictionary<string, Religion> Religions = new (new (string.Empty, Religion.Empty));

   // Revolutionary Colors
   public static readonly Dictionary<int, Color> RevolutionaryColors = [];
   public static readonly HashSet<Color> CustomCountryColors = [];

   // Government Reforms and Government Types
   public static BindingDictionary<string, Government> GovernmentTypes = new(new(string.Empty, Government.Empty));
   public static Dictionary<string, GovernmentReform> GovernmentReforms = [];

   // Country Groups
   public static BindingDictionary<Tag, Country> Countries = new(new(Tag.Empty, Country.Empty));

   // Province Groups
   public static HashSet<Province> LandProvinces = [];
   public static HashSet<Province> SeaProvinces = [];
   public static HashSet<Province> LakeProvinces = [];
   public static HashSet<Province> RNWProvinces = [];
   public static HashSet<Province> CoastalProvinces = [];
   public static HashSet<Province> NonLandProvinces = [];
   public static HashSet<Province> Impassable = [];
   public static Dictionary<string, ColonialRegion> ColonialRegions = [];

   // Modifiers
   public static Dictionary<string, EventModifier> EventModifiers = [];

   // Climate
   public static Dictionary<string, Climate> Climates = [];
   public static Dictionary<string, Climate> Weather = [];

   // Trade
   public static Dictionary<string, TradeCompanyInvestment> TradeCompanyInvestments = [];
   public static Dictionary<string, TradeCompany> TradeCompanies = [];

   public static Dictionary<string, ProvinceGroup> ProvinceGroups = [];
   // Ingame Groups
   public static Dictionary<string, Area> Areas = [];
   public static Dictionary<string, Region> Regions { get; set; } = [];

   public static Dictionary<string, SuperRegion> SuperRegions = [];
   public static Dictionary<string, Continent> Continents { get; set; } = [];


   // ------------ Saving ------------ \\ 

   /// <summary>
   /// This sets the Astrix on the MapWindow Text if the file is not saved
   /// </summary>
   public static SaveableType SaveableType
   {
      get => _saveableType;
      set
      {
         if (value == 0)
         {
            if (MapWindow.Text.EndsWith('*'))
               MapWindow.Text = MapWindow.Text[..^1];
         }
         else if(_saveableType == 0)
            MapWindow.Text += "*";
         
         _saveableType = value;
      }
   }


   // ------------ Localisation ------------ \\
   public static HashSet<LocObject> Localisation = [];
   public static Dictionary<Province, CultProvLocContainer> CustomProvinceNames = [];
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

   private static SaveableType _saveableType = 0;
}

#region ALPHA 1.0

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
// - [x] River fixes //UI
// - [x] Province ADJ localization fix
// - [x] Language in quick settings fix
// - [x] File saving popup for saving map mode
// - [x] 3/2 letters prefix from mods
// - [x] Support deleting in saving
// - [x] ToolStrip customizer tooltips and fix column width
// - [x] Logging and Crash Reporter ==> Map mode crashes catchen und reporten and user so that it restarts
// - [x] When searching select the results
// - [x] Settings window / saving
// - [x] Open LoadingScreen and MapWindow on the same screen
// - [x] Discord Rich Presence
// - [x] Terrain Editing and map mode
// - [x] Province Group parsing
// - [x] Fix Remove from province collection
// - [x] Fix Setting Saveable Type (Dictionary for each type) to prevent from setting it to 0 if there are other items still of that type
// - [x] province groups map mode and force file name
// - [x] empty culture and religion in colonial regions
// - [x] Fix Trade Node Saving
// - [x] Trade company saving
// - [x] Fix Settings not saving when modified
// - [x] Button to add a new Country
// - [x] Warning that it may corrupt files if there are error
// - [x] Save all and verify all save options
// - [x] Country Collection Editing
// - [x] Fix province saving
// - [x] Verify that all province modifications set the ObjEditingStatus
// - [x] Country saving broken
// - [x] Country Tags editing

// - [x] Fix date control
// - [x] Fix Empties in TagBoxes
// - [x] Fix mass editing loc
// - [x] Fix Unneeded quotes on capital
// - [x] Fix stupid text boxes --> Don't save empty changes
// - [x] Fix Localisation behaving weird
// - [x] Fix government reforms not saveable
// - [x] Save file at popup in center of screen
// - [x] Fix Saving state getting lost on multiple undos after saving
// - [x] Test Remove Command for countries
// - [x] Added Debbuging interface for edited objects
// - [x] Fix Saving state getting lost on undo on collections
// - [x] Fix Discord SDK crash
// - [x] Fix Custom cursors not working and changing back to normal cursor
// - [x] Adding metric for startup (Currently in settings will be separate later)
// -     Collection Editor:
//       - [x] Remove all buttons (with an empty to select?)
//       - [x] TryAdd instead of add
// - [x] Change all Saveable commands to NOT use SaveableHelper
// - [x] Fix Error sound playing on ReDo
// - [x] Fix Revolutionary Color Command
// - [x] Fix Command spamming in Natives interface
// - [x] Fix Names Interface not being saved
// - [x] Fix Capital box not saving on enter
// - [x] Disable country GUI when selecting several countries or invalid after having selected a valid
// - [x] Fix folder creation in root directory when folder is missing in mod
// - [x] Fix log location and remove all dumps to download
// - [x] Expose Setting to disable Discord Integration to settings menu
// - [x] Added Support for Replace_Path

#endregion

// Update Alpha 1.0.1
// - [x] Add Explorable Error log which provides a possible solution, and a detailed cause
// - [x] Generalized Window Handling
// - [x] Improved MapMode management
// - [x] Streamlined History management

// Update Alpha 1.0.2
// - [x] Fix all combo-boxes taking the input from the suggestion without the use ever using it (when both are empty)
// - [ ] Demonstration Video
// - [x] Fix Going back and forth in history causing provinces being in illegal states
// - [x] Added Icons and improved Custom toolbar
// - [x] Added Bookmark Support and selection in GUI
// - [x] Added Parsing of climate.txt
// - [x] Added Weather map mode
// - [x] Added Climate map mode
// - [ ] Allow deselecting using modifier key
// - [x] Added Search Engine to the ErrorBrowsing
// - [x] Added a setting how unsaved changes should be handled when closing the application
// - [x] Added Context Menu to open file/folder of files in ErrorBrowsing
// - [x] Added Support for DynamicProvinceNames
// - [x] Fixed CTD after closing SelectionDrawerInterface
// - [x] Added a nameGenerator which generates random names for provinces, monarchs and anything.
//     - [x] Allowed the NameGenerator to sample provinces/countries/monarchs from the selection to generate context fitting names
// - [x] Added Climate and Weather map modes

// TODO LIST Until Alpha 1.1
// - [x] Add history compacting
// - [x] Block Water provinces; should not be viable for countries
// - [ ] Loosing hover when selecting and deselecting a province
// - [███████████▒] Smart Gui reloading
// - [x] Rewrite Province.cs, Province Command and Gui
// - [-] Sounds for buttons and actions
// - [ ] UnitType loading optimization
// - [ ] Loading bar when saving using threads
// - [ ] In Depth deleting with pop up to show what objects are related to the one being deleted
// - [ ] Radial Menu for Map Modes
// - [ ] Trade fix   //UI
// - [ ] Implement Shaders for the map
// - [ ] Add Prefix ignoring in search boxes
// - [ ] Straits editing and creation
// - [██▒▒▒▒▒▒▒▒▒▒] Generalize Loading
// - [█▒▒▒▒▒▒▒▒▒▒▒] PDX language support
// - [ ] Item scaling on Graphics (Trade arrows, straits, capitals, text)
// - [ ] FPS count for map rendering
// - [ ] Add a way to create custom map modes
// - [-] Add a tradegoods creation and editing menu
// - [ ] Ideas making via drag and drop
// - [ ] Province Creation and editing
// - [ ] File syncing/hotreloading
// - [ ] Heat map for history entries in editor for them when selecting dates?
// - [x] Fix Country Color not live updating
// - [x] Fix Search not finding country tags

// - [ ] Grid for pixels
// - [ ] Tools:
      // - [ ] Pen
      // - [ ] Eraser
      // - [ ] Bucket
      // - [ ] Select/Move
      // - [ ] Magic Wand
// - [ ] Overlays (several)

// Next Up in what shall happen:
// - [x] Update Localisation Implementation
// - [x] Create List command
// - [x] Improve and create List GUI interface / elements
// - [x] Compacting command
// - [ ] Finish Loading overhaul
// - [ ] Fix border drawing



// Create command which takes an action which updates the map mode and the UI elements
// Implement Method to determine whether a control is physically visible

// TODO LIST Until Alpha 1.2
// - [ ] Fix tooltip preventing MouseWheel event
// - [ ] Resync files with project
