using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Editor.Controls;

namespace Editor.Forms.GetUserInput
{
   public partial class UserNumberInputForm : Form
   {
      private TableLayoutPanel _mainTableLayoutPanel;
      private Label _descriptionLabel;
      private Label _extensiveDescriptionLabel;
      private ComboBox? _numberComboBox;
      private ExtendedNumeric? _numericInput;
      private Button _okButton;
      private ToolTip _toolTip = new();

      // if true, the user can select a number from a combobox, else uses a numeric input
      public bool UseComboBox = false;

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
            else if (_numericInput != null)
               _numericInput.Maximum = value;
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
            else if (_numericInput != null)
               _numericInput.Maximum = value;
         }
      }

      private int _minValue = 0;
      private int _maxValue = 20;

      public int SelectedNumber
      {
         get
         {
            if (UseComboBox)
               return _numberComboBox?.SelectedIndex ?? -1;
            if (_numericInput?.Value != null)
               return (int)_numericInput?.Value!;
            return -1;
         }
      }

      public UserNumberInputForm(string title, string inputDesc, string extensiveDesc, int min = 0, int max = 20, bool useComboBox = false)
      {
         UseComboBox = useComboBox;
         Text = title;
         MinValue = min;
         MaxValue = max;

         InitializeComponent();

         _mainTableLayoutPanel = new()
         {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            Margin = new(0),
            RowStyles =
            {
               new (SizeType.Percent, 50),
               new (SizeType.Percent, 25),
               new (SizeType.Percent, 25),
            }
         };

         _descriptionLabel = new()
         {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Margin = new(3),
            Text = inputDesc
         };
         _mainTableLayoutPanel.Controls.Add(_descriptionLabel, 0, 1);

         _extensiveDescriptionLabel = new()
         {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Margin = new(3),
            Text = extensiveDesc
         };
         _mainTableLayoutPanel.Controls.Add(_extensiveDescriptionLabel, 0, 0);
         _mainTableLayoutPanel.SetColumnSpan(_extensiveDescriptionLabel, 2);

         if (UseComboBox)
         {
            _numberComboBox = new()
            {
               Dock = DockStyle.Fill,
               Margin = new(3),
               DropDownStyle = ComboBoxStyle.DropDownList,
            };
            
            _numberComboBox.SelectedIndex = 0;
            _mainTableLayoutPanel.Controls.Add(_numberComboBox, 1, 1);
         }
         else
         {
            _numericInput = new(null!)
            {
               Dock = DockStyle.Fill,
               Margin = new(3),
               Minimum = MinValue,
               Maximum = MaxValue == -1 ? int.MaxValue : MaxValue,
            };
            _mainTableLayoutPanel.Controls.Add(_numericInput, 1, 1);
            _toolTip.SetToolTip(_numericInput, $"Min: {min}\nMax: {max}");
         }

         _okButton = new()
         {
            Dock = DockStyle.Fill,
            Text = "OK",
            Margin = new(3)
         };

         _okButton.Click += (sender, e) =>
         {
            if (UseComboBox && _numberComboBox?.SelectedIndex == -1)
            {
               MessageBox.Show("Please select a number from the list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
               return;
            }
            DialogResult = DialogResult.OK;
            Close();
         };

         _mainTableLayoutPanel.Controls.Add(_okButton, 0, 2);

         Controls.Add(_mainTableLayoutPanel);
         FormBorderStyle = FormBorderStyle.FixedDialog;
         MaximizeBox = false;
         MinimizeBox = false;
         StartPosition = FormStartPosition.CenterParent;

      }

      [AllowNull] public sealed override string Text
      {
         get { return base.Text; }
         set { base.Text = value; }
      }

      public void SetRecommended(int amount)
      {
         if (!UseComboBox)
            _toolTip.SetToolTip(_numericInput!, $"Min: {MinValue}\nRecommended Max: {amount}");
      }

      public int GetValue()
      {
         if (UseComboBox)
            return _numberComboBox?.SelectedIndex ?? -1;
         return (int)_numericInput?.Value!;
      }

      public static int ShowGet(string title, string inputDesc, string extensiveDesc, int recommended, int min = 0, int max = 20, bool useComboBox = false, int defaultValue = -1)
      {
         var form = new UserNumberInputForm(title, inputDesc, extensiveDesc, min, max, useComboBox);
         form.SetRecommended(recommended);
         if (form.ShowDialog() == DialogResult.OK)
            return form.GetValue();
         return defaultValue;
      }
      public static int ShowGet(string title, string inputDesc, string extensiveDesc, int min = 0, int max = 20, bool useComboBox = false, int defaultValue = -1)
      {
         var form = new UserNumberInputForm(title, inputDesc, extensiveDesc, min, max, useComboBox);
         if (form.ShowDialog() == DialogResult.OK)
            return form.GetValue();
         return defaultValue;
      }

      private void GenerateComboBoxItems()
      {
         Debug.Assert(_numberComboBox != null, "numberComboBox is null");
         _numberComboBox.SuspendLayout();
         _numberComboBox.Items.Clear();
         for (var i = MinValue; i <= MaxValue; i++)
            _numberComboBox.Items.Add(i);
         _numberComboBox.ResumeLayout();
      }
   }
}
