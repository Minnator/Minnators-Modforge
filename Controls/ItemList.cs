using Editor.Helper;

namespace Editor.Controls
{
   public enum ItemTypes
   {
      String,
      Tag,
      Id
   }

   public partial class ItemList : UserControl
   {
      // This autoupdates when there is a tag added or removed
      private readonly TagComboBox _itemsComboBox = ControlFactory.GetTagComboBox();

      public ItemTypes ItemType { get; set; }
      public ItemList(ItemTypes type)
      {
         InitializeComponent();
         ItemType = type;

         tableLayoutPanel1.Controls.Add(_itemsComboBox, 1, 0);
         _itemsComboBox.KeyDown += ItemsComboBox_KeyDown;
         _itemsComboBox.SelectedIndexChanged += ItemsComboBox_SelectedIndexChanged;
      }

      // Create a setter for the Title
      public void SetTitle(string title)
      {
         TitleLabel.Text = title;
      }

      public void InitializeItems(List<string> items)
      {
         items.Sort();
         _itemsComboBox.Items.Clear();
         foreach (var item in items) 
            _itemsComboBox.Items.Add(item);
      }

      public List<string> GetItems()
      {
         return FlowLayout.Controls.Cast<ItemButton>().Select(button => button.Item).ToList();
      }

      public void AddIfUnique(string item)
      {
         foreach (ItemButton button in FlowLayout.Controls)
            if (button.Item == item)
               return;

         AddItem(item);
      }

      public void AddItem(string item)
      {
         FlowLayout.Controls.Add(new ItemButton(item, ItemType));
         _itemsComboBox.Text = "";

         _itemsComboBox.Focus();
      }

      //EnterPressOnItemsComboBox
      private void ItemsComboBox_KeyDown(object sender, KeyEventArgs e)
      {
      }

      // When an item is autocompleted in the combobox, add it to the list
      private void ItemsComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         var item = _itemsComboBox.SelectedItem?.ToString();
         if (item == null)
            return;

         AddIfUnique(item);
      }
   }


   public sealed class ItemButton : Button
   {
      public ItemTypes ItemType { get; set; }
      public string Item { get; set; }
      private ToolTip _toolTip = null!;

      public ItemButton(string item, ItemTypes itemType)
      {
         Item = item;
         ItemType = itemType;
         Text = item;
         Width = 48;
         Height = 21;
      }

      protected override void OnMouseClick(MouseEventArgs e)
      {
         // remove this from the partents list
         _toolTip?.RemoveAll();
         _toolTip?.Dispose();
         Parent?.Controls.Remove(this);
         Dispose();
      }

      // Add a tooltip to the button
      protected override void OnMouseHover(EventArgs e)
      {
         base.OnMouseHover(e);
         _toolTip = new();
         _toolTip.SetToolTip(this, GetToolTip());
      }

      private string GetToolTip()
      {
         switch (ItemType)
         {
            case ItemTypes.String:
               return Item;
            case ItemTypes.Tag:
               return $"{Item} ({Localisation.GetLoc(Item)})";
            case ItemTypes.Id:
               if (int.TryParse(Item, out var id))
                  if (Globals.Provinces.ContainsKey(id))
                     return $"{Item} ({Globals.Provinces[id].GetLocalisation()})";
               return Item;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }
   }

}
