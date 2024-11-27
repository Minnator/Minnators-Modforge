using System.ComponentModel;

namespace Editor.Forms.PopUps
{
   public class StringCollectionEditor : Form
   {
      private readonly BindingList<string> _items;
      private readonly ListBox _listBox;
      private readonly ComboBox _comboBox;
      private readonly Button _removeButton, _moveUpButton, _moveDownButton, _addButton;

      public BindingList<string> Items => _items;

      public StringCollectionEditor(List<string> availableItems, BindingList<string> items)
      {
         _items = items;

         base.Text = "Custom Collection Editor";
         Width = 600;
         Height = 400;

         _listBox = new()
         {
            Dock = DockStyle.Fill,
            DataSource = items,
            DisplayMember = "ToString"
         };
         _listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;

         _comboBox = new()
         {
            Dock = DockStyle.Fill,
            DataSource = availableItems,
            AutoCompleteSource = AutoCompleteSource.ListItems,
            AutoCompleteMode = AutoCompleteMode.SuggestAppend,
            DropDownStyle = ComboBoxStyle.DropDown,
         };

         _addButton = new() { Text = "", Dock = DockStyle.Fill, Margin = new(1), Image = Properties.Resources.GreePlus };
         _removeButton = new() { Text = "", Dock = DockStyle.Fill, Enabled = false, Margin = new(1), Image = Properties.Resources.RedX };
         _moveUpButton = new() { Text = "", Dock = DockStyle.Fill, Enabled = false, Margin = new(1), Image = Properties.Resources.Up };
         _moveDownButton = new() { Text = "", Dock = DockStyle.Fill, Enabled = false, Margin = new(1), Image = Properties.Resources.Down };

         _addButton.Click += AddButton_Click;
         _removeButton.Click += RemoveButton_Click;
         _moveUpButton.Click += MoveUpButton_Click;
         _moveDownButton.Click += MoveDownButton_Click;

         TableLayoutPanel mainLayoutPanel = new()
         {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            ColumnStyles =
         {
            new(SizeType.Percent, 50),
            new(SizeType.Absolute, 35),
            new(SizeType.Percent, 50),
         },
            Margin = new(0),
         };

         TableLayoutPanel buttonTlp = new()
         {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            ColumnStyles =
         {
            new(SizeType.Percent, 100)
         },
            RowStyles =
         {
            new(SizeType.Absolute, 35),
            new(SizeType.Absolute, 35),
            new(SizeType.Absolute, 35),
            new(SizeType.Absolute, 35),
            new(SizeType.Percent, 100),
         },
            Margin = new(0),
         };

         mainLayoutPanel.Controls.Add(_listBox, 0, 0);
         mainLayoutPanel.Controls.Add(buttonTlp, 1, 0);
         mainLayoutPanel.Controls.Add(_comboBox, 2, 0);

         buttonTlp.Controls.Add(_moveUpButton, 0, 0);
         buttonTlp.Controls.Add(_moveDownButton, 0, 1);
         buttonTlp.Controls.Add(_addButton, 0, 2);
         buttonTlp.Controls.Add(_removeButton, 0, 3);

         Controls.Add(mainLayoutPanel);
      }
      private void MoveUpButton_Click(object? sender, EventArgs e)
      {
         var index = _listBox.SelectedIndex;
         if (index > 0)
         {
            var item = _items[index];
            _items.RemoveAt(index);
            _items.Insert(index - 1, item);
            _listBox.SelectedIndex = index - 1;
         }
      }

      private void MoveDownButton_Click(object? sender, EventArgs e)
      {
         var index = _listBox.SelectedIndex;
         if (index < _items.Count - 1)
         {
            var item = _items[index];
            _items.RemoveAt(index);
            _items.Insert(index + 1, item);
            _listBox.SelectedIndex = index + 1;
         }
      }
      private void AddButton_Click(object? sender, EventArgs e)
      {
         if (_comboBox.SelectedItem is string selectedItem && !_items.Contains(selectedItem))
            _items.Add(selectedItem);
      }

      private void RemoveButton_Click(object? sender, EventArgs e)
      {
         if (_listBox.SelectedItem is string selectedItem)
            _items.Remove(selectedItem);
      }

      private void InitializeComponent()
      {

      }

      private void ListBox_SelectedIndexChanged(object? sender, EventArgs e)
      {
         if (_listBox.SelectedItem != null)
         {
            _removeButton.Enabled = true;
            _moveUpButton.Enabled = _listBox.SelectedIndex > 0;
            _moveDownButton.Enabled = _listBox.SelectedIndex < _items.Count - 1;
         }
         else
         {
            _removeButton.Enabled = false;
            _moveUpButton.Enabled = false;
            _moveDownButton.Enabled = false;
         }
      }
   }
}