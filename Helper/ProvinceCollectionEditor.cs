using Editor.DataClasses.Commands;
using Editor.Events;

namespace Editor.Helper
{
   public static class ProvinceCollectionEditor
   {
      public static void ModifyTradeCompany(string companyKey, List<int> deltaProvinces, bool add)
      {
         if (add)
            Globals.TradeCompanies[companyKey].Provinces.UnionWith(deltaProvinces);
         else
            Globals.TradeCompanies[companyKey].Provinces.ExceptWith(deltaProvinces);

         ProvinceCollectionEvents.RaiseOnTradeCompanyChanged(null, new (companyKey, [..deltaProvinces]));
      }
   }
}