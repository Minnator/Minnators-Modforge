using Editor.DataClasses.GameDataClasses;

namespace Editor.Controls;

public static class ControlFactory
{
   public static NumberTextBox GetNumberTextBox()
   {
      return new ();
   }

   public static PannablePictureBox GetPannablePictureBox(ref Panel panel, MapWindow mapWindow)
   {
      return new (ref panel, mapWindow);
   }

   public static ToolStripMenuItem GetToolStripMenuItem(string text, EventHandler onClick, Image image = null!)
   {
      return new(text, image, onClick);
   }

   public static SearchResultButton GetSearchResultButton(bool isProvince, int id, Tag tag)
   {
      return new ()
      {
         Margin = new (3),
         Width = 194,
         Text = isProvince ? Globals.Provinces[id].GetLocalisation() : Globals.Countries[tag].GetLocalisation(),
         Visible = true,
         IsProvince = isProvince,
         ProvinceId = id,
         CountryTag = tag
      };
   }

   public static TagComboBox GetTagComboBox()
   {
      return new ()
      {
         Margin = new(3, 1, 3, 3)
      };
   }

   public static ItemList GetItemList(ItemTypes itemType, List<string> items, string title)
   {
      var list = new ItemList(itemType);
      list.InitializeItems(items);
      list.SetTitle(title);
      list.Margin = new(3, 1, 3, 3);
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
      return new (itemType);
   }

   public static ItemButton GetTagItemButton(string item, ItemTypes type)
   {
      return new (item, type);
   }

   public static ItemButton GetStringItemButton(string item, ItemTypes type)
   {
      return new (item, type){Width = 75};
   }

   public static ExtendedComboBox GetExtendedComboBox()
   {
      return new ()
      {
         Margin = new(3, 1, 3, 3)
      };
   }

   public static ExtendedNumeric GetExtendedNumeric()
   {
      return new ()
      {
         Margin = new(3,1,3,3)
      };
   }
}