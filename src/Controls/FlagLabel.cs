using Editor.DataClasses.GameDataClasses;

namespace Editor.Controls
{

   public class FlagLabel : Label
   {
      public enum FlagLayout {
         Square,
         Stretch
      }

      public Country country;
      public Color BorderColor = Color.Black;
      public int BorderWidth = 1;

      public FlagLabel(Country country)
      {
         base.Dock = DockStyle.Fill;
         Margin = new(1);
         this.country = country;
      }

      public new FlagLayout Layout { get; set; } = FlagLayout.Square;

      public void SetCountry(Country countryIn)
      {
         this.country = countryIn;
         Invalidate();
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);
         // Get the flag image
         var flag = country.GetFlagBitmap();
         // Draw the flag
         switch (Layout)
         {
            case FlagLayout.Square:
               var smallestSided = Math.Min(ClientRectangle.Width, ClientRectangle.Height);
               var drawingRect = new Rectangle((ClientRectangle.Width - smallestSided) / 2, 0, smallestSided, smallestSided);
               e.Graphics.DrawImage(flag, drawingRect);
               var halfBorder = BorderWidth / 2;
               e.Graphics.DrawRectangle(new (BorderColor, BorderWidth), drawingRect.X + halfBorder, halfBorder, drawingRect.Width - halfBorder, drawingRect.Height - halfBorder - 1);
               break;
            case FlagLayout.Stretch:
               e.Graphics.DrawImage(flag, ClientRectangle);
               break;
         }

         if (!Enabled) 
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.White)), ClientRectangle);
      }
   }
}