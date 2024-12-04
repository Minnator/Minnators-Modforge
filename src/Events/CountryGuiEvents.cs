using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Settings;
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

      public static void RevolutionColorPickerButton_Click(object? sender, MouseEventArgs e)
      {
         if (sender is not ThreeColorStripesButton button || Selection.SelectedCountry == Country.Empty)
            return;

         // Right click to reset the color
         if (e.Button == MouseButtons.Right)
         {
            var max = Globals.RevolutionaryColors.Count;
            var index1 = Globals.Random.Next(max);
            var index2 = Globals.Random.Next(max);
            var index3 = Globals.Random.Next(max);

            Selection.SelectedCountry.CommonCountry.RevolutionaryColor = Color.FromArgb(255, index1, index2, index3);
            button.SetColorIndexes(index1, index2, index3);
            return;
         }

         var revColorPicker = new RevolutionaryColorPicker();
         revColorPicker.SetIndexes(Selection.SelectedCountry.CommonCountry.RevolutionaryColor.R, Selection.SelectedCountry.CommonCountry.RevolutionaryColor.G, Selection.SelectedCountry.CommonCountry.RevolutionaryColor.B);
         revColorPicker.OnColorsChanged += (o, tuple) =>
         {
            Selection.SelectedCountry.CommonCountry.RevolutionaryColor = Color.FromArgb(tuple.Item1, tuple.Item2, tuple.Item3);
            Globals.MapWindow.RevolutionColorPickerButton.SetColorIndexes(tuple.Item1, tuple.Item2, tuple.Item3);
         };
         revColorPicker.ShowDialog();
      }

      public static void OnCountryDeselected(object? sender, Country e)
      {
         Globals.MapWindow.ClearCountryGui();
      }

      public static void SetGuiEventHandlers()
      {
         Globals.Settings.Misc.PropertyChanged += (_, args) =>
         {
            if (args.PropertyName == nameof(Settings.Misc.Language))
               LocalisationLoading.Load();
         };

         Globals.Settings.Gui.PropertyChanged += (_, args) =>
         {
            switch (args.PropertyName)
            {
               case nameof(GuiSettings.ShowCountryFlagInCE):
                  Globals.MapWindow.CountryFlagLabel.Visible = Globals.Settings.Gui.ShowCountryFlagInCE;
                  break;
               case nameof(GuiSettings.MapModes):
                  Globals.MapWindow.UpdateMapModeButtons(false);
                  break;
            }

         };

      }

      public static void GraphicalCultureBox_SelectedIndexChanged(object? sender, EventArgs e)
      {
         if (sender is not ComboBox box || box.SelectedItem == null)
            return;
         if (Selection.SelectedCountry == Country.Empty)
            return;
         Selection.SelectedCountry.CommonCountry.GraphicalCulture = box.SelectedItem.ToString()!;
      }

      public static void UnitTypeBox_SelectedIndexChanged(object? sender, EventArgs e)
      {
         if (sender is not ComboBox box || box.SelectedItem == null)
            return;
         if (Selection.SelectedCountry == Country.Empty)
            return;
         Selection.SelectedCountry.HistoryCountry.UnitType = box.SelectedItem.ToString()!;
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

      public static void CapitalTextBox_LostFocus(object? sender, EventArgs e)
      {
         if (sender is not TextBox box)
            return;
         if (Selection.SelectedCountry == Country.Empty)
            return;
         if (int.TryParse(box.Text, out var capital) && Globals.ProvinceIdToProvince.TryGetValue(capital, out var cap))
            Selection.SelectedCountry.HistoryCountry.Capital = cap;
      }

      public static void OnlyNumbers_KeyPress(object? sender, KeyPressEventArgs e)
      {
         if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            e.Handled = true;
      }

      public static void GovernmentReforms_OnItemAdded(object? sender, ProvinceEditedEventArgs e)
      {
         if (Selection.SelectedCountry == Country.Empty)
            return;
         if (e.Value is not string reform || !Globals.GovernmentReforms.ContainsKey(reform))
            return;
         Selection.SelectedCountry.HistoryCountry.GovernmentReforms.Add(reform);
      }

      public static void GovernmentReforms_OnItemRemoved(object? sender, ProvinceEditedEventArgs e)
      {
         if (Selection.SelectedCountry == Country.Empty)
            return;
         if (e.Value is not string reform) // we dont check if it is a valid reform to allow broken stuff being fixed
            return;
         Selection.SelectedCountry.HistoryCountry.GovernmentReforms.Remove(reform);
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

      public static void CountryNameLoc_Changed(object? sender, EventArgs e)
      {
         if (Selection.SelectedCountry == Country.Empty)
            return;

         Localisation.AddOrModifyLocObject(Selection.SelectedCountry.GetTitleLocKey, Globals.MapWindow.CountryLoc.Text);
      }

      public static void CountryAdjectiveLoc_Changed(object? sender, EventArgs e)
      {
         if (Selection.SelectedCountry == Country.Empty)
            return;

         Localisation.AddOrModifyLocObject(Selection.SelectedCountry.GetAdjectiveLocKey, Globals.MapWindow.CountryADJLoc.Text);
      }
   }
}