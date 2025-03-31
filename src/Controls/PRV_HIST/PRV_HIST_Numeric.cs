namespace Editor.Controls.PRV_HIST
{
   public sealed class PrvHistNumeric : TableLayoutPanel
   {
      public NumericUpDown Numeric { get; set; }
      public Label Label { get; set; }
      public Label TotalLabel { get; set; }

      public PrvHistNumeric()
      {
         AutoSize = false;

         ColumnCount = 5;
         RowCount = 1;

         ColumnStyles.Clear();
         ColumnStyles.Add(new(SizeType.Percent, 45));
         ColumnStyles.Add(new(SizeType.Percent, 5));
         ColumnStyles.Add(new(SizeType.Percent, 20));
         ColumnStyles.Add(new(SizeType.Percent, 10));
         ColumnStyles.Add(new(SizeType.Percent, 20));

         RowStyles.Clear();
         RowStyles.Add(new(SizeType.Percent, 100));

         Label = new()
         {
            Text = "---",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
         };

         Numeric = new()
         {
            Minimum = -100,
            Maximum = 100,
            Value = 0,
            Dock = DockStyle.Fill,
         };

         TotalLabel = new()
         {
            Text = "0",
            Dock = DockStyle.Fill,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft
         };

         var deltaLabel = new Label
         {
            Text = "Δ",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight
         };

         var sumLabel = new Label
         {
            Text = "Sum",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight
         };

         Controls.Add(Label, 0, 0);
         Controls.Add(deltaLabel, 1, 0);
         Controls.Add(Numeric, 2, 0);
         Controls.Add(sumLabel, 3, 0);
         Controls.Add(TotalLabel, 4, 0);
      }

      public PrvHistNumeric(string label) : this()
      {
         Label.Text = label;
      }

   }



}
