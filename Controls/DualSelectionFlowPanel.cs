using DockStyle = System.Windows.Forms.DockStyle;

namespace Editor.Controls
{
   public sealed class DualSelectionFlowPanel : Control
   {
      public readonly FlowLayoutPanel FlowLayoutPanel;
      public readonly TableLayoutPanel TableLayoutPanel;
      public readonly ComboBox LeftComboBox;
      public readonly ComboBox RightComboBox;
      private readonly Button _addButton;
      public bool UseIndexing;
      public int MaxIndex;
      public int MinIndex;
      public List<KeyValuePair<string, string>> DualContent = [];
      public IndexingType Indexing = IndexingType.None;

      public DualSelectionFlowPanel()
      {
         FlowLayoutPanel = new()
         {
            WrapContents = false,
            FlowDirection = FlowDirection.TopDown,
            Margin = new(1),
            BorderStyle = BorderStyle.FixedSingle,
            AutoScroll = true,
            Dock = DockStyle.Fill,
         };

         TableLayoutPanel = new()
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

         LeftComboBox = new()
         {
            Dock = DockStyle.Fill,
            Margin = new(1,1,1,1)
         };

         RightComboBox = new()
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

         TableLayoutPanel.Controls.Add(LeftComboBox, 0, 0);
         TableLayoutPanel.Controls.Add(RightComboBox, 1, 0);
         TableLayoutPanel.Controls.Add(_addButton, 2, 0);

         TableLayoutPanel.Controls.Add(FlowLayoutPanel, 0, 1);
         TableLayoutPanel.SetColumnSpan(FlowLayoutPanel, 3);
         Controls.Add(TableLayoutPanel);
      }
      public enum BoxType
      {
         Left,
         Right
      }

      public enum IndexingType
      {
         None,
         Left, 
         Right,
         Both
      }

      public List<KeyValuePair<string, string>> GetDualContent()
      {
         return DualContent;
      }

      public void SetComboBoxItems(ICollection<string> items, BoxType boxType, bool setAsDropDown)
      {
         var box = boxType == BoxType.Left ? LeftComboBox : RightComboBox;
         box.Items.Clear();
         box.Items.AddRange(items.ToArray());
         box.DropDownStyle = setAsDropDown ? ComboBoxStyle.DropDownList : ComboBoxStyle.DropDown;
      }

      public void AddButtonClicked(object? sender, EventArgs e)
      {
         var left = LeftComboBox.Text;
         var right = RightComboBox.Text;

         if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right))
            return;

         AddToFlowPanel(left, right);

         switch (Indexing)
         {
            case IndexingType.None:
               DualContent.Add(new(left, right));
               break;
            case IndexingType.Left:
               if (LeftComboBox.SelectedIndex >= MinIndex && LeftComboBox.SelectedIndex <= MaxIndex)
                  DualContent.Add(new(LeftComboBox.SelectedIndex.ToString(), right));
               break;
            case IndexingType.Right:
               if (RightComboBox.SelectedIndex >= MinIndex && RightComboBox.SelectedIndex <= MaxIndex)
                  DualContent.Add(new(left, RightComboBox.SelectedIndex.ToString()));
               break;
            case IndexingType.Both:
               if (LeftComboBox.SelectedIndex >= MinIndex && LeftComboBox.SelectedIndex <= MaxIndex
                   && RightComboBox.SelectedIndex >= MinIndex && RightComboBox.SelectedIndex <= MaxIndex)
                  DualContent.Add(new(LeftComboBox.SelectedIndex.ToString(), RightComboBox.SelectedIndex.ToString()));
               break;
         }
      }

      private void AddToFlowPanel(string left, string right)
      {
         FlowLayoutPanel.Controls.Add(GetDualContentPanel(left, right, FlowLayoutPanel.Width - 25));
      }

      private TableLayoutPanel GetDualContentPanel(string left, string right, int width)
      {
         var panel = new TableLayoutPanel
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
         label.MouseDown += (sender, args) => FlowLayoutPanel.Controls.Remove(panel);
         labelRight.MouseDown += (sender, args) => FlowLayoutPanel.Controls.Remove(panel);

         return panel;
      }

      public void SetDualContent(List<KeyValuePair<string, string>> keyValuePairs)
      {
         FlowLayoutPanel.Controls.Clear();
         foreach (var pair in keyValuePairs)
            AddToFlowPanel(pair.Key, pair.Value);
      }
   }
}