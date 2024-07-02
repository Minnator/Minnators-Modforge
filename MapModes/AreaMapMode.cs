using System;
using System.Collections.Generic;
using System.Drawing;
using Editor.Helper;
using Editor.Loading;

namespace Editor.MapModes;

public sealed class AreaMapMode : Interfaces.MapMode
{
   public AreaMapMode()
   {

   }

   public override string GetMapModeName()
   {
      return "Areas";
   }

   public override Color GetProvinceColor(int id)
   {
      if (Globals.Provinces.TryGetValue(id, out var province))
         if (Globals.Areas.TryGetValue(province.Area, out var area))
            return area.Color;
      return Color.DarkGray;
   }
}