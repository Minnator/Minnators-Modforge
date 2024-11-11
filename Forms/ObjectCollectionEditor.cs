using System.ComponentModel;

namespace Editor.Forms;

public class ObjectCollectionEditor<T> : Form where T : class, new()
{
   private ListBox listBox;
   private PropertyGrid propertyGrid;
   private Button addButton, removeButton, moveUpButton, moveDownButton;

   internal readonly BindingList<T> _items;

   public ObjectCollectionEditor(BindingList<T> items)
   {
      _items = items ?? throw new ArgumentNullException(nameof(items));

      base.Text = "Custom Collection Editor";
      Width = 600;
      Height = 400;

      listBox = new()
      {
         Dock = DockStyle.Fill,
         Width = 200,
         DataSource = items,
         DisplayMember = "ToString"
      };
      listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;

      propertyGrid = new()
      {
         Dock = DockStyle.Fill,
         Width = 300
      };

      addButton = new() { Text = "", Dock = DockStyle.Fill, Margin = new(1), Image = Properties.Resources.GreePlus };
      removeButton = new() { Text = "", Dock = DockStyle.Fill, Enabled = false, Margin = new(1), Image = Properties.Resources.RedX };
      moveUpButton = new() { Text = "", Dock = DockStyle.Fill, Enabled = false, Margin = new(1), Image = Properties.Resources.Up };
      moveDownButton = new() { Text = "", Dock = DockStyle.Fill, Enabled = false, Margin = new(1), Image = Properties.Resources.Down };

      addButton.Click += AddButton_Click;
      removeButton.Click += RemoveButton_Click;
      moveUpButton.Click += MoveUpButton_Click;
      moveDownButton.Click += MoveDownButton_Click;

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

      mainLayoutPanel.Controls.Add(listBox, 0, 0);
      mainLayoutPanel.Controls.Add(buttonTlp, 1, 0);
      mainLayoutPanel.Controls.Add(propertyGrid, 2, 0);

      buttonTlp.Controls.Add(moveUpButton, 0, 0);
      buttonTlp.Controls.Add(moveDownButton, 0, 1);
      buttonTlp.Controls.Add(addButton, 0, 2);
      buttonTlp.Controls.Add(removeButton, 0, 3);

      Controls.Add(mainLayoutPanel);
   }

   private void ListBox_SelectedIndexChanged(object? sender, EventArgs e)
   {
      if (listBox.SelectedItem != null)
      {
         propertyGrid.SelectedObject = listBox.SelectedItem;
         removeButton.Enabled = true;
         moveUpButton.Enabled = listBox.SelectedIndex > 0;
         moveDownButton.Enabled = listBox.SelectedIndex < _items.Count - 1;
      }
      else
      {
         propertyGrid.SelectedObject = null;
         removeButton.Enabled = false;
         moveUpButton.Enabled = false;
         moveDownButton.Enabled = false;
      }
   }

   private void AddButton_Click(object? sender, EventArgs e)
   {
      T newItem = new();
      _items.Add(newItem);
      listBox.SelectedItem = newItem;
   }

   private void RemoveButton_Click(object? sender, EventArgs e)
   {
      if (listBox.SelectedItem is T selectedItem)
         _items.Remove(selectedItem);
   }

   private void MoveUpButton_Click(object? sender, EventArgs e)
   {
      var index = listBox.SelectedIndex;
      if (index > 0)
      {
         var item = _items[index];
         _items.RemoveAt(index);
         _items.Insert(index - 1, item);
         listBox.SelectedIndex = index - 1;
      }
   }

   private void MoveDownButton_Click(object? sender, EventArgs e)
   {
      var index = listBox.SelectedIndex;
      if (index < _items.Count - 1)
      {
         T item = _items[index];
         _items.RemoveAt(index);
         _items.Insert(index + 1, item);
         listBox.SelectedIndex = index + 1;
      }
   }
}