namespace Editor.Controls.NewControls
{
   public class BorderedPictureBox : PictureBox
   {
      public int BorderWidth { get; set; } = 1;
      public Color BorderColor { get; set; } = Color.Black;
      public bool ShowBorder { get; set; } = true;


      protected override void OnPaint(PaintEventArgs pe)
      {
         base.OnPaint(pe);

         if (ShowBorder) 
            pe.Graphics.DrawRectangle(new (BorderColor, BorderWidth), BorderWidth - 1, BorderWidth - 1, Width - BorderWidth - 1, Height - BorderWidth - 1);
      }
   }
}