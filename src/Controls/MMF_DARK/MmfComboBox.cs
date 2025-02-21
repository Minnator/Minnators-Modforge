using System.Drawing.Drawing2D;

namespace Editor.Controls.MMF_DARK
{
   public class MmfComboBox : ComboBox
   {
      private int _borderWidth = 2;
      private int _cornerRadius = 0;
      private bool _showBorder;
      private Color _borderColor = Color.Black;

      public bool ShowBorder
      {
         get => _showBorder;
         set => _showBorder = value;
      }

      public int BorderWidth
      {
         get => _borderWidth;
         set => _borderWidth = value;
      }

      public Color BorderColor
      {
         get => _borderColor;
         set => _borderColor = value;
      }

      public int CornerRadius
      {
         get => _cornerRadius;
         set => _cornerRadius = value;
      }


      public MmfComboBox()
      {
         SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
         DrawMode = DrawMode.OwnerDrawFixed;

         DrawMode = DrawMode.OwnerDrawFixed;
         DropDownStyle = ComboBoxStyle.DropDownList; // Prevents user input
         ItemHeight = 20; // Adjusts dropdown item height
      }

      protected override void WndProc(ref Message m)
      {
         base.WndProc(ref m);

         var labelRect = new Rectangle(3, 3, Width - 23, Height - 6);
         if (m.Msg == 0xF) // WM_PAINT
         {
            using var g = Graphics.FromHwnd(Handle);

            if (Parent != null && CornerRadius > 0)
               g.Clear(Parent.BackColor);

            var rect = ClientRectangle with { Width = ClientRectangle.Width - BorderWidth, Height = ClientRectangle.Height - BorderWidth };
            // Draw the label part (selected item area)
            if (CornerRadius > 0)
            {
               using var path = ControlHelper.CreateRoundedRectanglePath(rect, CornerRadius + BorderWidth / 2);
               g.FillPath(new SolidBrush(BackColor), path);
            }

            // Draw the selected item text
            if (SelectedIndex >= 0)
            {
               var text = Items[SelectedIndex]?.ToString();
               using Brush textBrush = new SolidBrush(ForeColor);
               g.DrawString(text, Font, textBrush, labelRect);
            }

            DrawArrow(g, new(Width - 21, 0, 21, Height));

            // Draw border
            if (ShowBorder)
            {
               using var penBorder = new Pen(_borderColor, _borderWidth);

               if (_cornerRadius > 0)
               {
                  penBorder.Alignment = PenAlignment.Center;
                  penBorder.Width = MathF.Min(2.5f, Math.Max(2.5f, BorderWidth));
                  using var fadePath = ControlHelper.CreateRoundedRectanglePath(rect, (int)penBorder.Width + BorderWidth / 2);
                  penBorder.Color = Parent?.BackColor ?? BackColor;
                  g.SmoothingMode = SmoothingMode.HighQuality;
                  g.DrawPath(penBorder, fadePath);
               }
               else
               {
                  penBorder.Alignment = PenAlignment.Inset;
                  g.SmoothingMode = SmoothingMode.AntiAlias;
                  g.DrawRectangle(penBorder, 0, 0, Width - 0.5F, Height - 0.5F);
               }
            }
         }
      }

      // Override OnDrawItem to customize dropdown items
      protected override void OnDrawItem(DrawItemEventArgs e)
      {
         if (e.Index < 0) return;

         e.DrawBackground();

         using Brush textBrush = new SolidBrush(ForeColor);
         e.Graphics.DrawString(Items[e.Index]?.ToString(), Font, textBrush, e.Bounds);

         e.DrawFocusRectangle();
      }

      private void DrawArrow(Graphics g, Rectangle bounds)
      {
         using var arrowPen = new Pen(ForeColor, 1);
         var centerX = bounds.Left + bounds.Width / 2;
         var centerY = bounds.Top + bounds.Height / 2;

         // Left diagonal line \
         g.DrawLine(arrowPen, centerX - 5, centerY - 3, centerX, centerY + 2);

         // Right diagonal line /
         g.DrawLine(arrowPen, centerX, centerY + 2, centerX + 5, centerY - 3);
      }
   }
}