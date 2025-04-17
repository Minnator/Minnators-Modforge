using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using Editor.Controls.PROPERTY;
using Editor.Controls.PRV_HIST;
using Editor.DataClasses.DataStructures;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Events;
using Editor.Helper;
using Editor.Saving;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using Button = System.Windows.Forms.Button;
using Image = System.Drawing.Image;

namespace Editor.Controls;

public static class ControlFactory
{
   private const string SET_ADD_TT = "If checked the history entry will SET the value.\nIf unchecked the history entry will ADD the value.";
   #region enums

   public enum ImageButtonType
   {
      GreenPlus,
      RedMinus,
      OrangePlus,
      RedX,
      Map
   }

   public enum DefaultMarginType
   {
      Default,
      Slim
   }

   #endregion

   private static void SetSetAddGeneralToolTip(Control control, string text = SET_ADD_TT)
   {
      Globals.MapWindow.GeneralToolTip.SetToolTip(control, text);
   }

   public static PrvHistNumeric GetPrvHistNumeric(string label)
   {
      return new(label)
      {
         Dock = DockStyle.Fill,
         Margin = new(0)
      };
   }

   public static PrvHistComboBox GetPrvHistComboBox(string label)
   {
      return new(label)
      {
         Dock = DockStyle.Fill,
         Margin = new(0)
      };
   }

   public static PrvHistIntUi GetPrvHistIntUi(string label, int value = 0, int min = 0, int max = 100)
   {
      var ui = new PrvHistIntUi(label, value, min, max)
      {
         Dock = DockStyle.Fill,
         Margin = new(0)
      };
      SetSetAddGeneralToolTip(ui.SetCheckBox);
      return ui;
   }

   public static PrvHistFloatUi GetPrvHistFloatUi(string label, float value = 0, float min = 0, float max = 100)
   {
      var ui = new PrvHistFloatUi(label, value, min, max)
      {
         Dock = DockStyle.Fill,
         Margin = new(0)
      };
      SetSetAddGeneralToolTip(ui.SetCheckBox);
      return ui;
   }


   public static PrvHistSetAddUi GetPrvHistUi(string label, Control control, bool hasSetBox = false)
   {
      var ui = new PrvHistSetAddUi(label, control, hasSetBox)
      {
         Dock = DockStyle.Fill,
         Margin = new(0)
      };
      if (hasSetBox)
         SetSetAddGeneralToolTip(ui.SetCheckBox);
      return ui;
   }

   public static PrvHistBoolUi GetPrvHistBoolUi(string label, bool isChecked)
   {
      var ui = new PrvHistBoolUi(label, isChecked)
      {
         Dock = DockStyle.Fill,
         Margin = new(0)
      };
      return ui;
   }
   
   public static PrvHistCheckBox GetPrvHistCheckBox(string label)
   {
      return new(label)
      {
         Dock = DockStyle.Fill,
         Margin = new(0)
      };
   }

   public static PrvHistTagBox GetPrvHistTagBox(string label)
   {
      return new(label)
      {
         Dock = DockStyle.Fill,
         Margin = new(0)
      };
   }

   public static PrvHistSeparator GetDefaultSeparator() => GetPrvHistSeparator(Color.Black, 2);

   public static PrvHistSeparator GetPrvHistSeparator(Color color, int thickness)
   {
      return new()
      {
         Color = color,
         Thickness = thickness,
         Dock = DockStyle.Fill,
         Margin = new(0)
      };
   }


   public static ProvinceHistoryEntryTreeView GetProvinceHistoryTreeView()
   {
      return new(typeof(Province).GetProperty(nameof(Province.History)), ref LoadGuiEvents.ProvLoadAction, () => Selection.GetSelectedProvinces);
   }

   public static PropertyTreeView<Province, TProperty, TItem> GetPropertyTreeView<TProperty, TItem>(
      PropertyInfo? propertyInfo,
      ref LoadGuiEvents.LoadAction<Province> loadHandle)
      where TProperty : List<TItem>
   {
      return new(propertyInfo,
                 ref loadHandle,
                 () => Selection.GetSelectedProvinces);
   }

   public static PropertyQuickAssignControl<CommonCountry, TProperty, TItem> GetPropertyQuickAssignControlCC<TProperty, TItem>(List<TItem> source,
      PropertyInfo prop,
      PropertyInfo? displayMember,
      string description,
      int maxItems,
      Func<int, List<TItem>> autoSelect) where TItem : notnull where TProperty : List<TItem>, new()
   {
      return new(
         source,
         prop,
         displayMember,
         description,
         maxItems,
         autoSelect,
         () => [Selection.SelectedCountry.CommonCountry],
         ref LoadGuiEvents.CommonCountryLoadAction);
   }

