using Editor.Controls;
using Editor.DataClasses;
using Editor.Helper;

namespace Editor.Forms
{
   public partial class Search : Form
   {
      public Search()
      {
         InitializeComponent();
      }

      private void SearchInput_TextChanged(object sender, EventArgs e)
      {
         var provinceMatches = SearchHelper.SearchForProvinces(SearchInput.Text);
         var countryMatches = SearchHelper.SearchForCountries(SearchInput.Text);

         DisplayResults(provinceMatches, countryMatches);
      }

      private void DisplayResults(List<int> provinceMatches, List<Tag> countryMatches)
      {
         SearchResultsPanel.Controls.Clear();

         SearchResultsPanel.SuspendLayout();
         foreach (var id in provinceMatches)
         {
            var button = ControlFactory.GetSearchResultButton(true, id, null!);
            SearchResultsPanel.Controls.Add(button);
         }

         foreach (var tag in countryMatches)
         {
            var button = ControlFactory.GetSearchResultButton(false, 0, tag);
            SearchResultsPanel.Controls.Add(button);
         }
         SearchResultsPanel.ResumeLayout();
      }
   }
}
