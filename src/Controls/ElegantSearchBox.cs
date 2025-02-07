using System.Drawing.Drawing2D;

namespace Editor.Controls
{
   public class ElegantSearchBox : TextBox, IElegantControl
   {
      public int CornerRadius { get; set; } = 10;
      public bool HasBorder { get; set; } = true;
      public Color BorderColor { get; set; } = Color.Black;

      public ElegantSearchBox()
      {
         SetStyle(ControlStyles.UserPaint, true);
         SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
         SetStyle(ControlStyles.AllPaintingInWmPaint, true);
         SetStyle(ControlStyles.SupportsTransparentBackColor, true);
         SetStyle(ControlStyles.ResizeRedraw, true);

         BorderStyle = BorderStyle.None;
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);

         var g = e.Graphics;

         if (Parent != null)
            g.Clear(Parent!.BackColor);
         g.SmoothingMode = SmoothingMode.AntiAlias;


         var rect = ClientRectangle;
         var backRect = new Rectangle(0, 0, rect.Width - 1, rect.Height - 1);
         using var path = ControlHelper.CreateRoundedRectanglePath(backRect, CornerRadius);
         using var backBrush = new SolidBrush(BackColor);
         g.FillPath(backBrush, path);

         if (HasBorder)
         {

            using var borderPen = new Pen(BorderColor, 3);
            g.DrawPath(borderPen, path);
         }
         // Draw the text manually
         var textRect = new RectangleF(5, 5, rect.Width - 10, rect.Height - 10);
         using var textBrush = new SolidBrush(ForeColor);
         var format = new StringFormat
         {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Near
         };

         // Draw the text within the bounds of the rounded rectangle
         g.DrawString(Text, Font, textBrush, textRect, format);

         // Draw the caret (cursor) if it's within the textbox
         if (Focused && !string.IsNullOrEmpty(Text) && SelectionLength == 0)
         {
            var caretPos = GetPositionFromCharIndex(SelectionStart);
            var caretRect = new RectangleF(caretPos.X, caretPos.Y, 2, Font.Height);
            using var caretBrush = new SolidBrush(ForeColor);
            g.FillRectangle(caretBrush, caretRect);
         }

      }
   }
}