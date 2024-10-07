﻿namespace Editor.Controls.Initialisation.ProvinceCollectionEditors
{
   public static class CollectionEditorCountry
   {
      
      public static List<string> CountrySelected(string s)
      {
         Globals.Selection.Clear();
         if (Globals.Countries.TryGetValue(s, out var country))
         {
            Globals.Selection.AddRange(country.GetProvinceIds());
            List<string> provNames = [];
            foreach (var prov in country.GetProvinceIds())
               provNames.Add(prov.ToString());
            if (Globals.MapWindow.FocusSelectionCheckBox.Checked)
               Globals.Selection.FocusSelection();
            return provNames;
         }
         return [];
      }

      public static List<string> ModifyExitingCountry(string s, bool b)
      {
         if (!Globals.Countries.TryGetValue(s, out var country))
            return [];

         //Globals.HistoryManager.AddCommand(new CModifyExistingCountry(s, Globals.Selection.SelectedProvinces, b));

         return [];
      }

      public static List<string> CreateNewCountry(string s)
      {
         if (Globals.Countries.TryGetValue(s, out _))
            return [];

         //Globals.HistoryManager.AddCommand(new CCreateNewCountry(s, Globals.Selection.SelectedProvinces));

         if (Globals.Countries.TryGetValue(s, out var newCountry))
            return [];
         return [];
      }

      public static void DeleteCountry(string s)
      {
         if (!Globals.Countries.TryGetValue(s, out _))
            return;

         //Globals.HistoryManager.AddCommand(new CRemoveCountry(s));
      }

      public static void SingleItemModified(string s, string idStr)
      {
         if (!Globals.Countries.TryGetValue(s, out var country) || !int.TryParse(idStr, out var id))
            return;

         if (!Globals.Provinces.TryGetValue(id, out var prov))
            return;

         //Globals.HistoryManager.AddCommand(new CModifyExistingCountry(s, [id], false));
      }

   }
}