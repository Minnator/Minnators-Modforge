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

   #region Good Color Sacles

   public static Color[] GetGreenBlueScale(int sampleCount)
   {
      var color1 = ColorTranslator.FromHtml("#A5CC82");
      var color2 = ColorTranslator.FromHtml("#00467F");

      return SampleColorScale(sampleCount, color1, color2);
   }

   #endregion

   public static Color NormalizeColorToScale(int min, int max, int current, Color[] colors)
   {
      if (min == max)
         return colors[0];

      // Normalize the current value to a range between 0 and 1
      var normalized = (float)(current - min) / (max - min);

      // Interpolate between the colors
      var colorIndex = (int)Math.Round((normalized * (colors.Length - 1)));
      return colors[colorIndex];
   }

}