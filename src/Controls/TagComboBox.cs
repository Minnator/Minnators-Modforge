using System.ComponentModel;
using Editor.Events;
using Editor.Helper;

namespace Editor.Controls
{
   public sealed class TagComboBox : ComboBox
   {
      public EventHandler<ProvinceEditedEventArgs>? OnTagChanged = delegate { };
      public TagComboBox()
      {
         AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         AutoCompleteSource = AutoCompleteSource.ListItems;
         Dock = DockStyle.Fill;
         Height = 21;
         
         DataSource = new BindingSource
         {
            DataSource = Globals.Countries
         };
         Globals.Countries.AddControl(this);
      }
      
      protected override void OnSelectedIndexChanged(EventArgs e)
      {
         base.OnSelectedIndexChanged(e);
         //OnTagChanged?.Invoke(this, new (Selection.GetSelectedProvinces, Text));
      }
   }
   
   public static class ComboBoxExtensions
   {
      public static void Clear(this ComboBox box)
      {
         box.Text = "";
         box.SelectedIndex = -1;
      }
   }
}