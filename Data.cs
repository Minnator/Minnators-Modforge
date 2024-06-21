using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using Editor.DataClasses;

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
   // ProvPtr is the index of the province in the Provinces array
   public static Province[] Provinces = null!;
   public static Dictionary<Color, int> ColorToProvPtr = null!;
   public static Dictionary<int, int[]> AdjacentProvinces = null!;
   
   // returns the province by its color
   public static Province GetProvince(Color color) => Provinces[ColorToProvPtr[color]];

   public static List<Province> GetAllProvincesInRectangle(Rectangle rect)
   {
      var provinces = new List<Province>();
      foreach (var province in Provinces)
      {
         if (province.Bounds.IntersectsWith(rect))
            provinces.Add(province);
      }
      return provinces;
   }
}