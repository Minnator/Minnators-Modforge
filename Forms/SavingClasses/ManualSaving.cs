using Editor.Savers;

namespace Editor.Forms.SavingClasses
{
   public partial class ManualSaving : Form
   {
      public ManualSaving()
      {
         InitializeComponent();

         SetCheckBoxStatus(Globals.ModifiedData);
      }

      public void SetCheckBoxStatus(ModifiedData data)
      {
         // set the checked status of the checkboxes based on the data
         SavingCheckedListBox.SetItemChecked(0, data.SaveProvinces);
         SavingCheckedListBox.SetItemChecked(1, data.SaveAreas);
         SavingCheckedListBox.SetItemChecked(2, data.SaveRegions);
         SavingCheckedListBox.SetItemChecked(3, data.SaveTradeNodes);
         SavingCheckedListBox.SetItemChecked(4, data.SaveTradeCompanies);
         SavingCheckedListBox.SetItemChecked(5, data.ColonialRegions);
         SavingCheckedListBox.SetItemChecked(6, data.SuperRegions);
         SavingCheckedListBox.SetItemChecked(7, data.Continents);
         SavingCheckedListBox.SetItemChecked(8, data.ProvinceGroups);
      }

      public ModifiedData GetModifiedDataSelection()
      {
         return new()
         {
            SaveProvinces = SavingCheckedListBox.GetItemChecked(0),
            SaveAreas = SavingCheckedListBox.GetItemChecked(1),
            SaveRegions = SavingCheckedListBox.GetItemChecked(2),
            SaveTradeNodes = SavingCheckedListBox.GetItemChecked(3),
            SaveTradeCompanies = SavingCheckedListBox.GetItemChecked(4),
            ColonialRegions = SavingCheckedListBox.GetItemChecked(5),
            SuperRegions = SavingCheckedListBox.GetItemChecked(6),
            Continents = SavingCheckedListBox.GetItemChecked(7),
            ProvinceGroups = SavingCheckedListBox.GetItemChecked(8),
         };
      }

      private void SaveSelectedButton_Click(object sender, EventArgs e)
      {
          SavingUtil.SaveAllModified(GetModifiedDataSelection());
      }
   }
}
