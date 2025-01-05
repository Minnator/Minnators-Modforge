using Editor.Controls;

namespace Editor.src.Forms.GetUserInput
{
   public partial class UserNumberInputForm : Form
   {
      private TableLayoutPanel mainTableLayoutPanel;
      private Label descriptionLabel;
      private ComboBox numberComboBox;
      private ExtendedNumeric numericInput;
      private Button okButton;

      // if true, the user can select a number from a combobox, else uses a numeric input
      public bool UseComboBox = true;

      public int MinValue
      {
         get => _minValue;
         set
         {
            if (_minValue >= MaxValue)
               throw new ArgumentException("MinValue must be less than MaxValue");
            _minValue = value;
            if (UseComboBox)
               GenerateComboBoxItems();
            else
               numericInput.Minimum = value;
         }
      }

      public int MaxValue
      {
         get => _maxValue;
         set
         {
            if (_maxValue <= MinValue)
               throw new ArgumentException("MaxValue must be greater than MinValue");
            _maxValue = value;
            if (UseComboBox)
               GenerateComboBoxItems();
            else
               numericInput.Maximum = value;
         }
      }

      private int _minValue = 0;
      private int _maxValue = 20;

      public int SelectedNumber => UseComboBox ? numberComboBox.SelectedIndex : (int)numericInput.Value;
      
      public UserNumberInputForm()
      {
         InitializeComponent();

         mainTableLayoutPanel = new()
         {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = new(0)
         };

         descriptionLabel = new()
         {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Margin = new(3),
            Text = "Select a number"
         };

         if (UseComboBox)
         {
            numberComboBox = new()
            {
               Dock = DockStyle.Fill,
               Margin = new(3)
            };
            
            numberComboBox.SelectedIndex = 0;
            mainTableLayoutPanel.Controls.Add(numberComboBox, 0, 1);
         }
         else
         {
            numericInput = new("")
            {
               Dock = DockStyle.Fill,
               Margin = new(3),
               Minimum = MinValue,
               Maximum = MaxValue
            };
            mainTableLayoutPanel.Controls.Add(numericInput, 0, 1);
         }
      }

      private void GenerateComboBoxItems()
      {
         numberComboBox.SuspendLayout();
         numberComboBox.Items.Clear();
         for (var i = MinValue; i <= MaxValue; i++)
            numberComboBox.Items.Add(i);
         numberComboBox.ResumeLayout();
      }
   }
}
