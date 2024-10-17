using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

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

         // Highlight the province or country
         return; //TODO why this so fucked
         if (IsProvince)
            Selection.HighlightProvince(Province);
         else
            Selection.HighlightCountry(CountryTag);
      }
   }
}