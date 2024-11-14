using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Forms.Feature.SavingClasses
{
   public partial class ManualSaving : Form
   {
      public ManualSaving()
      {
         InitializeComponent();

         SetCheckBoxStatus(Globals.SaveableType);
      }

      public void SetCheckBoxStatus(SaveableType data)
      {
         if ((data & SaveableType.SaveProvinces) != 0)
            SavingCheckedListBox.SetItemChecked(0, true);
         if ((data & SaveableType.Area) != 0)
            SavingCheckedListBox.SetItemChecked(1, true);
         if ((data & SaveableType.Region) != 0)
            SavingCheckedListBox.SetItemChecked(2, true);
         if ((data & SaveableType.TradeNode) != 0)
            SavingCheckedListBox.SetItemChecked(3, true);
         if ((data & SaveableType.TradeCompany) != 0)
            SavingCheckedListBox.SetItemChecked(4, true);
         if ((data & SaveableType.ColonialRegion) != 0)
            SavingCheckedListBox.SetItemChecked(5, true);
         if ((data & SaveableType.SuperRegion) != 0)
            SavingCheckedListBox.SetItemChecked(6, true);
         if ((data & SaveableType.Continent) != 0)
            SavingCheckedListBox.SetItemChecked(7, true);
         if ((data & SaveableType.ProvinceGroup) != 0)
            SavingCheckedListBox.SetItemChecked(8, true);
         if ((data & SaveableType.EventModifier) != 0)
            SavingCheckedListBox.SetItemChecked(9, true);
         if ((data & SaveableType.Localisation) != 0)
            SavingCheckedListBox.SetItemChecked(10, true);
         if ((data & SaveableType.Country) != 0)
            SavingCheckedListBox.SetItemChecked(11, true);
      }

      public SaveableType GetModifiedDataSelection()
      {
         SaveableType data = 0;
         if (SavingCheckedListBox.GetItemChecked(0))
            data |= SaveableType.SaveProvinces;
         if (SavingCheckedListBox.GetItemChecked(1))
            data |= SaveableType.Area;
         if (SavingCheckedListBox.GetItemChecked(2))
            data |= SaveableType.Region;
         if (SavingCheckedListBox.GetItemChecked(3))
            data |= SaveableType.TradeNode;
         if (SavingCheckedListBox.GetItemChecked(4))
            data |= SaveableType.TradeCompany;
         if (SavingCheckedListBox.GetItemChecked(5))
            data |= SaveableType.ColonialRegion;
         if (SavingCheckedListBox.GetItemChecked(6))
            data |= SaveableType.SuperRegion;
         if (SavingCheckedListBox.GetItemChecked(7))
            data |= SaveableType.Continent;
         if (SavingCheckedListBox.GetItemChecked(8))
            data |= SaveableType.ProvinceGroup;
         if (SavingCheckedListBox.GetItemChecked(9))
            data |= SaveableType.EventModifier;
         if (SavingCheckedListBox.GetItemChecked(10))
            data |= SaveableType.Localisation;
         if (SavingCheckedListBox.GetItemChecked(11))
            data |= SaveableType.Country;

         return data;
      }

      private void SaveSelectedButton_Click(object sender, EventArgs e)
      {
         SaveMaster.SaveAllChanges(saveableType:GetModifiedDataSelection());
         //SavingUtil.SaveAllModified(GetModifiedDataSelection());
      }

      private void ManualSaving_KeyDown(object sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Escape)
            Close();
      }
   }
}
