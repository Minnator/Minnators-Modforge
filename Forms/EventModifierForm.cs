using Editor.Controls;
using Editor.Helper;
using Editor.Parser;
using ComboBox = System.Windows.Forms.ComboBox;

namespace Editor.Forms
{
   public partial class EventModifierForm : Form
   {
      private DualSelectionFlowPanel _customAttrPanel;
      private DualSelectionFlowPanel _modifierPanel;

      private readonly ComboBox tempBox;
      public EventModifierForm()
      {
         StartPosition = FormStartPosition.CenterScreen;
         InitializeComponent();

         EventModNameTL.Controls.Add(Globals.MapWindow._modifierComboBox, 1, 0);
         tempBox = new ()
         {
            Dock = DockStyle.Fill,
            Margin = Globals.MapWindow._modifierComboBox.Margin
         };
         Globals.MapWindow.ModifiersLayoutPanel.Controls.Add(tempBox, 1, 1);
         Globals.MapWindow._modifierComboBox.SelectedIndexChanged += OnModifierComboBoxOnSelectedIndexChanged;

         Closing += CloseEvent;

         // GUI
         _customAttrPanel = new();
         _customAttrPanel.SuspendLayout();
         _customAttrPanel.Dock = DockStyle.Fill;
         _customAttrPanel.SetComboBoxItems([..ModifierParser.CustomModifierTrigger], DualSelectionFlowPanel.BoxType.Left, true);
         _customAttrPanel.SetComboBoxItems(["yes", "no"], DualSelectionFlowPanel.BoxType.Right, true);
         _modifierPanel = new();
         _modifierPanel.Dock = DockStyle.Fill;
         List<string> modKeywords = [];
         modKeywords.AddRange(ModifierParser.CountryModifiers);
         modKeywords.AddRange(ModifierParser.ProvinceModifiers);
         _modifierPanel.SetComboBoxItems(modKeywords, DualSelectionFlowPanel.BoxType.Left, true);

         CustomAttributesLP.Controls.Add(_customAttrPanel, 1, 0);
         ModifiersLP.Controls.Add(_modifierPanel, 1, 0);

         _customAttrPanel.ResumeLayout(true);
      }

      private void OnModifierComboBoxOnSelectedIndexChanged(object? sender, EventArgs args)
      {
         if (!Globals.Modifiers.TryGetValue(Globals.MapWindow._modifierComboBox.Text, out var modifier))
            return;

         LocalisationTextBox.Text = Localisation.GetLoc(modifier.Name);
         DescriptionTextBox.Text = Localisation.GetLoc($"desc_{modifier.Name}");
      }


      private void CloseEvent(object? sender, EventArgs e)
      {
         Globals.MapWindow.ModifiersLayoutPanel.Controls.Remove(tempBox);
         Globals.MapWindow._modifierComboBox.SelectedIndexChanged -= OnModifierComboBoxOnSelectedIndexChanged;
         Globals.MapWindow.ModifiersLayoutPanel.Controls.Add(Globals.MapWindow._modifierComboBox, 1, 1);
      }


      private void EventModifiersLayoutPanel_Paint(object sender, PaintEventArgs e)
      {

      }

      private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
      {

      }

      private void CancelButton_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void EventModifierForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
      {

      }

      private void EventModifierForm_KeyDown(object sender, KeyEventArgs e)
      {
         // Close the form when the escape key is pressed
         if (e.KeyCode == Keys.Escape)
            Close();
      }
   }
}
