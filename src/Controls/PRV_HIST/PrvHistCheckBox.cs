namespace Editor.Controls.PRV_HIST
{
   public class PrvHistCheckBox : TableLayoutPanel
   {
      public Label Label { get; set; }
      public CheckBox CheckBox { get; set; }

      public PrvHistCheckBox()
      {
         AutoSize = false;

         ColumnCount = 2;
         RowCount = 1;

         ColumnStyles.Clear();
         ColumnStyles.Add(new(SizeType.Percent, 45));
         ColumnStyles.Add(new(SizeType.Percent, 55));

         RowStyles.Clear();
         RowStyles.Add(new(SizeType.Percent, 100));

         Label = new()
         {
            Text = "---",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
         };

         CheckBox = new()
         {
            Checked = false,
            Dock = DockStyle.Fill,
         };

         Controls.Add(Label, 0, 0);
         Controls.Add(CheckBox, 1, 0);
      }

      public PrvHistCheckBox(string text) : this()
      {
         Label.Text = text;
      }
   }
}