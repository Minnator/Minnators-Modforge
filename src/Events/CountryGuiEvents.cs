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
         Globals.MapWindow.LoadCountryToGui(country);
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
            if (args.PropertyName == nameof(GuiSettings.ShowCountryFlagInCE))
               Globals.MapWindow.CountryFlagLabel.Visible = Globals.Settings.Gui.ShowCountryFlagInCE;
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
   }
}