﻿using Editor.Controls.PROPERTY;
using System.Reflection;
using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;
using Editor.Helper;
using Editor.Saving;
using Newtonsoft.Json.Linq;

namespace Editor.Controls
{
   public class CollectionEditor2<T, Q> : Control where T : ProvinceCollection<Q> where Q : ProvinceComposite // The Sequel
   {

      #region GUI ELEMENTS
      private GroupBox _groupBox = null!;
      private Label _titleLabel = null!;
      private TableLayoutPanel _nameTlp = null!;
      private TableLayoutPanel _tlp = null!;
      public ExtendedComboBox _extendedComboBox = null!;
      private FlowLayoutPanel _flowLayout = null!;
      
      #endregion

      public bool AllowSeaTiles { get; init; } = true;

      private readonly ItemTypes _itemTypes;
      private readonly MapModeType _mapModeType;
      private readonly SaveableType _saveableType;
      private readonly SaveableType _saveableSubType;
      public CollectionEditor2(ItemTypes types, SaveableType type, SaveableType subType, MapModeType mapModeType)
      {
         _itemTypes = types;
         _saveableType = type;
         _saveableSubType = subType;
         _mapModeType = mapModeType;
         
         InitializeComponents(_mapModeType.ToString());
         SetComboBoxItems([.. ProvColHelper.GetProvinceCollectionNames(_saveableType)]);
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
            RowStyles = { new(SizeType.Absolute, 30), new(SizeType.Percent, Width = 100) },
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

         _extendedComboBox = new("")
         {
            Margin = new(1, 5, 1, 1),
            Dock = DockStyle.Fill,
            AutoCompleteMode = AutoCompleteMode.SuggestAppend,
            AutoCompleteSource = AutoCompleteSource.CustomSource,
         };
         _extendedComboBox.SelectedIndexChanged += ComboBoxIndexChanged;
         _extendedComboBox.KeyDown += (sender, e) =>
         {
            if (e.Handled)
               return;

            if (e.KeyCode == Keys.Escape)
            {
               Clear();
               e.Handled = true;
            }
         };
         Globals.MapWindow.GeneralToolTip.SetToolTip(_extendedComboBox, "Select the collection to edit\n       or \ntype the name for a new collection\n\nESC clear / cancel");

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
         _nameTlp.Controls.Add(_extendedComboBox, 2, 0);
         _nameTlp.Controls.Add(addButton, 3, 0);
         _nameTlp.Controls.Add(removeButton, 4, 0);

         _tlp.Controls.Add(_nameTlp, 0, 0);
         _tlp.Controls.Add(_flowLayout, 0, 1);

         _groupBox.Controls.Add(_tlp);

         Controls.Add(_groupBox);
         Dock = DockStyle.Fill;
      }
      public void SetComboBoxItems(List<string> items)
      {
         items.Sort();
         _extendedComboBox.Items.Clear();
         foreach (var item in items)
         {
            _extendedComboBox.Items.Add(item);
            _extendedComboBox.AutoCompleteCustomSource.Add(item);
         }
      }
      private void SwitchToMapMode(object? sender, MouseEventArgs e)
      {
         MapModeManager.SetCurrentMapMode(_mapModeType);
      }

      private void OnAddButtonClick(object? sender, MouseEventArgs e)
      {
         var item = _extendedComboBox.Text;
         if (string.IsNullOrWhiteSpace(item))
            return;
         
         ProvinceCollection<Q> collection;
         if (e.Button == MouseButtons.Right)
         {
            var provincesToAdd = Selection.GetSelectedProvinces.Where(Globals.LandProvinces.Contains).ToList();
            collection = ProvColHelper.CreateNewObject<T>(SavingUtil.ApplyModPrefix(item), Globals.ColorProvider.GetRandomColor(), _saveableType);
            collection.NewAddRange(ProvColHelper.GetProvinceCollectionOfTypeForProvinces<Q>(provincesToAdd, _saveableSubType), true);
         }
         else
         {
            if (!ProvColHelper.GetProvinceCollectionForTypeAndName(_saveableType, item, out collection))
               return;
            var provincesToAdd = Selection.GetSelectedProvinces.Where(Globals.LandProvinces.Contains).ToList();
            collection.NewAddRange(ProvColHelper.GetProvinceCollectionOfTypeForProvinces<Q>(provincesToAdd, _saveableSubType));
         }
      }

