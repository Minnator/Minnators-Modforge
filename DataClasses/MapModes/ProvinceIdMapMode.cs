using System;
using System.Drawing;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.Helper;

namespace Editor.MapModes;

public sealed class ProvinceIdMapMode : MapMode
{
   
   public override MapModeType GetMapModeName()
   {
      return MapModeType.Province;
   }

   public override int GetProvinceColor(Province id)
   {
      return id.Color.ToArgb();
   }
   
}