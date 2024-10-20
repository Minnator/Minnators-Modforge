namespace Editor.Controls
{
   public class ColorPickerButton : Button
   {
      public ColorPickerButton()
      {
         SetColor(Color.FromArgb(255,255,255));
      }

      public Color GetColor => BackColor;
      public void SetColor(Color color)
      {
         BackColor = color;
         SetColorText(color);
      }

      public void ClearColor()
      {
         SetColor(Color.FromArgb(255,255,255));
      }

      private void SetColorText(Color color)
      {
         Text = $"({color.R}/{color.G}/{color.B})";
      }

      protected override void OnClick(EventArgs e)
      {
         using var colorDialog = new ColorDialog();
         colorDialog.Color = BackColor;
         if (colorDialog.ShowDialog() == DialogResult.OK)
            SetColor(colorDialog.Color);
         base.OnClick(e);
      }
   }
}