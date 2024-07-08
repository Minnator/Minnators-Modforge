using System;
using System.Collections.Generic;
using System.Drawing;
using Editor.Helper;
using Editor.Loading;

namespace Editor.MapModes;

public sealed class RegionsMapMode : Interfaces.MapMode
{
   public RegionsMapMode()
   {

   }

   public override string GetMapModeName()
   {
      return "Regions";
   }

   public override Color GetProvinceColor(int id)
   {
      if (Globals.Provinces.TryGetValue(id, out var province))
         if (Globals.Areas.TryGetValue(province.Area, out var areas))
            if (Globals.Regions.TryGetValue(areas.Region, out var region))
               return region.Color;
      return Color.DarkGray;
   }
}