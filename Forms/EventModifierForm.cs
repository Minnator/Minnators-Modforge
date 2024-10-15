using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Editor.Controls;
using Editor.Helper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
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
         _customAttrPanel.Dock = DockStyle.Fill;
         _modifierPanel = new();
         _modifierPanel.Dock = DockStyle.Fill;

         CustomAttributesLP.Controls.Add(_customAttrPanel, 1, 0);
         ModifiersLP.Controls.Add(_modifierPanel, 1, 0);
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
