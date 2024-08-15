using System.ComponentModel;
using Editor.Controls;
using Editor.Helper;
using Editor.Loading;
using Editor.Resources;

namespace Editor.Forms.Loadingscreen
{
   public partial class LoadingScreen : Form
   {
      private const bool SKIP_LOADING = true;
      private readonly CustomProgressBar ProgressBar;
      private MemoryStream ms;

      public LoadingScreen()
      {
         InitializeComponent();
         ContinueButton.Enabled = false;

         Globals.LoadingStageChanged += LoadingScreen_LoadingStageChanged;

         ProgressBar = new();
         ProgressBar.Dock = DockStyle.Fill;
         tableLayoutPanel1.Controls.Add(ProgressBar, 0, 2);
         
         StartLoadingAnimation();

         StartPosition = FormStartPosition.CenterScreen;

         //LoadButton_Click(null, null);
      }

      ~LoadingScreen()
      {
         ms.Dispose();
      }
      
      private void LoadingScreen_LoadingStageChanged(object? sender, int e)
      {
         ProgressBar.Value = Math.Min(100, (int)((float)e / Globals.LOADING_STAGES * 100));
         ProgressBar.Invalidate();
      }

      private void StartLoadingAnimation()
      {
         if (!GifToBytes.ConvertFormattedHexToBytes(StaticByteResources.LOADING_GIF, out var bytes))
            throw new ArgumentException("Can not convert loading animation");
         ms = new (bytes);
         LoadingAnimation.Image = Image.FromStream(ms);
         LoadingAnimation.SizeMode = PictureBoxSizeMode.Zoom;
      }
      
      private void LoadButton_Click(object sender, EventArgs e)
      {
         LoadButton.Enabled = false;
         //create a background worker to load the data
         var bw = new BackgroundWorker();
         bw.WorkerReportsProgress = true;
         bw.DoWork += OnBwOnDoWork;
         bw.RunWorkerCompleted += (s, e) => LoadingCompleted();
         bw.ProgressChanged += (s, e) =>
         {
            ProgressBar.Value = Math.Min(100, (int)((float)e.ProgressPercentage / Globals.LOADING_STAGES * 100));
            ProgressBar.Invalidate();
         };
         bw.RunWorkerAsync();
      }

      private void OnBwOnDoWork(object? s, DoWorkEventArgs e)
      {
         if (s is not BackgroundWorker bw)
            return;
         var project = Globals.MapWindow.Project;
         var progress = 0;
         
         TradeGoodsLoading.Load(project);
         bw.ReportProgress(++progress);
         TradeNodeLoading.Load(project);
         bw.ReportProgress(++progress);
         TechnologyGroupsLoading.Load(project);
         bw.ReportProgress(++progress);
         BuildingsLoading.Load(project);
         bw.ReportProgress(++progress);
         ReligionLoading.Load(project);
         bw.ReportProgress(++progress);
         LoadingManager.LoadDefinitionAndMap(project); //TODO SLOW
         bw.ReportProgress(++progress);
         DefaultMapLoading.Load(project.VanillaPath);
         bw.ReportProgress(++progress);
         AreaLoading.Load(project.VanillaPath, project.ColorProvider);
         bw.ReportProgress(++progress);
         RegionLoading.Load(project.VanillaPath, project.ColorProvider);
         bw.ReportProgress(++progress);
         SuperRegionLoading.Load(project.VanillaPath, project.ColorProvider);
         bw.ReportProgress(++progress);
         ContinentLoading.Load(project.VanillaPath, project.ColorProvider);
         bw.ReportProgress(++progress);
         LocalisationLoading.Load(project.ModPath, project.VanillaPath, project.Language);
         bw.ReportProgress(++progress);
         ProvinceParser.ParseAllUniqueProvinces(project.ModPath, project.VanillaPath); //TODO SLOW
         bw.ReportProgress(++progress);
         CultureLoading.LoadCultures(project);
         bw.ReportProgress(++progress);
         CountryLoading.LoadCountries(project); //TODO SLOW
         bw.ReportProgress(++progress);

         GC.Collect();
         Globals.LoadingStage = progress;
      }

      private void LoadingCompleted()
      {
         ContinueButton.Enabled = true;
         if (SKIP_LOADING)
            ContinueButton.PerformClick();
      }

      private void ContinueButton_Click(object sender, EventArgs e)
      {
         Globals.MapWindow.Initialize();
         Close();
      }
   }
}
