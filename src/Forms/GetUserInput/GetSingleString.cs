namespace Editor.Forms.GetUserInput
{
   public partial class GetSingleString : Form
   {
      public string UserInput { get; private set; } = string.Empty;

      public GetSingleString(string title, string description)
      {
         StartPosition = FormStartPosition.CenterScreen;
         Text = title;
         label1.Text = description;
         InitializeComponent();
      }

      private void button1_Click(object sender, EventArgs e)
      {
         UserInput = textBox1.Text;  // Capture the input
         this.DialogResult = DialogResult.OK;
         this.Close();
      }
   }
}
