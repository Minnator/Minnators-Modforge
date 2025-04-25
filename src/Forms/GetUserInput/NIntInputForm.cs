using Editor.Controls;
using static System.Decimal;

namespace Editor.src.Forms.GetUserInput
{
   public partial class NIntInputForm : Form
   {
      private ExtendedNumeric[] _numericInputs = [];
      private Label[] _labels = [];
      private readonly Button _confirmButton = new()
      {
         Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top,
         Text = "Confirm",
      };

      public NIntInputForm()
      {
         StartPosition = FormStartPosition.CenterParent;
         InitializeComponent();
         _confirmButton.Click += ConfirmButton_Click;
      }

      public static float[] ShowGet(NIntInputObj[] objs, string title)
      {
         var intForm = new NIntInputForm();
         intForm.Text = title;
         intForm.GenerateRows(objs.Length);
         intForm.PopulateRows(objs);

         if (intForm.ShowDialog() == DialogResult.OK)
         {
            var results = new float[intForm._numericInputs.Length];
            for (var i = 0; i < intForm._numericInputs.Length; i++)
               results[i] = (float)intForm._numericInputs[i].Value;
            return results;
         }
         return [-1, -1];
      }

      private void GenerateRows(int amount)
      {
         MainTlp.RowStyles.Clear();
         MainTlp.RowCount = amount + 1;
         for (var row = 0; row <= amount; row++) 
            MainTlp.RowStyles.Add(new (SizeType.Percent, 100f / (amount)));
         MainTlp.RowStyles.Add(new(SizeType.Absolute, 30)); // for the button

         Height += Math.Min((amount - 1) * 28, Screen.FromControl(this).WorkingArea.Height);
      }

      private void PopulateRows(NIntInputObj[] objs)
      {
         _labels = new Label[objs.Length];
         _numericInputs = new ExtendedNumeric[objs.Length];

         for (var i = 0; i < objs.Length; i++)
         {
            var obj = objs[i];
            _labels[i] = new()
            {
               Text = obj.Desc,
               Dock = DockStyle.Fill,
               TextAlign = ContentAlignment.MiddleLeft,
            };

            _numericInputs[i] = new(null!)
            {
               Dock = DockStyle.Fill,
               Minimum = (decimal)obj.Min,
               Maximum = (decimal)obj.Max == -1 ? MaxValue : (decimal)obj.Max,
               Value = (decimal)Math.Max(obj.Default, obj.Min),
               DecimalPlaces = obj.IsFloat ? 2 : 0,
            };

            MainTlp.Controls.Add(_labels[i], 0, i);
            MainTlp.Controls.Add(_numericInputs[i], 1, i);
         }

         MainTlp.Controls.Add(_confirmButton, 1, objs.Length);
      }

      private void ConfirmButton_Click(object? sender, EventArgs e)
      {
         DialogResult = DialogResult.OK;
         Close();
      }

      public readonly struct NIntInputObj(string desc, float min, float max, float defaultValue, bool isFloat)
      {
         public string Desc { get; } = desc;
         public float Min { get; } = min;
         public float Max { get; } = max;
         public float Default { get; } = defaultValue;
         public bool IsFloat { get; } = isFloat;
      }
   }

   
}
