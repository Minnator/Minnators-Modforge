using System;
using System.Collections.Generic;
using System.Drawing;
using Editor.DataClasses.GameDataClasses;
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
      return MapModeType.Province.ToString();
   }

   public override int GetProvinceColor(Province provinceId)
   {
      return provinceId.Color.ToArgb();
   }

   public override string GetSpecificToolTip(Province provinceId)
   {
      return $"Province: {provinceId.Id} ({provinceId.GetLocalisation()})";
   }
}