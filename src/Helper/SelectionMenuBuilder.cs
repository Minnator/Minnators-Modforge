using Editor.Controls;
using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Region = Editor.DataClasses.Saveables.Region;

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
         menu.Items.Add(new ToolStripSeparator());
         menu.Items.Add(ControlFactory.GetToolStripMenuItem("Copy Province Ids", (sender, args) =>
         {
            Clipboard.SetText(string.Join(' ', Selection.GetSelectedProvinces.Select(x => x.Id).Order()));
         }));

         return menu;
      }

      private static ToolStripMenuItem GetAreaSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Area", (sender, e) =>
         {
            if (Selection.LastHoveredProvince == Province.Empty)
               return;

            if (Selection.LastHoveredProvince.Area != Area.Empty)
            {
               Selection.AddOrRemoveAllFromSelection(Selection.LastHoveredProvince.Area.GetProvinces());
               Globals.ZoomControl.Invalidate();
            }



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
               {
                  Selection.AddOrRemoveAllFromSelection(Selection.LastHoveredProvince.Area.Region.GetProvinces());
                  Globals.ZoomControl.Invalidate();
               }
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
                  {
                     Selection.AddOrRemoveAllFromSelection(Selection.LastHoveredProvince.Area.Region.SuperRegion.GetProvinces());
                     Globals.ZoomControl.Invalidate();
                  }
         });
      }

      private static ToolStripMenuItem GetContinentSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Continent", (sender, args) =>
         {
            if (Selection.LastHoveredProvince == Province.Empty)
               return;

            if (Selection.LastHoveredProvince.Continent != Continent.Empty)
            {
               Selection.AddOrRemoveAllFromSelection(Selection.LastHoveredProvince.Continent.GetProvinces());
               Globals.ZoomControl.Invalidate();
            }
         });
      }

      private static ToolStripMenuItem GetCountrySelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Country", (sender, args) =>
         {
            if (Selection.LastHoveredProvince == Province.Empty)
               return;

            if (Globals.Countries.TryGetValue (Selection.LastHoveredProvince.Owner, out var country))
            {
               Selection.AddOrRemoveAllFromSelection(Selection.LastHoveredProvince.Owner.GetProvinces());
               Globals.ZoomControl.Invalidate();
            }
         });
      }

      private static ToolStripMenuItem GetCultureGroupSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Culture Group", (sender, args) =>
         {
            if (Selection.LastHoveredProvince == Province.Empty)
               return;

            {
               Selection.AddOrRemoveAllFromSelection(Selection.LastHoveredProvince.Culture.CultureGroup.GetProvinces());
               Globals.ZoomControl.Invalidate();
            }
         });
      }

      private static ToolStripMenuItem GetCultureSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Culture", (sender, args) =>
         {
            if (Selection.LastHoveredProvince == Province.Empty)
               return;

            {
               Selection.AddOrRemoveAllFromSelection(Selection.LastHoveredProvince.Culture.GetProvinces());
               Globals.ZoomControl.Invalidate();
            }
         });
      }

      private static ToolStripMenuItem GetTradeNodeSelector()
      {
         return ControlFactory.GetToolStripMenuItem("Select Trade Node", (sender, args) =>
         {

            if (Selection.LastHoveredProvince == Province.Empty)
               return;

            var node = TradeNodeHelper.GetTradeNodeByProvince(Selection.LastHoveredProvince); 
            Selection.AddOrRemoveAllFromSelection(node.GetProvinces());
            Globals.ZoomControl.Invalidate();
         });
      }
   }
}