namespace Editor.Forms
{
   public partial class EnterPathForm : Form
   {
      public EnterPathForm()
      {
         InitializeComponent();
         StartPosition = FormStartPosition.CenterScreen;
      }


      private static void OpenFileDialog(string startPath, string filterText, out string folder)
      {
         folder = string.Empty;
         // create a file dialog that opens with the start path and only allows folders to be selected
         if (!Path.Exists(startPath))
            return;

         using var dialog = new OpenFileDialog();
         dialog.InitialDirectory = startPath;
         dialog.CheckFileExists = false;
         dialog.CheckPathExists = true;
         dialog.FileName = filterText;

         if (dialog.ShowDialog() == DialogResult.OK)
            folder = Path.GetDirectoryName(dialog.FileName) ?? Environment.SpecialFolder.MyDocuments.ToString();
      }

      private void SelectVanillaPathButton_Click(object sender, EventArgs e)
      {
         var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
         OpenFileDialog(documentsFolder, "vanilla base game", out var folder);

         VanillaPathTextBox.Text = folder;
      }

      private void SelectModPathButton_Click(object sender, EventArgs e)
      {
         var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
         var path = Path.Combine(documentsFolder, "Paradox Interactive", "Europa Universalis IV", "mod");
         if (Path.Exists(path))
            documentsFolder = path;
         OpenFileDialog(documentsFolder, "mod folder", out var folder);

         ModPathTextBox.Text = folder;
      }

      private void ContinueButton_Click(object sender, EventArgs e)
      {

         // TODO uncomment on release
         ModPathTextBox.Text = Consts.MOD_PATH;
         VanillaPathTextBox.Text = Consts.VANILLA_PATH;
         
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

         Globals.MapWindow.Project.ModPath = ModPathTextBox.Text;
         Globals.MapWindow.Project.VanillaPath = VanillaPathTextBox.Text;

         Close();
      }

      private void CancelButton_Click(object sender, EventArgs e)
      {
         Globals.MapWindow.SHUT_DOWN = true;
         Close();
      }
   }
}
