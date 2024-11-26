using Editor.DataClasses.Settings;

namespace Editor.Forms.Feature
{
   public partial class SettingsWindow : Form
   {

      public SettingsWindow()
      {
         InitializeComponent();
         CreateTabsForSettings();
      }

      private void CreateTabsForSettings()
      {
         SettingsTabs.TabPages.Clear();
         var settingsProperties = typeof(Settings)
            .GetProperties()
            .Where(prop => prop.PropertyType is { IsClass: true, Namespace: "Editor.DataClasses.Settings" });

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

            tabPage.Controls.Add(propertyGrid);

            SettingsTabs.TabPages.Add(tabPage);
         }
      }

      private void SaveButton_Click(object sender, EventArgs e)
      {
         SettingsSaver.Save(Globals.Settings);
         Globals.Settings.Gui.Invalidate(nameof(GuiSettings.MapModes));
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
