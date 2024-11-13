#nullable enable

using Editor.Helper;

namespace Editor.Forms.Feature.AdvancedSelections
{
   public partial class AdvancedSelectionsForm : Form
   {
      private List<ISelectionModifier> SelectionModifiers = [];
      private ProvinceEnumHelper.ProvAttrType _lastAttrType = ProvinceEnumHelper.ProvAttrType.Int;

      public AdvancedSelectionsForm()
      {
         InitializeComponent();
         SelectionModifiers.Add(new Deselection());
         SelectionModifiers.Add(new Select());

         InitComboBoxes();

         AttributeSelectionComboBox.SelectedIndexChanged += (sender, e) =>
         {
            var newAttrType = GetAttribute().GetAttributeType();
            if (newAttrType != _lastAttrType)
            {
               AddValidOperations(newAttrType);
               _lastAttrType = newAttrType;
            }
         };
      }

      private void InitComboBoxes()
      {
         foreach (var selection in SelectionModifiers) 
            ActionTypeComboBox.Items.Add(selection.Name);
         ActionTypeComboBox.SelectedIndex = 0;

         AttributeSelectionComboBox.Items.AddRange([.. Enum.GetNames<ProvinceEnumHelper.ProvAttrGet>()]);

         AddValidOperations(ProvinceEnumHelper.ProvAttrType.Int);

         SelectionSource.Items.AddRange([.. Enum.GetNames<ProvinceSource>()]);
      }

      private void AddValidOperations(ProvinceEnumHelper.ProvAttrType type)
      {
         OperationComboBox.Items.Clear();
         switch (type)
         {
            case ProvinceEnumHelper.ProvAttrType.String:
            case ProvinceEnumHelper.ProvAttrType.Bool:
            case ProvinceEnumHelper.ProvAttrType.Tag:
               OperationComboBox.Items.AddRange(["=", "!="]);  
               break;
            case ProvinceEnumHelper.ProvAttrType.Int:
            case ProvinceEnumHelper.ProvAttrType.Float:
               OperationComboBox.Items.AddRange(["=", "!=", "<", ">", ">=", "<="]);
               break;
         }
      }

      private void ConfirmButton_Click(object sender, EventArgs e)
      {
         if (ActionTypeComboBox.SelectedIndex == -1 || AttributeSelectionComboBox.SelectedIndex == -1 || AttributeValueInput.Text == string.Empty || OperationComboBox.SelectedIndex == -1)
            return;

         GetSelectionModifier().Execute(GetSource(), GetOperation(), GetAttribute(), GetAttributeValue());
         Globals.ZoomControl.Invalidate();
      }

      private ProvinceEnumHelper.ProvAttrGet GetAttribute()
      {
         return (ProvinceEnumHelper.ProvAttrGet)AttributeSelectionComboBox.SelectedIndex;
      }

      private object GetAttributeValue()
      {
         return AttributeValueInput.Text;
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
