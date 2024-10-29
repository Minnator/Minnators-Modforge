using Editor.Commands;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.Forms.AdvancedSelections;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor.Helper
{
   public static class SelectionMenuBuilder
   {
      public static ContextMenuStrip GetSelectionMenu()
      {
         var menu = new ContextMenuStrip();
         
         menu.Items.Add(GetAreaSelector());
         menu.Items.Add(GetRegionSelector());
         menu.Items.Add(GetSuperRegionSelector());
         menu.Items.Add(GetContinentSelector());
         menu.Items.Add(GetCountrySelector());
         menu.Items.Add(GetCultureGroupSelector());
         menu.Items.Add(GetCultureSelector());
         menu.Items.Add(GetTradeNodeSelector());
         
         /*
         foreach (ToolStripMenuItem item in menu.Items)
         {
            Debug.WriteLine($"{item.Text} --> {item.Enabled}");
         }
         */

         return menu;
      }

      private static ToolStripMenuItem GetAreaSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Area", (sender, e) =>
         {
            if (Selection.LastHoveredProvince == Province.Empty)
               return;

            if (Selection.LastHoveredProvince.Area != Area.Empty)
               Globals.HistoryManager.AddCommand(new CCollectionSelection(Selection.LastHoveredProvince.Area), CommandHistoryType.ComplexSelection);
            
         });
      }

      private static ToolStripMenuItem GetRegionSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Region", (sender, args) =>
         {
            if (Selection.LastHoveredProvince == Province.Empty)
               return;

            if (Selection.LastHoveredProvince.Area != Area.Empty)
               if (Selection.LastHoveredProvince.Area.Region != Region.Empty)
                     Globals.HistoryManager.AddCommand(new CCollectionSelection(Selection.LastHoveredProvince.Area.Region.GetProvinces()), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripMenuItem GetSuperRegionSelector ()
      {
         return ControlFactory.GetToolStripMenuItem("Select Super Region", (sender, args) =>
         {
            if (Selection.LastHoveredProvince == Province.Empty)
               return;

            if (Selection.LastHoveredProvince.Area != Area.Empty)
               if (Selection.LastHoveredProvince.Area.Region != Region.Empty)
                  if (Selection.LastHoveredProvince.Area.Region.SuperRegion != SuperRegion.Empty)
                        Globals.HistoryManager.AddCommand(new CCollectionSelection(Selection.LastHoveredProvince.Area.Region.SuperRegion.GetProvinces()), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripMenuItem GetContinentSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Continent", (sender, args) =>
         {
            if (Selection.LastHoveredProvince == Province.Empty)
               return;

            if (Selection.LastHoveredProvince.Continent != Continent.Empty)
                  Globals.HistoryManager.AddCommand(new CCollectionSelection(Selection.LastHoveredProvince.Continent), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripMenuItem GetCountrySelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Country", (sender, args) =>
         {
            if (Selection.LastHoveredProvince == Province.Empty)
               return;

            if (Globals.Countries.TryGetValue (Selection.LastHoveredProvince.Owner, out var country))
                  Globals.HistoryManager.AddCommand(new CCollectionSelection(country), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripMenuItem GetCultureGroupSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Culture Group", (sender, args) =>
         {
            if (Selection.LastHoveredProvince == Province.Empty)
               return;

            Globals.HistoryManager.AddCommand(new CCollectionSelection(Selection.LastHoveredProvince.GetProvincesWithSameCultureGroup()), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripMenuItem GetCultureSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Culture", (sender, args) =>
         {
            if (Selection.LastHoveredProvince == Province.Empty)
               return;

            Globals.HistoryManager.AddCommand(new CCollectionSelection(Selection.LastHoveredProvince.GetProvincesWithSameCulture()), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripMenuItem GetTradeNodeSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Trade Node", (sender, args) =>
         {

            if (Selection.LastHoveredProvince == Province.Empty)
               return;

            var node = TradeNodeHelper.GetTradeNodeByProvince(Selection.LastHoveredProvince);
            Globals.HistoryManager.AddCommand(new CCollectionSelection([.. node.GetProvinces()]), CommandHistoryType.ComplexSelection);
         });
      }
   }
}