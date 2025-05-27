using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.src.Forms.PopUps;
using Newtonsoft.Json.Linq;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Forms.Feature
{
   public partial class ListDeltaSetSelection : Form
   {
      private struct ListMaster
      {
         public ListMaster()
         {

         }
         public List<string> SharedAdd = [];
         public List<string> SharedRemove = [];
         public List<string> SharedContained = [];
         public List<string> SharedNotContained = [];
         public List<string> PartiallyAdded = [];
         public List<string> PartiallyRemoved = [];
         public List<string> PartiallyContained = [];
         public List<string> PartiallyNotContained = [];
      }

      private class ItemAmounts {
         public int Added = 0;
         public int Removed = 0;
         public int PreContained = 0; //includes added
         public int Contained => PreContained - Added;
         public int Remaining(int numberOfItems)
         {
            return numberOfItems - Contained - Removed;
         }
         public void AddToList(ListMaster master, int numberOfItems, string name)
         {
            if (Added == numberOfItems)
               master.SharedAdd.Add(name);
            else if (Removed == numberOfItems)
               master.SharedRemove.Add(name);
            else if (Contained == numberOfItems)
               master.SharedContained.Add(name);
            else if (Remaining(numberOfItems) == 0)
               master.SharedNotContained.Add(name);
            else
            {
               if (Added > 0)
                  master.PartiallyAdded.Add(name);
               if (Removed > 0)
                  master.PartiallyRemoved.Add(name);
               if (Contained > 0)
                  master.PartiallyContained.Add(name);
               if (Remaining(numberOfItems) > 0)
                  master.PartiallyNotContained.Add(name);
            }
         }
      }

      public enum DisplayMode
      {
         RawString,
         DisplayMember
      }

      private List<string> _sourceList = [];
      private List<string> _selectionListList = [];

      private readonly string[] _sourceObj;
      private readonly string[] _selectedObj;

      private bool _confirmClose = false;
      private DisplayMode _mode = DisplayMode.RawString;
      private string? _displayMember;
      private Timer _searchTimer;

      private string addName;
      private string rmvName;

      public DisplayMode Mode
      {
         get => _mode;
         set
         {
            _mode = value;
            _selectionListList = GetObjsAsString(_selectedObj);
            _sourceList = GetObjsAsString(_sourceObj).Except(_selectionListList).ToList();
            SourceListView.Items.Clear();
            SelectionListView.Items.Clear();
            PopulateListView(_sourceObj, SourceListView);
            PopulateListView(_selectedObj, SelectionListView);
            SourceSearchBox.Items.Clear();
            SelectionSearchBox.Items.Clear();
            SourceSearchBox.Items.AddRange([.. _sourceList]);
            SelectionSearchBox.Items.AddRange([.. _selectionListList]);
         }
      }

      public List<string> SelectionList => _selectionListList;

      public ListDeltaSetSelection(string text, string[] source, string addName, string rmvName)
      {
         this.addName = addName;
         this.rmvName = rmvName;

         InitializeComponent();
         Text = $"{text} - Delta Selection";

         _sourceObj = source;
         _selectedObj = [];

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

         SourceListView.Columns.Add("items");
         SelectionListView.Columns.Add("items");

         SourceSearchBox.KeyDown += OnKeyPress;
         SelectionSearchBox.KeyDown += OnKeyPress;

         SourceListView.SelectedIndexChanged += OnItemClicked;
         SelectionListView.SelectedIndexChanged += OnItemClicked;
         
         FormClosing += ListDeltaSetSelection_FormClosing;
         KeyDown += EscClose_KeyDown;
      }

      private void ClassifyItems()
      {
         foreach (var province in Selection.GetSelectedProvinces)
         {
            var index = ProvinceHistoryManager.BinarySearchDateExact(province.History, ProvinceHistoryManager.CurrentLoadedDate);
            if (index == -1)
               continue;
            var entry = province.History[index];

            

         }
      }

      // assumes no wrong add or rmv... So no add and then a remove or add when it is already added
      private (int add, int rmv) GetSharedAddRmvItems<T>(ProvinceHistoryEntry entry)
      {
         var add = 0;
         var rmv = 0;
         // rmv rmv = rmv / contained: rmv add = 0 / not contained: rmv add = add
         // add rmv = rmv = 0

         // add      partially added und partially contained
         // contained 
         var addTokens = new List<string>();
         var rmvTokens = new List<string>();
         var containsTokens = new List<string>();
         


         foreach (var rawToken in entry.Effects)
         {
            if (rawToken is not SimpleEffect<T> token)
               continue;
            if (token.GetTokenName().Equals(addName))
            {
               var tokenVal = token._value.Val;
               var tokenName = tokenVal?.ToString();

               Debug.Assert(tokenVal != null, "tokenVal != null");
               Debug.Assert(tokenName != null, "tokenName != null");

               var indexOfRmv = rmvTokens.IndexOf(tokenName);

               if (indexOfRmv > -1) // maybe combine without searching index twice
               {
                  rmvTokens.RemoveAt(indexOfRmv);
                  continue;
               }

               if (addTokens.Contains(tokenName))
               {
                  continue;
               }
            }
            else if (token.GetTokenName().Equals(rmvName))
            {
               var tokenVal = token._value.Val;
               var tokenName = tokenVal?.ToString();

               Debug.Assert(tokenVal != null, "tokenVal != null");
               Debug.Assert(tokenName != null, "tokenName != null");

               var indexOfAdd = addTokens.IndexOf(tokenName);

               if (indexOfAdd > -1) // maybe combine without searching index twice
               {
                  rmvTokens.RemoveAt(indexOfAdd);
                  continue;
               }
               if (addTokens.Contains(token.GetTokenName()))
               {
                  // if we have an add and a remove then we remove the add from the list
                  addTokens.Remove(token.GetTokenName());
                  add--;
               }
               else
               {
                  rmv++;
                  rmvTokens.Add(token.GetTokenName());
               }
            }
         }

         return (add, rmv);
      }

      private void PopulateListView(string[] input, ListView view)
      {
         view.Items.Clear();
         if (Mode == DisplayMode.DisplayMember && _displayMember != null)
         {
            foreach (var item in input)
            {
               var value = item.GetType().GetProperty(_displayMember)?.GetValue(item)?.ToString() ?? string.Empty;
               view.Items.Add(value);
            }
         }
         else
         {
            foreach (var item in input) 
               view.Items.Add(item);
         }
      }

      public string[] SourceListViewItems => SourceListView.Items.Cast<string>().ToArray();
      public string[] SelectionListViewItems => SelectionListView.Items.Cast<string>().ToArray();

      public static void MoveItem(List<string> source, List<string> destination, ListView sourceBox, ListView destBox, ComboBox sourceC, ComboBox selectedC, int index)
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
            SourceListView.Items.Clear();
            PopulateListView(_sourceObj, SourceListView);
         }
         else if (sender == SelectionSearchBox)
         {
            SelectionListView.Items.Clear();
            PopulateListView(_selectedObj, SelectionListView);
         }
      }

      private void SearchActiveSearchBox()
      {
         _searchTimer.Stop();
         if (SourceSearchBox.Focused)
         {
            var searchText = SourceSearchBox.Text.ToLower();
            SourceListView.Items.Clear();
            PopulateListView(_sourceList.Where(x => x.ToLower().Contains(searchText)).ToArray(), SourceListView);

         }
         else if (SelectionSearchBox.Focused)
         {
            var searchText = SelectionSearchBox.Text.ToLower();
            SelectionListView.Items.Clear();
            var items = _selectionListList.Where(x => x.ToLower().Contains(searchText));
            PopulateListView(items.ToArray(), SelectionListView);
         }
      }

      private List<string> GetObjsAsString(string[] objs)
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
         if (sender is ListView { SelectedIndices.Count: >= 1 } listView)
         {
            if (listView == SourceListView)
            {
               Debug.Assert(listView.SelectedIndices[0] < _sourceList.Count, "ListView.SelectedIndex < _sourceList.Count");
               MoveItem(_sourceList, _selectionListList, SourceListView, SelectionListView, SourceSearchBox, SelectionSearchBox, listView.SelectedIndices[0]);
            }
            else
            {
               Debug.Assert(listView.SelectedIndices[0] < _selectionListList.Count, "ListView.SelectedIndex < _sourceList.Count");
               MoveItem(_selectionListList, _sourceList, SelectionListView, SourceListView, SelectionSearchBox, SourceSearchBox, listView.SelectedIndices[0]);
            }
         }
         SetDeltaToolStrip();
      }

      public void SetDeltaToolStrip()
      {
         var total = _sourceObj.Length + _selectedObj.Length;
         var sourceCount = _sourceList.Count;
         var selectionCount = _selectionListList.Count;
         var (added, removed) = CountChanges();

         DeltaLabel.Text = $"Delta: {added + removed} | Added: {added} | Removed: {removed} | Source: {sourceCount} | Selected: {selectionCount} | Total: {total}";
      }

      public (List<string> added, List<string> removed) GetDelta()
      {
         var selectionItems = GetObjsAsString(_selectedObj);
         var added = _selectionListList.Except(selectionItems).ToList();
         var removed = selectionItems.Except(_selectionListList).ToList();

         return (added, removed);
      }

      public List<string> GetSet => _selectionListList;

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
            _selectionListList = [.. GetObjsAsString(_selectedObj)];
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
