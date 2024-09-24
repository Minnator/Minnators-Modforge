﻿using Editor.Events;

namespace Editor.Controls
{
   public class CollectionEditor : Control
   {
      private GroupBox _groupBox = null!;
      private Label _titleLabel = null!;
      private TableLayoutPanel _nameTlp = null!;
      private TableLayoutPanel _tlp = null!;
      private ExtendedComboBox _extendedComboBox = null!;
      private FlowLayoutPanel _flowLayout = null!;

      public event EventHandler<ProvinceEditedEventArgs> OnItemAdded = delegate { };
      public event EventHandler<ProvinceEditedEventArgs> OnItemRemoved = delegate { };

      private Func<string, List<string>> _onSelectionAction; // Item name, return list of items to add as buttons
      private Func<string, bool, List<string>> _onAddedOrRemovedFunc; // Item name, add or remove, returns list of items to add as buttons
      private Func<string, List<string>> _onNewCreated; // Item name, returns list of items to add as buttons
      private Action<string> _onDeleted; // Item name

      private ItemTypes _itemTypes;

      public CollectionEditor(string name, ItemTypes itemTypes, Func<string, List<string>> onSelectionAction, Func<string, bool, List<string>> onAddedOrRemovedFunc, Func<string, List<string>> onNewCreated, Action<string> onDeleted)
      {
         _itemTypes = itemTypes;
         InitializeComponents(name);
         _onSelectionAction = onSelectionAction;
         _onAddedOrRemovedFunc = onAddedOrRemovedFunc;
         _onNewCreated = onNewCreated;
         _onDeleted = onDeleted;
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
            RowStyles = { new(SizeType.Absolute, 30) },
         };

         _nameTlp = new()
         {
            RowCount = 1,
            ColumnCount = 4,
            Dock = DockStyle.Fill,
            Margin = new(0),
            ColumnStyles =
            {
               new(SizeType.Percent, 40),
               new(SizeType.Percent, 60),
               new(SizeType.Absolute, 30),
               new(SizeType.Absolute, 30),
            },
         };

         var addButton = ControlFactory.GetImageButton(ControlFactory.ImageButtonType.GreenPlus, "Add selection to current collection\n'right click' to create new collection from selection");
         addButton.MouseUp += OnAddButtonClick;

         var removeButton = ControlFactory.GetImageButton(ControlFactory.ImageButtonType.RedMinus, "Remove selection from current collection");
         removeButton.MouseUp += OnRemoveButtonClick;

         _extendedComboBox = new()
         {
            Margin = new(1, 5, 1, 1),
            Dock = DockStyle.Fill,
            AutoCompleteMode = AutoCompleteMode.SuggestAppend,
            AutoCompleteSource = AutoCompleteSource.ListItems,
         };
         _extendedComboBox.SelectedIndexChanged += ComboBoxIndexChanged;

         _flowLayout = new()
         {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            WrapContents = true,
            Margin = new(3, 1, 3, 3),
         };

         _nameTlp.Controls.Add(_titleLabel, 0, 0);
         _nameTlp.Controls.Add(_extendedComboBox, 1, 0);
         _nameTlp.Controls.Add(addButton, 2, 0);
         _nameTlp.Controls.Add(removeButton, 3, 0);

         _tlp.Controls.Add(_nameTlp, 0, 0);
         _tlp.Controls.Add(_flowLayout, 0, 1);

         _groupBox.Controls.Add(_tlp);

         Controls.Add(_groupBox);
      }

      private void OnAddButtonClick(object? sender, MouseEventArgs e)
      {
         var item = _extendedComboBox.Text;
         if (string.IsNullOrWhiteSpace(item))
            return;

         if (e.Button == MouseButtons.Left)
            AddItemsUnique(_onAddedOrRemovedFunc.Invoke(item, true));
         else if (e.Button == MouseButtons.Right)
         {
            Clear();
            AddItemsUnique(_onNewCreated.Invoke(item));
            _extendedComboBox.Items.Add(item);
            _extendedComboBox.SelectedItem = item;
         }
      }

      private void OnRemoveButtonClick(object? sender, MouseEventArgs e)
      {
         var item = _extendedComboBox.Text;
         if (string.IsNullOrWhiteSpace(item))
            return;

         if (e.Button == MouseButtons.Left)
            AddItemsUnique(_onAddedOrRemovedFunc.Invoke(item, false));
         else if (e.Button == MouseButtons.Right)
         {
            Clear();
            _onDeleted.Invoke(item);
            _extendedComboBox.Items.Remove(item);
         }
      }

      // When an item is autocompleted in the combobox, add it to the list
      private void ComboBoxIndexChanged(object? sender, EventArgs e)
      {
         var item = _extendedComboBox.SelectedItem?.ToString();
         if (item == null)
            return;

         Clear();
         AddItemsUnique(_onSelectionAction.Invoke(item));
         _extendedComboBox.Text = item;
      }


      public void SetComboBoxItems(List<string> items)
      {
         items.Sort();
         _extendedComboBox.Items.Clear();
         foreach (var item in items)
            _extendedComboBox.Items.Add(item);
      }

      public void AddToComboBoxItems(string newItem)
      {
         _extendedComboBox.Items.Add(newItem);
         _extendedComboBox.SelectedItem = newItem;
      }

      public void RemoveItemFromComboBox(string item)
      {
         _extendedComboBox.Items.Remove(item);
      }

      public void InitializeItems(List<string> items)
      {
         items.Sort();
         _extendedComboBox.Items.Clear();
         foreach (var item in items)
            _extendedComboBox.Items.Add(item);
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
         foreach (var item in items)
            AddIfUnique(item);
      }

      public void AddItems(List<string> items)
      {
         foreach (var item in items)
            AddItem(item);
      }

      public void AddItem(string item)
      {
         _flowLayout.Controls.Add(ControlFactory.GetItemButton(item, _itemTypes));

         OnItemAdded?.Invoke(this, new(Globals.Selection.GetSelectedProvinces, item));

         _extendedComboBox.Text = "";
         _extendedComboBox.Focus();
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