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
         {
            var company = province.GetTradeCompany;
            if (company != string.Empty)
               return Globals.TradeCompanies[company].Color.ToArgb();
         }
         return Color.DimGray.ToArgb();
      }

      public override MapModeType MapModeType => MapModeType.TradeCompany;

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
         {
            var companyName = province.GetTradeCompany;
            if (companyName == string.Empty)
               return "Trade company: [Unknown]";
            if (Globals.TradeCompanies.TryGetValue(companyName, out var company))
               return $"Trade company: {company.Name} ({company.GetLocalisation()})";
         }
         return "Trade company: [Unknown]";
      }

      public override bool ShouldProvincesMerge(Province p1, Province p2)
      {
         return p1.GetTradeCompany == p2.GetTradeCompany;
      }
   }

}