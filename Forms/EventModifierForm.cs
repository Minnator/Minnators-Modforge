using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Interfaces;
using Editor.Parser;
using ComboBox = System.Windows.Forms.ComboBox;

namespace Editor.Forms
{
   public partial class EventModifierForm : Form
   {
      private DualSelectionFlowPanel _customAttrPanel;
      private ColoredModifierPanel _modifierPanel;

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
         Globals.MapWindow._modifierComboBox.TextChanged += OnModifierComboBoxTextChanged;

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
         _modifierPanel.MaxIndex = Globals.ModifierKeys.Length - 1;
         _modifierPanel.MinIndex = 0;

         CustomAttributesLP.Controls.Add(_customAttrPanel, 1, 0);
         ModifiersLP.Controls.Add(_modifierPanel, 1, 0);

         _customAttrPanel.ResumeLayout(true);
      }

      private void OnModifierComboBoxTextChanged(object? sender, EventArgs e)
      {
         if (Globals.EventModifiers.ContainsKey(((ComboBox)sender!).Text))
            return;

         _customAttrPanel.LeftComboBox.Clear();
         _customAttrPanel.RightComboBox.Clear();
         _customAttrPanel.FlowLayoutPanel.Controls.Clear();

         _modifierPanel.LeftComboBox.Clear();
         _modifierPanel.RightComboBox.Clear();
         _modifierPanel.FlowLayoutPanel.Controls.Clear();

         LocalisationTextBox.Text = "";
         DescriptionTextBox.Text = "";
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
         if (!Globals.EventModifiers.TryGetValue(Globals.MapWindow._modifierComboBox.Text, out var modifier))
            return;

         LocalisationTextBox.Text = Localisation.GetLoc(modifier.GetTitleLocKey);
         DescriptionTextBox.Text = Localisation.GetLoc(modifier.GetDescriptionLocKey);
         _customAttrPanel.SetDualContent(modifier.TriggerAttribute);

         List<ModifierDefinition> definitions = [];
         List<string> values = [];
         foreach (var mod in modifier.Modifiers)
         {
            definitions.Add(ModifierParser.ModifierDefinitions[mod.Name]);
            values.Add(mod.Value.ToString()!);
         }
         _modifierPanel.AddModifiers(definitions, values);
      }


      private void CloseEvent(object? sender, EventArgs e)
      {
         Globals.MapWindow.ModifiersLayoutPanel.Controls.Remove(tempBox);
         Globals.MapWindow._modifierComboBox.SelectedIndexChanged -= OnModifierComboBoxOnSelectedIndexChanged;
         Globals.MapWindow._modifierComboBox.TextChanged -= OnModifierComboBoxTextChanged;
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
         if (e.Modifiers == Keys.Control)
         {
            switch (e.KeyCode)
            {
               case Keys.M:
                  ModifyButton.PerformClick();
                  break;
               case Keys.S:
                  SaveButton.PerformClick();
                  break;
               case Keys.X:
                  CancelButton.PerformClick();
                  break;
            }
         }
      }

      private void SaveButton_Click(object sender, EventArgs e)
      {
         AddOrUpdateModifier();
      }

      private bool AddOrUpdateModifier()
      {
         var name = Globals.MapWindow._modifierComboBox.Text;
         var title = LocalisationTextBox.Text;
         var desc = DescriptionTextBox.Text;

         if (string.IsNullOrEmpty(name))
            return false;
         if (!GetModifiersFromGui(out var mods))
            return false;
         if (!GetCustomAttributesFromGui(out var customAttrs))
            return false;

         if (Globals.EventModifiers.TryGetValue(name, out var modifier))
         {
            modifier.Modifiers = mods;
            modifier.TriggerAttribute = customAttrs;
            modifier.EditingStatus = ObjEditingStatus.Modified;
            SetModifierLocalisation(ref title, ref desc, ref modifier);
            return true;
         }

         

         //TODO is not correctly added to combobox
         EventModifier eventMod = new(name)
         {
            Modifiers = mods,
            TriggerAttribute = customAttrs,
         };
         SetModifierLocalisation(ref title, ref desc, ref eventMod);
         Globals.EventModifiers.Add(name, eventMod);
         Globals.MapWindow._modifierComboBox.Items.Add(name);
         Globals.MapWindow._modifierComboBox.AutoCompleteCustomSource.Add(name);
         Globals.MapWindow._modifierComboBox.AutoCompleteMode = AutoCompleteMode.None;
         Globals.MapWindow._modifierComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         return true;
      }

      private void SetModifierLocalisation(ref string title, ref string desc, ref EventModifier modifier)
      {
         Localisation.AddOrModifyLocObject(modifier.GetTitleLocKey, title);
         Localisation.AddOrModifyLocObject(modifier.GetDescriptionLocKey, desc);
      }

      private bool GetModifiersFromGui(out List<Modifier> modifiers)
      {
         modifiers = [];
         foreach (var kvp in _modifierPanel.GetDualContent())
         {
            if (int.TryParse(kvp.Key, out var index))
            {
               modifiers.Add(new(index, kvp.Value));
            }
            else
            {
               MessageBox.Show("Error: Could not parse index to <int> during modifier indexing");
               modifiers = [];
               return false;
            }
         }

         return true;
      }

      private bool GetCustomAttributesFromGui(out List<KeyValuePair<string, string>> customAttributes)
      {
         customAttributes = _customAttrPanel.GetDualContent();
         return true;
      }

      private void ModifyButton_Click(object sender, EventArgs e)
      {
         AddOrUpdateModifier();
      }
   }
}
