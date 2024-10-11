using Editor.DataClasses.GameDataClasses;
using Editor.Events;

namespace Editor.DataClasses.MapModes
{
   public class TradeCompanyMapMode : MapMode
   {
      public override bool IsLandOnly => true;
      public override bool ShowOccupation => false;

      public TradeCompanyMapMode()
      {
         ProvinceCollectionEventHandler.OnTradeCompanyChanged += UpdateProvinceCollection;
      }

      public override int GetProvinceColor(int id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
         {
            var company = province.GetTradeCompany;
            if (company != string.Empty)
               return Globals.TradeCompanies[company].Color.ToArgb();
         }
         return Color.DimGray.ToArgb();
      }

      public override string GetMapModeName()
      {
         return "Trade Companies";
      }

      public override string GetSpecificToolTip(int provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
         {
            var companyName = province.GetTradeCompany;
            if (companyName == string.Empty)
               return "Trade company: [Unknown]";
            if (Globals.TradeCompanies.TryGetValue(companyName, out var company))
               return $"Trade company: {company.CodeName} ({company.GetLocalisation()})";
         }
         return "Trade company: [Unknown]";
      }
   }

}