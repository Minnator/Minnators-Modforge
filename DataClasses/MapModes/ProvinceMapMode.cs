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

   public override Color GetProvinceColor(int provinceId)
   {
      return Globals.Provinces[provinceId].Color;
   }

   public override string GetSpecificToolTip(int provinceId)
   {
      return Globals.Provinces[provinceId].GetLocalisation();
   }
}