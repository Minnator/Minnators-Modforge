namespace Editor.Controls
{
   public class ColorIndexButton : Button
   {
      [Flags]
      public enum Borders
      {
         Top,
         Right,
         Bottom,
         Left
      }

      private int index = 0;
      public bool Draggable = false;
      public bool ShowPreview = false;
      public bool IsHovered = false;
      public int PreviewIndex = 0;
      public Borders BorderFlags = Borders.Top | Borders.Right | Borders.Bottom | Borders.Left;

      public EventHandler<int> OnIndexChanged = delegate { };

      public int Index
      {
         get => index;
         set
         {
            index = value;
            OnIndexChanged.Invoke(this, value);
         }
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);
         
         var drawingRect = ClientRectangle;
         var color = Color.White;
         if (Globals.RevolutionaryColors.TryGetValue(Index, out color)) {}

         // Fill Color
         e.Graphics.FillRectangle(new SolidBrush(color), drawingRect);

         // Border
         if (IsHovered)
            e.Graphics.DrawRectangle(new (Color.Aqua, 1), drawingRect with{Width = drawingRect.Width - 1, Height = drawingRect.Height - 1});
         else
         {
            if (BorderFlags.HasFlag(Borders.Top))
               e.Graphics.DrawLine(new(Color.Black, 1), drawingRect.X, drawingRect.Y, drawingRect.X + drawingRect.Width - 1, drawingRect.Y);
            if (BorderFlags.HasFlag(Borders.Right))
               e.Graphics.DrawLine(new(Color.Black, 1), drawingRect.X + drawingRect.Width - 1, drawingRect.Y, drawingRect.X + drawingRect.Width - 1, drawingRect.Y + drawingRect.Height - 1);
            if (BorderFlags.HasFlag(Borders.Bottom))
               e.Graphics.DrawLine(new(Color.Black, 1), drawingRect.X, drawingRect.Y + drawingRect.Height - 1, drawingRect.X + drawingRect.Width - 1,  drawingRect.Height - 1);
            if (BorderFlags.HasFlag(Borders.Left))
               e.Graphics.DrawLine(new(Color.Black, 1), drawingRect.X, drawingRect.Y, drawingRect.X, drawingRect.Y + drawingRect.Height - 1);
         }

         // Drag and Drop Preview
         if (ShowPreview)
         {
            Rectangle previewRect = new(drawingRect.X + 3, drawingRect.Y + 3, drawingRect.Width - 6, drawingRect.Height - 6);
            e.Graphics.DrawRectangle(new(Globals.RevolutionaryColors[PreviewIndex], 4), previewRect);
         }
      }

      protected override void OnMouseEnter(EventArgs e)
      {
         base.OnMouseEnter(e);
         IsHovered = true;
         Invalidate();
      }

      protected override void OnMouseLeave(EventArgs e)
      {
         base.OnMouseLeave(e);
         IsHovered = false;
         Invalidate();
      }

      protected override void OnMouseDown(MouseEventArgs e)
      {
         base.OnMouseDown(e);
         if (Draggable)
         {
            var data = new DataObject();
            data.SetData("Color", Index);
            DoDragDrop(data, DragDropEffects.Move);
         }
      }

      protected override void OnDragEnter(DragEventArgs drgevent)
      {
         base.OnDragEnter(drgevent);
         if (AllowDrop && drgevent.Data?.GetData("Color") is int preViewIndex)
         {
            drgevent.Effect = DragDropEffects.Move;
            ShowPreview = true;
            PreviewIndex = preViewIndex;
            Invalidate();
         }
      }

      protected override void OnDragLeave(EventArgs e)
      {
         base.OnDragLeave(e);
         ShowPreview = false;
         Invalidate();
      }

      // When dropping the button set the color
      protected override void OnDragDrop(DragEventArgs drgevent)
      {
         base.OnDragDrop(drgevent);
         if (AllowDrop && drgevent.Data?.GetData("Color") is int colorIndex)
         {
            Index = colorIndex;
            Invalidate();
         }
      }


   }
}