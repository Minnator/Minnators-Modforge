using System.Diagnostics;
using Editor.Events;

namespace Editor.Controls
{
   public class CollectionEditor : Control
   {
      private GroupBox _groupBox = null!;
      private Label _titleLabel = null!;
      private TableLayoutPanel _nameTlp = null!;
      private TableLayoutPanel _tlp = null!;
      public ExtendedComboBox ExtendedComboBox = null!;
      private FlowLayoutPanel _flowLayout = null!;

      public event EventHandler<ProvinceEditedEventArgs> OnItemAdded = delegate { };
      public event EventHandler<ProvinceEditedEventArgs> OnItemRemoved = delegate { };

      private readonly Func<string, List<string>> _onSelectionAction; // Item name, return list of items to add as buttons
      private readonly Func<string, bool, List<string>> _onAddedOrRemovedFunc; // Item name, add or remove, returns list of items to add as buttons
      private readonly Func<string, List<string>> _onNewCreated; // Item name, returns list of items to add as buttons
      private readonly Action<string> _onDeleted; // Item name
      private readonly Action<string, string> _onSingleRemoved; // Item name, item to remove

      private readonly string _name;
      private readonly string _mapModeName;

      private readonly ItemTypes _itemTypes;

      // TODO add a small button to flip to the according MapMode
      public CollectionEditor(string name, string mapModeName, ItemTypes itemTypes, Func<string, List<string>> onSelectionAction, Func<string, bool, List<string>> onAddedOrRemovedFunc, Func<string, List<string>> onNewCreated, Action<string> onDeleted, Action<string, string> onSingleRemoved)
      {
         _name = name;
         _mapModeName = mapModeName;
         _itemTypes = itemTypes;
         InitializeComponents(name);
         _onSelectionAction = onSelectionAction;
         _onAddedOrRemovedFunc = onAddedOrRemovedFunc;
         _onNewCreated = onNewCreated;
         _onDeleted = onDeleted;
         _onSingleRemoved = onSingleRemoved;
      }

      private void InitializeComponents(string name)
      {
         _groupBox = new()
         {
            Text = $"{name} editing",
            Dock = DockStyle.Fill,
            Margin = new(0),
         };

         _titleLabel = new()
         {
            Text = $"Select {name}",
            Dock = DockStyle.Fill,
            Margin = new(0),
            TextAlign = ContentAlignment.MiddleLeft,
         };
         Globals.MapWindow.GeneralToolTip.SetToolTip(_titleLabel, $"Select the {name} to edit\n       or \ntype the name for a new {name}");

         _tlp = new()
         {
            RowCount = 2,
            ColumnCount = 1,
            Dock = DockStyle.Fill,
            Margin = new(0),
            RowStyles = { new(SizeType.Absolute, 30), new (SizeType.Percent, Width = 100) },
         };

         _nameTlp = new()
         {
            RowCount = 1,
            ColumnCount = 5,
            Dock = DockStyle.Fill,
            Margin = new(0),
            ColumnStyles =
            {
               new(SizeType.Percent, 40),
               new(SizeType.Absolute, 30),
               new(SizeType.Percent, 60),
               new(SizeType.Absolute, 30),
               new(SizeType.Absolute, 30),
            },
         };

         var addButton = ControlFactory.GetImageButton(ControlFactory.ImageButtonType.GreenPlus, "Add selection to current collection\n'right click' to create new collection from selection");
         addButton.MouseUp += OnAddButtonClick;

         var removeButton = ControlFactory.GetImageButton(ControlFactory.ImageButtonType.RedMinus, "Remove selection from current collection\n'right click' to delete the entire area");
         removeButton.MouseUp += OnRemoveButtonClick;

         var mapModeButton = ControlFactory.GetImageButton(ControlFactory.ImageButtonType.Map, "Switch to the according map mode");
         mapModeButton.MouseUp += SwitchToMapMode;

         ExtendedComboBox = new()
         {
            Margin = new(1, 5, 1, 1),
            Dock = DockStyle.Fill,
            AutoCompleteMode = AutoCompleteMode.SuggestAppend,
            AutoCompleteSource = AutoCompleteSource.ListItems,
         };
         ExtendedComboBox.SelectedIndexChanged += ComboBoxIndexChanged;

         _flowLayout = new()
         {
            Dock = DockStyle.Fill,
            WrapContents = true,
            Margin = new(3, 1, 3, 3),
            BorderStyle = BorderStyle.FixedSingle,
            AutoScroll = true,
         };

         _nameTlp.Controls.Add(_titleLabel, 0, 0);
         _nameTlp.Controls.Add(mapModeButton, 1, 0);
         _nameTlp.Controls.Add(ExtendedComboBox, 2, 0);
         _nameTlp.Controls.Add(addButton, 3, 0);
         _nameTlp.Controls.Add(removeButton, 4, 0);

         _tlp.Controls.Add(_nameTlp, 0, 0);
         _tlp.Controls.Add(_flowLayout, 0, 1);

         _groupBox.Controls.Add(_tlp);

         Controls.Add(_groupBox);
      }

