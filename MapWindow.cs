using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text;
using Editor.Controls;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Forms;
using Editor.Helper;
using Editor.Loading;

namespace Editor
{
   public partial class MapWindow : Form
   {
      #region CustomEditingControls

      private ItemList Claims;
      private ItemList PermanentClaims;
      private ItemList Cores;
      private ItemList Buildings;
      private ItemList DiscoveredBy;

      #endregion

      public PannablePictureBox MapPictureBox = null!;
      public readonly DateControl DateControl = new(DateTime.MinValue, DateControlLayout.Horizontal);

      public readonly ModProject Project = new()
      {
         Name = "DAVID",
         ModPath = Consts.MOD_PATH,
         VanillaPath = Consts.VANILLA_PATH
      };

      public MapWindow()
      {
         Globals.MapWindow = this;
         //pause gui updates
         SuspendLayout();
         InitGui();

         LoadingManager.LoadGameAndModDataToApplication(Project, this);
         LoadingManager.InitializeComponents(this);

         //Needs to be after loading the game data to populate the gui with it
         InitializeEditGui();
         //resume gui updates
         ResumeLayout();
         // Enable the Application
         Globals.LoadingLog.Close();
         //ResourceUsageHelper.Initialize(this);
         Globals.State = State.Running;
         DateControl.Date = new(1444, 11, 11);
         MapModeComboBox.SelectedIndex = 11;
      }


      // Loads the map into the created PannablePictureBox
      private void InitGui()
      {
         InitializeComponent();
         MapPictureBox = ControlFactory.GetPannablePictureBox(ref MapPanel, this);
         MapPanel.Controls.Add(MapPictureBox);

         TopStripLayoutPanel.Controls.Add(DateControl, 4, 0);
         DateControl.OnDateChanged += OnDateChanged;

         ProvincePreviewMode.Items.AddRange([.. Enum.GetNames(typeof(ProvinceEditingStatus))]);
         ProvincePreviewMode.SelectedIndex = 2;

      }

      private void InitializeEditGui()
      {
         OwnerTagBox = ControlFactory.GetTagComboBox();
         ControllerTagBox = ControlFactory.GetTagComboBox();
         OwnerControllerLayoutPanel.Controls.Add(OwnerTagBox, 1, 0);
         OwnerControllerLayoutPanel.Controls.Add(ControllerTagBox, 1, 1);

         Cores = ControlFactory.GetItemList(ItemTypes.Tag, [.. Globals.Countries.Keys], "Cores");
         Claims = ControlFactory.GetItemList(ItemTypes.Tag, [.. Globals.Countries.Keys], "Regular");
         PermanentClaims = ControlFactory.GetItemList(ItemTypes.Tag, [.. Globals.Countries.Keys], "Permanent");
         Buildings = ControlFactory.GetItemListObjects(ItemTypes.String, [.. Globals.Buildings], "Building");
         DiscoveredBy = ControlFactory.GetItemList(ItemTypes.String, [.. Globals.TechnologyGroups], "TechGroup");

         CoresAndClaimLayoutPanel.Controls.Add(PermanentClaims, 0, 0);
         CoresAndClaimLayoutPanel.Controls.Add(Claims, 1, 0);
         CoresGroupBox.Controls.Add(Cores);
         Cores.Location = new(0, 18);
         BuildingsGroupBox.Controls.Add(Buildings);
         Buildings.Location = new(0, 18);
         DiscoveredByGroupBox.Controls.Add(DiscoveredBy);
         DiscoveredBy.Location = new(0, 18);

         List<string> culturesString = [.. Globals.Cultures.Keys];
         culturesString.Sort();
         CultureComboBox.Items.AddRange([.. culturesString]);

         List<string> religionsString = [.. Globals.Religions.Keys];
         religionsString.Sort();
         ReligionComboBox.Items.AddRange([.. religionsString]);
      }

