using Editor.DataClasses;
using Editor.Helper;
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
         if ((data & ModifiedData.SaveProvinces) != 0)
            SavingCheckedListBox.SetItemChecked(0, true);
         if ((data & ModifiedData.Areas) != 0)
            SavingCheckedListBox.SetItemChecked(1, true);
         if ((data & ModifiedData.Regions) != 0)
            SavingCheckedListBox.SetItemChecked(2, true);
         if ((data & ModifiedData.TradeNode) != 0)
            SavingCheckedListBox.SetItemChecked(3, true);
         if ((data & ModifiedData.SaveTradeCompanies) != 0)
            SavingCheckedListBox.SetItemChecked(4, true);
         if ((data & ModifiedData.ColonialRegions) != 0)
            SavingCheckedListBox.SetItemChecked(5, true);
         if ((data & ModifiedData.SuperRegions) != 0)
            SavingCheckedListBox.SetItemChecked(6, true);
         if ((data & ModifiedData.Continents) != 0)
            SavingCheckedListBox.SetItemChecked(7, true);
         if ((data & ModifiedData.ProvinceGroups) != 0)
            SavingCheckedListBox.SetItemChecked(8, true);
         if ((data & ModifiedData.EventModifiers) != 0)
            SavingCheckedListBox.SetItemChecked(9, true);
         if ((data & ModifiedData.Localisation) != 0)
            SavingCheckedListBox.SetItemChecked(10, true);
      }

      public ModifiedData GetModifiedDataSelection()
      {
         ModifiedData data = 0;
         if (SavingCheckedListBox.GetItemChecked(0))
            data |= ModifiedData.SaveProvinces;
         if (SavingCheckedListBox.GetItemChecked(1))
            data |= ModifiedData.Areas;
         if (SavingCheckedListBox.GetItemChecked(2))
            data |= ModifiedData.Regions;
         if (SavingCheckedListBox.GetItemChecked(3))
            data |= ModifiedData.TradeNode;
         if (SavingCheckedListBox.GetItemChecked(4))
            data |= ModifiedData.SaveTradeCompanies;
         if (SavingCheckedListBox.GetItemChecked(5))
            data |= ModifiedData.ColonialRegions;
         if (SavingCheckedListBox.GetItemChecked(6))
            data |= ModifiedData.SuperRegions;
         if (SavingCheckedListBox.GetItemChecked(7))
            data |= ModifiedData.Continents;
         if (SavingCheckedListBox.GetItemChecked(8))
            data |= ModifiedData.ProvinceGroups;
         if (SavingCheckedListBox.GetItemChecked(9))
            data |= ModifiedData.EventModifiers;
         if (SavingCheckedListBox.GetItemChecked(10))
            data |= ModifiedData.Localisation;

         return data;
      }

      private void SaveSelectedButton_Click(object sender, EventArgs e)
      {
         FileManager.SaveChanges(modifiedData:GetModifiedDataSelection());
         //SavingUtil.SaveAllModified(GetModifiedDataSelection());
      }

      private void ManualSaving_KeyDown(object sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Escape)
            Close();
      }
   }
}
