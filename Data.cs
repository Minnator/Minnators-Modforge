using System.Collections.Generic;
using System.Drawing;
using Editor.DataClasses;

namespace Editor;

//contains all required and used data across the application and instances of forms.
public static class Data
{
   public static Point[] BorderPixels = null!;
   public static Point[] Pixels = null!;

   public static Province[] Provinces = null!;
   public static Dictionary<Color, int> ColorToProvPtr = null!;

   public static Province GetProvince(Color color)
   {
      return Provinces[ColorToProvPtr[color]];
   }
}