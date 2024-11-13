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
         Selection.ClearSelection();

         if (IsProvince)
         {
            Globals.ZoomControl.FocusOn(Province.Bounds);
            Selection.AddProvinceToSelection(Province);
         }
         else
         {
            var provinces = Globals.Countries[CountryTag].GetProvinces();
            Globals.ZoomControl.FocusOn(Geometry.GetBounds(provinces));
            Selection.AddProvincesToSelection(provinces);
         }
      }
   }
}