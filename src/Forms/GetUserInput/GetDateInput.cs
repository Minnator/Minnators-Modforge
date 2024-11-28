using Editor.DataClasses.Misc;

namespace Editor.src.Forms.GetUserInput
{
   public partial class GetDateInput : Form
   {
      public Date DefaultDate = new(1444, 11, 11);



      public GetDateInput(string description)
      {
         StartPosition = FormStartPosition.CenterParent;
         InitializeComponent();
         label1.Text = description;

         // Set the month combobox
         for (var i = 1; i <= 12; i++)
            MonthBox.Items.Add(i);
         SetDays(DefaultDate.Month);

         SetDate(DefaultDate);

         YearBox.Items.Add("1444");
         YearBox.KeyPress += YearBox_KeyPress;

         MonthBox.SelectedIndexChanged += (sender, args) =>
         {
            var sIndex = DayBox.SelectedIndex;
            SetDays(MonthBox.SelectedIndex + 1);
            if (DayBox.Items.Count > sIndex)
               DayBox.SelectedIndex = sIndex;
         };

         DialogResult = DialogResult.None;
      }

      public void SetDate(Date date)
      {
         DayBox.SelectedIndex = date.Day - 1;
         MonthBox.SelectedIndex = date.Month - 1;
         YearBox.Text = date.Year.ToString();
      }

      // Sets the items of the day combobox based on the month
      public void SetDays(int month)
      {
         DayBox.Items.Clear();
         for (var i = 1; i <= Date.DaysInMonth((byte)month); i++)
            DayBox.Items.Add(i);
      }

      public bool GetDate(out Date date)
      {
         return Date.TryParseDate($"{DayBox.Text}.{MonthBox.Text}.{YearBox.Text}", out date);
      }

      // block non digit input and leading zeros for the year
      private void YearBox_KeyPress(object? sender, KeyPressEventArgs e)
      {
         if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            e.Handled = true;
         if (YearBox.Text.Length == 0 && e.KeyChar == '0')
            e.Handled = true;
      }

      private void ConfirmButton_Click(object sender, EventArgs e)
      {
         DialogResult = GetDate(out var _) ? DialogResult.OK : DialogResult.Cancel;
      }

      private void CancelButton_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
      }
   }
}
