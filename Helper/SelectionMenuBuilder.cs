using System.Diagnostics;
using Editor.Commands;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;

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
         
         /*
         foreach (ToolStripMenuItem item in menu.Items)
         {
            Debug.WriteLine($"{item.Text} --> {item.Enabled}");
         }
         */

         return menu;
      }

      private static ToolStripItem GetAreaSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Area", (sender, e) =>
         {
            if (Globals.Provinces.TryGetValue(Globals.MapWindow.MapPictureBox.LastInvalidatedProvince, out var province))
               if (Globals.Areas.TryGetValue(province.Area, out var area))
                  Globals.HistoryManager.AddCommand(new CCollectionSelection(Globals.MapWindow.MapPictureBox, area), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripItem GetRegionSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Region", (sender, args) =>
         {
            if (Globals.Provinces.TryGetValue(Globals.MapWindow.MapPictureBox.LastInvalidatedProvince, out var province))
               if (Globals.Areas.TryGetValue(province.Area, out var area))
                  if (Globals.Regions.TryGetValue(area.Region, out var region))
                     Globals.HistoryManager.AddCommand(new CCollectionSelection(Globals.MapWindow.MapPictureBox, region), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripItem GetSuperRegionSelector ()
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

      private static ToolStripItem GetContinentSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Continent", (sender, args) =>
         {
            if (Globals.Provinces.TryGetValue(Globals.MapWindow.MapPictureBox.LastInvalidatedProvince, out var province))
               if (Globals.Continents.TryGetValue(province.Continent, out var continent))
                  Globals.HistoryManager.AddCommand(new CCollectionSelection(Globals.MapWindow.MapPictureBox, continent), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripItem GetCountrySelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Country", (sender, args) =>
         {
            if (Globals.Provinces.TryGetValue(Globals.MapWindow.MapPictureBox.LastInvalidatedProvince, out var province))
               Globals.HistoryManager.AddCommand(new CCollectionSelection(Globals.MapWindow.MapPictureBox, Globals.Countries[province.Owner]), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripItem GetCultureGroupSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Culture Group", (sender, args) =>
         {
            if (Globals.Provinces.TryGetValue(Globals.MapWindow.MapPictureBox.LastInvalidatedProvince, out var province))
               Globals.HistoryManager.AddCommand(new CCollectionSelection(Globals.MapWindow.MapPictureBox, province.GetProvincesWithSameCultureGroup()), CommandHistoryType.ComplexSelection);
         });
      }

      private static ToolStripItem GetCultureSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Culture", (sender, args) =>
         {
            if (Globals.Provinces.TryGetValue(Globals.MapWindow.MapPictureBox.LastInvalidatedProvince, out var province))
               Globals.HistoryManager.AddCommand(new CCollectionSelection(Globals.MapWindow.MapPictureBox, province.GetProvincesWithSameCulture()), CommandHistoryType.ComplexSelection);
         });
      }

      public static void SetContextMenuStrip(Province province, ContextMenuStrip menu)
      {
         if (string.IsNullOrEmpty(province.Area))
            menu.Items[0].Enabled = false;
         if (string.IsNullOrEmpty(province.Area) || string.IsNullOrEmpty(Globals.Areas[province.Area].Region))
            menu.Items[1].Enabled = false;
         if (string.IsNullOrEmpty(province.Area) || string.IsNullOrEmpty(Globals.Areas[province.Area].Region) || string.IsNullOrEmpty(Globals.Regions[Globals.Areas[province.Area].Region].SuperRegion))
            menu.Items[2].Enabled = false;
         if (string.IsNullOrEmpty(province.Continent))
            menu.Items[3].Enabled = false;
         if (string.IsNullOrEmpty(province.Owner))
            menu.Items[4].Enabled = false;
         if (string.IsNullOrEmpty(province.Culture) || string.IsNullOrEmpty(Globals.Cultures[province.Culture].CultureGroup))
            menu.Items[5].Enabled = false;
         if (string.IsNullOrEmpty(province.Culture))
            menu.Items[6].Enabled = false;

      }
   }
}