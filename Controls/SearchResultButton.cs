using Editor.DataClasses.GameDataClasses;

namespace Editor.Controls
{
   public class SearchResultButton : Button
   {
      public bool IsProvince { get; set; }
      public Province Province { get; set; } = Province.Empty;
      public Tag CountryTag { get; set; }

      public SearchResultButton()
      {
         Click += SearchResultButton_Click;
      }

      private void SearchResultButton_Click(object? sender, EventArgs e)
      {
         Globals.ZoomControl.FocusOn(IsProvince
            ? Province.Center
            : Globals.Countries[CountryTag].Capital.Center);
      }
   }
}