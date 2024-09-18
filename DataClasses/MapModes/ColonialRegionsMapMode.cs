namespace Editor.DataClasses.MapModes
{
   public class ColonialRegionsMapMode : MapMode
   {
      public override bool IsLandOnly => true;
      public override bool ShowOccupation => false;

      public override Color GetProvinceColor(int id)
      {
         foreach (var cr in Globals.ColonialRegions.Values)
            if (cr.Provinces.Contains(id))
               return cr.Color;
         return Color.DimGray;
      }

      public override string GetMapModeName()
      {
         return "Colonial Regions";
      }

      public override string GetSpecificToolTip(int provinceId)
      {
         foreach (var cr in Globals.ColonialRegions.Values)
            if (cr.Provinces.Contains(provinceId))
               return $"Colonial region: {cr.Name} ({cr.GetLocalisation()})";
         return "Colonial region: [Unknown]";
      }
   }
}