using Editor.Helper;

namespace Editor.Forms.SavingClasses
{
   public partial class GetSavingFileForm : Form
   {
      public string InitPath { get; set; }
      public string NewPath { get; set; } = string.Empty;
      private string Ending { get; set; } = string.Empty;
      public GetSavingFileForm(string initPath, string desc, string ending)
      {
         InitializeComponent();
         InitPath = initPath;
         DescriptionLabel.Text = desc;
         Ending = ending;
      }

      private void OpenFileDialogButton_Click(object sender, EventArgs e)
      {
         IO.OpenFileSelection(InitPath, "Select a file ", out var path);
         ExistingFilePath.Text = path;
      }

      private void button1_Click(object sender, EventArgs e)
      {
         if (NewFile.Checked)
         {
            NewPath = PathTextBox.Text;
            if (File.Exists(Path.Combine(InitPath, $"{NewPath}.txt")) || string.IsNullOrWhiteSpace(NewPath))
            {
               MessageBox.Show("File already exists, please choose a different name", "Filename already used", MessageBoxButtons.OK, MessageBoxIcon.Information);
               return;
            }
         }
         else if (ExistingFile.Checked)
         {
            if (!File.Exists(ExistingFilePath.Text))
            {
               MessageBox.Show("The selected File does not exist!", "File does not exist", MessageBoxButtons.OK, MessageBoxIcon.Error);
               return;
            }
            if (ExistingFile.Text.EndsWith(Ending))
               NewPath = ExistingFilePath.Text;
            else
            {
               MessageBox.Show($"The selected file does not have the correct ending ({Ending})!", "Wrong file ending", MessageBoxButtons.OK, MessageBoxIcon.Error);
               return;
            }
         }
         DialogResult = DialogResult.OK;
         Close();
      }

      private void ExistingFile_CheckedChanged(object sender, EventArgs e)
      {

      }
   }
}
