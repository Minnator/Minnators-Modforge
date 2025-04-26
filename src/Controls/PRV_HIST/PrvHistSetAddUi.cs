using System.Diagnostics;
using Editor.Controls.PROPERTY;
using Editor.DataClasses.Saveables;
using Editor.Events;
using System.Reflection;
using Editor.Forms.Feature;
using static Editor.Forms.PopUps.CreateCountryForm;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.ErrorHandling;
using static Editor.Loading.Enhanced.PCFL.Implementation.PCFL_Scope;

namespace Editor.Controls.PRV_HIST
{
   public class PrvHistCollectionUi : PrvHistSetAddUi
   {
      public TableLayoutPanel Tlp { get; }
      public Button EditButton { get; }
      private Label _shortInfoLabel;

      public PrvHistCollectionUi(string text, bool hasSetBox = true) 
         : base(text, new TableLayoutPanel
         {
            ColumnCount = 2,
            RowCount = 1,
            Dock = DockStyle.Fill,
         }, hasSetBox)
      {
         Tlp = (TableLayoutPanel)Controls[2]; // keep a reference directly
         Tlp.ColumnStyles.Clear();
         Tlp.RowStyles.Clear();
         Tlp.ColumnStyles.Add(new(SizeType.Absolute, 30));
         Tlp.ColumnStyles.Add(new(SizeType.Percent, 100));
         Tlp.RowStyles.Add(new(SizeType.Percent, 100));
         Tlp.Dock = DockStyle.Fill;

         EditButton = new()
         {
            Dock = DockStyle.Fill,
            Text = "...",
            Font = new("Arial", 8, FontStyle.Regular),
            Padding = new(0),
            Margin = new(0),
         };

         EditButton.Click += EditButton_Click;

         _shortInfoLabel = new()
         {
            Text = "Short Info",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new("Arial", 8, FontStyle.Regular),
         };

         Tlp.Controls.Add(EditButton, 0, 0);
         Tlp.Controls.Add(_shortInfoLabel, 1, 0);

         SetShortInfo();
      }

      public string ShortInfoText
      {
         get => _shortInfoLabel.Text;
         set => _shortInfoLabel.Text = value;
      }

      private void EditButton_Click(object? sender, EventArgs e)
      {
         var form = new ListDeltaSetSelection("Edit", ["Test1", "Test2", "Test3", "Test5"], ["Test4"], SetCheckBox.Checked);
         form.ShowDialog();

         if (form.IsSetCheckBoxChecked)
         {
            Debug.WriteLine("Items to set:");
            foreach (var toSet in form.GetSet)
               Debug.WriteLine(toSet);
         }
         else
         {
            var delta = form.GetDelta();

            Debug.WriteLine("Items to add:");
            foreach (var toSet in delta.added)
               Debug.WriteLine(toSet);
            Debug.WriteLine("Items to remove:");
            foreach (var toSet in delta.removed)
               Debug.WriteLine(toSet);
         }

         SetShortInfo();
      }

      private void SetShortInfo()
      {
         //TODO once list is gettable in here
      }
   }

   public class PrvHistFloatUi : PrvHistSetAddUi
   {
      public NumericUpDown FloatNumeric { get; }

      public PrvHistFloatUi(string text, float value = 0, float min = 0, float max = 100)
         : base(text, new NumericUpDown
         {
            Dock = DockStyle.Fill,
            TextAlign = HorizontalAlignment.Right,
            DecimalPlaces = 2,
            Increment = 0.05m,
         })
      {
         FloatNumeric = (NumericUpDown)Controls[2]; // keep a reference directly
         FloatNumeric.Minimum = (decimal)min;
         FloatNumeric.Maximum = (decimal)max;
         FloatNumeric.Value = (decimal)value;
      }
   }

   public class PrvHistTextBoxUi : PrvHistSetAddUi
   {
      public TextBox TextBox { get; }

      public PrvHistTextBoxUi(string text)
         : base(text, new TextBox
         {
            Dock = DockStyle.Fill,
         }, false)
      {
         TextBox = (TextBox)Controls[2]; // keep a reference directly
      }
   }

   public class PrvHistDropDownUi : PrvHistSetAddUi
   {
      public ComboBox DropDown { get; }

      public PrvHistDropDownUi(string text, bool isDropDownList = false)
         : base(text, new ComboBox
         {
            Dock = DockStyle.Fill,
         }, false)
      {
         DropDown = (ComboBox)Controls[2]; // keep a reference directly
         DropDown.DropDownStyle = isDropDownList ? ComboBoxStyle.DropDownList : ComboBoxStyle.DropDown;
      }
   }

