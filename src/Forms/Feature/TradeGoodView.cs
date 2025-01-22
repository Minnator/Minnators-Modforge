using System.Globalization;
using Editor.DataClasses.GameDataClasses;

namespace Editor.src.Forms.Feature
{
   public partial class TradeGoodView : Form
   {
      public TradeGoodView()
      {
         InitializeComponent();
         CustomDrawing();
         PopulateListViewWithTradeGoods();
      }

      public void PopulateListViewWithTradeGoods()
      {
         TradeGoodListView.Items.Clear();
         TradeGoodListView.BeginUpdate();
         TradeGoodListView.View = View.Details;
         TradeGoodListView.Columns.Clear();
         TradeGoodListView.Columns.Add("Name", -2);
         TradeGoodListView.Columns.Add("Color", -2);
         TradeGoodListView.Columns.Add("Price", -2);
         TradeGoodListView.Columns.Add("Modifier", -2);
         TradeGoodListView.Columns.Add("Province Modifier", -2);

         foreach (var tradeGood in Globals.TradeGoods.Values)
         {
            if (tradeGood == TradeGood.Empty)
               continue;
            var color = new ListViewItem.ListViewSubItem
            {
               Text = $"{tradeGood.Color.R}/{tradeGood.Color.G}/{tradeGood.Color.B}",
               BackColor = tradeGood.Color,
            };

            var price = new ListViewItem.ListViewSubItem
            {
               Text = tradeGood.Price.Value.ToString(CultureInfo.InvariantCulture)
            };

            var modifier = new ListViewItem.ListViewSubItem
            {
               Text = string.Join(", ", tradeGood.Modifier.Select(kvp => $"{kvp.Key}: {kvp.Value}"))
            };

            var province = new ListViewItem.ListViewSubItem
            {
               Text = string.Join(", ", tradeGood.ProvinceModifier.Select(kvp => $"{kvp.Key}: {kvp.Value}"))
            };

            var item = new ListViewItem()
            {
               Text = tradeGood.Name,
               Tag = tradeGood,
            };
            item.SubItems.Add(color);
            item.SubItems.Add(price);
            item.SubItems.Add(modifier);
            item.SubItems.Add(province);

            TradeGoodListView.Items.Add(item);

         }

         TradeGoodListView.EndUpdate();
         //TradeGoodListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
         //TradeGoodListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
      }

      private void CustomDrawing()
      {
         // Handle DrawColumnHeader event to draw headers
         TradeGoodListView.DrawColumnHeader += (sender, e) =>
         {
            e.Graphics.FillRectangle(
                                     new SolidBrush(SystemColors.ControlLight),
                                     e.Bounds
                                    );

            e.DrawText();       // Default text
         };

         TradeGoodListView.DrawItem += (sender, e) =>
         {

         };


         TradeGoodListView.DrawSubItem += (sender, e) =>
         {
            // Draw the background for the subitem
            e.Graphics.FillRectangle(
                                     new SolidBrush(e.SubItem?.BackColor ?? SystemColors.Window),
                                     e.Bounds
                                    );

            // Draw subitem text
            using var textBrush = new SolidBrush(e.SubItem?.ForeColor ?? SystemColors.ControlText);
            e.Graphics.DrawString(
                                  e.SubItem?.Text ?? string.Empty,
                                  e.SubItem?.Font ?? SystemFonts.DefaultFont,
                                  textBrush,
                                  e.Bounds
                                 );

            // Draw focus rectangle for selected state
            if ((e.ItemState & ListViewItemStates.Focused) != 0)
            {
               ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds);
            }
         };
      }
   }
}
