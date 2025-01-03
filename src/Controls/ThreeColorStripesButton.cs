﻿namespace Editor.Controls
{
   public class ThreeColorStripesButton : Button
   {
      public int ColorIndex1 { get; set; }
      public int ColorIndex2 { get; set; }
      public int ColorIndex3 { get; set; }

      public ThreeColorStripesButton()
      {
         ColorIndex1 = 0;
         ColorIndex2 = 0;
         ColorIndex3 = 0;
         base.ForeColor = Color.Black;
      }

      public void SetColorIndexes(int one, int two, int three)
      {
         ColorIndex1 = one;
         ColorIndex2 = two;
         ColorIndex3 = three;
         Invalidate();
      }

      public void Clear()
      {
         ColorIndex1 = 0;
         ColorIndex2 = 0;
         ColorIndex3 = 0;
         Invalidate();
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);

         Rectangle rect = new (3, 3, ClientRectangle.Width - 4, ClientRectangle.Height - 4);
         var width = (rect.Width) / 3;

         if (!Globals.RevolutionaryColors.TryGetValue(ColorIndex1, out var col1)
             || !Globals.RevolutionaryColors.TryGetValue(ColorIndex2, out var col2)
             || !Globals.RevolutionaryColors.TryGetValue(ColorIndex3, out var col3))
         {
            MessageBox.Show($"Revolutionary Color index not found: ({ColorIndex1}, {ColorIndex2}, {ColorIndex1})", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }


         using (var brush1 = new SolidBrush(col1))
         using (var brush2 = new SolidBrush(col2))
         using (var brush3 = new SolidBrush(col3))
         {
            e.Graphics.FillRectangle(brush1, new(rect.Left, rect.Top, width, rect.Height - 2));
            e.Graphics.FillRectangle(brush2, new(rect.Left + width, rect.Top, width, rect.Height - 2));
            e.Graphics.FillRectangle(brush3, new(rect.Left + 2 * width, rect.Top, width, rect.Height - 2));
         }

         // Draw index text centered in each stripe
         TextRenderer.DrawText(e.Graphics, ColorIndex1.ToString(), Font,
            new Rectangle(rect.Left, rect.Top - 1, width, rect.Height), ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

         TextRenderer.DrawText(e.Graphics, ColorIndex2.ToString(), Font,
            new Rectangle(rect.Left + width, rect.Top - 1, width, rect.Height), ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

         TextRenderer.DrawText(e.Graphics, ColorIndex3.ToString(), Font,
            new Rectangle(rect.Left + 2 * width, rect.Top - 1, width, rect.Height), ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

         // Draw a border around the button
         using (var borderPen = new Pen(Color.Black, 1)) // Adjust color and thickness as needed
         {
            e.Graphics.DrawRectangle(borderPen, 2, 2, rect.Width - 1, rect.Height - 1);
         }
         
         if (!Enabled)
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.White)), ClientRectangle);
      }


   }
}