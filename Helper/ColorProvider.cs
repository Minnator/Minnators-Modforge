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
}