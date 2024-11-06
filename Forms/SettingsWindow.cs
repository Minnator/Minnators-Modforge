using System.Windows.Forms;
using Editor.DataClasses.Settings;

namespace Editor.Forms
{
   public partial class SettingsWindow : Form
   {
      private Settings settings;

      public SettingsWindow(Settings settings)
      {
         this.settings = settings;
         InitializeComponent();
         CreateTabsForSettings();
      }

      private void CreateTabsForSettings()
      {
         // Get all the properties of the Settings class that are subclass settings
         var settingsProperties = typeof(Settings)
            .GetProperties()
            .Where(prop => prop.PropertyType is { IsClass: true, Namespace: "Editor.DataClasses.Settings" });

         foreach (var prop in settingsProperties)
         {
            // Create a new TabPage for each setting subclass
            var tabPage = new TabPage(prop.Name)
            {
               AutoScroll = true
            };

            // Create a PropertyGrid and set its selected object to the property value
            var propertyGrid = new PropertyGrid
            {
               Dock = DockStyle.Fill,
               SelectedObject = prop.GetValue(settings)
            };

            // Add PropertyGrid to the tab page
            tabPage.Controls.Add(propertyGrid);

            // Add tab page to the TabControl
            SettingsTabs.TabPages.Add(tabPage);
         }
      }

      private void SaveButton_Click(object sender, EventArgs e)
      {
         SettingsSaver.Save(Globals.Settings);
         Close();
      }

      private void CancelButton_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void ResetButton_Click(object sender, EventArgs e)
      {
         Globals.Settings = new();
      }
   }
}
