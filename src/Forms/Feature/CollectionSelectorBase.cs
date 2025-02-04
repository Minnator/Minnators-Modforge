using System.ComponentModel;
using Timer = System.Windows.Forms.Timer;

namespace Editor.src.Forms.Feature
{
   public partial class CollectionSelectorBase : Form
   {
      private const string UNSELECTABLE = "UNSELECTABLE";
      private readonly List<string> _sourceItems = [];
      private readonly List<string> _selectedItems = [];
      private readonly List<string> _sourceItemsConst;

      private Timer _searchTimer = new();
      private readonly int _maxItems = -1;

      public CollectionSelectorBase(List<string> sourceItems, int maxItems = -1)
      {
         _maxItems = maxItems;
         InitializeComponent();
         _sourceItemsConst = [.. sourceItems];

         SetSourceItems();

         SearchTextBox.AutoCompleteMode = AutoCompleteMode.None;
         SearchTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
         SearchTextBox.AutoCompleteCustomSource = new();
         SearchTextBox.AutoCompleteCustomSource.AddRange(_sourceItems.ToArray());

         SelectedSearchTextBox.AutoCompleteMode = AutoCompleteMode.None;
         SelectedSearchTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
         SelectedSearchTextBox.AutoCompleteCustomSource = new();
         SelectedSearchTextBox.AutoCompleteCustomSource.AddRange(_selectedItems.ToArray());

         _searchTimer.Interval = 300;
         _searchTimer.Tick += (sender, e) =>
         {
            _searchTimer.Stop();
            // check which textbox has Focus
            if (SearchTextBox.Focused)
               BrowseListView(SearchTextBox.Text, SourceListView, _sourceItems);
            else if (SelectedSearchTextBox.Focused)
               BrowseListView(SelectedSearchTextBox.Text, SelectedListView, _selectedItems);
         };

         SourceListView.Columns[0].Width = SourceListView.ClientSize.Width - 16;
         SelectedListView.Columns[0].Width = SelectedListView.ClientSize.Width - 16;
      }

      public new void ShowDialog()
      {
         SetSourceItems();
         base.ShowDialog();
      }

      public void SetSourceItems()
      {
         _sourceItems.Clear();
         SourceListView.Items.Clear();
         foreach (var item in _sourceItemsConst)
         {
            if (_selectedItems.Contains(item))
               continue;
            SourceListView.Items.Add(new ListViewItem(item));
            _sourceItems.Add(item);
         }
      }

      public void SetConstSourceItems(List<string> newItems)
      {
         _sourceItemsConst.Clear();
         _sourceItemsConst.AddRange(newItems);
         SetSourceItems();
      }

      internal void SetSelectedItems(List<string> items)
      {
         _selectedItems.Clear();
         SelectedListView.Items.Clear();
         foreach (var item in items)
         {
            SelectedListView.Items.Add(new ListViewItem(item));
            
            _selectedItems.Add(item);
         }

         SetSourceItems();
      }

      private void AddButton_Click(object? sender, EventArgs e)
      {
         if (SourceListView.SelectedItems.Count == 0)
            return;
         for (var i = 0; i < Math.Min(SourceListView.SelectedItems.Count, _maxItems - SelectedListView.Items.Count);)
         {
            var item = SourceListView.SelectedItems[i];
            if (item.Tag?.Equals(UNSELECTABLE) ?? false)
               return;
            SourceListView.Items.Remove(item);
            SelectedListView.Items.Add(item);
            _selectedItems.Add(item.Text);

            SelectedSearchTextBox.AutoCompleteCustomSource.Add(item.Text);
            SearchTextBox.AutoCompleteCustomSource.Remove(item.Text);
         }
      }

      private void RemoveButton_Click(object? sender, EventArgs e)
      {
         if (SelectedListView.SelectedItems.Count == 0)
            return;
         foreach (ListViewItem item in SelectedListView.SelectedItems)
         {
            SelectedListView.Items.Remove(item);
            _sourceItems.Add(item.Text);
            SourceListView.Items.Add(item);
            _selectedItems.Remove(item.Text);

            SelectedSearchTextBox.AutoCompleteCustomSource.Remove(item.Text);
            SearchTextBox.AutoCompleteCustomSource.Add(item.Text);
         }
      }

      private void MoveUpButton_Click(object? sender, EventArgs e)
      {
         if (SelectedListView.SelectedItems.Count == 0)
            return;
         foreach (ListViewItem item in SelectedListView.SelectedItems)
         {
            var index = item.Index;
            if (index == 0)
               return;
            SelectedListView.Items.Remove(item);
            SelectedListView.Items.Insert(index - 1, item);
         }
      }

      private void MoveDownButton_Click(object? sender, EventArgs e)
      {
         if (SelectedListView.SelectedItems.Count == 0)
            return;
         foreach (ListViewItem item in SelectedListView.SelectedItems)
         {
            var index = item.Index;
            if (index == SelectedListView.Items.Count - 1)
               return;
            SelectedListView.Items.Remove(item);
            SelectedListView.Items.Insert(index + 1, item);
         }
      }

      private void OkButton_Click(object? sender, EventArgs e)
      {
         DialogResult = DialogResult.OK;
         Close();
      }

      private void CancelButton_Click(object? sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
         Close();
      }

      public List<string> GetSelectedItems()
      {
         var items = new List<string>();
         foreach (ListViewItem item in SelectedListView.Items)
            items.Add(item.Text);
         return items;
      }

      private void BrowseListView(string searchString, ListView view, List<string> items)
      {
         view.Items.Clear();
         foreach (var item in items)  
            if (item.Contains(searchString))
               view.Items.Add(new ListViewItem(item));

         if (view.Items.Count == 0)
            view.Items.Add(new ListViewItem("No items found") { ForeColor = Color.Gray, Tag = UNSELECTABLE });
      }

      public void SearchButton_MouseDown(object? sender, MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left)
            BrowseListView(SearchTextBox.Text, SourceListView, _sourceItems);
         else if (e.Button == MouseButtons.Right)
         {
            SourceListView.Items.Clear();
            foreach (var item in _sourceItems)
               SourceListView.Items.Add(item);

            SearchTextBox.Clear();
         }
      }

      public void SearchTextBox_KeyDown(object? sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Enter)
            BrowseListView(SearchTextBox.Text, SourceListView, _sourceItems);
      }

      public void SelectedSearchTextBox_KeyDown(object? sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Enter)
            BrowseListView(SelectedSearchTextBox.Text, SelectedListView, _selectedItems);
      }

      public void SelectedSearchButton_MouseDown(object? sender, MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left)
            BrowseListView(SelectedSearchTextBox.Text, SelectedListView, _selectedItems);
         else if (e.Button == MouseButtons.Right)
         {
            SelectedListView.Items.Clear();
            foreach (var item in _selectedItems)
               SelectedListView.Items.Add(item);

            SelectedSearchTextBox.Clear();
         }
      }

      private void AnySearchTextBox_TextChanged(object? sender, EventArgs e)
      {
         _searchTimer.Stop();
         _searchTimer.Start();
      }

      private void SourceListView_DoubleClick(object sender, EventArgs e)
      {
         AddButton_Click(sender, e);
      }
   }
}
