using System.Globalization;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;

namespace Editor.Forms.Feature
{
   public partial class DefinesEditor : Form
   {
      public DefinesEditor()
      {
         InitializeComponent();

         DefineNameComboBox.DataSource = new BindingSource(Globals.Defines, null);
         DefineNameComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
         DefineNameComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

         DefineValueTextBox.Enabled = false;
         DefineNameComboBox.SelectedIndexChanged += SelectedIndexChanged;
         DefineNameComboBox.KeyDown += PreviewComboKeyDown;

         //DefinesListView.ItemListView.Columns.Add("Index", -2, HorizontalAlignment.Center);
         DefinesListView.ItemListView.Columns.Add("Namespace", -2, HorizontalAlignment.Center);
         DefinesListView.ItemListView.Columns.Add("Value", -2, HorizontalAlignment.Center);
         PopulateListView();

         DefinesListView.ItemListView.SelectedIndexChanged += ListViewSelectedIndexChange;
         DefinesListView.SearchInputBox.TimerInterval = Globals.Settings.Misc.EXT_ControlSettings.TextBoxEditConfirmationInterval;
      }

      private void PreviewComboKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Enter)
         {
            if (DefineValueTextBox.Enabled)
               DefineValueTextBox.Focus();
            e.Handled = true;
         }
         else if ((ModifierKeys & Keys.Control) != 0 && e.KeyCode == Keys.Right)
         {
            var completeText = DefineNameComboBox.Text;
            for (var i = DefineNameComboBox.SelectionStart; i < completeText.Length; i++)
            {
               if (completeText[i] == '.')
               {
                  e.Handled = true;
                  DefineNameComboBox.SelectionLength = completeText.Length - i + 1;
                  DefineNameComboBox.SelectionStart = i + 1;

                  break;
               }
            }
            if (e.Handled != true)
            {
               e.Handled = true;
               DefineNameComboBox.SelectionStart = completeText.Length;
            }
         }
         else if ((ModifierKeys & Keys.Control) != 0 && e.KeyCode == Keys.Left)
         {
            if (DefineNameComboBox.SelectionStart == 0)
            {
               e.Handled = true;
               DefineNameComboBox.SelectAll();
               return;
            }

            var completeText = DefineNameComboBox.Text;
            var start = DefineNameComboBox.SelectionStart - 1;

            if (completeText[start] == '.')
               start--;

            for (var i = start; i >= 0; i--)
            {
               if (completeText[i] == '.')
               {
                  e.Handled = true;
                  DefineNameComboBox.SelectionStart = i + 1;
                  DefineNameComboBox.SelectionLength = completeText.Length - i - 1;
                  break;
               }
            }

            if (!e.Handled)
            {
               e.Handled = true;
               DefineNameComboBox.SelectionStart = 0;
               DefineNameComboBox.SelectionLength = completeText.Length;
            }
         }
      }

      private void SelectedIndexChanged(object? sender, EventArgs e)
      {
         if (Globals.Defines.TryGetValue(DefineNameComboBox.Text, out var define))
         {
            DefineValueTextBox.Enabled = true;
            DefineValueTextBox.Text = define.GetValueAsText;
         }
         else
         {
            DefineValueTextBox.Text = "";
            DefineValueTextBox.Enabled = false;
         }
      }

      private void PopulateListView()
      {
         DefinesListView.ItemListView.Items.Clear();
         DefinesListView.ItemListView.BeginUpdate();

         var index = 0;
         foreach (var define in Globals.Defines.Values)
         {
            var item = new ListViewItem(define.GetNameSpaceString());
            item.SubItems.Add(define.GetValueAsText);
            item.Tag = define;
            DefinesListView.ItemListView.Items.Add(item);
            index++;
         }

         DefinesListView.ItemListView.MaxColumnWidth = 500;
         DefinesListView.ItemListView.EndUpdate();
      }

      private void ListViewSelectedIndexChange(object? sender, EventArgs e)
      {
         if (DefinesListView.ItemListView.SelectedItems.Count == 0)
            return;

         var define = (Define)DefinesListView.ItemListView.SelectedItems[0].Tag!;
         DefineNameComboBox.Text = define.GetNameSpaceString();
         DefineValueTextBox.Text = define.GetValueAsText;
      }

      private void SaveButton_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void CancelButton_Click(object sender, EventArgs e)
      {
         RestoreToSessionStart();
         Close();
      }

      private void RestoreToSessionStart()
      {
         // TODO
      }

      private void DefineValueTextBox_KeyDown(object sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Enter)
         {
            var define = (Define)DefinesListView.ItemListView.SelectedItems[0].Tag!;
            if (define.SetValue(DefineValueTextBox.Text))
            {
               var item = DefinesListView.ItemListView.SelectedItems[0];
               item.SubItems[1].Text = DefineValueTextBox.Text;
            }
            else
               MessageBox.Show($"The given value was not of type {define.Type} which the define has!", "Invalid value type", MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
         }
      }
   }
}
