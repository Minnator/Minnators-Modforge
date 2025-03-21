﻿using Editor.DataClasses.Commands;

namespace Editor.Forms.Feature
{
   public partial class ToolTipCustomizer : Form
   {
      private int _lastSelectedIndex = -1;

      public ToolTipCustomizer()
      {
         InitGui();
         LoadOldToolTip();
      }

      private void LoadOldToolTip()
      {
         foreach (var line in Globals.Settings.ToolTip.ToolTipText.Split('\n'))
         {
            if (line.Length == 0)
               continue;
            ToolTipPreview.Items.Add(line);
         }
      }

      private void InitGui()
      {
         InitializeComponent();
         ToolTipPreview.Columns.Add(new ColumnHeader("Tooltip Row"));
         ToolTipPreview.View = View.Details;
         ToolTipPreview.FullRowSelect = true;
         ToolTipPreview.MultiSelect = false;
         // Size columns to fit
         ToolTipPreview.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
         ToolTipPreview.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

         AttributeListBox.Items.AddRange([.. ToolTipBuilder.PropertyInfo.Select(x => (object)x.Name)]);
      }

      private void AddButton_Click(object sender, System.EventArgs e)
      {
         if (InputTextBox.Text.Length == 0 || !IsValidToolTipString(InputTextBox.Text))
            return;
         // insert at last selected index
         if (_lastSelectedIndex == -1)
            ToolTipPreview.Items.Add(InputTextBox.Text);
         else
            ToolTipPreview.Items.Insert(_lastSelectedIndex, InputTextBox.Text);
         InputTextBox.Text = string.Empty;
         _lastSelectedIndex = -1;
      }

      private static bool IsValidToolTipString(string text)
      {
         var numOfDollarSigns = 0;
         foreach (var c in text)
         {
            if (c == '$')
               numOfDollarSigns++;
         }
         return numOfDollarSigns % 2 == 0;
      }

      private void RemoveButton_Click(object sender, System.EventArgs e)
      {
         if (ToolTipPreview.SelectedIndices.Count < 1)
            return;

         ToolTipPreview.Items.RemoveAt(ToolTipPreview.SelectedIndices[0]);
      }

      private void MoveUpButton_Click(object sender, System.EventArgs e)
      {
         if (ToolTipPreview.SelectedIndices.Count < 1)
            return;

         var index = ToolTipPreview.SelectedIndices[0];
         if (index == 0)
            return;

         var item = ToolTipPreview.Items[index];
         ToolTipPreview.Items.RemoveAt(index);
         ToolTipPreview.Items.Insert(index - 1, item);
         ToolTipPreview.SelectedIndices.Clear();
         ToolTipPreview.SelectedIndices.Add(index - 1);
      }

      private void MoveDownButton_Click(object sender, System.EventArgs e)
      {
         if (ToolTipPreview.SelectedIndices.Count < 1)
            return;

         var index = ToolTipPreview.SelectedIndices[0];
         if (index == ToolTipPreview.Items.Count - 1)
            return;

         var item = ToolTipPreview.Items[index];
         ToolTipPreview.Items.RemoveAt(index);
         ToolTipPreview.Items.Insert(index + 1, item);
         ToolTipPreview.SelectedIndices.Clear();
         ToolTipPreview.SelectedIndices.Add(index + 1);
      }

      private void ConfirmButton_Click(object sender, System.EventArgs e)
      {
         var str = string.Empty;
         foreach (ListViewItem item in ToolTipPreview.Items)
            str += item.Text + "\n";

         HistoryManager.AddCommand(new CChangeToolTipText(Globals.Settings.ToolTip.ToolTipText, str));
      }

      private void CancelButton_Click(object sender, System.EventArgs e)
      {
         Dispose();
      }

      private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
      {

      }

      private void ShowAttributes_Click(object sender, EventArgs e)
      {
         // Interface to show all attributes
      }

      private void AttributeListBox_MouseDoubleClick(object sender, MouseEventArgs e)
      {
         object selectedItem = AttributeListBox.SelectedItem;
         if (selectedItem == null)
            return;
         int selectionStart = InputTextBox.SelectionStart;
         string str = $"${selectedItem}$";
         InputTextBox.Text = InputTextBox.Text.Insert(selectionStart, str);
         InputTextBox.SelectionStart = checked(selectionStart + str.Length);
         InputTextBox.Focus();
      }

      private void ToolTipPreview_DoubleClick(object sender, EventArgs e)
      {
         var item = ToolTipPreview.SelectedItems[0];
         if (item == null)
            return;
         InputTextBox.Text = item.Text;
         _lastSelectedIndex = ToolTipPreview.SelectedIndices[0];
         ToolTipPreview.Items.Remove(item);
      }
   }
}
