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

   }
}