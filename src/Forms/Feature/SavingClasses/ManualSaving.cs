using Editor.DataClasses.Misc;
using Editor.Saving;

namespace Editor.Forms.Feature.SavingClasses
{
   public partial class ManualSaving : Form
   {
      public ManualSaving()
      {
         InitializeComponent();
         GenerateTlpCheckBoxes();
      }

      private void GenerateTlpCheckBoxes()
      {
         var names = Enum.GetNames(typeof(SaveableType));
         CheckboxesTLP.RowCount = names.Length;

         for (var i = 0; i < names.Length - 1; i++) 
            CheckboxesTLP.RowStyles.Add(new (SizeType.Absolute, 20));
         CheckboxesTLP.RowStyles.Add(new (SizeType.Percent, 100));


         for (var i = 0; i < names.Length - 1; i++)
         {
            // Is modifiable by the user
            var checkBox = new CheckBox
            {
               Text = names[i],
               Margin = new(1),
               TextAlign = ContentAlignment.MiddleLeft,
               Dock = DockStyle.Fill
            };

            checkBox.Checked = Globals.SaveableType.HasFlag((SaveableType) (1 << i));
            CheckboxesTLP.Controls.Add(checkBox, 1, i);

            var wasModifiedCheckBox = new CheckBox
            {
               Text = "",
               Margin = new(1, 1, 4, 1),
               TextAlign = ContentAlignment.MiddleRight,
               Dock = DockStyle.Fill,
               Enabled = false,
               RightToLeft = RightToLeft.Yes,
            };

            wasModifiedCheckBox.Checked = Globals.SaveableType.HasFlag((SaveableType) (1 << i));
            CheckboxesTLP.Controls.Add(wasModifiedCheckBox, 0, i);
         }
      }

      public SaveableType GetItemsToSave()
      {
         SaveableType data = 0;
         for (var i = 0; i < CheckboxesTLP.RowCount - 1; i++)
         {
            if (CheckboxesTLP.GetControlFromPosition(1, i) is not CheckBox checkBox) 
               continue;
            if (checkBox.Checked)
               data |= (SaveableType) (1 << i);
         }

         return data;
      }

      private void SaveSelectedButton_Click(object sender, EventArgs e)
      {
         SaveMaster.SaveAllChanges(saveableType: GetItemsToSave());
         for (var i = 0; i < CheckboxesTLP.RowCount; i++)
         {
            var state = Globals.SaveableType.HasFlag((SaveableType)(1 << i));
            if (CheckboxesTLP.GetControlFromPosition(0, i) is CheckBox wasModifiedCheckBox)
               wasModifiedCheckBox.Checked = state;
            if (CheckboxesTLP.GetControlFromPosition(1, i) is CheckBox checkBox)
               checkBox.Checked = state;
         }
      }

      private void ManualSaving_KeyDown(object sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Escape)
            Close();
      }

      private void MarkAllModified_Click(object sender, EventArgs e)
      {
         for (var i = 0; i < CheckboxesTLP.RowCount - 1; i++)
         {
            if (CheckboxesTLP.GetControlFromPosition(0, i) is not CheckBox toCompareBox || toCompareBox.Checked == false)
               continue;
            if (CheckboxesTLP.GetControlFromPosition(1, i) is CheckBox checkBox) 
               checkBox.Checked = true;
         }
      }

      private void UnmarkAllSelected_Click(object sender, EventArgs e)
      {
         for (var i = 0; i < CheckboxesTLP.RowCount - 1; i++)
            if (CheckboxesTLP.GetControlFromPosition(1, i) is CheckBox checkBox) 
               checkBox.Checked = false;
      }
   }
}
