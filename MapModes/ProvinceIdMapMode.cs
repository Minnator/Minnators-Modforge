using System.Drawing;

namespace Editor.MapModes;

public sealed class ProvinceIdMapMode : Interfaces.MapMode
{

   public ProvinceIdMapMode()
   {
      RenderMapMode(GetProvinceColor);
   }

   public override string GetMapModeName()
   {
      return "Province Id";
   }

   public override Color GetProvinceColor(int id)
   {
      return Globals.Provinces[id].Color;
   }
   
}