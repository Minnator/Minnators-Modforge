using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text;
using Editor.Controls;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Forms;
using Editor.Forms.Loadingscreen;
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

      private ExtendedComboBox ReligionComboBox;
      private ExtendedComboBox CultureComboBox;

      private ExtendedNumeric TaxNumeric;
      private ExtendedNumeric ProdNumeric;
      private ExtendedNumeric ManpNumeric;

      #endregion

      public PannablePictureBox MapPictureBox = null!;
      public readonly DateControl DateControl = new(DateTime.MinValue, DateControlLayout.Horizontal);
      private LoadingScreen ls;

      public readonly ModProject Project = new()
      {
         Name = "DAVID",
         ModPath = Consts.MOD_PATH,
         VanillaPath = Consts.VANILLA_PATH
      };

      public MapWindow()
      {
         Globals.MapWindow = this;
         RunLoadingScreen();
      }

      public void Initialize()
      {
         Hide();
         //pause gui updates
         SuspendLayout();
         InitGui();
         

         // MUST BE LAST in the loading sequence
         LoadingManager.InitMapModes(this);
         Globals.LoadingStage++;
         LoadingManager.InitializeComponents(this);
         Globals.LoadingStage++;


         //Needs to be after loading the game data to populate the gui with it
         InitializeEditGui();
         Globals.LoadingStage++;
         //resume gui updates
         // Enable the Application
         Globals.LoadingLog.Close();
         ResourceUsageHelper.Initialize(this);

         // ALL LOADING COMPLETE - Set the application to running

         Globals.State = State.Running;
         DateControl.Date = new(1444, 11, 11);
         Globals.LoadingStage++;
         MapModeComboBox.SelectedIndex = 11;
         ResumeLayout();
         Globals.LoadingStage++;
         Show();
         MapPictureBox.FocusOn(new(3100, 600));

         AfterLoad();
      }

      private void AfterLoad()
      {
      }

      private void RunLoadingScreen()
      {
         ls = new();
         ls.ShowDialog();
      }

      // Loads the map into the created PannablePictureBox
      private void InitGui()
      {
         InitializeComponent();
         MapPictureBox = ControlFactory.GetPannablePictureBox(ref MapPanel, this);
         MapPanel.Controls.Add(MapPictureBox);
         Globals.Selection = new(MapPictureBox);

         TopStripLayoutPanel.Controls.Add(DateControl, 4, 0);
         DateControl.OnDateChanged += OnDateChanged;

         ProvincePreviewMode.Items.AddRange([.. Enum.GetNames(typeof(ProvinceEditingStatus))]);
         ProvincePreviewMode.SelectedIndex = 2;

      }

      private void InitializeEditGui()
      {
         OwnerTagBox = ControlFactory.GetTagComboBox();
         OwnerTagBox.OnTagChanged += ProvinceEditingEvents.OnOwnerChanged;
         ControllerTagBox = ControlFactory.GetTagComboBox();
         ControllerTagBox.OnTagChanged += ProvinceEditingEvents.OnControllerChanged;
         OwnerControllerLayoutPanel.Controls.Add(OwnerTagBox, 1, 0);
         OwnerControllerLayoutPanel.Controls.Add(ControllerTagBox, 1, 1);

         Cores = ControlFactory.GetItemList(ItemTypes.Tag, [.. Globals.Countries.Keys], "Cores");
         Cores.OnItemAdded += ProvinceEditingEvents.OnCoreAdded;
         Cores.OnItemRemoved += ProvinceEditingEvents.OnCoreRemoved;
         Claims = ControlFactory.GetItemList(ItemTypes.Tag, [.. Globals.Countries.Keys], "Regular");
         Claims.OnItemAdded += ProvinceEditingEvents.OnClaimAdded;
         Claims.OnItemRemoved += ProvinceEditingEvents.OnClaimRemoved;
         PermanentClaims = ControlFactory.GetItemList(ItemTypes.Tag, [.. Globals.Countries.Keys], "Permanent");
         //TODO: Implement PermanentClaims.OnItemAdded += ProvinceEditingEvents.OnPermanentClaimAdded;

         Buildings = ControlFactory.GetItemListObjects(ItemTypes.String, [.. Globals.Buildings], "Building");
         Buildings.OnItemAdded += ProvinceEditingEvents.OnBuildingAdded;
         Buildings.OnItemRemoved += ProvinceEditingEvents.OnBuildingRemoved;
         DiscoveredBy = ControlFactory.GetItemList(ItemTypes.String, [.. Globals.TechnologyGroups], "TechGroup");
         DiscoveredBy.OnItemAdded += ProvinceEditingEvents.OnDiscoveredByAdded;
         DiscoveredBy.OnItemRemoved += ProvinceEditingEvents.OnDiscoveredByRemoved;

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
         CultureComboBox = ControlFactory.GetExtendedComboBox();
         RCCLayoutPanel.Controls.Add(CultureComboBox, 1, 1);
         CultureComboBox.Items.AddRange([.. culturesString]);
         CultureComboBox.OnDataChanged += ProvinceEditingEvents.OnCultureChanged;

         List<string> religionsString = [.. Globals.Religions.Keys];
         religionsString.Sort();
         ReligionComboBox = ControlFactory.GetExtendedComboBox();
         RCCLayoutPanel.Controls.Add(ReligionComboBox, 1, 0);
         ReligionComboBox.Items.AddRange([.. religionsString]);
         ReligionComboBox.OnDataChanged += ProvinceEditingEvents.OnReligionChanged;

         TaxNumeric = ControlFactory.GetExtendedNumeric();
         TaxNumeric.Minimum = 1;
         TaxNumeric.Maximum = 1000;
         TaxNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextBaseTaxChanged;
         DevelopmentLayoutPanel.Controls.Add(TaxNumeric, 1, 0);

         ProdNumeric = ControlFactory.GetExtendedNumeric();
         ProdNumeric.Minimum = 1;
         ProdNumeric.Maximum = 1000;
         ProdNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextBaseProductionChanged;
         DevelopmentLayoutPanel.Controls.Add(ProdNumeric, 1, 1);

         ManpNumeric = ControlFactory.GetExtendedNumeric();
         ManpNumeric.Minimum = 1;
         ManpNumeric.Maximum = 1000;
         ManpNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextBaseManpowerChanged;
         DevelopmentLayoutPanel.Controls.Add(ManpNumeric, 1, 2);

         CapitalNameTextBox.Leave += ProvinceEditingEvents.OnCapitalNameChanged;

         IsCityCheckBox.CheckedChanged += ProvinceEditingEvents.OnIsCityChanged;
         IsHreCheckBox.CheckedChanged += ProvinceEditingEvents.OnIsHreChanged;
         IsParlimentSeatCheckbox.CheckedChanged += ProvinceEditingEvents.OnIsSeatInParliamentChanged;
         HasRevoltCheckBox.CheckedChanged += ProvinceEditingEvents.OnHasRevoltChanged;
      }

      // ======================== Province GUI Update Methods ========================
      #region Province Gui
      /// <summary>
      /// This will only load the province attributes to the gui which are shared by all provinces
      /// </summary>
      public void LoadSelectedProvincesToGui()
      {
         Globals.EditingStatus = EditingStatus.LoadingInterface;
         SuspendLayout();
         ClearProvinceGui();
         if (Globals.Selection.GetSharedAttribute(ProvAttr.claims, out var result) && result is List<Tag> tags)
            Claims.AddItemsUnique([.. tags]);
         if (Globals.Selection.GetSharedAttribute(ProvAttr.permanent_claims, out result) && result is List<Tag> permanentTags)
            PermanentClaims.AddItemsUnique([.. permanentTags]);
         if (Globals.Selection.GetSharedAttribute(ProvAttr.cores, out result) && result is List<Tag> coreTags)
            Cores.AddItemsUnique([.. coreTags]);
         if (Globals.Selection.GetSharedAttribute(ProvAttr.buildings, out result) && result is List<string> buildings)
            Buildings.AddItemsUnique(buildings);
         if (Globals.Selection.GetSharedAttribute(ProvAttr.discovered_by, out result) && result is List<string> techGroups)
            DiscoveredBy.AddItemsUnique(techGroups);
         if (Globals.Selection.GetSharedAttribute(ProvAttr.owner, out result) && result is Tag owner)
            OwnerTagBox.Text = owner;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.controller, out result) && result is Tag controller)
            ControllerTagBox.Text = controller;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.religion, out result) && result is string religion)
            ReligionComboBox.Text = religion;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.culture, out result) && result is string culture)
            CultureComboBox.Text = culture;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.capital, out result) && result is string capital)
            CapitalNameTextBox.Text = capital;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.is_city, out result) && result is bool isCity)
            IsCityCheckBox.Checked = isCity;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.hre, out result) && result is bool isHre)
            IsHreCheckBox.Checked = isHre;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.seat_in_parliament, out result) && result is bool isSeatInParliament)
            IsParlimentSeatCheckbox.Checked = isSeatInParliament;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.revolt, out result) && result is bool hasRevolt)
            HasRevoltCheckBox.Checked = hasRevolt;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.base_tax, out result) && result is int baseTax)
            TaxNumeric.Value = baseTax;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.base_production, out result) && result is int baseProduction)
            ProdNumeric.Value = baseProduction;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.base_manpower, out result) && result is int baseManpower)
            ManpNumeric.Value = baseManpower;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.local_autonomy, out result) && result is float localAutonomy)
            AutonomyNumeric.Value = (int)localAutonomy;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.devastation, out result) && result is float devastation)
            DevastationNumeric.Value = (int)devastation;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.prosperity, out result) && result is float prosperity)
            ProsperityNumeric.Value = (int)prosperity;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.trade_good, out result) && result is string tradeGood)
            TradeGoodsComboBox.Text = tradeGood;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.center_of_trade, out result) && result is int centerOfTrade)
            TradeCenterComboBox.Text = centerOfTrade.ToString();
         if (Globals.Selection.GetSharedAttribute(ProvAttr.extra_cost, out result) && result is int extraCost)
            ExtraCostNumeric.Value = extraCost;
         ResumeLayout();
         Globals.EditingStatus = EditingStatus.Idle;
      }

      public void LoadProvinceToGui(Province province)
      {
         Globals.EditingStatus = EditingStatus.LoadingInterface;
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
         TaxNumeric.Value = province.BaseTax;
         ProdNumeric.Value = province.BaseProduction;
         ManpNumeric.Value = province.BaseManpower;
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
         Globals.EditingStatus = EditingStatus.Idle;
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
         TaxNumeric.Value = 1;
         ProdNumeric.Value = 1;
         ManpNumeric.Value = 1;
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
         EditingModeLabel.Text = Globals.Selection.Count <= 1
            ? "Idle Mode: Single Province"
            : $"Idle Mode: Multi Province ({Globals.Selection.Count})";
      }

      public void SetIsEditedLabel()
      {
         if (Globals.Selection.SelectedProvinces.Count == 1)
         {
            if (!Globals.Provinces.TryGetValue(Globals.Selection.SelectedProvinces[0], out var prov))
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
               case Keys.Z:
                  Globals.HistoryManager.Undo();
                  break;
               case Keys.Y:
                  Globals.HistoryManager.Redo();
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
