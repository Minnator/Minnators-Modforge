using System;
using System.Collections.Generic;
using System.Drawing;
using Editor.DataClasses.MapModes;
using Editor.Events;
using Editor.Helper;
using Editor.Loading;

namespace Editor.MapModes;

public sealed class ContinentMapMode : MapMode
{
   public ContinentMapMode()
   {
      ProvinceEventHandler.OnProvinceContinentChanged += UpdateProvince;
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

   public override string GetSpecificToolTip(int provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         if (Globals.Continents.TryGetValue(province.Continent, out var continent))
            return $"Continent: {continent.Name} ({Localisation.GetLoc(continent.Name)})";
      return "Continent: [Unknown]";
   }
}