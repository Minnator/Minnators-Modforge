using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
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
         tempBox = new()
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
         _customAttrPanel.SetComboBoxItems([.. ModifierParser.CustomModifierTrigger.Keys], DualSelectionFlowPanel.BoxType.Left, true);
         _customAttrPanel.SetComboBoxItems(["yes", "no"], DualSelectionFlowPanel.BoxType.Right, true);
         _modifierPanel = new();
         _modifierPanel.Dock = DockStyle.Fill;

         _modifierPanel.SetComboBoxItems(Globals.ModifierKeys, DualSelectionFlowPanel.BoxType.Left, false);
         _modifierPanel.LeftComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         _modifierPanel.LeftComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
         _modifierPanel.LeftComboBox.SelectedIndexChanged += SuggestDefaultValues;
         _modifierPanel.Indexing = DualSelectionFlowPanel.IndexingType.Left;
         _modifierPanel.UseIndexing = true;
         _modifierPanel.MaxIndex = Globals.ModifierKeys.Length - 1;
         _modifierPanel.MinIndex = 0;

         CustomAttributesLP.Controls.Add(_customAttrPanel, 1, 0);
         ModifiersLP.Controls.Add(_modifierPanel, 1, 0);

         _customAttrPanel.ResumeLayout(true);
      }

      private void SuggestDefaultValues(object? sender, EventArgs e)
      {
         if (sender is not ComboBox comboBox)
            return;

         if (comboBox.SelectedIndex > Globals.ModifierKeys.Length || comboBox.SelectedIndex < 0
             || !Globals.ModifierValueTypes.TryGetValue(comboBox.SelectedIndex, out var type))
            return;

         _modifierPanel.SetComboBoxItems(FormHelper.GetCompletionSuggestion(type), DualSelectionFlowPanel.BoxType.Right, false);
         _modifierPanel.RightComboBox.SelectedIndex = _modifierPanel.RightComboBox.Items.Count / 2;
      }

      private void OnModifierComboBoxOnSelectedIndexChanged(object? sender, EventArgs args)
      {
         if (!Globals.VanillaModifiers.TryGetValue(Globals.MapWindow._modifierComboBox.Text, out var modifier))
            return;

         LocalisationTextBox.Text = Localisation.GetLoc(modifier.Name);
         DescriptionTextBox.Text = Localisation.GetLoc($"desc_{modifier.Name}");

         _customAttrPanel.SetDualContent(modifier.TriggerAttribute);
         List<KeyValuePair<string, string>> modList = [];
         foreach (var mod in modifier.Modifiers)
            modList.Add(new(Globals.ModifierKeys[mod.Name], mod.Value.ToString()!));
         _modifierPanel.SetDualContent(modList);
      }


      private void CloseEvent(object? sender, EventArgs e)
      {
         Globals.MapWindow.ModifiersLayoutPanel.Controls.Remove(tempBox);
         Globals.MapWindow._modifierComboBox.SelectedIndexChanged -= OnModifierComboBoxOnSelectedIndexChanged;
         Globals.MapWindow.ModifiersLayoutPanel.Controls.Add(Globals.MapWindow._modifierComboBox, 1, 1);
      }

      private void CancelButton_Click(object sender, EventArgs e)
      {
         Close();
      }
      
      private void EventModifierForm_KeyDown(object sender, KeyEventArgs e)
      {
         // Close the form when the escape key is pressed
         //if (e.KeyCode == Keys.Escape)
         //   Close();
      }

      private void SaveButton_Click(object sender, EventArgs e)
      {  
         if (!GetModifierFromGui(out var mod))
            return;
      }

      private bool GetModifierFromGui(out EventModifier eventMod)
      {
         var name = Globals.MapWindow._modifierComboBox.Text;
         var title = LocalisationTextBox.Text;
         var desc = DescriptionTextBox.Text;
         var customAttr = _customAttrPanel.GetDualContent();
         eventMod = EventModifier.Empty;

         if (string.IsNullOrEmpty(name))
            return false;

         if (Globals.VanillaModifiers.TryGetValue(name, out var modifier))
         {
            // TODO how to handle this? If an existing is edited
         }

         // How to handle if it is an existing modifier from vanilla?
         // How to handle if it is an existing modifier from a mod?
         // How to handle if it is a new modifier? add to random mod file or create new with certain name and add to that if already existing

         eventMod = new (name){ TriggerAttribute = customAttr };
         var dualContent = _modifierPanel.GetDualContent();
         foreach (var kvp in dualContent) 
            eventMod.Modifiers.Add(new(int.Parse(kvp.Key), kvp.Value));

         
         return true;
      }
   }
}