   public static PropertyQuickAssignControl<HistoryCountry, TProperty, TItem> GetPropertyQuickAssignControlHC<TProperty, TItem>(List<TItem> source,
      PropertyInfo prop,
      PropertyInfo? displayMember,
      string description,
      int maxItems,
      Func<int, List<TItem>> autoSelect) where TItem : notnull where TProperty : List<TItem>, new()
   {
      return new(
         source,
         prop,
         displayMember,
         description,
         maxItems,
         autoSelect,
         () => [Selection.SelectedCountry.HistoryCountry],
         ref LoadGuiEvents.HistoryCountryLoadAction);
   }

   public static PropertyNamesEditor<CommonCountry, TProperty> GetPropertyNamesEditorCommonCountry<TProperty>(PropertyInfo? propertyInfo, string desc) where TProperty : List<string>, new()
   {
      return new(propertyInfo, ref LoadGuiEvents.CommonCountryLoadAction, () => [Selection.SelectedCountry.CommonCountry], desc)
      {
         Dock = DockStyle.Fill,
         Margin = new(0)
      };
   }

   public static PropertyCheckBox<Province> GetExtendedCheckBoxProvince(PropertyInfo propInfo)
   {
      return new(propInfo, ref LoadGuiEvents.ProvLoadAction, () => Selection.GetSelectedProvinces)
      {
         Margin = new(1)
      };
   }

   public static PropertyCheckBox<HistoryCountry> GetExtendedCheckBoxCountry(PropertyInfo propInfo)
   {
      return new(propInfo, ref LoadGuiEvents.HistoryCountryLoadAction, () => [Selection.SelectedCountry.HistoryCountry])
      {
         Margin = new(1)
      };
   }

   public static FlagLabel GetFlagLabel()
   {
      return new(Country.Empty);
   }

   public static MapModeButton GetMapModeButton(char hotkey, int mapModeIndex)
   {
      return new (hotkey, mapModeIndex);
   }

   public static ThreeColorStripesButton GetThreeColorsButton()
   {
      return new ()
      {
         Margin = new(1),
         Dock = DockStyle.Fill
      };
   }

   public static Button GetImageButton(ImageButtonType ibType, string toolTip)
   {
      Button button = new()
      {
         Text = string.Empty,
         Image = ibType switch
         {
            ImageButtonType.GreenPlus => Properties.Resources.GreenPlusBg,
            ImageButtonType.RedMinus => Properties.Resources.RedMinus,
            ImageButtonType.OrangePlus => Properties.Resources.OrangePlus,
            ImageButtonType.RedX => Properties.Resources.RedX,
            ImageButtonType.Map => Properties.Resources.map,
            _ => null
         },
         Size = new(26, 26),
         Dock = DockStyle.Fill,
         Margin = new(1, 3, 1, 3),
      };
      
      Globals.MapWindow.GeneralToolTip.SetToolTip(button, toolTip);

      return button;
   }

   public static NumberTextBox GetNumberTextBox()
   {
      return new ();
   }

   public static ExtendedComboBox GetExtendedComboBox(string propName, List<string> content, int selectedIndex = -1)
   {
      var ec = new ExtendedComboBox(propName);
      if (content.Count == 0)
         return ec;
      ec.Items.AddRange([..content]);
      if (selectedIndex >= 0 && selectedIndex < ec.Items.Count)
         ec.SelectedIndex = selectedIndex;
      return ec;
   }

   public static ToolStripMenuItem GetToolStripMenuItem(string text, EventHandler onClick, Image image = null!)
   {
      return new(text, image, onClick);
   }

   public static SearchResultButton GetSearchResultButton(bool isProvince, Province id, Tag tag)
   {
      return new ()
      {
         Margin = new (3),
         Width = 188,
         Text = isProvince ? id.TitleLocalisation : Globals.Countries[tag].TitleLocalisation,
         Visible = true,
         IsProvince = isProvince,
         Province = id,
         CountryTag = tag
      };
   }

   public static TagComboBox GetTagComboBox(bool ignoreEmpty = false)
   {
      return new()
      {
         Margin = new(3, 1, 3, 3),
         Dock = DockStyle.Fill,
         IgnoreEmpty = ignoreEmpty
      };
   }

