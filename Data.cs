using System.Drawing;
using Editor.DataClasses;

namespace Editor;

//contains all required and used data across the application and instances of forms.
public static class Data
{
   public static Point[][] Borders = null!;
   public static Point[][] ProvincesPixels = null!;

   public static Province[] Provinces = null!;
}