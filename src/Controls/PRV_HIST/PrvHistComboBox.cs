namespace Editor.Controls.PRV_HIST
{
   public class PrvHistComboBox : TableLayoutPanel
   {
      public Label Label { get; set; }
      public ComboBox ComboBox { get; set; }

      public PrvHistComboBox()
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

         ComboBox = new()
         {
            Dock = DockStyle.Fill,
         };

         Controls.Add(Label, 0, 0);
         Controls.Add(ComboBox, 1, 0);
      }

      public PrvHistComboBox(string text) : this()
      {
         Label.Text = text;
      }
   }

   public class PrvHistTagBox : TableLayoutPanel
   {
      public Label Label { get; set; }
      public TagComboBox TagBox { get; set; }

      public PrvHistTagBox()
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

         TagBox = ControlFactory.GetTagComboBox(true);

         Controls.Add(Label, 0, 0);
         Controls.Add(TagBox, 1, 0);
      }

      public PrvHistTagBox(string text) : this()
      {
         Label.Text = text;
      }
   }
}
