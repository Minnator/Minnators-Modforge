using Editor.DataClasses.Settings;

namespace Editor.Forms.Feature
{
   public partial class SettingsWindow : Form
   {

      public SettingsWindow()
      {
         InitializeComponent();
         SettingsTabs.DrawItem += TabControl_DrawItem; 
         CreateTabsForSettings();

         var maxTextWidth = 0;
         foreach (TabPage tabPage in SettingsTabs.TabPages)
         {
            var sizeText = TextRenderer.MeasureText(tabPage.Text, SettingsTabs.Font);
            if (sizeText.Width > maxTextWidth)
               maxTextWidth = sizeText.Width;
         }

         SettingsTabs.ItemSize = new Size(28, maxTextWidth + 3);
      }

      private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
      {
         var g = e.Graphics;
         var text = SettingsTabs.TabPages[e.Index].Text;
         var sizeText = g.MeasureString(text, SettingsTabs.Font);

         var x = e.Bounds.Left + 3;
         var y = e.Bounds.Top + (e.Bounds.Height - sizeText.Height) / 2;

         g.DrawString(text, SettingsTabs.Font, Brushes.Black, x, y);
      }


      private void CreateTabsForSettings()
      {
         SettingsTabs.TabPages.Clear();
         var settingsProperties = typeof(Settings)
                .GetProperties()
                .Where(prop => prop.PropertyType.IsSubclassOf(typeof(SubSettings)))
                .Where(prop =>
                {
                   var instance = prop.GetValue(Globals.Settings) as SubSettings; 
                   return instance is { IsAvailable: true }; 
                });

         foreach (var prop in settingsProperties)
         {
            var tabPage = new TabPage(prop.Name)
            {
               AutoScroll = true
            };

            var propertyGrid = new PropertyGrid
            {
               Dock = DockStyle.Fill,
               SelectedObject = prop.GetValue(Globals.Settings)
            };
            propertyGrid.ExpandAllGridItems();
            
            tabPage.Controls.Add(propertyGrid);

            SettingsTabs.TabPages.Add(tabPage);
         }
      }

      private void SaveButton_Click(object sender, EventArgs e)
      {
         SettingsSaver.Save(Globals.Settings);
         Globals.Settings.Gui.Invalidate(nameof(GuiSettings.MapModes));
         Globals.ZoomControl.BorderColor = Globals.Settings.Rendering.Map.MapBorderColor;
         Globals.ZoomControl.Invalidate();
         Close();
      }

      private void ResetAll_Click(object sender, EventArgs e)
      {
         Globals.Settings = new();
         CreateTabsForSettings();
      }

      private void ResetButton_Click(object sender, EventArgs e)
      {
         if (SettingsTabs.SelectedTab == null || SettingsTabs.SelectedIndex == -1)
            return;
         var currentPropertyInTab = SettingsTabs.SelectedTab.Text;
         var property = typeof(Settings).GetProperty(currentPropertyInTab);
         if (property == null)
            return;

         property.SetValue(Globals.Settings, Activator.CreateInstance(property.PropertyType));
         // Refresh the property grid
         foreach (Control control in SettingsTabs.SelectedTab.Controls)
         {
            if (control is PropertyGrid propertyGrid)
               propertyGrid.SelectedObject = property.GetValue(Globals.Settings);
         }
      }

      private void SettingsWindow_FormClosing(object sender, FormClosingEventArgs e)
      {
         Globals.Settings.Gui.Invalidate(nameof(GuiSettings.MapModes));
      }
   }
}
