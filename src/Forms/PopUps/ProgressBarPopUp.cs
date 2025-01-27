using Editor.Controls;

namespace Editor.Forms.PopUps
{
   public partial class ProgressBarPopUp : Form
   {
      private CustomProgressBar progressBar;

      public ProgressBarPopUp()
      {
         StartPosition = FormStartPosition.CenterScreen;
         InitializeComponent();
         progressBar = new CustomProgressBar
         {
            DisplayStyle = ProgressBarDisplayText.Percentage,
            TaskCount = 100,
            Dock = DockStyle.Fill
         };
         LayoutPanel.Controls.Add(progressBar, 0, 1);
      }

      public static ProgressBarPopUp GetProgressBar(int steps, int start = 0)
      {
         var popUp = new ProgressBarPopUp();
         popUp.progressBar.Maximum = steps;
         popUp.progressBar.Value = start;
         return popUp;
      }

      public void UpdateProgress(int value = 1)
      {
         if (InvokeRequired) 
            progressBar.Invoke(new Action(() => progressBar.Value += value));
         else
            progressBar.Value += value;
      }
   }
}
