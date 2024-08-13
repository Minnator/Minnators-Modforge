using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Policy;
using Editor.Commands;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Settings;
using Editor.Forms;
using Editor.Helper;
using static Editor.Helper.ProvinceEventHandler;
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


public enum MapModeRendering
{
   Live,
   LiveBackground,
   Cached,
}

public enum EditingStatus
{
   LoadingInterface,
   Idle,
   Saving,
   Interrupted
}

#endregion

//contains all required and used data across the application and instances of forms.
public static class Globals {
   #region LoadingScreen
   public static event EventHandler<int> LoadingStageChanged = delegate { };

   public static int LoadingStage
   {
      get => _loadingStage;
      set
      {
         _loadingStage = value;
         LoadingStageChanged?.Invoke(null, _loadingStage);
      }
   }
   private static int _loadingStage = 0;
   public static int LoadingStages = 19;
   public static string LoadingAnimationGif = "C:\\Users\\david\\Downloads\\test.gif";
   #endregion
   
   public static ConsoleForm? ConsoleForm = null;
   public static Search? SearchForm = null;
   public static MapWindow MapWindow = null!;

   // SETTINGS
   public static readonly Settings Settings = new();
   public static ProvinceEditingStatus ProvinceEditingStatus = ProvinceEditingStatus.PreviewUntilSelection;

   // SELECTION
   public static Selection Selection = null!;

   // Date of history
   public static DateTime Date
   {
      get
      {
         return MapWindow.DateControl.Date;
      }
   }
   // Logs
   public static readonly Log LoadingLog = new(@"C:\Users\david\Downloads", "Loading"); //TODO: make this a setting and not hardcoded
   public static readonly Log ErrorLog = new(@"C:\Users\david\Downloads", "Error"); //TODO: make this a setting and not hardcoded

   // Contains the current state of the application
   public static State State = State.Loading;
   public static EditingStatus EditingStatus = EditingStatus.Idle;
   public static MapModeRendering MapModeRendering { get; set; } = MapModeRendering.Live;

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

   // ToolTip
   public static string ToolTipText = $"$MAPMODE_SPECIFIC$\n------------------\nId:   $id$\nName: $name$\nArea: $area$ ($area%L$)\nRevolt: $revolt$\nIsOccupied: $is_occupied$";

   // Contains the border pixels of the provinces
   public static Point[] BorderPixels = [];
   public static Point[] Pixels = [];

   // Maps the name of TradeGoods to the TradeGood object
   public static Dictionary<string, TradeGood> TradeGoods = [];
   public static Dictionary<string, TradeNode> TradeNodes = [];

   // Culture Groups and Cultures
   public static Dictionary<string, CultureGroup> CultureGroups = [];
   public static Dictionary<string, Culture> Cultures = [];

   // Contains the provinces and options to access them
   public static Dictionary<int, Province> Provinces = [];
   public static Dictionary<Color, int> ColorToProvId = [];
   public static Dictionary<int, int[]> AdjacentProvinces = [];
   public static HashSet<int> Capitals = [];

   // TechnologyGroups
   public static HashSet<string> TechnologyGroups = [];

   // Religion Stuff
   public static List<ReligiousGroup> ReligionGroups = [];
   public static Dictionary<string, Religion> Religions = [];

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
   public static HashSet<int> LandProvinces = null!;
   public static HashSet<int> SeaProvinces = null!;
   public static HashSet<int> LakeProvinces = null!;
   public static HashSet<int> CoastalProvinces = null!;
   public static int[] NonLandProvinceIds = null!;
   public static int[] LandProvinceIds = null!;
   // In Game Groups
   public static Dictionary<string, Area> Areas = null!;
   public static Dictionary<string, Region> Regions { get; set; } = [];
   public static Dictionary<string, SuperRegion> SuperRegions { get; set; } = [];
   public static Dictionary<string, Continent> Continents { get; set; } = [];

   // Localisation
   public static Dictionary<string, string> Localisation { get; set; } = [];
   public static Dictionary<string, string> LocalisationCollisions { get; set; } = [];


   public static List<Building> Buildings { get; set; }= [];
   public static HashSet<string> BuildingKeys { get; set; }= [];

   public static readonly HashSet<string> UniqueAttributeKeys = [
      "add_claim", "add_core", "add_local_autonomy", "add_nationalism", "base_manpower", "base_production", "base_tax", "capital", "center_of_trade", "controller", "culture", "discovered_by", "extra_cost", "fort_15th", "hre", "is_city", "native_ferocity", "native_hostileness", "native_size", "owner", "religion", "seat_in_parliament", "trade_goods", "tribal_owner", "unrest", "shipyard", "revolt_risk"
   ];

   public static readonly HashSet<string> Scopes = [
      "every_province", "every_owned_province"
   ];

   public static readonly HashSet<string> Effects = [
      "change_price", "add_opinion", "add_estate_loyalty", "change_estate_land_share", "add_ruler_modifier", "add_country_modifier", "add_base_tax", "add_base_production", "add_base_manpower", "add_local_autonomy", "remove_core", "revolt", "remove_claim", "reformation_center", "add_province_modifier", "remove_province_modifier", "add_to_trade_company", "add_trade_company_investment", "add_permanent_province_modifier", "remove_permanent_province_modifier", "add_province_triggered_modifier", "remove_province_triggered_modifier", "textile", "set_global_flag"
   ];

   public static readonly HashSet<string> ScriptedEffects = [
      "change_jains_or_burghers_land_share_effect", "change_rajputs_or_marathas_or_nobles_land_share_effect", "change_brahmins_or_church_land_share_effect", "change_rajputs_or_marathas_or_nobles_land_share_effect", 
   ];// TODo read in from file

   public static readonly HashSet<string> Triggers = [
      
   ];

   public static readonly HashSet<string> Conditions = [
      "if", "else", "else_if", "limit"
   ];

   public static List<string> ToolTippableAttributes = [ 
      "base_manpower", "base_tax", "base_production",  "total_development", "area", "continent", "claims", "cores", "controller", "owner", "tribal_owner", "center_of_trade", "extra_cost", "native_ferocity", "native_hostileness", "native_size", "revolt_risk", ">local_autonomy", "nationalism", "discovered_by", "capital", "culture", "religion", "has_fort_15th", "is_hre", "is_city", "is_seat_in_parliament", "trade_good", "history", "multiline_attributes", "id", "name", "has_revolt", "is_occupied"
   ];

   public static List<string> SelectionModifiers = [
      "Deselection"
   ];

}