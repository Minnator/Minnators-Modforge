using System.Collections.Generic;
using System.Drawing;
using Editor.Commands;
using Editor.DataClasses;
using Editor.Forms.AdvancedSelections;
using Editor.Helper;
using Region = Editor.DataClasses.Region;

namespace Editor;

public enum HistoryType
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
   Waiting
}

public enum MapModeRendering
{
   Live,
   LiveBackground,
   Cached,
}

//contains all required and used data across the application and instances of forms.
public static class Globals
{
   public static MapWindow MapWindow = null!;

   // Contains the current state of the application
   public static State State = State.Loading;
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
   public static string ToolTipText = $"Id:   [$id$]\nName: [$name$]\nArea: [$area$]";

   // Contains the border pixels of the provinces
   public static Point[] BorderPixels = null!;
   public static Point[] Pixels = null!;

   // Contains the provinces and options to access them
   public static Dictionary<int, Province> Provinces = null!;
   public static Dictionary<Color, int> ColorToProvId = null!;
   public static Dictionary<int, int[]> AdjacentProvinces = null!;

   // Country Groups
   public static Dictionary<Tag, Country> Countries = null!;

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


   public static readonly HashSet<string> UniqueAttributeKeys = [
      "add_claim", "add_core", "add_local_autonomy", "add_nationalism", "base_manpower", "base_production", "base_tax", "capital", "center_of_trade", "controller", "culture", "discovered_by", "extra_cost", "fort_15th", "hre", "is_city", "native_ferocity", "native_hostileness", "native_size", "owner", "religion", "seat_in_parliament", "trade_goods", "tribal_owner", "unrest", "shipyard", "revolt_risk"
   ];

   public static List<string> ToolTippableAttributes = [ 
      "base_manpower", "base_tax", "base_production",  "total_development", "area", "continent", "claims", "cores", "controller", "owner", "tribal_owner", "center_of_trade", "extra_cost", "native_ferocity", "native_hostileness", "native_size", "revolt_risk", "local_autonomy", "nationalism", "discovered_by", "capital", "culture", "religion", "has_fort_15th", "is_hre", "is_city", "is_seat_in_parliament", "trade_good", "history", "multiline_attributes", "id", "name"
   ];

   public static List<string> SelectionModifiers = [
      "Deselection"
   ];
}