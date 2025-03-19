using System.Diagnostics;

namespace Editor.Helper;

public class ColorProviderRgb(int seed = 1444)
{
   private readonly HashSet<Color> _usedColors = [];
   private readonly Random _rand = new(seed);

   public static readonly Color[] PredefinedColors =
   [
      Color.FromArgb(255, 128, 128),  // Light Red
      Color.FromArgb(128, 128, 255),  // Light Blue
      Color.FromArgb(128, 255, 128),  // Light Green
      Color.FromArgb(255, 165, 0),    // Orange
      Color.FromArgb(128, 0, 128),    // Purple
      Color.FromArgb(0, 255, 255),    // Cyan
      Color.FromArgb(255, 0, 255),    // Magenta
      Color.FromArgb(255, 255, 0),    // Yellow
      Color.FromArgb(255, 182, 193),  // Light Pink
      Color.FromArgb(0, 128, 0),      // Dark Green
      Color.FromArgb(75, 0, 130),     // Indigo
      Color.FromArgb(139, 69, 19),    // Brown
      Color.FromArgb(0, 128, 128),    // Teal
      Color.FromArgb(128, 128, 0),    // Olive
      Color.FromArgb(169, 169, 169),  // Dark Gray
      Color.FromArgb(238, 130, 238),  // Violet
      Color.FromArgb(64, 224, 208),   // Turquoise
      Color.FromArgb(255, 215, 0),    // Gold
      Color.FromArgb(192, 192, 192),  // Silver
      Color.FromArgb(255, 99, 71)     // Coral
   ];

   
   public static readonly Color[] MatLabPlasmaColors =
   [
      //Color.FromArgb( (int)(0.05038205347059877 * 255), (int)(0.029801736499741757 * 255), (int)(0.5279751010495176 * 255) ),
      Color.FromArgb( (int)(0.5453608398097519 * 255), (int)(0.03836817688235455 * 255), (int)(0.6472432548304646 * 255) ),
      Color.FromArgb( (int)(0.7246542772727967 * 255), (int)(0.1974236709187686 * 255), (int)(0.5379281037132716 * 255) ),
      Color.FromArgb( (int)(0.8588363515132411 * 255), (int)(0.35929521887338184 * 255), (int)(0.407891799954962 * 255) ),
      Color.FromArgb( (int)(0.9557564842476064 * 255), (int)(0.5338287173328614 * 255), (int)(0.2850080723374925 * 255) ),
      Color.FromArgb( (int)(0.9945257260387773 * 255), (int)(0.7382691276441445 * 255), (int)(0.16745985897148677 * 255) ),
      Color.FromArgb( (int)(0.9400151278782742 * 255), (int)(0.9751557856205376 * 255), (int)(0.131325887773911 * 255) )
   ];
   public Color GetRandomColor()
   {
      var color = Color.FromArgb(_rand.Next(256), _rand.Next(256), _rand.Next(256));
      lock (_usedColors)
      {
         while (_usedColors.Contains(color))
            color = Color.FromArgb(_rand.Next(256), _rand.Next(256), _rand.Next(256));
         _usedColors.Add(color);
      }
      return color;
   }

   public void Reset()
   {
      _usedColors.Clear();
   }

   public Color GetBlueColor()
   {
      // Returns a color with a blue hue
      var color = Color.FromArgb(_rand.Next(256), _rand.Next(256), 255);
      while (_usedColors.Contains(color))
         color = Color.FromArgb(_rand.Next(256), _rand.Next(256), 255);
      _usedColors.Add(color);
      return color;
   }

   public Color GetColorOnGreenRedShade(int min, int max, int current)
   {
      // Normalize the current value to a range between 0 and 1
      var normalized = (float)(current - min) / (max - min);

      // Interpolate between red and green
      var red = (int)(255 * (1 - normalized));
      var green = (int)(255 * normalized);
      
      if (red > 255)
         red = 255;
      if (green > 255)
         green = 255;
      if (red < 0)
         red = 0;
      if (green < 0)
         green = 0;

      return Color.FromArgb(red, green, 0);
   }

   public Color GetColorOnGreenRedShade(int min, int max, float current)
   {
      // Normalize the current value to a range between 0 and 1
      var normalized = (current - min) / (max - min);

      // Interpolate between red and green
      var red = (int)(255 * (1 - normalized));
      var green = (int)(255 * normalized);
      
      if (red > 255)
         red = 255;
      if (green > 255)
         green = 255;
      if (red < 0)
         red = 0;
      if (green < 0)
         green = 0;

      return Color.FromArgb(red, green, 0);
   }

   public static Color[] SampleColorScale(int sampleCount, Color color1, Color color2)
   {
      int rMin = color2.R;
      int gMin = color2.G;
      int bMin = color2.B;

      var colorList = new Color [sampleCount];
      for (var i = 0; i < sampleCount; i++)
      {
         var rAverage = rMin + (color1.R - rMin) * i / sampleCount;
         var gAverage = gMin + (color1.G - gMin) * i / sampleCount;
         var bAverage = bMin + (color1.B - bMin) * i / sampleCount;
         colorList[i] = Color.FromArgb(rAverage, gAverage, bAverage);
      }
      return colorList;
   }

   public static Color[] SampleColorScale(int sampleCount, params Color[] colors)
   {
      if (colors == null || colors.Length < 2)
         throw new ArgumentException("At least two colors are required.", nameof(colors));

      var colorList = new Color[sampleCount];
      var segments = colors.Length - 1; // Number of color transitions
      var step = (float)sampleCount / segments;

      for (var i = 0; i < sampleCount; i++)
      {
         var t = i / step;
         var index = (int)t;
         var blendFactor = t - index;

         if (index >= segments)
         {
            colorList[i] = colors[^1];
            continue;
         }

         var c1 = colors[index];
         var c2 = colors[index + 1];

         var r = (int)(c1.R + (c2.R - c1.R) * blendFactor);
         var g = (int)(c1.G + (c2.G - c1.G) * blendFactor);
         var b = (int)(c1.B + (c2.B - c1.B) * blendFactor);

         colorList[i] = Color.FromArgb(r, g, b);
      }
      return colorList;
   }



   #region Good Color Sacles

   public static Color[] GetPlasmaScale(int sampleCount)
   {
      return SampleColorScale(sampleCount, MatLabPlasmaColors);
   }

   public static Color[] GetGreenBlueScale(int sampleCount)
   {
      var color1 = ColorTranslator.FromHtml("#A5CC82");
      var color2 = ColorTranslator.FromHtml("#00467F");

      return SampleColorScale(sampleCount, color1, color2);
   }

   #endregion

   public static Color NormalizeColorToScale(int min, int max, int current, Color[] colors, bool defaultLow = true)
   {
      Debug.Assert(colors.Length > 1, "colors.Length > 1");

      if (min == max)
         if (defaultLow)
            return colors[0];
         else
            return colors[^1];

      // Normalize the current value to a range between 0 and 1
      var normalized = (float)(current - min) / (max - min);

      // Interpolate between the colors
      var colorIndex = (int)Math.Round((normalized * (colors.Length - 1)));
      return colors[colorIndex];
   }


}