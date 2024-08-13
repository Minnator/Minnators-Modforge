using Editor.Controls;

namespace Editor.Forms.Loadingscreen
{
   public partial class LoadingScreen : Form
   {
      private const bool SKIP_LOADING = false;
      private readonly CustomProgressBar ProgressBar;
      private MapWindow _mw;

      public LoadingScreen(MapWindow mw)
      {
         InitializeComponent();
         ContinueButton.Enabled = false;

         Globals.LoadingStageChanged += LoadingScreen_LoadingStageChanged;

         ProgressBar = new();
         ProgressBar.Dock = DockStyle.Fill;
         tableLayoutPanel1.Controls.Add(ProgressBar, 0, 2);

         _mw = mw;
      }

      private void LoadingScreen_LoadingStageChanged(object? sender, int e)
      {
         ProgressBar.Value = (int)((float)e / Globals.LoadingStages * 100);
      }
      
      private void LoadButton_Click(object sender, EventArgs e)
      {
         LoadButton.Enabled = false;
         _mw.Initialize();
         ContinueButton.Enabled = true;
         if (SKIP_LOADING)
            ContinueButton.PerformClick();
      }

      private void ContinueButton_Click(object sender, EventArgs e)
      {
         _mw.Visible = true;
         Close();
      }
   }
}