   public class PrvHistIntUi : PrvHistSetAddUi, IPrvHisSetOptSimplePropControl<int>
   {
      public NumericUpDown IntNumeric { get; }
      public PCFL_TokenParseDelegate EffectDelegate { get; init; }
      public PCFL_TokenParseDelegate SetEffectDelegate { get; init; }
      public PropertyInfo PropertyInfo { get; init; }

      public PrvHistIntUi(string text, PropertyInfo info, PCFL_TokenParseDelegate effect, PCFL_TokenParseDelegate setEffect, int value = 0, int min = 0, int max = 100)
         : base(text, new NumericUpDown
         {
            Dock = DockStyle.Fill,
            TextAlign = HorizontalAlignment.Right
         })
      {
         PropertyInfo = info;
         EffectDelegate = effect;
         SetEffectDelegate = setEffect;
         LoadGuiEvents.ProvHistoryLoadAction += ((IPropertyControl<Province, int>)this).LoadToGui;

         IntNumeric = (NumericUpDown)Controls[2]; // keep a reference directly
         IntNumeric.Minimum = min;
         IntNumeric.Maximum = max;
         IntNumeric.Value = value;
      }
      public void SetFromGui()
      {
         if (Globals.State != State.Running || !GetFromGui(out var value).Log())
            return;
         // TODO add a history command
      }
      public void SetDefault() => IntNumeric.Value = IntNumeric.Minimum;
      public void SetValue(int value) => IntNumeric.Value = value;
      public IErrorHandle GetFromGui(out int value)
      {
         value = (int)IntNumeric.Value;
         return ErrorHandle.Success;
      }
   }   
   
   public class PrvHistBoolUi : PrvHistSetAddUi, IPrvHistSimpleEffectPropControl<bool>
   {
      public CheckBox BoolCheckBox { get; }
      public PCFL_TokenParseDelegate EffectDelegate { get; init; }
      public PropertyInfo PropertyInfo { get; init; }

      public PrvHistBoolUi(string text, PropertyInfo info, PCFL_TokenParseDelegate effect, bool isChecked = false)
         : base(text, new CheckBox
         {
            Dock = DockStyle.Fill,
         }, false)
      {
         PropertyInfo = info;
         EffectDelegate = effect;
         LoadGuiEvents.ProvHistoryLoadAction += ((IPropertyControl<Province, bool>)this).LoadToGui;
         
         BoolCheckBox = (CheckBox)Controls[2]; // keep a reference directly
         BoolCheckBox.Checked = isChecked;

         BoolCheckBox.CheckedChanged += (_, _) => SetFromGui();
      }

      public void SetFromGui()
      {
         if (Globals.State != State.Running || !GetFromGui(out var value).Log())
            return;
         // TODO add a history command
      }
      public void SetDefault() => BoolCheckBox.Checked = false;
      public void SetValue(bool value) => BoolCheckBox.Checked = value;
      public IErrorHandle GetFromGui(out bool value)
      {
         value = BoolCheckBox.Checked;
         return ErrorHandle.Success;
      }
   }
   
   public class PrvHistSetAddUi : TableLayoutPanel
   {
      public Label Label { get; set; }
      public CheckBox SetCheckBox { get; set; }
      
      public PrvHistSetAddUi(string text, Control control, bool hasSetBox = true)
      {
         AutoSize = false;

         ColumnCount = 3;
         RowCount = 1;

         ColumnStyles.Clear();
         ColumnStyles.Add(new(SizeType.Percent, 33));
         ColumnStyles.Add(new(SizeType.Percent, 17));
         ColumnStyles.Add(new(SizeType.Percent, 50));

         RowStyles.Clear();
         RowStyles.Add(new(SizeType.Percent, 100));

         Label = new()
         {
            Text = "---",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
         };

         SetCheckBox = new()
         {
            Checked = false,
            Dock = DockStyle.Fill,
            Font = new("Arial", 8, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            Text = "Set/Add",
            Padding = new(0, 3, 0, 3),
            Margin = new(0, 3, 0, 3),
         };

         Controls.Add(Label, 0, 0);
         Controls.Add(SetCheckBox, 1, 0);
         Controls.Add(control, 2, 0);

         Label.Text = text;
         if (!hasSetBox)
         {
            SetCheckBox.Visible = false;
            ColumnStyles[0].Width = 50;
            ColumnStyles[0].SizeType = SizeType.Percent;
            ColumnStyles[1].Width = 0;
            ColumnStyles[1].SizeType = SizeType.Absolute;
         }
      }

      public sealed override bool AutoSize
      {
         get { return base.AutoSize; }
         set { base.AutoSize = value; }
      }
   }
}