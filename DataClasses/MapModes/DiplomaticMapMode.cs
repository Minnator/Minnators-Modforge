using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class DiplomaticMapMode : MapMode
   {
      public override bool IsLandOnly => true;

      public DiplomaticMapMode()
      {
         ProvinceEventHandler.OnProvinceClaimsChanged += UpdateProvince!;
         ProvinceEventHandler.OnProvinceCoresChanged += UpdateProvince!;
         //TODO add permanent claims
      }

      public override string GetMapModeName()
      {
         return "Diplomatic";
      }

      public override string GetSpecificToolTip(int provinceId)
      {
         var tooltip = string.Empty;



         return tooltip;
      }
   }
}