using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Editor.Forms.Feature
{
   public partial class ListDeltaSetSelection : Form
   {
      private List<string> _sourceList = [];
      private List<string> _selectionList = [];

      private readonly string[] _sourceItems = [];
      private readonly string[] _selectionItems = [];

      private bool _confirmClose = false;

      public ListDeltaSetSelection()
      {
         InitializeComponent();
      }

      public ListDeltaSetSelection(string text, string[] source, string[] selection, bool isSetCheckBox = true)
         : this()
      {
         Text = $"{text} - Delta Selection";

         _sourceList = source.ToList();
         _selectionList = selection.ToList();

         _sourceItems = source;
         _selectionItems = selection;

         SourceListBox.Items.AddRange([.. _sourceList]);
         SelectionListBox.Items.AddRange([.. _selectionList]);

         SourceListBox.SelectedIndexChanged += OnItemClicked;
         SelectionListBox.SelectedIndexChanged += OnItemClicked;

         IsSetCheckBox.Checked = isSetCheckBox;

         FormClosing += ListDeltaSetSelection_FormClosing;
         KeyDown += EscClose_KeyDown;
      }

      public bool IsSetCheckBoxChecked => IsSetCheckBox.Checked;
      public string[] SourceListBoxItems => SourceListBox.Items.Cast<string>().ToArray();
      public string[] SelectionListBoxItems => SelectionListBox.Items.Cast<string>().ToArray();

      public static void MoveItem(List<string> source, List<string> destination, ListBox sourceBox, ListBox destBox, int index)
      {
         if (index < 0 || index >= source.Count)
            return;

         var item = source[index];
         source.RemoveAt(index);
         sourceBox.Items.RemoveAt(index);
         destination.Add(item);
         destBox.Items.Add(item);
      }

      public void OnItemClicked(object? sender, EventArgs e)
      {
         Debug.Assert(_sourceList.Count == SourceListBox.Items.Count, "SourceListBox count mismatch with _sourceList count! This should not happen.");
         Debug.Assert(_selectionList.Count == SelectionListBox.Items.Count, "SelectionListBox count mismatch with _selectionList count! This should not happen.");
         if (sender is ListBox { SelectedIndex: >= 0 } listBox)
         {
            if (listBox == SourceListBox)
            {
               Debug.Assert(listBox.SelectedIndex < _sourceList.Count, "listBox.SelectedIndex < _sourceList.Count");
               MoveItem(_sourceList, _selectionList, SourceListBox, SelectionListBox, listBox.SelectedIndex);
            }
            else
            {
               Debug.Assert(listBox.SelectedIndex < _selectionList.Count, "listBox.SelectedIndex < _sourceList.Count");
               MoveItem(_selectionList, _sourceList, SelectionListBox, SourceListBox, listBox.SelectedIndex);
            }
         }

         Debug.Assert(_sourceList.Count == SourceListBox.Items.Count, "SourceListBox count mismatch with _sourceList count! This should not happen.");
         Debug.Assert(_selectionList.Count == SelectionListBox.Items.Count, "SelectionListBox count mismatch with _selectionList count! This should not happen.");

         SetDeltaToolStrip();
      }

      public void SetDeltaToolStrip()
      {
         var total = _sourceItems.Length + _selectionItems.Length;
         var sourceCount = _sourceList.Count;
         var selectionCount = _selectionList.Count;
         var (added, removed) = CountChanges();

         DeltaLabel.Text = $"Delta: {added + removed} | Added: {added} | Removed: {removed} | Source: {sourceCount} | Selected: {selectionCount} | Total: {total}";
      }

      public (List<string> added, List<string> removed) GetDelta()
      {
         var added = _selectionList.Except(_selectionItems).ToList();
         var removed = _selectionItems.Except(_selectionList).ToList();

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
            _selectionList = [.._selectionItems];
            _sourceList = [.._sourceItems];

            Debug.WriteLine("Did not modify data");
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