      // ======================== Province GUI Update Methods ========================
      #region Province Gui
      /// <summary>
      /// This will only load the province attributes to the gui which are shared by all provinces
      /// </summary>
      public void LoadSelectedProvincesToGui()
      {
         SuspendLayout();
         ClearProvinceGui();
         if (MapPictureBox.Selection.GetSharedAttribute("claims", out var result) && result is List<string> tags)
            Claims.AddItemsUnique(tags);
         if (MapPictureBox.Selection.GetSharedAttribute("permanent_claims", out result) && result is List<string> permanentTags)
            PermanentClaims.AddItemsUnique(permanentTags);
         if (MapPictureBox.Selection.GetSharedAttribute("cores", out result) && result is List<string> coreTags)
            Cores.AddItemsUnique(coreTags);
         if (MapPictureBox.Selection.GetSharedAttribute("buildings", out result) && result is List<string> buildings)
            Buildings.AddItemsUnique(buildings);
         if (MapPictureBox.Selection.GetSharedAttribute("discovered_by", out result) && result is List<string> techGroups)
            DiscoveredBy.AddItemsUnique(techGroups);
         if (MapPictureBox.Selection.GetSharedAttribute("owner", out result) && result is string owner)
            OwnerTagBox.Text = owner;
         if (MapPictureBox.Selection.GetSharedAttribute("controller", out result) && result is string controller)
            ControllerTagBox.Text = controller;
         if (MapPictureBox.Selection.GetSharedAttribute("religion", out result) && result is string religion)
            ReligionComboBox.Text = religion;
         if (MapPictureBox.Selection.GetSharedAttribute("culture", out result) && result is string culture)
            CultureComboBox.Text = culture;
         if (MapPictureBox.Selection.GetSharedAttribute("capital", out result) && result is string capital)
            CapitalNameTextBox.Text = capital;
         if (MapPictureBox.Selection.GetSharedAttribute("is_city", out result) && result is bool isCity)
            IsCityCheckBox.Checked = isCity;
         if (MapPictureBox.Selection.GetSharedAttribute("is_hre", out result) && result is bool isHre)
            IsHreCheckBox.Checked = isHre;
         if (MapPictureBox.Selection.GetSharedAttribute("is_seat_in_parliament", out result) && result is bool isSeatInParliament)
            IsParlimentSeatCheckbox.Checked = isSeatInParliament;
         if (MapPictureBox.Selection.GetSharedAttribute("has_revolt", out result) && result is bool hasRevolt)
            HasRevoltCheckBox.Checked = hasRevolt;
         if (MapPictureBox.Selection.GetSharedAttribute("base_tax", out result) && result is int baseTax)
            TaxNumericBox.Value = baseTax;
         if (MapPictureBox.Selection.GetSharedAttribute("base_production", out result) && result is int baseProduction)
            ProdNumericBox.Value = baseProduction;
         if (MapPictureBox.Selection.GetSharedAttribute("base_manpower", out result) && result is int baseManpower)
            ManpNumericBox.Value = baseManpower;
         if (MapPictureBox.Selection.GetSharedAttribute("local_autonomy", out result) && result is float localAutonomy)
            AutonomyNumeric.Value = (int)localAutonomy;
         if (MapPictureBox.Selection.GetSharedAttribute("devastation", out result) && result is float devastation)
            DevastationNumeric.Value = (int)devastation;
         if (MapPictureBox.Selection.GetSharedAttribute("prosperity", out result) && result is float prosperity)
            ProsperityNumeric.Value = (int)prosperity;
         if (MapPictureBox.Selection.GetSharedAttribute("trade_good", out result) && result is string tradeGood)
            TradeGoodsComboBox.Text = tradeGood;
         if (MapPictureBox.Selection.GetSharedAttribute("center_of_trade", out result) && result is int centerOfTrade)
            TradeCenterComboBox.Text = centerOfTrade.ToString();
         if (MapPictureBox.Selection.GetSharedAttribute("extra_cost", out result) && result is int extraCost)
            ExtraCostNumeric.Value = extraCost;
         ResumeLayout();
      }

