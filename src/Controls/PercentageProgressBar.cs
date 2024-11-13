using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor.Controls
{
   public enum ProgressBarDisplayText
   {
      Percentage,
      CustomText
   }

   class CustomProgressBar : ProgressBar
   {
      //Property to set to decide whether to print a % or Text
      public ProgressBarDisplayText DisplayStyle { get; set; }

      //Property to hold the custom text
      public String CustomText { get; set; } = "Loading..";

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
            // As we are doing this ourselves we need to draw the chunks on the progress bar
            var clip = rect with { Width = (int)Math.Round(((float)Value / Maximum) * rect.Width) };
            ProgressBarRenderer.DrawHorizontalChunks(g, clip);
         }

         // Set the Display text (Either a % amount or our custom text
         var percent = (int)(((double)Value / Maximum) * 100);
         var text = DisplayStyle == ProgressBarDisplayText.Percentage ? percent.ToString() + '%' : CustomText;

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
