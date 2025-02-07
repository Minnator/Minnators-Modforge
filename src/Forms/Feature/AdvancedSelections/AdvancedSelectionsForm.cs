#nullable enable

using System.Diagnostics;
using System.Reflection;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Forms.Feature.AdvancedSelections
{
   public partial class AdvancedSelectionsForm : Form
   {
      private List<ISelectionModifier> SelectionModifiers = [];
      private PropertyInfo[] toolTippables;

      public AdvancedSelectionsForm()
      {
         InitializeComponent();
         SelectionModifiers.Add(new Deselection());
         SelectionModifiers.Add(new Select());

         toolTippables = typeof(Province).GetProperties()
                                         .Where(prop => Attribute.IsDefined(prop, typeof(ToolTippable))).ToArray();

         InitComboBoxes();

         AttributeSelectionComboBox.SelectedIndexChanged += (sender, e) =>
         {
            AddValidOperations(GetAttribute());
         };
      }

      private void InitComboBoxes()
      {
         foreach (var selection in SelectionModifiers) 
            ActionTypeComboBox.Items.Add(selection.Name);
         ActionTypeComboBox.SelectedIndex = 0;

         AttributeSelectionComboBox.Items.AddRange(toolTippables.Select(x => x.Name).Cast<object>().ToArray());
         
         SelectionSource.Items.AddRange([.. Enum.GetNames<ProvinceSource>()]);
      }

      private void AddValidOperations(PropertyInfo info)
      {
         Debug.Assert(info != null, "this info must not be null");
         OperationComboBox.Items.Clear();

         if (info.PropertyType == typeof(float) || info.PropertyType == typeof(int))
            OperationComboBox.Items.AddRange(["=", "!=", "<", ">", ">=", "<="]);
         else
            OperationComboBox.Items.AddRange(["=", "!="]);  
      }

      private void ConfirmButton_Click(object sender, EventArgs e)
      {
         if (ActionTypeComboBox.SelectedIndex == -1 || AttributeSelectionComboBox.SelectedIndex == -1 || AttributeValueInput.Text == string.Empty || OperationComboBox.SelectedIndex == -1)
            return;

         GetSelectionModifier().Execute(GetSource(), GetOperation(), GetAttribute(), AttributeValueInput.Text);
         Globals.ZoomControl.Invalidate();
      }

      private PropertyInfo GetAttribute()
      {
         if (AttributeSelectionComboBox.SelectedIndex >= 0 && AttributeSelectionComboBox.SelectedIndex < toolTippables.Length)
            return toolTippables[AttributeSelectionComboBox.SelectedIndex];
         throw new EvilActions("There should never be this case of out of bounds!");
      }

      private Operations GetOperation()
      {
         return (Operations)OperationComboBox.SelectedIndex;
      }

      public ProvinceSource GetSource()
      {
         return (ProvinceSource)SelectionSource.SelectedIndex;
      }

      private ISelectionModifier GetSelectionModifier()
      {
         return SelectionModifiers[ActionTypeComboBox.SelectedIndex];
      }



      private void CancelButton_Click(object sender, EventArgs e)
      {
         Close();
      }
   }
}
