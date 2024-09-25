using System.ComponentModel;
using System.Diagnostics;
using Editor.Controls;
using Editor.Loading;
using Editor.Parser;

namespace Editor.Forms.Loadingscreen
{
   public partial class LoadingScreen : Form
   {
      private const bool SKIP_LOADING = true;
      private readonly CustomProgressBar ProgressBar;
      private MemoryStream _ms = null!;

      public LoadingScreen()
      {
         InitializeComponent();

         Globals.LoadingStageChanged += LoadingScreen_LoadingStageChanged;
          
         ProgressBar = new();
         ProgressBar.Dock = DockStyle.Fill;
         tableLayoutPanel1.Controls.Add(ProgressBar, 0, 2);
         
         StartLoadingAnimation();

         StartPosition = FormStartPosition.CenterScreen;

         LoadButton_Click(null!, null!);
      }

      ~LoadingScreen()
      {
         _ms.Dispose();
      }
      
      private void LoadingScreen_LoadingStageChanged(object? sender, int e)
      {
         ProgressBar.Value = Math.Min(100, (int)((float)e / Globals.LOADING_STAGES * 100));
         ProgressBar.Invalidate();
      }

      private void StartLoadingAnimation()
      {
         /*
         if (!GifToBytes.ConvertFormattedHexToBytes(StaticByteResources.LOADING_GIF, out var bytes))
            throw new ArgumentException("Can not convert loading animation");
         _ms = new (bytes);
         LoadingAnimation.Image = Image.FromStream(_ms);
         */
         LoadingAnimation.Image = Properties.Resources.LoadingGif;
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
         
         try
         {
            ScriptedEffectLoading.LoadScriptedEffects();
            bw.ReportProgress(++progress);
            ModifierLoading.Load();
            bw.ReportProgress(++progress);
            LocalisationLoading.Load();
            bw.ReportProgress(++progress);
            TradeGoodsLoading.Load();
            bw.ReportProgress(++progress);
            TradeNodeLoading.Load();
            bw.ReportProgress(++progress);
            TechnologyGroupsLoading.Load();
            bw.ReportProgress(++progress);
            BuildingsLoading.Load();
            bw.ReportProgress(++progress);
            TradeCompanyInvestmentsLoading.Load();
            bw.ReportProgress(++progress);
            ReligionLoading.Load();
            bw.ReportProgress(++progress);
            MapLoading.LoadDefinitionAndMap();
            bw.ReportProgress(++progress);
            DefaultMapLoading.CreateProvinceGroups();
            bw.ReportProgress(++progress);
            AreaLoading.LoadNew();
            bw.ReportProgress(++progress);
            RegionLoading.Load();
            bw.ReportProgress(++progress);
            SuperRegionLoading.Load();
            bw.ReportProgress(++progress);
            ContinentLoading.Load();
            bw.ReportProgress(++progress);
            ScopeParser.GenerateRuntimeScopes();
            bw.ReportProgress(++progress);
            ProvinceParser.ParseAllUniqueProvinces();
            bw.ReportProgress(++progress);
            TradeCompanyLoading.Load();
            bw.ReportProgress(++progress);
            ColonialRegionsLoading.Load();
            bw.ReportProgress(++progress);
            CultureLoading.LoadCultures();
            bw.ReportProgress(++progress);
            CountryLoading.LoadCountries();
            bw.ReportProgress(++progress);
            ScopeParser.GenerateCountryScope();
            bw.ReportProgress(++progress);
         }
         catch (Exception exception)
         {
            // Create a message box to inform the user that an error occurred
            MessageBox.Show(exception.Message + "\n\nTry restarting the application. There ia a run condition error in the code I could not find yet. Restarting resolves it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Debug.WriteLine(exception);
            throw;
         }
         

         GC.Collect();
         Globals.LoadingStage = progress;
      }

      private void LoadingCompleted()
      {
         Globals.MapWindow.Initialize();
         Close();
      }
   }
}
