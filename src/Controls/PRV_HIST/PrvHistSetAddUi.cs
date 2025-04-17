namespace Editor.Controls.PRV_HIST
{
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

   public class PrvHistIntUi : PrvHistSetAddUi
   {
      public NumericUpDown IntNumeric { get; }

      public PrvHistIntUi(string text, int value = 0, int min = 0, int max = 100)
         : base(text, new NumericUpDown
         {
            Dock = DockStyle.Fill,
            TextAlign = HorizontalAlignment.Right
         })
      {
         IntNumeric = (NumericUpDown)Controls[2]; // keep a reference directly
         IntNumeric.Minimum = min;
         IntNumeric.Maximum = max;
         IntNumeric.Value = value;
      }
   }   
   
   public class PrvHistBoolUi : PrvHistSetAddUi
   {
      public CheckBox BoolCheckBox { get; }

      public PrvHistBoolUi(string text, bool isChecked = false)
         : base(text, new CheckBox
         {
            Dock = DockStyle.Fill,
         }, false)
      {
         BoolCheckBox = (CheckBox)Controls[2]; // keep a reference directly
         BoolCheckBox.Checked = isChecked;
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