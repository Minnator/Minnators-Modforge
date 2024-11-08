using Editor.Helper;

namespace Editor.Forms
{
   public partial class EnterPathForm : Form
   {
      public EnterPathForm()
      {
         InitializeComponent();
         StartPosition = FormStartPosition.CenterScreen;

#if DEBUG
         Load += (sender, args) =>
         {
            ContinueButton.PerformClick();
         };
#endif
         
      }

      private void SelectVanillaPathButton_Click(object sender, EventArgs e)
      {
         var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
         IO.OpenFolderDialog(documentsFolder, "vanilla base game", out var folder);

         VanillaPathTextBox.Text = folder;
      }

      private void SelectModPathButton_Click(object sender, EventArgs e)
      {
         var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
         var path = Path.Combine(documentsFolder, "Paradox Interactive", "Europa Universalis IV", "mod");
         if (Path.Exists(path))
            documentsFolder = path;
         IO.OpenFolderDialog(documentsFolder, "mod folder", out var folder);

         ModPathTextBox.Text = folder;
      }

      private void ContinueButton_Click(object sender, EventArgs e)
      {
#if DEBUG
         ModPathTextBox.Text = Consts.MOD_PATH;
         VanillaPathTextBox.Text = Consts.VANILLA_PATH;
#endif
         
         if (string.IsNullOrEmpty(ModPathTextBox.Text) || string.IsNullOrEmpty(VanillaPathTextBox.Text))
         {
            MessageBox.Show("Please select both paths", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }

         if (!Path.Exists(ModPathTextBox.Text) || !Path.Exists(VanillaPathTextBox.Text))
         {
            MessageBox.Show("Please select valid paths", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }

         Globals.VanillaPath = VanillaPathTextBox.Text;
         Globals.ModPath = ModPathTextBox.Text;

         Close();
      }

      private void CancelButton_Click(object sender, EventArgs e)
      {
         Close();
      }
   }
}
