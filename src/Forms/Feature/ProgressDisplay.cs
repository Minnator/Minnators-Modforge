namespace Editor.Forms.Feature
{
   public partial class ProgressDisplay : Form
   {
      private int Max { get; set; }
      public ProgressDisplay(int max, int start, string desc)
      {
         InitializeComponent();

         ProgressBar.Maximum = 100;
         ProgressBar.Value = start;

         Description.Text = desc;
      }

      // Method to update the ProgressBar value from another thread
      public void UpdateProgress(int current)
      {
         var value = Math.Min(Max, (int)((float)current / Globals.LOADING_STAGES * 100));
         if (ProgressBar.InvokeRequired)
         {
            // If we're on another thread, invoke the method on the UI thread
            ProgressBar.Invoke(new (() => ProgressBar.Value = value));
         }
         else
         {
            // Otherwise, update the ProgressBar directly
            ProgressBar.Value = value;
         }
      }
   }
}
