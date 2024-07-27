using System.Windows.Forms;

#nullable enable

namespace Editor.Forms.AdvancedSelections
{
   public partial class AdvancedSelectionsForm : Form
   {
      private readonly Controls.PannablePictureBox _pb;
      public AdvancedSelectionsForm(Controls.PannablePictureBox pb)
      {
         _pb = pb;
         InitializeComponent();

         InitComboBoxes();
      }

      private void InitComboBoxes()
      {
         foreach (var selection in Globals.SelectionModifiers) 
            ActionTypeComboBox.Items.Add(selection);
         ActionTypeComboBox.SelectedIndex = 0;

         foreach (var attrs in Globals.ToolTippableAttributes)
            AttributeSelectionComboBox.Items.Add(attrs);
      }

      private void ConfirmButton_Click(object sender, System.EventArgs e)
      {
         if (ActionTypeComboBox.SelectedIndex == -1 || AttributeSelectionComboBox.SelectedIndex == -1 || AttributeValueInput.Text == string.Empty)
            return;

         GetSelectionModifier()?.Execute();
      }

      private ISelectionModifier? GetSelectionModifier()
      {
         switch (ActionTypeComboBox.Text)
         {
            case "Deselection":
               return new Deselection(AttributeSelectionComboBox.Text, AttributeValueInput.Text);
            default:
               return null;
         }
      }

      private void CancelButton_Click(object sender, System.EventArgs e)
      {
         Close();
      }
   }
}
