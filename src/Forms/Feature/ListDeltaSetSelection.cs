using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Editor.Helper;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Forms.Feature
{
   public partial class ListDeltaSetSelection<T> : Form
   {
      public enum DisplayMode
      {
         RawString,
         DisplayMember
      }

      private List<string> _sourceList = [];
      private List<string> _selectionList = [];

      private readonly T[] _sourceObj = [];
      private readonly T[] _selectedObj = [];

      private bool _confirmClose = false;
      private DisplayMode _mode = DisplayMode.RawString;
      private string? _displayMember;
      private Timer _searchTimer;

      public DisplayMode Mode
      {
         get => _mode;
         set
         {
            _mode = value;
            _selectionList = GetObjsAsString(_selectedObj);
            _sourceList = GetObjsAsString(_sourceObj).Except(_selectionList).ToList();
            SourceListBox.Items.Clear();
            SelectionListBox.Items.Clear();
            SourceListBox.Items.AddRange([.. _sourceList]);
            SelectionListBox.Items.AddRange([.. _selectionList]);
            SourceSearchBox.Items.Clear();
            SelectionSearchBox.Items.Clear();
            SourceSearchBox.Items.AddRange([.. _sourceList]);
            SelectionSearchBox.Items.AddRange([.. _selectionList]);
         }
      }

      public string? DisplayMember
      {
         get => _displayMember;
         set => _displayMember = value;
      }

      public List<string> Selection => _selectionList;

      public ListDeltaSetSelection(string text, T[] source, T[] selection, bool isSetCheckBox = true)
      {
         InitializeComponent();
         Text = $"{text} - Delta Selection";

         _sourceObj = source;
         _selectedObj = selection;

         Mode = DisplayMode.RawString;
         StartPosition = FormStartPosition.CenterParent;

         _searchTimer = new Timer
         {
            Interval = 250
         };
         _searchTimer.Tick += (s, e) =>
         {
            _searchTimer.Stop();
            SearchActiveSearchBox();
            SetDeltaToolStrip();
         };

         SourceSearchBox.KeyDown += OnKeyPress;
         SelectionSearchBox.KeyDown += OnKeyPress;

         SourceListBox.SelectedIndexChanged += OnItemClicked;
         SelectionListBox.SelectedIndexChanged += OnItemClicked;
         
         FormClosing += ListDeltaSetSelection_FormClosing;
         KeyDown += EscClose_KeyDown;
      }

      public string[] SourceListBoxItems => SourceListBox.Items.Cast<string>().ToArray();
      public string[] SelectionListBoxItems => SelectionListBox.Items.Cast<string>().ToArray();
      public bool IsSetCheckBoxChecked => IsSetCheckBox.Checked;

      public static void MoveItem(List<string> source, List<string> destination, ListBox sourceBox, ListBox destBox, ComboBox sourceC, ComboBox selectedC, int index)
      {
         if (index < 0 || index >= source.Count)
            return;

         var item = sourceBox.Items[index].ToString()!;
         source.Remove(item);
         sourceC.Items.Remove(item);
         sourceBox.Items.RemoveAt(index);
         destination.Add(item);
         destBox.Items.Add(item);
         selectedC.Items.Add(item);
      }

      private void OnKeyPress(object? sender, KeyEventArgs e)
      {
         _searchTimer.Stop();
         _searchTimer.Start();
         if (e.KeyCode == Keys.Enter)
         {
            SearchActiveSearchBox();
         }
         else if (e.KeyCode == Keys.Escape)
         {
            _searchTimer.Stop();
            ResetSearch((ComboBox)sender!);
         }
      }

      private void ResetSearch(ComboBox sender)
      {
         sender.Text = string.Empty;
         if (sender == SourceSearchBox)
         {
            SourceListBox.Items.Clear();
            SourceListBox.Items.AddRange([.. _sourceList]);
         }
         else if (sender == SelectionSearchBox)
         {
            SelectionListBox.Items.Clear();
            SelectionListBox.Items.AddRange([.. _selectionList]);
         }
      }

      private void SearchActiveSearchBox()
      {
         _searchTimer.Stop();
         if (SourceSearchBox.Focused)
         {
            var searchText = SourceSearchBox.Text.ToLower();
            SourceListBox.Items.Clear();
            SourceListBox.Items.AddRange([.. _sourceList.Where(x => x.ToLower().Contains(searchText))]);

         }
         else if (SelectionSearchBox.Focused)
         {
            var searchText = SelectionSearchBox.Text.ToLower();
            SelectionListBox.Items.Clear();
            var items = _selectionList.Where(x => x.ToLower().Contains(searchText));
            SelectionListBox.Items.AddRange([.. items]);
         }
      }

      private List<string> GetObjsAsString(T[] objs)
      {
         if (Mode == DisplayMode.DisplayMember)
         {
            Debug.Assert(_displayMember != null, "DisplayMember is null but must not be? Is the mode set correctly");
            return objs.Select(x => x?.GetType().GetProperty(_displayMember)?.GetValue(x)?.ToString() ?? string.Empty).ToList();
         }
         return objs.Select(x => x?.ToString() ?? string.Empty).ToList();
      }

      public void OnItemClicked(object? sender, EventArgs e)
      {
         if (sender is ListBox { SelectedIndex: >= 0 } listBox)
         {
            if (listBox == SourceListBox)
            {
               Debug.Assert(listBox.SelectedIndex < _sourceList.Count, "listBox.SelectedIndex < _sourceList.Count");
               MoveItem(_sourceList, _selectionList, SourceListBox, SelectionListBox, SourceSearchBox, SelectionSearchBox, listBox.SelectedIndex);
            }
            else
            {
               Debug.Assert(listBox.SelectedIndex < _selectionList.Count, "listBox.SelectedIndex < _sourceList.Count");
               MoveItem(_selectionList, _sourceList, SelectionListBox, SourceListBox, SelectionSearchBox, SourceSearchBox, listBox.SelectedIndex);
            }
         }
         SetDeltaToolStrip();
      }

      public void SetDeltaToolStrip()
      {
         var total = _sourceObj.Length + _selectedObj.Length;
         var sourceCount = _sourceList.Count;
         var selectionCount = _selectionList.Count;
         var (added, removed) = CountChanges();

         DeltaLabel.Text = $"Delta: {added + removed} | Added: {added} | Removed: {removed} | Source: {sourceCount} | Selected: {selectionCount} | Total: {total}";
      }

      public (List<string> added, List<string> removed) GetDelta()
      {
         var selectionItems = GetObjsAsString(_selectedObj);
         var added = _selectionList.Except(selectionItems).ToList();
         var removed = selectionItems.Except(_selectionList).ToList();

         return (added, removed);
      }

      public List<string> GetSet => _selectionList;

      public (int added, int removed) CountChanges()
      {
         var delta = GetDelta();
         return (delta.added.Count, delta.removed.Count);
      }

      private void ConfirmButton_Click(object sender, EventArgs e)
      {
         _confirmClose = true;
         Close();
      }

      private void ListDeltaSetSelection_FormClosing(object? sender, FormClosingEventArgs e)
      {
         if (!_confirmClose) // if the user did not confirm then we reset to with what we started
         {
            _selectionList = [.. GetObjsAsString(_selectedObj)];
            _sourceList = [.. GetObjsAsString(_sourceObj)];
         }
      }

      private void EscClose_KeyDown(object? sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Escape)
         {
            _confirmClose = false;
            Close();
         }
      }

      [AllowNull]
      public sealed override string Text
      {
         get { return base.Text; }
         set { base.Text = value; }
      }
   }
}
