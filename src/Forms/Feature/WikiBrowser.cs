
using Timer = System.Windows.Forms.Timer;

namespace Editor.Forms.Feature
{
   public partial class WikiBrowser : Form
   {
      private Timer _timer = new();

      public Action<ListView>? LoadItemsAction = null;

      public WikiBrowser() : this(null) { }

      public WikiBrowser(Action<ListView>? loadItemsAction)
      {
         InitializeComponent();
         LoadItemsAction = loadItemsAction;

         SetUpGUI();

         if (LoadItemsAction != null)
            LoadItemsAction.Invoke(EffectView);

         FormClosing += (sender, args) => _timer.Dispose();
      }

      private void SetUpGUI()
      {
         EffectView.View = View.Details;
         EffectView.FullRowSelect = true;
         EffectView.HotTracking = true;
         EffectView.MouseUp += EffectView_MouseUp;

         _timer.Interval = 300;

         _timer.Tick += OnTextChange;

         SearchTextBox.TextChanged += (sender, args) =>
         {
            _timer.Stop();
            _timer.Start();
         };
      }

      private void OnTextChange(object? sender, EventArgs eventArgs)
      {
         _timer.Stop();
         var text = SearchTextBox.Text;

         if (string.IsNullOrEmpty(text))
         {
            EffectView.Items.Clear();
            if (LoadItemsAction != null)
               LoadItemsAction.Invoke(EffectView);
            return;
         }

         List<ListViewItem> listViewItems;
         if (OnlySearchFirstColumn.Checked)
            listViewItems = SearchFirstColumn(text);
         else
            listViewItems = SearchAllColumns(text);
         listViewItems = listViewItems.OrderBy(item => item.Text).ToList();

         EffectView.Items.Clear();

         EffectView.BeginUpdate();
         foreach (var item in listViewItems)
            EffectView.Items.Add(item);
         EffectView.EndUpdate();

         EffectView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
         EffectView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
      }

      private List<ListViewItem> SearchAllColumns(string key)
      {
         key = key.ToLower();
         var result = new List<ListViewItem>();
         foreach (ListViewItem item in EffectView.Items)
         {
            var found = false;
            foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
            {
               if (subItem.Text.ToLower().Contains(key))
               {
                  found = true;
                  break;
               }
            }

            if (found)
               result.Add(item);
         }

         return result;
      }
      private List<ListViewItem> SearchFirstColumn(string key)
      {
         key = key.ToLower();
         var result = new List<ListViewItem>();
         foreach (ListViewItem item in EffectView.Items)
            if (item.Text.ToLower().Contains(key))
               result.Add(item);

         return result;
      }

      private void OnlySearchFirstColumn_CheckedChanged(object sender, EventArgs e)
      {
         OnTextChange(null, EventArgs.Empty);
      }
      
      private void EffectView_MouseUp(object? sender, MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Right)
         {
            var hitTest = EffectView.HitTest(e.Location);
            if (hitTest.Item != null && EffectView.ContextMenuStrip != null)
            {
               EffectView.ContextMenuStrip.Tag = hitTest.Item; // Store clicked item
               EffectView.ContextMenuStrip.Show(EffectView, e.Location);
            }
         }
      }

      public void SetContextMenu(ContextMenuStrip contextMenuStrip)
      {
         EffectView.ContextMenuStrip = contextMenuStrip;
      }
   }
}