      public void LoadProvinceToGui(Province province)
      {
         SuspendLayout();
         ClearProvinceGui();
         OwnerTagBox.Text = province.Owner;
         ControllerTagBox.Text = province.Controller;
         ReligionComboBox.Text = province.Religion;
         CultureComboBox.Text = province.Culture;
         CapitalNameTextBox.Text = province.Capital;
         IsCityCheckBox.Checked = province.IsCity;
         IsHreCheckBox.Checked = province.IsHre;
         IsParlimentSeatCheckbox.Checked = province.IsSeatInParliament;
         HasRevoltCheckBox.Checked = province.HasRevolt;
         TaxNumericBox.Value = province.BaseTax;
         ProdNumericBox.Value = province.BaseProduction;
         ManpNumericBox.Value = province.BaseManpower;
         Claims.AddItemsUnique([.. province.Claims]);
         PermanentClaims.AddItemsUnique([]); //TODO what is wrong here why no Province.PermanentClaims
         Cores.AddItemsUnique([.. province.Cores]);
         Buildings.AddItemsUnique(province.Buildings);
         DiscoveredBy.AddItemsUnique(province.DiscoveredBy);
         AutonomyNumeric.Value = (int)province.LocalAutonomy;
         DevastationNumeric.Value = (int)province.Devastation;
         ProsperityNumeric.Value = (int)province.Prosperity;
         TradeGoodsComboBox.Text = province.TradeGood;
         TradeCenterComboBox.Text = province.CenterOfTrade.ToString();
         ExtraCostNumeric.Value = province.ExtraCost;
         ResumeLayout();
      }

      public void ClearProvinceGui()
      {
         OwnerTagBox.Clear();
         ControllerTagBox.Clear();
         ReligionComboBox.Clear();
         CultureComboBox.Clear();
         CapitalNameTextBox.Clear();
         IsCityCheckBox.Checked = false;
         IsHreCheckBox.Checked = false;
         IsParlimentSeatCheckbox.Checked = false;
         HasRevoltCheckBox.Checked = false;
         TaxNumericBox.Value = 1;
         ProdNumericBox.Value = 1;
         ManpNumericBox.Value = 1;
         Claims.Clear();
         PermanentClaims.Clear();
         Cores.Clear();
         Buildings.Clear();
         DiscoveredBy.Clear();
         AutonomyNumeric.Value = 0;
         DevastationNumeric.Value = 0;
         ProsperityNumeric.Value = 0;
         TradeGoodsComboBox.Clear();
         TradeCenterComboBox.Clear();
         ExtraCostNumeric.Value = 0;
      }
      #endregion

      
      #region ToolStrip update methods
      public void SetSelectedProvinceSum(int sum)
      {
         SelectedProvinceSum.Text = $"ProvSum: [{sum}]";
      }

      #region Resource Updater MethodInvokation

      public void UpdateMemoryUsage(float memoryUsage)
      {
         if (InvokeRequired) Invoke(new MethodInvoker(delegate { RamUsageStrip.Text = $"RAM: [{Math.Round(memoryUsage)} MB]"; }));
      }

      public void UpdateCpuUsage(float cpuUsage)
      {
         if (InvokeRequired) Invoke(new MethodInvoker(delegate { CpuUsageStrip.Text = $"CPU: [{Math.Round(cpuUsage, 2)}%]"; }));
      }

      #endregion

      #region HistoryManager Event Handlers

      public void UpdateRedoDepth(object sender, int e) => RedoDepthLabel.Text = $"Redos [{e}]";
      public void UpdateUndoDepth(object sender, int e) => UndoDepthLabel.Text = $"Undos [{e}]";
      #endregion
      #endregion

      public void SetEditingMode()
      {
         EditingModeLabel.Text = MapPictureBox.Selection.Count <= 1 
            ? "Editing Mode: Single Province" 
            : $"Editing Mode: Multi Province ({MapPictureBox.Selection.Count})";
      }

      public void SetIsEditedLabel()
      {
         if (MapPictureBox.Selection.SelectedProvinces.Count == 1)
         {
            if (!Globals.Provinces.TryGetValue(MapPictureBox.Selection.SelectedProvinces[0], out var prov))
            {
               IsAlreadyEditedLabel.Text = "Edited: -";
               return;
            }
            IsAlreadyEditedLabel.Text = $"Edited: {prov.Status}";
         }
      }

      private void MapWindow_FormClosing(object sender, FormClosingEventArgs e)
      {
         ResourceUsageHelper.Dispose();
      }

      #region History interface interactions

      private void RevertInSelectionHistory(object sender, EventArgs e)
      {
         var historyTreeView = new HistoryTree(Globals.HistoryManager.RevertTo);
         historyTreeView.VisualizeFull(Globals.HistoryManager.GetRoot());
         historyTreeView.ShowDialog();
      }

      private void DeleteHistoryToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Globals.HistoryManager.Clear();
      }

      #endregion

      private void OnDateChanged(object? sender, EventArgs e)
      {
         ProvinceHistoryManager.LoadDate(Globals.Date);
      }

