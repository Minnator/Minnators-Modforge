using System.ComponentModel;

namespace Editor.Controls.EXT
{
   public partial class EXT_ListViewSearch : UserControl
   {
      public enum SearchBarLocationEnum
      {
         Hidden,
         Top,
         Bottom
      }

      private List<ListViewItem> _allItems = [];
      private SearchBarLocationEnum _searchBarLocation = SearchBarLocationEnum.Top;
      private EXT_TextBox _searchInputBox;
      private EXT_ColorableListView _itemListView;
      private Button _searchButton;

      [Category("EXT MMF Behavior")]
      public SearchBarLocationEnum SearchBarLocation
      {
         get => _searchBarLocation;
         set
         {
            _searchBarLocation = value;
            tableLayoutPanel1.Controls.Remove(_searchInputBox);
            tableLayoutPanel1.Controls.Remove(_searchButton);
            tableLayoutPanel1.Controls.Remove(_itemListView);
            switch (_searchBarLocation)
            {
               case SearchBarLocationEnum.Hidden:
                  tableLayoutPanel1.Controls.Add(_searchInputBox, 0, 0);
                  tableLayoutPanel1.SetRowSpan(_itemListView, 2);
                  break;
               case SearchBarLocationEnum.Top:
                  tableLayoutPanel1.Controls.Add(_searchInputBox, 0, 0);
                  tableLayoutPanel1.Controls.Add(_searchButton, 1, 0);
                  tableLayoutPanel1.Controls.Add(_itemListView, 0, 1);
                  break;
               case SearchBarLocationEnum.Bottom:
                  tableLayoutPanel1.Controls.Add(_itemListView, 0, 0);
                  tableLayoutPanel1.Controls.Add(_searchInputBox, 0, 1);
                  tableLayoutPanel1.Controls.Add(_searchButton, 1, 1);
                  break;
               default:
                  throw new ArgumentOutOfRangeException();
            }
            tableLayoutPanel1.SetColumnSpan(_itemListView, 2);
         }
      }

      public EXT_ListViewSearch()
      {
         InitializeComponent();

         SearchInputBox.UseTimer = true;
         SearchInputBox.ConfirmInput += (_, str) => Search(str);
         SearchInputBox.CancelInput += (_, _) => ResetSearch();

         SearchButton.MouseDown += SearchButton_MouseDown;

         _searchButton = SearchButton;
         _searchInputBox = SearchInputBox;
         _itemListView = ItemListView;
      }

      private void SearchButton_MouseDown(object? sender, MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left)
            Search(SearchInputBox.Text);
         else if (e.Button == MouseButtons.Right)
            ResetSearch();
      }

      private void Search(string str)
      {
         CacheItems();

         ItemListView.BeginUpdate();
         ItemListView.Items.Clear();

         foreach (var item in _allItems)
            if (item.Text.Contains(str, StringComparison.OrdinalIgnoreCase) ||
                item.SubItems.Cast<ListViewItem.ListViewSubItem>().Any(sub => sub.Text.Contains(str, StringComparison.OrdinalIgnoreCase)))
               ItemListView.Items.Add(item);

         ItemListView.ResizeToFitContentAndHeader();
         
         ItemListView.EndUpdate();
      }

      private void CacheItems()
      {
         if (_allItems.Count == 0) 
            _allItems = ItemListView.Items.Cast<ListViewItem>().ToList();
      }

      private void ResetSearch()
      {
         if (_allItems.Count == 0) return;

         ItemListView.BeginUpdate();
         ItemListView.Items.Clear();
         ItemListView.Items.AddRange(_allItems.ToArray());
         ItemListView.EndUpdate();
      }
   }
}
