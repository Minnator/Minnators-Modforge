using DockStyle = System.Windows.Forms.DockStyle;

namespace Editor.Controls
{
   public sealed class DualSelectionFlowPanel : Control
   {
      private readonly FlowLayoutPanel _flowLayoutPanel;
      public readonly TableLayoutPanel _tableLayoutPanel;

      private readonly ComboBox _leftComboBox;
      private readonly ComboBox _rightComboBox;

      private readonly Button _addButton;

      public DualSelectionFlowPanel()
      {
         _flowLayoutPanel = new()
         {
            WrapContents = false,
            FlowDirection = FlowDirection.TopDown,
            Margin = new(1),
            BorderStyle = BorderStyle.FixedSingle,
            AutoScroll = true,
            Dock = DockStyle.Fill,
         };

         _tableLayoutPanel = new()
         {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 2,
            Margin = new(1),
            ColumnStyles =
            {
               new(SizeType.Percent, 50f),
               new(SizeType.Percent, 50f),
               new(SizeType.Absolute, 25f)
            },
            RowStyles =
            {
               new(SizeType.Absolute, 25f),
               new(SizeType.Percent, 100f)
            }

         };

         _leftComboBox = new()
         {
            Dock = DockStyle.Fill,
            Margin = new(1,1,1,1)
         };

         _rightComboBox = new()
         {
            Dock = DockStyle.Fill,
            Margin = new(1, 1, 1, 1)
         };

         _addButton = new()
         {
            Text = "",
            Image = Properties.Resources.GreePlus,
            Dock = DockStyle.Fill,
            Margin = new(1, 1, 1, 1)
         };
         _addButton.Click += AddButtonClicked;

         _tableLayoutPanel.Controls.Add(_leftComboBox, 0, 0);
         _tableLayoutPanel.Controls.Add(_rightComboBox, 1, 0);
         _tableLayoutPanel.Controls.Add(_addButton, 2, 0);

         _tableLayoutPanel.Controls.Add(_flowLayoutPanel, 0, 1);
         _tableLayoutPanel.SetColumnSpan(_flowLayoutPanel, 3);
         Controls.Add(_tableLayoutPanel);
      }

      public enum BoxType
      {
         Left,
         Right
      }

      public void SetComboBoxItems(List<string> items, BoxType boxType, bool setAsDropDown)
      {
         var box = boxType == BoxType.Left ? _leftComboBox : _rightComboBox;
         box.Items.Clear();
         box.Items.AddRange(items.ToArray());
         box.DropDownStyle = setAsDropDown ? ComboBoxStyle.DropDownList : ComboBoxStyle.DropDown;
      }

      public void AddButtonClicked(object? sender, EventArgs e)
      {
         var left = _leftComboBox.Text;
         var right = _rightComboBox.Text;

         if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right))
            return;

         _flowLayoutPanel.Controls.Add(GetDualContentPanel(left, right, _flowLayoutPanel.Width - 25));
      }


      private TableLayoutPanel GetDualContentPanel(string left, string right, int width)
      {
         var panel = new TableLayoutPanel()
         {
            Width = width,
            Height = 21,
            BorderStyle = BorderStyle.FixedSingle,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new(0),
            ColumnStyles =
            {
               new (SizeType.Percent, 70f),
               new (SizeType.Percent, 30f)
            },
         };
         
         var label = new Label
         {
            Text = left,
            Font = new("Arial", 8.25f),
            TextAlign = ContentAlignment.MiddleRight,
            Dock = DockStyle.Fill,
            Margin = new(1, 1, 1, 1),
         };

         var labelRight = new Label
         {
            Text = right,
            Font = new("Arial", 8.25f),
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Fill,
            Margin = new(1, 1, 1, 1),
         };

         panel.Controls.Add(label, 0, 0);
         panel.Controls.Add(labelRight, 1, 0);

         // if any of the items are right clicked, remove the panel
         label.MouseDown += (sender, args) => _flowLayoutPanel.Controls.Remove(panel);
         labelRight.MouseDown += (sender, args) => _flowLayoutPanel.Controls.Remove(panel);

         return panel;
      }
   }
}