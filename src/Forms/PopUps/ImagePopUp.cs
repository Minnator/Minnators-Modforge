using System.Runtime.CompilerServices;
using Editor.Controls;
using Editor.Helper;

namespace Editor.Forms.PopUps
{
   public partial class ImagePopUp : Form
   {
      public ZoomControl PictureBox = null!;

      public ImagePopUp()
      {
         InitializeComponent();
      }
      public static void ShowImage(Bitmap bmp)
      {
         var popup = new ImagePopUp();
         popup.PictureBox = new (bmp);
         popup.Controls.Add(popup.PictureBox);
         popup.PictureBox.FocusOn(new Rectangle(0, 0, bmp.Width, bmp.Height));
         popup.Text = "Image Viewer";
         popup.ShowDialog();
      }

      public static void ShowGameIcon(GameIconDefinition definition)
      {
         var popup = new ImagePopUp();
         popup.PictureBox = new(definition.Icon);
         popup.Controls.Add(popup.PictureBox);
         popup.PictureBox.FocusOn(new Rectangle(0, 0, definition.Icon.Width, definition.Icon.Height));
         popup.Text = definition.IconType.ToString();
         popup.ShowDialog();
      }
   }

}