      private void SwitchToMapMode(object? sender, MouseEventArgs e)
      {
         Globals.MapModeManager.SetCurrentMapMode(_mapModeName);
      }

      private void OnAddButtonClick(object? sender, MouseEventArgs e)
      {
         var item = ExtendedComboBox.Text;
         if (string.IsNullOrWhiteSpace(item))
            return;

         if (e.Button == MouseButtons.Left)
         {
            AddItemsUnique(_onAddedOrRemovedFunc.Invoke(item, true));
         }
         else if (e.Button == MouseButtons.Right)
         {
            if (Globals.Areas.ContainsKey(item))
            {
               MessageBox.Show($"{_name} already exists", "Error", MessageBoxButtons.OK);
               return;
            }
            Clear();
            AddItemsUnique(_onNewCreated.Invoke(item));
            ExtendedComboBox.Items.Add(item);
            ExtendedComboBox.AutoCompleteCustomSource.Add(item);
            // Needs to be set to None to delete the item from the cashed? autocomplete list
            ExtendedComboBox.AutoCompleteMode = AutoCompleteMode.None;
            ExtendedComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         }
         ExtendedComboBox.Text = item;
         ExtendedComboBox.SelectionStart = 0;
         ExtendedComboBox.SelectionLength = item.Length;
      }

      private void OnRemoveButtonClick(object? sender, MouseEventArgs e)
      {
         var item = ExtendedComboBox.Text;
         if (string.IsNullOrWhiteSpace(item))
            return;

         if (e.Button == MouseButtons.Left)
         {
            Clear();
            AddItemsUnique(_onAddedOrRemovedFunc.Invoke(item, false));
            ExtendedComboBox.Text = item;
            ExtendedComboBox.SelectionStart = 0;
            ExtendedComboBox.SelectionLength = item.Length;
         }
         else if (e.Button == MouseButtons.Right)
         {
            Clear();
            _onDeleted.Invoke(item);
            ExtendedComboBox.Items.Remove(item);
            // Needs to be set to None to delete the item from the cashed? autocomplete list
            ExtendedComboBox.AutoCompleteMode = AutoCompleteMode.None;
            ExtendedComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            ExtendedComboBox.AutoCompleteCustomSource.Remove(item);
         }
      }

      // When an item is autocompleted in the combobox, add it to the list
      private void ComboBoxIndexChanged(object? sender, EventArgs e)
      {
         var item = ExtendedComboBox.SelectedItem?.ToString();
         if (item == null)
            return;

         Clear();
         AddItemsUnique(_onSelectionAction.Invoke(item));
         ExtendedComboBox.Text = item;
      }

      private void OnSingleRemoved(object? sender, string item)
      {
         var text = ExtendedComboBox.Text;
         if (string.IsNullOrWhiteSpace(text))
         {
            AddItem(item);
            return;
         }
         _onSingleRemoved.Invoke(text, item);
      }

      public void SetComboBoxItems(List<string> items)
      {
         items.Sort();
         ExtendedComboBox.Items.Clear();
         foreach (var item in items)
            ExtendedComboBox.Items.Add(item);
      }

      public void InitializeItems(List<string> items)
      {
         items.Sort();
         ExtendedComboBox.Items.Clear();
         foreach (var item in items)
            ExtendedComboBox.Items.Add(item);
      }

      public List<string> GetItems()
      {
         return _flowLayout.Controls.Cast<ItemButton>().Select(button => button.Item).ToList();
      }

      public void AddIfUnique(string item)
      {
         foreach (ItemButton button in _flowLayout.Controls)
            if (button.Item == item)
               return;

         AddItem(item);
      }

      public void AddItemsUnique(List<string> items)
      {
         _flowLayout.SuspendLayout();
         foreach (var item in items)   
            AddIfUnique(item);
         _flowLayout.ResumeLayout(true);
      }

      public void AddItems(List<string> items)
      {
         foreach (var item in items)
            AddItem(item);
      }

      public void AddItem(string item)
      {
         ItemButton button = null!;
         if (_itemTypes == ItemTypes.Id)
            button = ControlFactory.GetItemButton(item, _itemTypes);
         else if (_itemTypes ==ItemTypes.String)
            button = ControlFactory.GetItemButtonLong(item, _itemTypes);

         button.OnButtonClicked += OnSingleRemoved;
         _flowLayout.Controls.Add(button);

         OnItemAdded?.Invoke(this, new(Globals.Selection.GetSelectedProvinces, item));

         ExtendedComboBox.Text = "";
         ExtendedComboBox.Focus();
      }

      public void RemoveItem(string item)
      {
         OnItemRemoved?.Invoke(this, new(Globals.Selection.GetSelectedProvinces, item));
      }

      public void Clear()
      {
         _flowLayout.Controls.Clear();
      }
   }
}