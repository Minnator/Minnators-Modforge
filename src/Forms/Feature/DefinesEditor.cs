using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Security.Cryptography.Certificates;
using Editor.DataClasses.GameDataClasses;
using static Editor.DataClasses.GameDataClasses.Define;

namespace Editor.src.Forms.Feature
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


         DefinesListView.Columns.Add("Index");
         DefinesListView.Columns.Add("Namespace");
         DefinesListView.Columns.Add("Value");
         PopulateListView();
         DefinesListView.SelectedIndexChanged += ListViewSelectedIndexChange;

         DefinesListView.Refresh();
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
            DefineValueTextBox.Text = define.Value.ToString()!;
         }
         else
         {
            DefineValueTextBox.Text = "";
            DefineValueTextBox.Enabled = false;
         }
      }

      private void PopulateListView()
      {
         DefinesListView.Items.Clear();
         DefinesListView.BeginUpdate();

         var index = 0;
         foreach (var define in Globals.Defines.Values)
         {
            var item = new ListViewItem(index.ToString());
            item.SubItems.Add(define.GetNameSpaceString());
            item.SubItems.Add(define.Value.ToString());
            item.Tag = define;
            DefinesListView.Items.Add(item);
            index++;
         }

         DefinesListView.EndUpdate();

         DefinesListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
         DefinesListView.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
         if (DefinesListView.Columns[1].Width > 450)
            DefinesListView.Columns[1].Width = 450;
         DefinesListView.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      }

      private void ListViewSelectedIndexChange(object? sender, EventArgs e)
      {
         if (DefinesListView.SelectedItems.Count == 0)
            return;

         var define = (Define)DefinesListView.SelectedItems[0].Tag!;
         DefineNameComboBox.Text = define.GetNameSpaceString();
         DefineValueTextBox.Text = define.Value.ToString();
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
   }
}