   public static BindablePropertyComboBox<Province, TProperty, TKey> GetBindablePropertyComboBox<TProperty, TKey>(PropertyInfo propInfo, BindingDictionary<TKey, TProperty> items, bool hasEmptyItemAt0 = true) where TKey : notnull where TProperty : notnull
   {
      return new(propInfo, ref LoadGuiEvents.ProvLoadAction, () => Selection.GetSelectedProvinces, items)
      {
         Margin = new(3, 1, 3, 3),
         Dock = DockStyle.Fill,
         AutoCompleteSource = AutoCompleteSource.ListItems,
         AutoCompleteMode = AutoCompleteMode.SuggestAppend,
         ContextMenuStrip = new(),
      };
   }   
   public static BindablePropertyComboBox<CommonCountry, TProperty, TKey> GetBindablePropertyComboBoxCommonCountry<TProperty, TKey>(PropertyInfo propInfo, BindingDictionary<TKey, TProperty> items, bool hasEmptyItemAt0 = true) where TKey : notnull where TProperty : notnull
   {
      return new(propInfo, ref LoadGuiEvents.CommonCountryLoadAction, () => [Selection.SelectedCountry.CommonCountry], items)
      {
         Margin = new(3, 1, 3, 3),
         Dock = DockStyle.Fill,
         AutoCompleteSource = AutoCompleteSource.ListItems,
         AutoCompleteMode = AutoCompleteMode.SuggestAppend,
         ContextMenuStrip = new(),
      };
   }   
   public static BindablePropertyComboBox<HistoryCountry, TProperty, TKey> GetBindablePropertyComboBoxHistoryCountry<TProperty, TKey>(PropertyInfo propInfo,
      BindingDictionary<TKey, TProperty> items,
      bool hasEmptyItemAt0 = true,
      DefaultMarginType margin = DefaultMarginType.Default) where TKey : notnull where TProperty : notnull
   {
      BindablePropertyComboBox<HistoryCountry, TProperty, TKey> box = new(propInfo, ref LoadGuiEvents.HistoryCountryLoadAction, () => [Selection.SelectedCountry.HistoryCountry], items)
      {
         Margin = new(3, 1, 3, 3),
         Dock = DockStyle.Fill,
         AutoCompleteSource = AutoCompleteSource.ListItems,
         AutoCompleteMode = AutoCompleteMode.SuggestAppend,
         ContextMenuStrip = new(),
      };
      if (margin == DefaultMarginType.Slim)
         box.Margin = new(1);
      return box;
   }
   public static BindableListPropertyComboBox<CommonCountry, TProperty> GetBindableListPropertyComboBox<TProperty>(PropertyInfo propInfo,
                                                                                                                   BindingList<TProperty> items,
                                                                                                                   DefaultMarginType margin = DefaultMarginType.Default) where TProperty : notnull
   {
      BindableListPropertyComboBox<CommonCountry, TProperty> box =
         new(propInfo,
             ref LoadGuiEvents.CommonCountryLoadAction,
             () => [Selection.SelectedCountry.CommonCountry],
             items)
         {
            Margin = new(3, 1, 3, 3),
            Dock = DockStyle.Fill,
            AutoCompleteSource = AutoCompleteSource.ListItems,
            AutoCompleteMode = AutoCompleteMode.SuggestAppend,
            ContextMenuStrip = new(),
         };
      if (margin == DefaultMarginType.Slim)
         box.Margin = new(1);
      return box;
   }

   public static ListPropertyComboBox<HistoryCountry, TProperty> GetListPropertyComboBoxHistoryCountry<TProperty>(PropertyInfo propInfo,
                                                                                                                   List<TProperty> items,
                                                                                                                   DefaultMarginType margin = DefaultMarginType.Default) where TProperty : notnull
   {
      ListPropertyComboBox<HistoryCountry, TProperty> box =
         new(propInfo,
             ref LoadGuiEvents.HistoryCountryLoadAction,
             () => [Selection.SelectedCountry.HistoryCountry],
             items)
         {
            Margin = new(3, 1, 3, 3),
            Dock = DockStyle.Fill,
            AutoCompleteSource = AutoCompleteSource.ListItems,
            AutoCompleteMode = AutoCompleteMode.SuggestAppend,
            ContextMenuStrip = new(),
         };
      if (margin == DefaultMarginType.Slim)
         box.Margin = new(1);
      return box;
   }