      private void OnRemoveButtonClick(object? sender, MouseEventArgs e)
      {
         var item = _extendedComboBox.Text;
         if (string.IsNullOrWhiteSpace(item))
            return;

         var composites = ProvColHelper.GetProvinceCollectionOfTypeForProvinces<Q>(Selection.GetSelectedProvinces, _saveableSubType);
         
         if (!ProvColHelper.GetProvinceCollectionForTypeAndName(_saveableType, item, out ProvinceCollection<Q> collection))
            return;

         if (e.Button == MouseButtons.Right)
         {
            collection.NewRemoveFromGlobal();
         }
         else
            collection.NewRemoveRange(composites);
      }

      private void ComboBoxIndexChanged(object? sender, EventArgs e)
      {
         var item = _extendedComboBox.Text;
         if (string.IsNullOrWhiteSpace(item))
            return;

         if (!ProvColHelper.GetProvinceCollectionForTypeAndName<T>(_saveableType, item, out var collection))
            return;

         Selection.ClearSelection();
         Selection.AddProvincesToSelection(collection.GetProvinces());

         if (Globals.MapWindow.FocusSelectionCheckBox.Checked)
            Selection.FocusSelection();
         Globals.ZoomControl.Invalidate();
         
         _flowLayout.SuspendLayout();
         _flowLayout.Controls.Clear();
         foreach (var i in collection.SubCollection)
            AddItem(i.Name);
         _flowLayout.ResumeLayout(true);
      }

      // A single subtype should be removed from the collection
      private void OnSingleRemoved(object? sender, string item)
      {
         if (!ProvColHelper.GetProvinceCollectionForTypeAndName(_saveableSubType, item, out Q value))
            return;

         if (!ProvColHelper.GetProvinceCollectionForTypeAndName(_saveableType, _extendedComboBox.Text, out T collection))
            return;

         collection.NewRemove(value);
         Globals.ZoomControl.Invalidate();
      }

      public void ClearAndAddUniquely(ProvinceCollection<Q> collection)
      {
         _flowLayout.SuspendLayout();
         _flowLayout.Controls.Clear();
         foreach (var i in collection.SubCollection)
            AddItem(i.Name);
         _flowLayout.ResumeLayout(true);
      }

      public void AddIfUnique(string item)
      {
         foreach (ItemButton button in _flowLayout.Controls)
            if (button.Item == item)
               return;

         AddItem(item);
      }

      public void RemoveItem(string item)
      {
         var toRemove = _flowLayout.Controls.OfType<ItemButton>()
            .FirstOrDefault(c => c.Item == item);
         if (toRemove != null)
            _flowLayout.Controls.Remove(toRemove);
      }


      public void AddItem(string item)
      {
         ItemButton button = null!;
         if (_itemTypes == ItemTypes.Id)
            button = ControlFactory.GetItemButton(item, _itemTypes);
         else if (_itemTypes == ItemTypes.String)
            button = ControlFactory.GetItemButtonLong(item, _itemTypes);

         button.OnButtonClicked += OnSingleRemoved;
         _flowLayout.Controls.Add(button);
      }

      public void Clear()
      {
         _extendedComboBox.Text = string.Empty;
         _extendedComboBox.SelectedIndex = -1;
         _flowLayout.Controls.Clear();
      }

      public void OnCorrespondingDataChange(object? obj, ProvinceCollectionEventArguments<Q> e)
      {
         if (obj is not T collection)
            return;
         _flowLayout.SuspendLayout();
         var item = collection.Name;
         switch (e.Type)
         {
            case ProvinceCollectionType.AddGlobal:
               _extendedComboBox.Items.Add(item);
               _extendedComboBox.AutoCompleteCustomSource.Add(item);
               _extendedComboBox.AutoCompleteMode = AutoCompleteMode.None;
               _extendedComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
               _extendedComboBox.Text = item;
               _extendedComboBox.SelectionStart = 0;
               _extendedComboBox.SelectionLength = item.Length;
               ClearAndAddUniquely(collection);
               _extendedComboBox.Text = collection.Name;
               break;
            case ProvinceCollectionType.Add:
               foreach (var i in e.Composite)
                  AddIfUnique(i.Name);
               _extendedComboBox.Text = collection.Name;
               break;
            case ProvinceCollectionType.RemoveGlobal:
               _extendedComboBox.Items.Remove(item);
               _extendedComboBox.AutoCompleteCustomSource.Remove(item);
               _extendedComboBox.AutoCompleteMode = AutoCompleteMode.None;
               _extendedComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
               Clear();
               break;
            case ProvinceCollectionType.Remove:
               foreach (var i in e.Composite)
                  RemoveItem(i.Name);
               _extendedComboBox.Text = collection.Name;
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
         _flowLayout.ResumeLayout(true);
      }
   }
}