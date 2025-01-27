namespace Editor.Controls
{
   public enum ProgressBarDisplayText
   {
      Percentage,
      CustomText,
      Both,
      PercentageNumOfTasks,
      All,
      CustomPercentage
   }

   class CustomProgressBar : ProgressBar
   {
      //Property to set to decide whether to print a % or Text
      public ProgressBarDisplayText DisplayStyle { get; set; }
      public int TaskCompletedCount = 0;
      public int TaskCount = 0;

      //Property to hold the custom text
      public string CustomText { get; set; } = "Loading..";

      public CustomProgressBar()
      {
         // Modify the ControlStyles flags
         //http://msdn.microsoft.com/en-us/library/system.windows.forms.controlstyles.aspx
         SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer,
            true);
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         var rect = ClientRectangle;
         var g = e.Graphics;

         ProgressBarRenderer.DrawHorizontalBar(g, rect);
         rect.Inflate(-3, -3);
         if (Value > 0)
         {
            var clip = rect with { Width = (int)Math.Round(((float)Value / Maximum) * rect.Width) };
            ProgressBarRenderer.DrawHorizontalChunks(g, clip);
         }

         // Set the Display text (Either a % amount or our custom text
         var percent = (int)(((double)Value / Maximum) * 100);
         var text = string.Empty;
         switch (DisplayStyle)
         {
            case ProgressBarDisplayText.Percentage:
               text = percent + "%";
               break;
            case ProgressBarDisplayText.CustomText:
               text = CustomText;
               break;
            case ProgressBarDisplayText.Both:
               text = percent + "% " + CustomText;
               break;
            case ProgressBarDisplayText.PercentageNumOfTasks:
               text = $"{percent}% ({TaskCompletedCount}/{TaskCount})";
               break;
            case ProgressBarDisplayText.All:
               text = $"{percent}% ({TaskCompletedCount}/{TaskCount}) ({CustomText})";
               break;
            case ProgressBarDisplayText.CustomPercentage:
               text = $"{percent}% ({CustomText})";
               break;
         }

         using var f = new Font(FontFamily.GenericSerif, 10);
         var len = g.MeasureString(text, f);
         // Calculate the location of the text (the middle of progress bar)
         // Point location = new Point(Convert.ToInt32((rect.Width / 2) - (len.Width / 2)), Convert.ToInt32((rect.Height / 2) - (len.Height / 2)));
         var location = new Point(Convert.ToInt32((Width / 2) - (len.Width / 2)),
            Convert.ToInt32((Height / 2) - len.Height / 2));
         // The commented-out code will centre the text into the highlighted area only. This will centre the text regardless of the highlighted area.
         // Draw the custom text
         g.DrawString(text, f, Brushes.Black, location);
      }
   }
}
