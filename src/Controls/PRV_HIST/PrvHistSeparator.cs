using System.Diagnostics;

namespace Editor.Controls.PRV_HIST
{
   public sealed class PrvHistSeparator : Control
   {
      private Color _color;
      private int _thickness;

      public Color Color
      {
         get => _color;
         set => _color = value;
      }

      public int Thickness
      {
         get => _thickness;
         set => _thickness = value;
      }

      public PrvHistSeparator()
      {
         _color = Color.Black;

         Margin = new(3, 1, 3, 1);
         Padding = new(0);

         Height = _thickness;
         Anchor = AnchorStyles.Left | AnchorStyles.Right;

         Paint += OnPaint;

      }
      
      private void OnPaint(object? sender, PaintEventArgs e)
      {
         e.Graphics.FillRectangle(new SolidBrush(_color), _thickness, 0, Width - _thickness * 2, _thickness);
         e.Graphics.DrawLine(Pens.Black, 0, 0, 0, Height);
         e.Graphics.DrawLine(Pens.Black, Width - 1, 0, Width - 1, Height);
      }
   }
}