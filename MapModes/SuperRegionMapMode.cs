using System;
using System.Collections.Generic;
using System.Drawing;
using Editor.Helper;
using Editor.Loading;

namespace Editor.MapModes;

public sealed class SuperRegionMapMode : Interfaces.MapMode
{
   public SuperRegionMapMode()
   {
      RenderMapMode(GetProvinceColor);
   }


   public override string GetMapModeName()
   {
      return "Super Regions";
   }

   public override Color GetProvinceColor(int id)
   {
      if (Globals.Provinces.TryGetValue(id, out var province))
         if (Globals.Areas.TryGetValue(province.Area, out var areas))
            if (Globals.Regions.TryGetValue(areas.Region, out var region))
               if (Globals.SuperRegions.TryGetValue(region.SuperRegion, out var superRegion))
                  return superRegion.Color;
      return Color.DarkGray;
   }
}