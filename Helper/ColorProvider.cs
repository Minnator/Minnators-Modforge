using System;
using System.Collections.Generic;
using System.Drawing;

namespace Editor.Helper;

public class ColorProviderRgb(int seed = 1444)
{
   private readonly HashSet<Color> _usedColors = [];
   private readonly Random _rand = new(seed);

   public Color GetRandomColor()
   {
      var color = Color.FromArgb(_rand.Next(256), _rand.Next(256), _rand.Next(256));
      while (_usedColors.Contains(color))
         color = Color.FromArgb(_rand.Next(256), _rand.Next(256), _rand.Next(256));
      _usedColors.Add(color);
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
}