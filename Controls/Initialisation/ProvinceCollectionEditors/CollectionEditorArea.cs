using Editor.DataClasses.Commands;
using Editor.Helper;

namespace Editor.Controls.Initialisation.ProvinceCollectionEditors
{
   public static class CollectionEditorArea
   {
      public static List<string> AreaSelected(string s)
      {
         List<string> provName = [];
         Selection.ClearSelection();
         if (Globals.Areas.TryGetValue(s, out var area))
         {
            for (var i = 0; i < area.Provinces.Length; i++)
               provName.Add(area.Provinces[i].ToString());
            Selection.AddProvincesToSelection(area.Provinces);
            if (Globals.MapWindow.FocusSelectionCheckBox.Checked)
               Selection.FocusSelection();
         }
         return provName;
      }

      public static List<string> ModifyExitingArea(string s, bool b)
      {
         if (!Globals.Areas.TryGetValue(s, out var area))
            return [];

         Globals.HistoryManager.AddCommand(new CModifyExitingArea(s, Selection.GetSelectedProvinces, b));

         List<string> provNames = [];
         for (var i = 0; i < area.Provinces.Length; i++)
            provNames.Add(area.Provinces[i].ToString());
         return provNames;
      }

      public static List<string> CreateNewArea(string s)
      {
         Globals.HistoryManager.AddCommand(new CCreateNewArea(s, Selection.GetSelectedProvinces, Globals.MapWindow.AreaEditingGui.ExtendedComboBox));

         List<string> provName = [];
         for (var i = 0; i < Selection.GetSelectedProvinces.Count; i++)
            provName.Add(Selection.GetSelectedProvinces[i].ToString());
         return provName;
      }

      public static void RemoveArea(string s)
      {
         if (!Globals.Areas.TryGetValue(s, out _))
            return;

         Globals.HistoryManager.AddCommand(new CRemoveArea(s, Globals.MapWindow.AreaEditingGui.ExtendedComboBox));
      }

      public static void SingleItemModified(string s, string idStr)
      {
         if (!Globals.Areas.TryGetValue(s, out _) || !int.TryParse(idStr, out var id))
            return;

         if (!Globals.ProvinceIdToProvince.TryGetValue(id, out var province))
            return;

         Globals.HistoryManager.AddCommand(new CModifyExitingArea(s, [province], false));
      }
   }
}