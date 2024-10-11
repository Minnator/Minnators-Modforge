using Editor.DataClasses.Commands;
using Editor.Helper;

namespace Editor.Controls.Initialisation.ProvinceCollectionEditors
{
   public static class CollectionEditorRegion
   {
      public static List<string> RegionSelected(string s)
      {
         Selection.ClearSelection();
         if (!Globals.Regions.TryGetValue(s, out var region))
            return [];

         Selection.AddProvincesToSelection(region.GetProvinceIds());
         if (Globals.MapWindow.FocusSelectionCheckBox.Checked)
            Selection.FocusSelection();
         return region.Areas;
      }

      public static List<string> ModifyExitingRegion(string s, bool b)
      {
         if (!Globals.Regions.TryGetValue(s, out var region))
            return [];

         Globals.HistoryManager.AddCommand(new CModifyExistingRegion(s, ProvinceCollectionHelper.GetAreaNamesFromProvinces(Selection.GetSelectedProvinces), b));
         return region.Areas;
      }

      public static List<string> CreateNewRegion(string s)
      {
         if (Globals.Regions.TryGetValue(s, out _))
            return [];

         Globals.HistoryManager.AddCommand(new CAddNewRegion(s, 
            ProvinceCollectionHelper.GetAreaNamesFromProvinces(Selection.GetSelectedProvinces), Globals.MapWindow.RegionEditingGui.ExtendedComboBox));

         return Globals.Regions.TryGetValue(s, out var newRegion) ? newRegion.Areas : [];
      }

      public static void DeleteRegion(string s)
      {
         if (!Globals.Regions.TryGetValue(s, out _))
            return;

         Globals.HistoryManager.AddCommand(new CDeleteRegion(s, Globals.MapWindow.RegionEditingGui));
      }

      public static void SingleItemModified(string s, string idStr)
      {
         if (!Globals.Regions.TryGetValue(s, out _) || !Globals.Areas.ContainsKey(idStr))
            return;

         Globals.HistoryManager.AddCommand(new CModifyExistingRegion(s, [idStr], false));
      }
   }
}