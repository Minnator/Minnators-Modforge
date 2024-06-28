using System;
using System.Collections.Generic;
using System.Drawing;
using Editor.Helper;
using Editor.Loading;

namespace Editor.MapModes;

public sealed class ContinentMapMode : Interfaces.MapMode
{
   public ContinentMapMode()
   {
      RenderMapMode(GetProvinceColor);
   }

   public override string GetMapModeName()
   {
      return "Continents";
   }

   public override Color GetProvinceColor(int id)
   {
      if (Globals.Provinces.TryGetValue(id, out var province))
         if (Globals.Continents.TryGetValue(province.Continent, out var continent))
            return continent.Color;
      return Color.DarkGray;
   }

}