   public static BindablePropertyComboBox<Province, TProperty, TKey> GetTagComboBox<TProperty, TKey>(PropertyInfo propInfo, BindingDictionary<TKey, TProperty> items, bool hasEmptyItemAt0 = true) where TKey : notnull where TProperty : ProvinceCollection<Province>
   {
      var box = GetBindablePropertyComboBox(propInfo, items, hasEmptyItemAt0);
      box.Margin = new(3, 1, 3, 3);

      box.KeyPress += (sender, e) =>
      {
         if (!char.IsLetter(e.KeyChar) && e.KeyChar != (char)Keys.Back)
         {
            e.Handled = true; // Ignore non-letter keys except backspace
            return;
         }

         string newText;

         if (e.KeyChar == (char)Keys.Back)
         {
            // Handle backspace: remove selected text or last character
            if (box.SelectionLength > 0)
               newText = box.Text.Remove(box.SelectionStart, box.SelectionLength);
            else if (box.Text.Length > 0)
               newText = box.Text[..^1];
            else
               newText = string.Empty;
         }
         else
            newText = box.Text.Remove(box.SelectionStart, box.SelectionLength)
                         .Insert(box.SelectionStart, char.ToUpper(e.KeyChar).ToString());

         // Enforce max length of 3
         if (newText.Length > 3)
         {
            e.Handled = true;
            return;
         }

         box.Text = newText;
         box.SelectionStart = box.Text.Length;
         box.SelectionLength = 0;
         e.Handled = true; 
      };
      return box;
   }

   public static BindableFakePropertyComboBox<Province, TProperty, TKey> GetBindableFakePropertyComboBox<TProperty, TKey>(PropertyInfo propInfo, BindingDictionary<TKey, TProperty> items) where TKey : notnull where TProperty : ProvinceCollection<Province>
   {
      return new(propInfo, ref LoadGuiEvents.ProvLoadAction, () => Selection.GetSelectedProvinces, items)
      {
         Margin = new(3, 1, 3, 3)
      };
   }


   public static ItemList GetItemList(string propName, ItemTypes itemType, List<string> items, string title)
   {
      ComboBox box = itemType == ItemTypes.Tag ? GetTagComboBox(true) : GetExtendedComboBox(propName, items);
      var list = new ItemList(propName, itemType, box);
      if (itemType != ItemTypes.Tag)
         list.InitializeItems(items);
      list.SetTitle(title);
      list.Margin = new(3, 1, 3, 3);
      list.Dock = DockStyle.Fill;
      return list;
   }

   public static ItemList GetItemListObjects(string propName, ItemTypes itemType, List<object> items, string title)
   {
      List<string> strings = [];

      foreach (var item in items)
      {
         var str = item.ToString();
         if (str != null)
            strings.Add(str);
      }

      if (strings.Count > 1)
         return GetItemList(propName, itemType, strings, title);
      return new (propName, itemType, GetTagComboBox(true));
   }

   public static ItemButton GetItemButton(string item, ItemTypes type)
   {
      return new (item, type) { Width = 41, Height = 25 };
   }

   public static ItemButton GetItemButtonLong(string item, ItemTypes type)
   {
      return new(item, type) { Width = 110, Height = 25 };
   }

   public static ItemButton GetTagItemButton(string item, ItemTypes type)
   {
      return new (item, type);
   }

   public static ItemButton GetItemButtonFullWidth(string item, ItemTypes type, int width)
   {
      return new (item, type) { Width = width, Height = 25 };
   }

   public static ItemButton GetStringItemButton(string item, ItemTypes type)
   {
      return new (item, type){Width = 75, Height = 25};
   }

   public static PropertyComboBox<Province, TProperty> SimpleComboBoxProvince<TProperty>(PropertyInfo propertyInfo, bool useDefaultMargin = true)
   {
      if (useDefaultMargin)
         return new(propertyInfo, ref LoadGuiEvents.ProvLoadAction, () => Selection.GetSelectedProvinces)
         {
            Margin = new(3, 1, 3, 3)
         };
      return new(propertyInfo, ref LoadGuiEvents.ProvLoadAction, () => Selection.GetSelectedProvinces);
   }


   public static PropertyNumeric<Province, TProperty> GetPropertyNumeric<TProperty>(PropertyInfo? propertyInfo, TProperty defaultValue, decimal multiplier = 1) where TProperty : INumber<TProperty>
   {
      return new(propertyInfo, ref LoadGuiEvents.ProvLoadAction, () => Selection.GetSelectedProvinces, multiplier)
      {
         Margin = new(3, 1, 3, 3),
         Dock = DockStyle.Fill,
         DefaultValue = defaultValue,
         ContextMenuStrip = new(),
      };
   }

