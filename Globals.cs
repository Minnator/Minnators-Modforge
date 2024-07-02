using System.Collections.Generic;
using System.Drawing;
using Editor.Commands;
using Editor.DataClasses;
using Region = Editor.DataClasses.Region;

namespace Editor;

public enum HistoryType
{
   SimpleSelection,
   ComplexSelection,
   Action
}

public enum MapMode
{
   Provinces = 1, //default
   Areas = 2,
   Regions = 3,
   SuperRegions = 4,
   Continents = 5,
}

public enum State
{
   Loading,
   Running,
   Initializing,
   Waiting
}

//contains all required and used data across the application and instances of forms.
public static class Globals
{
   // Contains the current state of the application
   public static State State = State.Loading;

   // History Manager
   public static readonly HistoryManager HistoryManager = new(new CInitial());

   // Contains the current map mode
   public static MapModeManager MapModeManager = null!;

   // Contains the map image bounds and path
   public static int MapWidth;
   public static int MapHeight;
   public static string MapPath = null!;

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
   // In Game Groups
   public static Dictionary<string, Area> Areas = null!;
   public static Dictionary<string, Region> Regions { get; set; } = [];
   public static Dictionary<string, SuperRegion> SuperRegions { get; set; } = [];
   public static Dictionary<string, Continent> Continents { get; set; } = [];

   // Localisation
   public static Dictionary<string, string> Localisation { get; set; } = [];
   public static Dictionary<string, string> LocalisationCollisions { get; set; } = [];

}