      public void UpdateHoveredInfo(Province? province)
      {
         if (province == null)
         {
            ProvinceNameLabel.Text = "Province: -";
            OwnerCountryNameLabel.Text = "Owner: -";
            return;
         }
         ProvinceNameLabel.Text = $"Province: {province.GetLocalisation()}";
         OwnerCountryNameLabel.Text = $"Owner: {Localisation.GetLoc(province.Owner)}";
      }

      private void debugToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Globals.Provinces[1].BaseManpower = 50;
         Globals.Provinces[2].BaseManpower = 50;
         Globals.Provinces[3].BaseManpower = 50;
         Globals.Provinces[4].BaseManpower = 50;
      }

      private void MapModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (MapPictureBox.IsPainting)
            return;
         Globals.MapModeManager.SetCurrentMapMode(MapModeComboBox.SelectedItem.ToString());
         GC.Collect(); // We force the garbage collector to collect the old bitmap
      }

      private void gCToolStripMenuItem_Click(object sender, EventArgs e)
      {
         GC.Collect();
      }

      private void SaveCurrentMapModeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\";
         MapPictureBox.Image.Save($@"{downloadFolder}{MapModeComboBox.SelectedItem}.png", ImageFormat.Png);
      }

      private void openCustomizerToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var toolTipCustomizer = new ToolTipCustomizer();
         toolTipCustomizer.Show();
      }

      private void ShowToolTipMenuItem_Click(object sender, EventArgs e)
      {
         MapPictureBox.ShowToolTip = ShowToolTipMenuItem.Checked;
      }

      private void testToolStripMenuItem_Click(object sender, EventArgs e)
      {
         //var content = File.ReadAllText("C:\\Users\\david\\Downloads\\NestedBLocks.txt");
         var content = File.ReadAllText("S:\\SteamLibrary\\steamapps\\common\\Europa Universalis IV\\common\\cultures\\00_cultures.txt");
         var sw = Stopwatch.StartNew();
         var blocks = Parsing.GetElements(0, ref content);
         sw.Stop();
         Debug.WriteLine("Parsing cultures took: " + sw.ElapsedMilliseconds + "ms");

         var sb = new StringBuilder();
         foreach (var block in blocks)
         {
            DebugPrints.BuildBlockString(0, block, ref sb);
         }
         File.WriteAllText("C:\\Users\\david\\Downloads\\NestedBLocksOutput2.txt", sb.ToString());
      }

      private void telescopeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         DebugMaps.TelescopeImageBenchmark();
      }

      private void MapWindow_KeyDown(object sender, KeyEventArgs e)
      {
         if (ModifierKeys == Keys.Control)
         {
            switch (e.KeyCode)
            {
               case Keys.F:
                  Globals.SearchForm = FormHelper.OpenOrBringToFront(Globals.SearchForm);
                  break;
            }
         }

         switch (e.KeyCode)
         {
            case Keys.F1:
               Globals.ConsoleForm = FormHelper.OpenOrBringToFront(Globals.ConsoleForm);
               break;
         }
      }

      private void refStackToolStripMenuItem_Click(object sender, EventArgs e)
      {
         DebugMaps.CentroidPoints();
      }

      private void DateSelector_SelectedIndexChanged(object sender, EventArgs e)
      {

      }

      private void MapWindow_Load(object sender, EventArgs e)
      {
         MapPictureBox.FocusOn(new(3100, 600));
      }

      private void searchToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FormHelper.OpenOrBringToFront(Globals.SearchForm);
      }

      private void bestPointsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         DebugMaps.TestCenterPoints();
      }

      private void provDiffToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var testProv = new Province();

         testProv.BaseManpower = 100;
         testProv.BaseTax = 100;
         testProv.BaseProduction = 100;
         testProv.Claims.Add("TES");

         testProv.PrintModifiedProvinceValues(out var mod);
         Debug.WriteLine(mod);
      }

      private void ProvincePreviewMode_SelectedIndexChanged(object sender, EventArgs e)
      {
         Globals.ProvinceEditingStatus = (ProvinceEditingStatus)ProvincePreviewMode.SelectedIndex;
         // Close the menu strip// Close the menu when an item is selected
         filesToolStripMenuItem.DropDown.Close();
      }
   }
}
