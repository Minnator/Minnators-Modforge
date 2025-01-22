using System.Diagnostics;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Settings;
using Editor.DiscordGame_SDK;
using Editor.Forms.Feature;
using Editor.Helper;
using Editor.Loading;

namespace Editor.Events
{
   public static class CountryGuiEvents
   {
      internal static void CountryColorPickerButton_Click(object? sender, EventArgs e)
      {
         if (sender is not ColorPickerButton button)
            return;
         Selection.SelectedCountry.Color = button.GetColor;
      }

      internal static void TagSelectionBox_OnTagChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Tag.TryParse(e.Value.ToString()!, out var tag))
            return;
         if (tag == Tag.Empty)
            Selection.SetCountrySelected(Country.Empty);
         else if (Globals.Countries.TryGetValue(tag, out var country))
            Selection.SetCountrySelected(country);
      }

      public static void OnCountrySelected(object? sender, Country country)
      {
         //Globals.MapWindow.LoadCountryToGui(country);
      }

      public static void OnCountryDeselected(object? sender, Country e)
      {
         Globals.MapWindow.ClearCountryGui();
      }

      
      public static void TechGroupBox_SelectedIndexChanged(object? sender, EventArgs e)
      {
         if (sender is not ComboBox box || box.SelectedItem == null)
            return;
         if (Selection.SelectedCountry == Country.Empty)
            return;
         
         if (Globals.TechnologyGroups.TryGetValue(box.SelectedItem!.ToString()!, out var techGroup))
            Selection.SelectedCountry.HistoryCountry.TechnologyGroup = techGroup;
      }

      public static void FocusComboBox_SelectedIndexChanged(object? sender, EventArgs e)
      {
         if (sender is not ComboBox box || box.SelectedItem == null)
            return;
         if (Selection.SelectedCountry == Country.Empty)
            return;
         if (box.SelectedItem.ToString()!.Equals(Mana.NONE.ToString()))
            return;

         Selection.SelectedCountry.HistoryCountry.NationalFocus = Enum.Parse<Mana>(box.SelectedItem.ToString()!);
      }


      public static void GovernmentRankBox_SelectedIndexChanged(object? sender, EventArgs e)
      {
         if (sender is not ComboBox box || box.SelectedItem == null)
            return;
         if (Selection.SelectedCountry == Country.Empty)
            return;
         if (int.TryParse(box.SelectedItem.ToString()!, out var rank))
            Selection.SelectedCountry.HistoryCountry.GovernmentRank = rank;
      }


      public static void ShipNames_ContentModified(object? sender, string s)
      {
         if (Selection.SelectedCountry == Country.Empty || sender is not SmartTextBox { Parent: TableLayoutPanel
                {
                   Parent: NamesEditor ne
                }
             })
            return;
         Selection.SelectedCountry.CommonCountry.ShipNames = ne.GetNames();
      }

      public static void ArmyNames_ContentModified(object? sender, string s)
      {
         if (Selection.SelectedCountry == Country.Empty || sender is not SmartTextBox { Parent: TableLayoutPanel
                {
                   Parent: NamesEditor ne
                }
             })
            return;
         Selection.SelectedCountry.CommonCountry.ArmyNames = ne.GetNames();
      }

      public static void FleetNames_ContentModified(object? sender, string s)
      {
         if (Selection.SelectedCountry == Country.Empty || sender is not SmartTextBox { Parent: TableLayoutPanel
                {
                   Parent: NamesEditor ne
                }
             })
            return;
         Selection.SelectedCountry.CommonCountry.FleetNames = ne.GetNames();
      }

      public static void LeaderNames_ContentModified(object? sender, string s)
      {
         if (Selection.SelectedCountry == Country.Empty || sender is not SmartTextBox { Parent: TableLayoutPanel
                {
                   Parent: NamesEditor ne
                }
             })
            return;
         Selection.SelectedCountry.CommonCountry.LeaderNames = ne.GetNames();
      }

      public static void AddMonarchName_Click(object? sender, EventArgs e)
      {
         if (!InputHelper.GetStringIfNotEmpty(Globals.MapWindow.NameTextBox, out var name))
            return;
         if (!InputHelper.GetIntIfNotEmpty(Globals.MapWindow.ChanceTextBox, out var chance))
            return;

         Debug.Assert(Selection.SelectedCountry != Country.Empty, "Selection.SelectedCountry != Country.Empty when it must not be empty");

         MonarchName mName = new(name, chance);
         MonarchName.AddToGlobals(mName);
         Globals.MapWindow.AddMonarchNameToGui(mName);
      }

      public static void MonarchName_ContentModified(object? sender, string oldStr, string newStr, int chance)
      {
         Debug.Assert(Selection.SelectedCountry != Country.Empty, "Selection.SelectedCountry != Country.Empty when it must not be empty");
         
         MonarchName.UpdateGlobals(oldStr, new (newStr, chance));
      }

      public static void MonarchName_DeleteButton_Click(object? sender, string name)
      {
         Debug.Assert(Selection.SelectedCountry != Country.Empty, "Selection.SelectedCountry != Country.Empty when it must not be empty");

         MonarchName.DeleteFromGlobals(name);
      }
   }
}