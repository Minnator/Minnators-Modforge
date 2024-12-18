using Editor.DataClasses.GameDataClasses;
using Button = System.Windows.Forms.Button;

namespace Editor.Controls;

public static class ControlFactory
{
   #region enums

   public enum ImageButtonType
   {
      GreenPlus,
      RedMinus,
      OrangePlus,
      RedX,
      Map
   }

   #endregion

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

   public static ExtendedComboBox GetExtendedComboBox(List<string> content, int selectedIndex = -1)
   {
      var ec = new ExtendedComboBox();
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
         Width = 194,
         Text = isProvince ? id.TitleLocalisation : Globals.Countries[tag].TitleLocalisation,
         Visible = true,
         IsProvince = isProvince,
         Province = id,
         CountryTag = tag
      };
   }

   public static TagComboBox GetTagComboBox(bool ignoreEmpty = false)
   {
      return new ()
      {
         Margin = new(3, 1, 3, 3),
         Dock = DockStyle.Fill,
         IgnoreEmpty = ignoreEmpty
      };
   }

   public static ItemList GetItemList(ItemTypes itemType, List<string> items, string title)
   {
      ComboBox box = itemType == ItemTypes.Tag ? GetTagComboBox(true) : GetExtendedComboBox(items);
      var list = new ItemList(itemType, box);
      if (itemType != ItemTypes.Tag)
         list.InitializeItems(items);
      list.SetTitle(title);
      list.Margin = new(3, 1, 3, 3);
      list.Dock = DockStyle.Fill;
      return list;
   }

   public static ItemList GetItemListObjects(ItemTypes itemType, List<object> items, string title)
   {
      List<string> strings = [];

      foreach (var item in items)
      {
         var str = item.ToString();
         if (str != null)
            strings.Add(str);
      }

      if (strings.Count > 1)
         return GetItemList(itemType, strings, title);
      return new (itemType, GetTagComboBox(true));
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

   public static ExtendedComboBox GetExtendedComboBox(bool def = true)
   {
      if (def)
         return new ()
         {
            Margin = new(3, 1, 3, 3)
         };
      return new ();
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

   public static ExtendedNumeric GetExtendedNumeric()
   {
      return new ()
      {
         Margin = new(3,1,3,3)
      };
   }

   public static ColorPickerButton GetColorPickerButton()
   {
      return new ()
      {
         Margin = new (1),
         Dock = DockStyle.Fill
      };
   }
}