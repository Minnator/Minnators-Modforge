using Editor.Helper;

namespace Editor.Forms.SavingClasses
{
   public partial class GetSavingFileForm : Form
   {
      public string InitPath { get; set; }
      public string NewPath { get; set; } = string.Empty;
      private string Ending { get; set; }
      public bool RequireModDirectory { get; set; } = true;
      public bool UseGrouping { get; set; } = false;
      public GetSavingFileForm(string initPath, string desc, string ending)
      {
         InitializeComponent();
         InitPath = initPath;
         DescriptionLabel.Text = desc;
         Ending = ending;

         PathTextBox.PlaceholderText = $"modforge_{InitPath.Split(Path.DirectorySeparatorChar)[^1]}{ending}";
      }

      private void OpenFileDialogButton_Click(object sender, EventArgs e)
      {
         IO.OpenFileSelection(InitPath, "Select a file ", out var path);
         ExistingFilePath.Text = path;
      }

      public void SetPlaceHolderText(string text)
      {
         PathTextBox.PlaceholderText = text;
      }

      private void button1_Click(object sender, EventArgs e)
      {
         if (NewFile.Checked)
         {
            if (string.IsNullOrWhiteSpace(NewPath))
               NewPath = PathTextBox.PlaceholderText;
            if (!NewPath.EndsWith(Ending))
               NewPath += Ending;
            if (File.Exists(Path.Combine(InitPath, $"{NewPath}")) || string.IsNullOrWhiteSpace(NewPath))
            {
               MessageBox.Show("File already exists, please choose a different name", "Filename already used", MessageBoxButtons.OK, MessageBoxIcon.Information);
               return;
            }
            NewPath = Path.Combine(InitPath, NewPath);
            if (NewPath.Contains(Globals.MapPath))
               NewPath = NewPath[(Globals.ModPath.Length + 1)..];
         }
         else if (ExistingFile.Checked)
         {
            if (!File.Exists(ExistingFilePath.Text))
            {
               MessageBox.Show("The selected File does not exist!", "File does not exist", MessageBoxButtons.OK, MessageBoxIcon.Error);
               return;
            }

            if (!RequireModDirectory)
            {
               NewPath = ExistingFilePath.Text;
               goto returnResult;
            }

            // Check if ExitingFilePath starts with the Globals.ModPath
            if (!ExistingFilePath.Text.StartsWith(Globals.ModPath))
            {
               MessageBox.Show("The selected file is not in the MOD directory!", "Illegal directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
               return;
            }

            if (ExistingFilePath.Text.EndsWith(Ending))
            {
               NewPath = ExistingFilePath.Text[(Globals.ModPath.Length + 1)..];
            }
            else
            {
               MessageBox.Show($"The selected file does not have the correct ending ({Ending})!", "Wrong file ending", MessageBoxButtons.OK, MessageBoxIcon.Error);
               return;
            }
         }
      returnResult:
         DialogResult = DialogResult.OK;
         Close();
      }

      private void ExistingFile_CheckedChanged(object sender, EventArgs e)
      {
         var checkSender = (CheckBox)sender;
         if (checkSender.Checked)
         {
            if (checkSender == ExistingFile)
               NewFile.Checked = false;
            else
               ExistingFile.Checked = false;
         }
      }

      private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
      {

      }

      private void GroupingCheckbox_CheckedChanged(object sender, EventArgs e)
      {
         UseGrouping = GroupingCheckbox.Checked;
      }
   }
}
