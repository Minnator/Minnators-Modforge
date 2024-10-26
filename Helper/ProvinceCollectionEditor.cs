﻿using Editor.DataClasses.GameDataClasses;
using Editor.Events;

namespace Editor.Helper
{
   public static class ProvinceCollectionEditor
   {
      public static void ModifyTradeCompany(string companyKey, bool add, List<Province> deltaProvinces)
      {
         if (add)
            Globals.TradeCompanies[companyKey].Provinces.UnionWith(deltaProvinces);
         else
            Globals.TradeCompanies[companyKey].Provinces.ExceptWith(deltaProvinces);

         ProvinceCollectionEventHandler.RaiseOnTradeCompanyChanged(null, new (companyKey, deltaProvinces));
      }
   }
}