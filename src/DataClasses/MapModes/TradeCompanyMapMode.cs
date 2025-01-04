using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.MapModes
{
   public class TradeCompanyMapMode : MapMode
   {
      public override bool IsLandOnly => true;
      public override bool ShowOccupation => false;

      public TradeCompanyMapMode()
      {
         TradeCompany.ItemsModified += UpdateProvinceCollection;
         TradeCompany.ColorChanged += UpdateComposite<Province>;
      }


      public override int GetProvinceColor(Province id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
            return province.TradeCompany.Color.ToArgb();
         return Color.DimGray.ToArgb();
      }

      public override MapModeType MapModeType => MapModeType.TradeCompany;

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
         {
            var tc = province.TradeCompany;
            if (tc == TradeCompany.Empty)
               return "Trade company: [Unknown]";
            else
               return $"Trade company: {tc.Name} ({tc.GetLocalisation()})";
         }
         return "Trade company: [Unknown]";
      }

   }

}