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
         if ((data & ModifiedData.Area) != 0)
            SavingCheckedListBox.SetItemChecked(1, true);
         if ((data & ModifiedData.Region) != 0)
            SavingCheckedListBox.SetItemChecked(2, true);
         if ((data & ModifiedData.TradeNode) != 0)
            SavingCheckedListBox.SetItemChecked(3, true);
         if ((data & ModifiedData.TradeCompany) != 0)
            SavingCheckedListBox.SetItemChecked(4, true);
         if ((data & ModifiedData.ColonialRegion) != 0)
            SavingCheckedListBox.SetItemChecked(5, true);
         if ((data & ModifiedData.SuperRegion) != 0)
            SavingCheckedListBox.SetItemChecked(6, true);
         if ((data & ModifiedData.Continent) != 0)
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
            data |= ModifiedData.Area;
         if (SavingCheckedListBox.GetItemChecked(2))
            data |= ModifiedData.Region;
         if (SavingCheckedListBox.GetItemChecked(3))
            data |= ModifiedData.TradeNode;
         if (SavingCheckedListBox.GetItemChecked(4))
            data |= ModifiedData.TradeCompany;
         if (SavingCheckedListBox.GetItemChecked(5))
            data |= ModifiedData.ColonialRegion;
         if (SavingCheckedListBox.GetItemChecked(6))
            data |= ModifiedData.SuperRegion;
         if (SavingCheckedListBox.GetItemChecked(7))
            data |= ModifiedData.Continent;
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
