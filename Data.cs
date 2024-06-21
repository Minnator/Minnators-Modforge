﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using Editor.DataClasses;
using Region = Editor.DataClasses.Region;

namespace Editor;

//contains all required and used data across the application and instances of forms.
public static class Data
{
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

   // Province Groups
   public static HashSet<int> LandProvinces = null!;
   public static HashSet<int> SeaProvinces = null!;
   public static HashSet<int> LakeProvinces = null!;
   public static HashSet<int> CoastalProvinces = null!;
   // In Game Groups
   public static Dictionary<string, Area> Areas = null!;
   public static Dictionary<string, Region> Regions { get; set; }
   public static Dictionary<string, SuperRegion> SuperRegions { get; set; }
   public static Dictionary<string, Continent> Continents { get; set; }


   // returns the province by its color
   public static Province GetProvince(Color color) => Provinces[ColorToProvId[color]];

}