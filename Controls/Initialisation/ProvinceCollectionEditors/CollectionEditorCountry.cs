using Editor.DataClasses.Commands;
using Editor.Helper;

namespace Editor.Controls.Initialisation.ProvinceCollectionEditors
{
   public static class CollectionEditorCountry
   {
      
      public static List<string> CountrySelected(string s)
      {
         Selection.ClearSelection();
         if (Globals.Countries.TryGetValue(s, out var country))
         {
            Selection.AddProvincesToSelection(country.GetProvinceIds());
            List<string> provNames = [];
            foreach (var prov in country.GetProvinceIds())
               provNames.Add(prov.ToString());
            if (Globals.MapWindow.FocusSelectionCheckBox.Checked)
               Selection.FocusSelection();
            return provNames;
         }
         return [];
      }

      public static List<string> ModifyExitingCountry(string s, bool b)
      {
         if (!Globals.Countries.TryGetValue(s, out var country))
            return [];

         Globals.HistoryManager.AddCommand(new CModifyExistingCountry(s, Selection.GetSelectedProvincesIds.ToList(), b, Globals.MapWindow.CountryEditingGui));
         
         if (!b) // If we remove we only want to remove the province from the list which we removed and dont want to add it again
         {
            List<string> newProvNames = [];
            foreach (var prov in country.GetProvinceIds()) 
               newProvNames.Add(prov.ToString());
            return newProvNames;
         }

         List<string> provNames = [];
         foreach (var prov in Selection.GetSelectedProvincesIds)
            provNames.Add(prov.ToString());
         return provNames;
      }

      public static List<string> CreateNewCountry(string s)
      {
         if (Globals.Countries.TryGetValue(s, out _))
            return [];

         //Globals.HistoryManager.AddCommand(new CCreateNewCountry(s, Selection.SelectedProvinces));

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