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
      private List<string> _sourceList = [];
      private List<string> _selectionListList = [];

      private readonly string[] _sourceObj;

      public bool IsAdd = true;

      private Timer _searchTimer;

      public List<string> SelectionList => _selectionListList;

      public ListDeltaSetSelection(string text, string[] source)
      {
         InitializeComponent();
         Text = $"{text} - Delta Selection";

         _sourceObj = source;

         StartPosition = FormStartPosition.CenterParent;

         _searchTimer = new Timer
         {
            Interval = 2500
         };
         _searchTimer.Tick += (s, e) =>
         {
            _searchTimer.Stop();
            SearchActiveSearchBox();
         };

         SourceSearchBox.KeyDown += OnKeyPress;

         SourceListBox.SelectedIndexChanged += OnItemClicked;
         SelectionListBox.SelectedIndexChanged += OnItemClicked;

         PopulateControls(_sourceObj);


         KeyDown += EscClose_KeyDown;
      }

      private void PopulateControls(string[] input)
      {
         SourceListBox.Items.Clear();
         SourceSearchBox.Items.Clear();
         foreach (var item in input)
         {
            if (string.IsNullOrEmpty(item))
               continue;

            _sourceList.Add(item);
            SourceListBox.Items.Add(item);
            SourceSearchBox.Items.Add(item);
         }
      }

      private void MoveItem(List<string> source, ListBox sourceBox, bool fromSourceToSelection, int index)
      {
         if (index < 0 || index >= source.Count)
            return;

         var item = sourceBox.Items[index].ToString()!;
         if (fromSourceToSelection)
         {
            SelectionListBox.Items.Remove(item);
            SourceListBox.Items.Add(item);
            SourceSearchBox.Items.Add(item);
            _selectionListList.Remove(item);
         }
         else
         {
            SelectionListBox.Items.Add(item);
            SourceSearchBox.Items.Remove(item);
            SourceListBox.Items.Remove(item);
            _selectionListList.Add(item);
         }
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
         // all keys will be forced to be upper
         else
         {
            if (sender is ComboBox comboBox)
            {
               var text = comboBox.Text;
               if (!string.IsNullOrEmpty(text) && char.IsLower(text[^1]))
               {
                  comboBox.Text = text.ToUpperInvariant();
                  comboBox.SelectionStart = text.Length;
               }
            }
         }
      }

      private void ResetSearch(ComboBox sender)
      {
         sender.Text = string.Empty;
         if (sender == SourceSearchBox)
         {
            SourceListBox.Items.Clear();
            PopulateControls(_sourceObj);
         }
      }

      private void SearchActiveSearchBox()
      {
         _searchTimer.Stop();
         if (SourceSearchBox.Focused)
         {
            var searchText = SourceSearchBox.Text.ToLower();
            SourceListBox.Items.Clear();
            _sourceList = new(_sourceObj);
            PopulateControls(_sourceList.Where(x => x.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)).ToArray());
         }
      }

      public void OnItemClicked(object? sender, EventArgs e)
      {
         if (sender is ListBox { SelectedIndices.Count: >= 1 } listView)
         {
            if (listView == SourceListBox)
            {
               Debug.Assert(listView.SelectedIndices[0] < _sourceList.Count, "ListView.SelectedIndex < _sourceList.Count");
               MoveItem(_sourceList, SourceListBox, false, listView.SelectedIndices[0]);
            }
            else
            {
               Debug.Assert(listView.SelectedIndices[0] < _selectionListList.Count, "ListView.SelectedIndex < _sourceList.Count");
               MoveItem(_selectionListList, SelectionListBox, true, listView.SelectedIndices[0]);
            }
         }
      }

      private void ConfirmButton_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void EscClose_KeyDown(object? sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Escape)
         {
            SelectionList.Clear();
            SelectionList.AddRange(_selectionListList);
            Close();
         }
      }

      private void button2_Click(object sender, EventArgs e)
      {
         IsAdd = true;
         Close();
      }

      private void button1_Click(object sender, EventArgs e)
      {
         IsAdd = false;
         Close();
      }

      [AllowNull]
      public sealed override string Text
      {
         get { return base.Text; }
         set { base.Text = value; }
      }
   }
}
