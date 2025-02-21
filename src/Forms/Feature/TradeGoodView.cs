using System.Globalization;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Forms.Feature
{
   public partial class TradeGoodView : Form
   {
      public TradeGoodView()
      {
         InitializeComponent();
         CustomDrawing();

         TradeGoodListView.View = View.Details;
         TradeGoodListView.Columns.Clear();
         TradeGoodListView.Columns.Add("Name", -2);
         TradeGoodListView.Columns.Add("Icon", -2);
         TradeGoodListView.Columns.Add("Color", -2);
         TradeGoodListView.Columns.Add("Price", -2);
         TradeGoodListView.Columns.Add("Modifier", -2);
         TradeGoodListView.Columns.Add("Province", -2);
         PopulateListViewWithTradeGoods();
      }

      public void PopulateListViewWithTradeGoods()
      {
         TradeGoodListView.Items.Clear();
         TradeGoodListView.BeginUpdate();

         var cnt = 0;
         foreach (var tradeGood in Globals.TradeGoods.Values)
         {
            if (tradeGood == TradeGood.Empty)
               continue;

            var icon = new ListViewItem.ListViewSubItem
            {
               Tag = cnt
            };

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

            item.SubItems.Add(icon);
            item.SubItems.Add(color);
            item.SubItems.Add(price);
            item.SubItems.Add(modifier);
            item.SubItems.Add(province);

            TradeGoodListView.Items.Add(item);
            cnt++;
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
            e.Graphics.FillRectangle(new SolidBrush(e.SubItem?.BackColor ?? SystemColors.Window), e.Bounds);

            // Create a StringFormat for centering text
            var stringFormat = new StringFormat
            {
               Alignment = StringAlignment.Center, 
               LineAlignment = StringAlignment.Center,
               Trimming = StringTrimming.EllipsisCharacter 
            };

            // Draw the subitem text centered
            using var textBrush = new SolidBrush(e.SubItem?.ForeColor ?? SystemColors.ControlText);
            e.Graphics.DrawString(
                                  e.SubItem?.Text ?? string.Empty,
                                  e.SubItem?.Font ?? SystemFonts.DefaultFont,
                                  textBrush,
                                  e.Bounds,
                                  stringFormat
                                 );

            // Draw focus rectangle for selected state
            if ((e.ItemState & ListViewItemStates.Focused) != 0)
            {
               ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds);
            }

            if (e.ColumnIndex != 1)
               return;

            if (e.SubItem!.Tag is not int index)
               return;
            
            var strip = GameIconDefinition.GetIconDefinition(GameIcons.TradeGoods) as GameIconStrip;

            e.Graphics.DrawImage(
                                 strip.IconStrip[index],
                                 e.Bounds.Left,
                                 e.Bounds.Top,
                                 18,
                                 18
                                );
         };
      }

      private void TradeGoodListView_MouseDoubleClick(object sender, MouseEventArgs e)
      {
         var mousePosition = TradeGoodListView.PointToClient(Control.MousePosition);
         var hit = TradeGoodListView.HitTest(mousePosition);
         var columnIndex = hit?.Item?.SubItems.IndexOf(hit.SubItem);

         var item = TradeGoodListView.GetItemAt(e.X, e.Y);
         if (item == null)
            return;

         var tradeGood = item.Tag as TradeGood;

         switch (columnIndex)
         {
            case 1:
               var colorDialog = new ColorDialog
               {
                  Color = tradeGood.Color
               };
               if (colorDialog.ShowDialog() == DialogResult.OK)
               {
                  tradeGood.Color = colorDialog.Color;
                  PopulateListViewWithTradeGoods();
               }
               break;
         }
         
      }
   }
}
