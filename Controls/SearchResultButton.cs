using Editor.DataClasses.GameDataClasses;

namespace Editor.Controls
{
   public class SearchResultButton : Button
   {
      public bool IsProvince { get; set; }
      public int ProvinceId { get; set; }
      public Tag CountryTag { get; set; }

      public SearchResultButton()
      {
         Click += SearchResultButton_Click;
      }

      private void SearchResultButton_Click(object? sender, EventArgs e)
      {
         Globals.MapWindow.MapPictureBox.FocusOn(IsProvince
            ? Globals.Provinces[ProvinceId].Center
            : Globals.Provinces[Globals.Countries[CountryTag].Capital].Center);
      }
   }
}