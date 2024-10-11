using System;
using System.Collections.Generic;
using System.Drawing;
using Editor.DataClasses.MapModes;
using Editor.Helper;
using Editor.Loading;

namespace Editor.MapModes;

public sealed class ProvinceMapMode : MapMode
{
   public ProvinceMapMode()
   {

   }

   public override string GetMapModeName()
   {
      return "Provinces";
   }

   public override int GetProvinceColor(int provinceId)
   {
      return Globals.Provinces[provinceId].Color.ToArgb();
   }

   public override string GetSpecificToolTip(int provinceId)
   {
      var province = Globals.Provinces[provinceId];
      return $"Province: {province.Id} ({province.GetLocalisation()})";
   }
}