   public static PropertyNumeric<Country, TProperty> GetPropertyNumericCountry<TProperty>(PropertyInfo? propertyInfo, TProperty defaultValue, decimal multiplier = 1) where TProperty : INumber<TProperty>
   {
      return new(propertyInfo, ref LoadGuiEvents.CountryLoadAction, () => [Selection.SelectedCountry], multiplier)
      {
         Margin = new(3, 1, 3, 3),
         Dock = DockStyle.Fill,
         DefaultValue = defaultValue,
         ContextMenuStrip = new(),
      };
   }
   
   public static LocalisationTextBox<Province> GetProvinceLocBox(PropertyInfo? propertyInfo)
   {
      return new(propertyInfo, ref LoadGuiEvents.ProvLoadAction, () => Selection.GetSelectedProvinces)
      {
         Margin = new(3, 1, 3, 3),
         Dock = DockStyle.Fill,
         ContextMenuStrip = new(),
      };
   }

   public static PropertyTextBox<Province> GetPropertyTextBox(PropertyInfo? propertyInfo)
   {
      return new(propertyInfo, ref LoadGuiEvents.ProvLoadAction, () => Selection.GetSelectedProvinces)
      {
         Margin = new(3, 1, 3, 3),
         Dock = DockStyle.Fill,
         ContextMenuStrip = new(),
      };
   }
   public static PropertyTextBox<Country> GetPropertyTextBoxCountry(PropertyInfo? propertyInfo, DefaultMarginType margin = DefaultMarginType.Default)
   {
      PropertyTextBox<Country> box = new(propertyInfo, ref LoadGuiEvents.CountryLoadAction, () => [Selection.SelectedCountry])
      {
         Margin = new(3, 1, 3, 3),
         Dock = DockStyle.Fill,
         ContextMenuStrip = new(),
      };
      if (margin == DefaultMarginType.Slim)
         box.Margin = new(1);
      return box;
   }
   public static PropertyTextBox<HistoryCountry> GetPropertyTextBoxHistoryCountry(PropertyInfo? propertyInfo)
   {
      return new(propertyInfo, ref LoadGuiEvents.HistoryCountryLoadAction, () => [Selection.SelectedCountry.HistoryCountry])
      {
         Margin = new(3, 1, 3, 3),
         Dock = DockStyle.Fill
      };
   }

   public static PropertyLabel<Province> GetPropertyLabel(PropertyInfo? propertyInfo)
   {
      return new(propertyInfo, ref LoadGuiEvents.ProvLoadAction)
      {
         Margin = new(3, 1, 3, 3),
         Dock = DockStyle.Fill,
         TextAlign = ContentAlignment.MiddleLeft
      };
   }
   public static PropertyLabel<HistoryCountry> GetPropertyLabelHistoryCountry(PropertyInfo? propertyInfo)
   {
      return new(propertyInfo, ref LoadGuiEvents.HistoryCountryLoadAction)
      {
         Margin = new(3, 1, 3, 3),
         Dock = DockStyle.Fill,
         TextAlign = ContentAlignment.MiddleLeft
      };
   }

   public static ExtendedComboBox GetExtendedComboBox(string propName, bool def = true)
   {
      if (def)
         return new (propName)
         {
            Margin = new(3, 1, 3, 3)
         };
      return new (propName);
   }

   public static ComboBox GetListComboBox(List<string> items, Padding margin, bool hasEmptyItemAt0 = true)
   {
      var cb = new ComboBox
      {
         Margin = margin,
         Dock = DockStyle.Fill,
         DropDownStyle = ComboBoxStyle.DropDownList
      };
      if (hasEmptyItemAt0)
         cb.Items.Add(string.Empty);
      cb.Items.AddRange([..items]);
      return cb;
   }

   public static ExtendedNumeric GetExtendedNumeric(string propName)
   {
      return new (propName)
      {
         Margin = new(3,1,3,3)
      };
   }

   public static PropertyColorButton<CommonCountry> GetColorPickerButtonCommonCountry(PropertyInfo? propertyInfo)
   {
      return new(propertyInfo, ref LoadGuiEvents.CommonCountryLoadAction,
         () => [Selection.SelectedCountry.CommonCountry])
      {
         Margin = new(1),
         Dock = DockStyle.Fill
      };
   }

   public static ColorPickerButton GetColorPickerButton()
   {
      return new()
      {
         Margin = new(1),
         Dock = DockStyle.Fill
      };
   }
}