using Editor.Commands;
using Editor.Controls;

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
            if (Globals.Provinces.TryGetValue(Globals.MapWindow.MapPictureBox.LastInvalidatedProvince, out var province))
               if (Globals.Areas.TryGetValue(province.Area, out var area))
                  Globals.HistoryManager.AddCommand(new CCollectionSelection(Globals.MapWindow.MapPictureBox, area), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripMenuItem GetRegionSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Region", (sender, args) =>
         {
            if (Globals.Provinces.TryGetValue(Globals.MapWindow.MapPictureBox.LastInvalidatedProvince, out var province))
               if (Globals.Areas.TryGetValue(province.Area, out var area))
                  if (Globals.Regions.TryGetValue(area.Region, out var region))
                     Globals.HistoryManager.AddCommand(new CCollectionSelection(Globals.MapWindow.MapPictureBox, region), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripMenuItem GetSuperRegionSelector ()
      {
         return ControlFactory.GetToolStripMenuItem("Select Super Region", (sender, args) =>
         {
            if (Globals.Provinces.TryGetValue(Globals.MapWindow.MapPictureBox.LastInvalidatedProvince, out var province))
               if (Globals.Areas.TryGetValue(province.Area, out var area))
                  if (Globals.Regions.TryGetValue(area.Region, out var region))
                     if (Globals.SuperRegions.TryGetValue(region.SuperRegion, out var superRegion))
                        Globals.HistoryManager.AddCommand(new CCollectionSelection(Globals.MapWindow.MapPictureBox, superRegion), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripMenuItem GetContinentSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Continent", (sender, args) =>
         {
            if (Globals.Provinces.TryGetValue(Globals.MapWindow.MapPictureBox.LastInvalidatedProvince, out var province))
               if (Globals.Continents.TryGetValue(province.Continent, out var continent))
                  Globals.HistoryManager.AddCommand(new CCollectionSelection(Globals.MapWindow.MapPictureBox, continent), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripMenuItem GetCountrySelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Country", (sender, args) =>
         {
            if (Globals.Provinces.TryGetValue(Globals.MapWindow.MapPictureBox.LastInvalidatedProvince, out var province))
               if (Globals.Countries.TryGetValue (province.Owner, out var country))
                  Globals.HistoryManager.AddCommand(new CCollectionSelection(Globals.MapWindow.MapPictureBox, country), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripMenuItem GetCultureGroupSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Culture Group", (sender, args) =>
         {
            if (Globals.Provinces.TryGetValue(Globals.MapWindow.MapPictureBox.LastInvalidatedProvince, out var province))
               Globals.HistoryManager.AddCommand(new CCollectionSelection(Globals.MapWindow.MapPictureBox, province.GetProvincesWithSameCultureGroup()), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripMenuItem GetCultureSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Culture", (sender, args) =>
         {
            if (Globals.Provinces.TryGetValue(Globals.MapWindow.MapPictureBox.LastInvalidatedProvince, out var province))
               Globals.HistoryManager.AddCommand(new CCollectionSelection(Globals.MapWindow.MapPictureBox, province.GetProvincesWithSameCulture()), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripMenuItem GetTradeNodeSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Trade Node", (sender, args) =>
         {
            if (Globals.Provinces.TryGetValue(Globals.MapWindow.MapPictureBox.LastInvalidatedProvince,
                   out var province))
            {
               var node = TradeNodeHelper.GetTradeNodeByProvince(province.Id);
               Globals.HistoryManager.AddCommand(new CCollectionSelection(Globals.MapWindow.MapPictureBox, [.. node.Members]), CommandHistoryType.ComplexSelection);
            }
         });
      }
   }
}