namespace Editor.Controls.Initialisation.ProvinceCollectionEditors
{
   public static class CollectionEditorTradeCompany
   {
      
      public static List<string> TradeCompanySelected(string s)
      {
         Globals.Selection.Clear();
         if (Globals.TradeCompanies.TryGetValue(s, out var tradeCompany))
         {
            Globals.Selection.AddRange(tradeCompany.GetProvinceIds());
            if (Globals.MapWindow.FocusSelectionCheckBox.Checked)
               Globals.Selection.FocusSelection();
            List<string> provNames = [];
            foreach (var prov in tradeCompany.GetProvinceIds())
               provNames.Add(prov.ToString());
            return provNames;
         }
         return [];
      }

      public static List<string> ModifyExitingTradeCompany(string s, bool b)
      {
         if (!Globals.TradeCompanies.TryGetValue(s, out var tradeCompany))
            return [];

         //Globals.HistoryManager.AddCommand(new CModifyExistingTradeCompany(s, Globals.Selection.SelectedProvinces, b));

         return [];
      }

      public static List<string> CreateNewTradeCompany(string s)
      {
         if (Globals.TradeCompanies.TryGetValue(s, out _))
            return [];

         //Globals.HistoryManager.AddCommand(new CCreateNewTradeCompany(s, Globals.Selection.SelectedProvinces));

         if (Globals.TradeCompanies.TryGetValue(s, out var newTradeCompany))
            return [];
         return [];
      }

      public static void DeleteTradeCompany(string s)
      {
         if (!Globals.TradeCompanies.TryGetValue(s, out _))
            return;

         //Globals.HistoryManager.AddCommand(new CRemoveTradeCompany(s));
      }

      public static void SingleItemModified(string s, string idStr)
      {
         if (!Globals.TradeCompanies.TryGetValue(s, out var tradeCompany) || !int.TryParse(idStr, out var id))
            return;

         if (!Globals.Provinces.TryGetValue(id, out var prov))
            return;

         //Globals.HistoryManager.AddCommand(new CModifyExistingTradeCompany(s, [id], false));
      }
   }
}