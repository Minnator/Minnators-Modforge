using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime;
using System.Text;
using Editor.Controls;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Settings;
using Editor.Events;
using Editor.Forms;
using Editor.Forms.Crash_Reporter;
using Editor.Forms.Loadingscreen;
using Editor.Forms.SavingClasses;
using Editor.Helper;
using Editor.Loading;
using Editor.Savers;
using static Editor.Helper.ProvinceEnumHelper;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor
{
   public partial class MapWindow : Form
   {
      #region CustomEditingControls

      private ItemList _claims = null!;
      private ItemList _permanentClaims = null!;
      private ItemList _cores = null!;
      private ItemList _buildings = null!;
      private ItemList _discoveredBy = null!;

      private ExtendedComboBox _religionComboBox = null!;
      private ExtendedComboBox _cultureComboBox = null!;
      public ExtendedComboBox ModifierComboBox = null!;
      private ExtendedComboBox _modifierTypeComboBox = null!;

      private ExtendedNumeric _taxNumeric = null!;
      private ExtendedNumeric _prdNumeric = null!;
      private ExtendedNumeric _mnpNumeric = null!;

      private ExtendedNumeric _autonomyNumeric = null!;
      private ExtendedNumeric _devastationNumeric = null!;
      private ExtendedNumeric _prosperityNumeric = null!;
      private ExtendedNumeric _extraCostNumeric = null!;

      private TagComboBox _tribalOwner = null!;
      private ExtendedComboBox _tradeCompanyInvestments = null!;

      private NumberTextBox _nativesSizeTextBox = null!;
      private NumberTextBox _nativeFerocityTextBox = null!;
      private NumberTextBox _nativeHostilityTextBox = null!;

      private ToolTip _savingButtonsToolTip = null!;

      public CollectionEditor2<Area, Province> AreaEditingGui = null!;
      public CollectionEditor2<Region, Area> RegionEditingGui = null!;
      public CollectionEditor2<SuperRegion, Region> SuperRegionEditingGui = null!;
      public CollectionEditor2<TradeCompany, Province> TradeCompanyEditingGui = null!;
      public CollectionEditor2<Country, Province> CountryEditingGui = null!;
      public CollectionEditor2<TradeNode, Province> TradeNodeEditingGui = null!;
      public CollectionEditor2<ProvinceGroup, Province> ProvinceGroupsEditingGui = null!;

      public Screen? StartScreen = null;

      #endregion

      public readonly DateControl DateControl = new(DateTime.MinValue, DateControlLayout.Horizontal);
      private LoadingScreen _ls = null!;

      public MapWindow()
      {
         Globals.State = State.Loading;
         Globals.MapWindow = this;

         RunLoadingScreen();

         if (StartScreen != null)
         {
            // Center the new form on the StartScreen
            var screen = Globals.MapWindow.StartScreen;
            if (screen != null)
               Location = new(screen.Bounds.X + (screen.Bounds.Width - Width) / 2, screen.Bounds.Y + (screen.Bounds.Height - Height) / 2);
         }

         if (LinkHelper.IsDiscordRunning)
            DiscordActivityManager.StartDiscordActivity();
      }

      public void Initialize()
      {
         Hide();
         //pause gui updates
         SuspendLayout();
         Globals.Settings = SettingsLoader.Load();

         InitGui();
         Text = $"{Text} | {Globals.DescriptorData.Name}";

         // MUST BE LAST in the loading sequence
         InitMapModes();
         Globals.LoadingStage++;
         Globals.HistoryManager.UndoDepthChanged += UpdateUndoDepth!;
         Globals.HistoryManager.RedoDepthChanged += UpdateRedoDepth!;
         Globals.LoadingStage++;


         //Needs to be after loading the game data to populate the gui with it
         InitializeEditGui();
         Globals.LoadingStage++;
         //resume gui updates
         // Enable the Application
         Globals.LoadingLog.Close();
         ResourceUsageHelper.Initialize(this);

         // ALL LOADING COMPLETE - Set the application to running

         DateControl.Date = new(1444, 11, 11);
         CountryLoading.AssignProvinces();
         Globals.State = State.Running;
         Globals.LoadingStage++;
         Globals.MapModeManager.SetCurrentMapMode(MapModeType.Country);
         ResumeLayout();
         Globals.LoadingStage++;

         StartPosition = FormStartPosition.CenterScreen;
         Globals.ZoomControl.FocusOn(new(3100, 600), 1f);
         Show();

         // Activate this window
         Activate();
         Globals.ZoomControl.Invalidate();
         AfterLoad();

         RestructureModifierNameDict();
      }

      public void RestructureModifierNameDict()
      {
      }

      private void AfterLoad()
      {
         GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
         GC.Collect();
         GC.WaitForPendingFinalizers();

      }

      public void InitMapModes()
      {
         var sw = Stopwatch.StartNew();
         Globals.MapModeManager = new(); // Initialize the MapModeManager
         MapModeComboBox.Items.Clear();
         MapModeComboBox.Items.AddRange([.. Enum.GetNames<MapModeType>()]);
         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Initializing MapModes", sw.ElapsedMilliseconds);
      }

      private void RunLoadingScreen()
      {
         _ls = new();
         _ls.ShowDialog();
      }

      // Loads the map into the created PannablePictureBox
      private void InitGui()
      {
         InitializeComponent();
         Globals.ZoomControl = new(new(Globals.MapWidth, Globals.MapHeight));
         MapLayoutPanel.Controls.Add(Globals.ZoomControl, 0, 0);
         Selection.Initialize();
         GuiDrawing.Initialize();

         TopStripLayoutPanel.Controls.Add(DateControl, 4, 0);
         DateControl.OnDateChanged += OnDateChanged;

         SelectionTypeBox.Items.AddRange([.. Enum.GetNames<SelectionType>()]);
         SelectionTypeBox.SelectedIndex = 0;
      }


      #region EditGui
      /// <summary>
      /// Initializes all Tabs of the EditGui
      /// </summary>
      private void InitializeEditGui()
      {
         InitializeProvinceEditGui();
         InitializeProvinceCollectionEditGui();
         InitializeCountryEditGui();
         InitializeMapModeButtons();
         InitializeInfoStrip();
      }

      private void InitializeInfoStrip()
      {
         Globals.MapModeManager.MapModeChanged += (sender, mode) =>
         {
            MapModeTimesInfo.Text = $"MapMode times [ms]: last: {Globals.MapModeManager.LasMapModeTime} | Min: {Globals.MapModeManager.MinMapModeTime} | Max: {Globals.MapModeManager.MaxMapModeTime} | Avg: {Globals.MapModeManager.AverageMapModeTime}";
         };
         MapModeTimesInfo.ToolTipText =
            "Time only counts in the time it takes the Rendering Pipeline to render the map and Invalidate the control.\nOther GUI updates are not included.";
         RamUsageStrip.ToolTipText = "Current RAM usage of the application\nThis is inflated by HeapAllocation from the GC and will free memory if your PC needs some.";
         UndoDepthLabel.ToolTipText = "Undo stack depth\nThis is the amount of actions you can undo/redo";
         RedoDepthLabel.ToolTipText = "Redo stack depth\nThis is the amount of actions you can redo/undo";
         CpuUsageStrip.ToolTipText = "Current CPU usage of the application";
         SelectedProvinceSum.ToolTipText = "Sum of selected provinces";
      }

      private void InitializeMapModeButtons()
      {
         var button01 = ControlFactory.GetMapModeButton('q');
         button01.SetMapMode(MapModeType.Province);
         var button2 = ControlFactory.GetMapModeButton('w');
         button2.SetMapMode(MapModeType.Country);
         var button3 = ControlFactory.GetMapModeButton('e');
         button3.SetMapMode(MapModeType.TradeNode);
         var button4 = ControlFactory.GetMapModeButton('r');
         button4.SetMapMode(MapModeType.Area);
         var button5 = ControlFactory.GetMapModeButton('t');
         button5.SetMapMode(MapModeType.Regions);
         var button6 = ControlFactory.GetMapModeButton('y');
         button6.SetMapMode(MapModeType.SuperRegion);
         var button7 = ControlFactory.GetMapModeButton('u');
         button7.SetMapMode(MapModeType.Continent);
         var button8 = ControlFactory.GetMapModeButton('i');
         button8.SetMapMode(MapModeType.Development);
         var button9 = ControlFactory.GetMapModeButton('o');
         button9.SetMapMode(MapModeType.CenterOfTrade);
         var button10 = ControlFactory.GetMapModeButton('p');
         button10.SetMapMode(MapModeType.Autonomy);
         MMButtonsTLPanel.Controls.Add(button01, 0, 0);
         MMButtonsTLPanel.Controls.Add(button2, 1, 0);
         MMButtonsTLPanel.Controls.Add(button3, 2, 0);
         MMButtonsTLPanel.Controls.Add(button4, 3, 0);
         MMButtonsTLPanel.Controls.Add(button5, 4, 0);
         MMButtonsTLPanel.Controls.Add(button6, 5, 0);
         MMButtonsTLPanel.Controls.Add(button7, 6, 0);
         MMButtonsTLPanel.Controls.Add(button8, 7, 0);
         MMButtonsTLPanel.Controls.Add(button9, 8, 0);
         MMButtonsTLPanel.Controls.Add(button10, 9, 0);
      }

      private void InitializeProvinceCollectionEditGui()
      {
         CountryEditingGui = new(ItemTypes.Id, SaveableType.Country, SaveableType.Province, MapModeType.Country);
         Country.ItemsModified += CountryEditingGui.OnCorrespondingDataChange;

         AreaEditingGui = new(ItemTypes.Id, SaveableType.Area, SaveableType.Province, MapModeType.Area);
         Area.ItemsModified += AreaEditingGui.OnCorrespondingDataChange;

         RegionEditingGui = new(ItemTypes.String, SaveableType.Region, SaveableType.Area, MapModeType.Regions);
         DataClasses.GameDataClasses.Region.ItemsModified += RegionEditingGui.OnCorrespondingDataChange;

         SuperRegionEditingGui = new(ItemTypes.String, SaveableType.SuperRegion, SaveableType.Region, MapModeType.SuperRegion);
         SuperRegion.ItemsModified += SuperRegionEditingGui.OnCorrespondingDataChange;

         TradeCompanyEditingGui = new(ItemTypes.Id, SaveableType.TradeCompany, SaveableType.Province, MapModeType.TradeCompany);
         TradeCompany.ItemsModified += TradeCompanyEditingGui.OnCorrespondingDataChange;

         TradeNodeEditingGui = new(ItemTypes.Id, SaveableType.TradeNode, SaveableType.Province, MapModeType.TradeNode);
         TradeNode.ItemsModified += TradeNodeEditingGui.OnCorrespondingDataChange;

         ProvinceGroupsEditingGui = new(ItemTypes.Id, SaveableType.ProvinceGroup, SaveableType.Province, MapModeType.ProvinceGroup);
         ProvinceGroup.ItemsModified += ProvinceGroupsEditingGui.OnCorrespondingDataChange;

         ProvinceCollectionsLayoutPanel.Controls.Add(CountryEditingGui, 0, 0);
         ProvinceCollectionsLayoutPanel.Controls.Add(RegionEditingGui, 0, 0);
         ProvinceCollectionsLayoutPanel.Controls.Add(AreaEditingGui, 0, 1);
         ProvinceCollectionsLayoutPanel.Controls.Add(SuperRegionEditingGui, 0, 2);
         ProvinceCollectionsLayoutPanel.Controls.Add(TradeCompanyEditingGui, 0, 3);
         ProvinceCollectionsLayoutPanel.Controls.Add(TradeNodeEditingGui, 0, 4);
         ProvinceCollectionsLayoutPanel.Controls.Add(ProvinceGroupsEditingGui, 0, 5);
      }


      private void InitializeProvinceEditGui()
      {
         // Tooltips for saving Buttons
         _savingButtonsToolTip = new();
         _savingButtonsToolTip.AutoPopDelay = 10000;
         _savingButtonsToolTip.InitialDelay = 100;
         _savingButtonsToolTip.ShowAlways = true;

         SaveAllModifiedButton.MouseEnter += OnSavingModifiedEnter;
         SaveAllProvincesButton.MouseEnter += OnSavingAllEnter;
         SaveCurrentSelectionButton.MouseEnter += OnSavingSelectionEnter;

         OwnerTagBox = ControlFactory.GetTagComboBox();
         OwnerTagBox.OnTagChanged += ProvinceEditingEvents.OnOwnerChanged;
         ControllerTagBox = ControlFactory.GetTagComboBox();
         ControllerTagBox.OnTagChanged += ProvinceEditingEvents.OnControllerChanged;
         OwnerControllerLayoutPanel.Controls.Add(OwnerTagBox, 1, 0);
         OwnerControllerLayoutPanel.Controls.Add(ControllerTagBox, 1, 1);

         _cores = ControlFactory.GetItemList(ItemTypes.Tag, [.. Globals.Countries.Keys], "Cores");
         _cores.OnItemAdded += ProvinceEditingEvents.OnCoreAdded;
         _cores.OnItemRemoved += ProvinceEditingEvents.OnCoreRemoved;
         _claims = ControlFactory.GetItemList(ItemTypes.Tag, [.. Globals.Countries.Keys], "Regular");
         _claims.OnItemAdded += ProvinceEditingEvents.OnClaimAdded;
         _claims.OnItemRemoved += ProvinceEditingEvents.OnClaimRemoved;
         _permanentClaims = ControlFactory.GetItemList(ItemTypes.Tag, [.. Globals.Countries.Keys], "Permanent");
         _permanentClaims.OnItemAdded += ProvinceEditingEvents.OnPermanentClaimAdded;
         _permanentClaims.OnItemRemoved += ProvinceEditingEvents.OnPermanentClaimRemoved;

         _buildings = ControlFactory.GetItemListObjects(ItemTypes.String, [.. Globals.Buildings], "Building");
         _buildings.OnItemAdded += ProvinceEditingEvents.OnBuildingAdded;
         _buildings.OnItemRemoved += ProvinceEditingEvents.OnBuildingRemoved;
         _discoveredBy = ControlFactory.GetItemList(ItemTypes.String, [.. Globals.TechnologyGroups.Keys], "TechGroup");
         _discoveredBy.OnItemAdded += ProvinceEditingEvents.OnDiscoveredByAdded;
         _discoveredBy.OnItemRemoved += ProvinceEditingEvents.OnDiscoveredByRemoved;

         TradeCenterComboBox.SelectedIndexChanged += ProvinceEditingEvents.OnTradeCenterChanged;

         LocalisationTextBox.LostFocus += (_, _) =>
         {
            LocObjectModifications.ModifyIfExistsOtherwiseAdd(LocalisationLabel.Text, LocalisationTextBox.Text);
         };
         ProvAdjTextBox.LostFocus += (_, _) =>
         {
            LocObjectModifications.ModifyIfExistsOtherwiseAdd(ProvAdjLabel.Text, ProvAdjTextBox.Text);
         };

         CoresAndClaimLayoutPanel.Controls.Add(_permanentClaims, 0, 0);
         CoresAndClaimLayoutPanel.Controls.Add(_claims, 1, 0);
         CoresGroupBox.Controls.Add(_cores);
         _cores.Location = new(0, 18);
         BuildingsGroupBox.Controls.Add(_buildings);
         _buildings.Location = new(0, 18);
         DiscoveredByGroupBox.Controls.Add(_discoveredBy);
         _discoveredBy.Location = new(0, 18);

         List<string> tradeGoodsString = [.. Globals.TradeGoods.Keys];
         tradeGoodsString.Sort();
         TradeGoodsComboBox.Items.AddRange([.. tradeGoodsString]);
         TradeGoodsComboBox.SelectedIndexChanged += ProvinceEditingEvents.OnTradeGoodChanged;

         List<string> culturesString = [.. Globals.Cultures.Keys];
         culturesString.Sort();
         _cultureComboBox = ControlFactory.GetExtendedComboBox();
         RCCLayoutPanel.Controls.Add(_cultureComboBox, 1, 1);
         _cultureComboBox.Items.AddRange([.. culturesString]);
         _cultureComboBox.OnDataChanged += ProvinceEditingEvents.OnCultureChanged;

         List<string> religionsString = [.. Globals.Religions.Keys];
         religionsString.Sort();
         _religionComboBox = ControlFactory.GetExtendedComboBox();
         RCCLayoutPanel.Controls.Add(_religionComboBox, 1, 0);
         _religionComboBox.Items.AddRange([.. religionsString]);
         _religionComboBox.OnDataChanged += ProvinceEditingEvents.OnReligionChanged;

         _taxNumeric = ControlFactory.GetExtendedNumeric();
         _taxNumeric.Minimum = 0;
         _taxNumeric.Maximum = 1000;
         _taxNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextBaseTaxChanged;
         _taxNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpBaseTaxChanged;
         _taxNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpTaxButtonButtonPressedMedium;
         _taxNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpBaseTaxButtonPressedLarge;
         _taxNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownBaseTaxChanged;
         _taxNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownBaseTaxButtonPressedMedium;
         _taxNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownBaseTaxButtonPressedLarge;
         DevelopmentLayoutPanel.Controls.Add(_taxNumeric, 1, 0);

         _prdNumeric = ControlFactory.GetExtendedNumeric();
         _prdNumeric.Minimum = 0;
         _prdNumeric.Maximum = 1000;
         _prdNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextBaseProductionChanged;
         _prdNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpBaseProductionChanged;
         _prdNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumProduction;
         _prdNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeProduction;
         _prdNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownBaseProductionChanged;
         _prdNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumProduction;
         _prdNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeProduction;
         DevelopmentLayoutPanel.Controls.Add(_prdNumeric, 1, 1);

         _mnpNumeric = ControlFactory.GetExtendedNumeric();
         _mnpNumeric.Minimum = 0;
         _mnpNumeric.Maximum = 1000;
         _mnpNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextBaseManpowerChanged;
         _mnpNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpBaseManpowerChanged;
         _mnpNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumManpower;
         _mnpNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeManpower;
         _mnpNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownBaseManpowerChanged;
         _mnpNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumManpower;
         _mnpNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeManpower;
         DevelopmentLayoutPanel.Controls.Add(_mnpNumeric, 1, 2);

         _autonomyNumeric = ControlFactory.GetExtendedNumeric();
         _autonomyNumeric.Minimum = 0;
         _autonomyNumeric.Maximum = 100;
         _autonomyNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextLocalAutonomyChanged;
         _autonomyNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpLocalAutonomyChanged;
         _autonomyNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumLocalAutonomy;
         _autonomyNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeLocalAutonomy;
         _autonomyNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownLocalAutonomyChanged;
         _autonomyNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumLocalAutonomy;
         _autonomyNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeLocalAutonomy;
         FloatLayoutPanel.Controls.Add(_autonomyNumeric, 1, 0);

         _devastationNumeric = ControlFactory.GetExtendedNumeric();
         _devastationNumeric.Minimum = 0;
         _devastationNumeric.Maximum = 100;
         _devastationNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextDevastationChanged;
         _devastationNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpDevastationChanged;
         _devastationNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumDevastation;
         _devastationNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeDevastation;
         _devastationNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownDevastationChanged;
         _devastationNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumDevastation;
         _devastationNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeDevastation;
         FloatLayoutPanel.Controls.Add(_devastationNumeric, 1, 1);

         _prosperityNumeric = ControlFactory.GetExtendedNumeric();
         _prosperityNumeric.Minimum = 0;
         _prosperityNumeric.Maximum = 100;
         _prosperityNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextProsperityChanged;
         _prosperityNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpProsperityChanged;
         _prosperityNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumProsperity;
         _prosperityNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeProsperity;
         _prosperityNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownProsperityChanged;
         _prosperityNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumProsperity;
         _prosperityNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeProsperity;
         FloatLayoutPanel.Controls.Add(_prosperityNumeric, 1, 2);

         _extraCostNumeric = ControlFactory.GetExtendedNumeric();
         _extraCostNumeric.Minimum = 0;
         _extraCostNumeric.Maximum = 1000;
         _extraCostNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextExtraCostChanged;
         _extraCostNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpExtraCostChanged;
         _extraCostNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumExtraCost;
         _extraCostNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeExtraCost;
         _extraCostNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownExtraCostChanged;
         _extraCostNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumExtraCost;
         _extraCostNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeExtraCost;
         TradePanel.Controls.Add(_extraCostNumeric, 1, 2);

         CapitalNameTextBox.Leave += ProvinceEditingEvents.OnCapitalNameChanged;

         IsCityCheckBox.CheckedChanged += ProvinceEditingEvents.OnIsCityChanged;
         IsHreCheckBox.CheckedChanged += ProvinceEditingEvents.OnIsHreChanged;
         IsParlimentSeatCheckbox.CheckedChanged += ProvinceEditingEvents.OnIsSeatInParliamentChanged;
         HasRevoltCheckBox.CheckedChanged += ProvinceEditingEvents.OnHasRevoltChanged;

         MWAttirbuteCombobox.Items.AddRange([.. Enum.GetNames<ProvAttrGet>()]);

         // NATIVES TAB
         _tribalOwner = ControlFactory.GetTagComboBox();
         _tribalOwner.OnTagChanged += ProvinceEditingEvents.OnTribalOwnerChanged;
         NativesLayoutPanel.Controls.Add(_tribalOwner, 1, 0);

         _nativesSizeTextBox = ControlFactory.GetNumberTextBox();
         _nativesSizeTextBox.OnDataChanged += ProvinceEditingEvents.OnNativeSizeChanged;
         NativesLayoutPanel.Controls.Add(_nativesSizeTextBox, 1, 1);

         _nativeFerocityTextBox = ControlFactory.GetNumberTextBox();
         _nativeFerocityTextBox.OnDataChanged += ProvinceEditingEvents.OnNativeFerocityChanged;
         NativesLayoutPanel.Controls.Add(_nativeFerocityTextBox, 1, 2);

         _nativeHostilityTextBox = ControlFactory.GetNumberTextBox();
         _nativeHostilityTextBox.OnDataChanged += ProvinceEditingEvents.OnNativeHostilityChanged;
         NativesLayoutPanel.Controls.Add(_nativeHostilityTextBox, 1, 3);

         // TRADE_COMPANIES TAB
         _tradeCompanyInvestments = ControlFactory.GetExtendedComboBox();
         _tradeCompanyInvestments.Items.AddRange([.. Globals.TradeCompanyInvestments.Keys]);
         _tradeCompanyInvestments.OnDataChanged += ProvinceEditingEvents.OnTradeCompanyInvestmentChanged;
         TradeCompaniesLayoutPanel.Controls.Add(_tradeCompanyInvestments, 1, 0);

         // MODIFIERS TAB
         InitializeModifierTab();

         // Localisation GroupBox

      }

      #endregion



      private void InitializeModifierTab()
      {
         ModifierComboBox = ControlFactory.GetExtendedComboBox();
         ModifierComboBox.Items.AddRange([.. Globals.EventModifiers.Keys]);
         // No data changed here as they are added via the "Add" button
         ModifiersLayoutPanel.Controls.Add(ModifierComboBox, 1, 1);

         _modifierTypeComboBox = ControlFactory.GetExtendedComboBox(["CountryModifier", "ProvinceModifier"]);
         _modifierTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
         _modifierTypeComboBox.Margin = new(2, 1, 2, 0);
         ModTypeSubLayout.Controls.Add(_modifierTypeComboBox, 0, 0);

         ModifiersListView.View = View.Details;
         ModifiersListView.FullRowSelect = true;
         ModifiersListView.Columns.Add("Name");
         ModifiersListView.Columns.Add("Duration");
         ModifiersListView.Columns.Add("Type");
         // Space the columns out
         ModifiersListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
         ModifiersListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
      }


      // ======================== Province GUI Update Methods ========================
      #region Province Gui

      /// <summary>
      /// Is executed once a province is selected
      /// </summary>
      public void ProvinceClick()
      {
         LoadSelectedProvincesToGui();
      }

      /// <summary>
      /// This will only load the province attributes to the gui which are shared by all provinces
      /// </summary>
      public void LoadSelectedProvincesToGui()
      {
         Globals.EditingStatus = EditingStatus.LoadingInterface;
         SuspendLayout();
         ClearProvinceGui();
         if (Selection.GetSharedAttribute(ProvAttrGet.claims, out var result) && result is List<Tag> tags)
            _claims.AddItemsUnique([.. tags]);
         if (Selection.GetSharedAttribute(ProvAttrGet.permanent_claims, out result) && result is List<Tag> permanentTags)
            _permanentClaims.AddItemsUnique([.. permanentTags]);
         if (Selection.GetSharedAttribute(ProvAttrGet.cores, out result) && result is List<Tag> coreTags)
            _cores.AddItemsUnique([.. coreTags]);
         if (Selection.GetSharedAttribute(ProvAttrGet.buildings, out result) && result is List<string> buildings)
            _buildings.AddItemsUnique(buildings);
         if (Selection.GetSharedAttribute(ProvAttrGet.discovered_by, out result) && result is List<string> techGroups)
            _discoveredBy.AddItemsUnique(techGroups);
         if (Selection.GetSharedAttribute(ProvAttrGet.owner, out result) && result is Tag owner)
            OwnerTagBox.Text = owner;
         if (Selection.GetSharedAttribute(ProvAttrGet.controller, out result) && result is Tag controller)
            ControllerTagBox.Text = controller;
         if (Selection.GetSharedAttribute(ProvAttrGet.religion, out result) && result is string religion)
            _religionComboBox.Text = religion;
         if (Selection.GetSharedAttribute(ProvAttrGet.culture, out result) && result is string culture)
            _cultureComboBox.Text = culture;
         if (Selection.GetSharedAttribute(ProvAttrGet.capital, out result) && result is string capital)
            CapitalNameTextBox.Text = capital;
         if (Selection.GetSharedAttribute(ProvAttrGet.is_city, out result) && result is bool isCity)
            IsCityCheckBox.Checked = isCity;
         if (Selection.GetSharedAttribute(ProvAttrGet.hre, out result) && result is bool isHre)
            IsHreCheckBox.Checked = isHre;
         if (Selection.GetSharedAttribute(ProvAttrGet.seat_in_parliament, out result) && result is bool isSeatInParliament)
            IsParlimentSeatCheckbox.Checked = isSeatInParliament;
         if (Selection.GetSharedAttribute(ProvAttrGet.revolt, out result) && result is bool hasRevolt)
            HasRevoltCheckBox.Checked = hasRevolt;
         if (Selection.GetSharedAttribute(ProvAttrGet.base_tax, out result) && result is int baseTax)
            _taxNumeric.Value = baseTax;
         if (Selection.GetSharedAttribute(ProvAttrGet.base_production, out result) && result is int baseProduction)
            _prdNumeric.Value = baseProduction;
         if (Selection.GetSharedAttribute(ProvAttrGet.base_manpower, out result) && result is int baseManpower)
            _mnpNumeric.Value = baseManpower;
         if (Selection.GetSharedAttribute(ProvAttrGet.local_autonomy, out result) && result is float localAutonomy)
            _autonomyNumeric.Value = (int)localAutonomy;
         if (Selection.GetSharedAttribute(ProvAttrGet.devastation, out result) && result is float devastation)
            _devastationNumeric.Value = (int)devastation;
         if (Selection.GetSharedAttribute(ProvAttrGet.prosperity, out result) && result is float prosperity)
            _prosperityNumeric.Value = (int)prosperity;
         if (Selection.GetSharedAttribute(ProvAttrGet.trade_good, out result) && result is string tradeGood)
            TradeGoodsComboBox.Text = tradeGood;
         if (Selection.GetSharedAttribute(ProvAttrGet.center_of_trade, out result) && result is int centerOfTrade)
            TradeCenterComboBox.Text = centerOfTrade.ToString();
         if (Selection.GetSharedAttribute(ProvAttrGet.extra_cost, out result) && result is int extraCost)
            _extraCostNumeric.Value = extraCost;
         if (Selection.GetSharedAttribute(ProvAttrGet.tribal_owner, out result) && result is Tag tribalOwner)
            _tribalOwner.Text = tribalOwner;
         if (Selection.GetSharedAttribute(ProvAttrGet.native_size, out result) && result is int nativeSize)
            _nativesSizeTextBox.Text = nativeSize.ToString();
         if (Selection.GetSharedAttribute(ProvAttrGet.native_ferocity, out result) && result is float nativeFerocity)
            _nativeFerocityTextBox.Text = nativeFerocity.ToString(CultureInfo.InvariantCulture);
         if (Selection.GetSharedAttribute(ProvAttrGet.native_hostileness, out result) && result is float nativeHostileness)
            _nativeHostilityTextBox.Text = nativeHostileness.ToString(CultureInfo.InvariantCulture);
         // TODO The Gui needs to be able to represent several trade company investments
         //if (Selection.GetSharedAttribute(ProvAttrGet.trade_company_investment, out result) && result is string tradeCompanyInvestments)
         //   _tradeCompanyInvestments.Text = tradeCompanyInvestments;
         if (Selection.GetSelectedProvinces.Count == 1)
         {
            AddAllModifiersToListView(Selection.GetSelectedProvinces[0]);
            LocalisationTextBox.Text = Selection.GetSelectedProvinces[0].GetLocalisation();
            LocalisationLabel.Text = Selection.GetSelectedProvinces[0].GetLocalisationString();
            ProvAdjTextBox.Text = Localisation.GetLoc(Selection.GetSelectedProvinces[0].GetLocalisationAdj);
            ProvAdjLabel.Text = Selection.GetSelectedProvinces[0].GetLocalisationAdj;
         }
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
         _religionComboBox.Text = province.Religion;
         _cultureComboBox.Text = province.Culture;
         CapitalNameTextBox.Text = province.Capital;
         IsCityCheckBox.Checked = province.IsCity;
         IsHreCheckBox.Checked = province.IsHre;
         IsParlimentSeatCheckbox.Checked = province.IsSeatInParliament;
         HasRevoltCheckBox.Checked = province.HasRevolt;
         _taxNumeric.Value = province.BaseTax;
         _prdNumeric.Value = province.BaseProduction;
         _mnpNumeric.Value = province.BaseManpower;
         _claims.AddItemsUnique([.. province.Claims]);
         _permanentClaims.AddItemsUnique([]);
         _cores.AddItemsUnique([.. province.Cores]);
         _buildings.AddItemsUnique(province.Buildings);
         _discoveredBy.AddItemsUnique(province.DiscoveredBy);
         _autonomyNumeric.Value = (int)province.LocalAutonomy;
         _devastationNumeric.Value = (int)province.Devastation;
         _prosperityNumeric.Value = (int)province.Prosperity;
         TradeGoodsComboBox.Text = province.TradeGood;
         TradeCenterComboBox.Text = province.CenterOfTrade.ToString();
         _extraCostNumeric.Value = province.ExtraCost;
         _tribalOwner.Text = province.TribalOwner;
         _nativesSizeTextBox.Text = province.NativeSize.ToString();
         _nativeFerocityTextBox.Text = province.NativeFerocity.ToString(CultureInfo.InvariantCulture);
         _nativeHostilityTextBox.Text = province.NativeHostileness.ToString(CultureInfo.InvariantCulture);
         // _tradeCompanyInvestments.Text = province.TradeCompanyInvestments;
         AddAllModifiersToListView(province);
         ResumeLayout();
         Globals.EditingStatus = EditingStatus.Idle;
      }

      public void ClearProvinceGui()
      {
         OwnerTagBox.Clear();
         ControllerTagBox.Clear();
         _religionComboBox.Clear();
         _cultureComboBox.Clear();
         CapitalNameTextBox.Clear();
         IsCityCheckBox.Checked = false;
         IsHreCheckBox.Checked = false;
         IsParlimentSeatCheckbox.Checked = false;
         HasRevoltCheckBox.Checked = false;
         _taxNumeric.Value = 1;
         _prdNumeric.Value = 1;
         _mnpNumeric.Value = 1;
         _claims.Clear();
         _permanentClaims.Clear();
         _cores.Clear();
         _buildings.Clear();
         _discoveredBy.Clear();
         _autonomyNumeric.Value = 0;
         _devastationNumeric.Value = 0;
         _prosperityNumeric.Value = 0;
         TradeGoodsComboBox.Clear();
         TradeCenterComboBox.Clear();
         _extraCostNumeric.Value = 0;
         _tribalOwner.Clear();
         _nativesSizeTextBox.Text = "0";
         _nativeFerocityTextBox.Text = "0";
         _nativeHostilityTextBox.Text = "0";
         ModifierComboBox.Text = string.Empty;
         ModifiersListView.Items.Clear();
         DurationTextBox.Text = string.Empty;
         _tradeCompanyInvestments.Text = string.Empty;
         LocalisationTextBox.Text = string.Empty;
         LocalisationLabel.Text = string.Empty;
         ProvAdjTextBox.Text = string.Empty;
         ProvAdjLabel.Text = string.Empty;
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
         if (InvokeRequired) Invoke(new MethodInvoker(delegate { CpuUsageStrip.Text = $"CPU: [{Math.Round(cpuUsage, 2):F2}%]"; }));
      }

      #endregion

      #region HistoryManager Event Handlers

      private void UpdateRedoDepth(object sender, int e) => RedoDepthLabel.Text = $"Redos [{e}]";
      private void UpdateUndoDepth(object sender, int e) => UndoDepthLabel.Text = $"Undos [{e}]";
      #endregion
      #endregion

      public void SetEditingMode()
      {
         EditingModeLabel.Text = Selection.Count <= 1
            ? "Idle Mode: Single Province"
            : $"Idle Mode: Multi Province ({Selection.Count})";
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
         var result = MessageBox.Show("Are you sure you want to delete the history?", "Delete History", MessageBoxButtons.OKCancel);
         if (result == DialogResult.OK)
            Globals.HistoryManager.Clear();
      }

      #endregion

      private void OnSavingAllEnter(object? sender, EventArgs e)
      {
         _savingButtonsToolTip.SetToolTip(SaveAllProvincesButton, $"Save all provinces ({Globals.LandProvinceIds.Length})");
      }

      private void OnSavingModifiedEnter(object? sender, EventArgs e)
      {
         _savingButtonsToolTip.SetToolTip(SaveAllModifiedButton, $"Save modified provinces ({EditingHelper.GetModifiedProvinces().Count})");
      }

      private void OnSavingSelectionEnter(object? sender, EventArgs e)
      {
         _savingButtonsToolTip.SetToolTip(SaveCurrentSelectionButton, $"Save selection ({Selection.Count})");
      }

      private static void OnDateChanged(object? sender, EventArgs e)
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

      private void AddModifiersToList(ModifierAbstract modifier, ModifierType type)
      {
         var item = new ListViewItem(modifier.Name);
         item.SubItems.Add(modifier.Duration.ToString());
         item.SubItems.Add(type.ToString());
         ModifiersListView.Items.Add(item);

         ModifiersListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
         ModifiersListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
      }

      private void AddAllModifiersToListView(Province province)
      {
         ModifiersListView.Items.Clear();

         foreach (var modifier in province.ProvinceModifiers)
            AddModifiersToList(modifier, ModifierType.ProvinceModifier);

         foreach (var modifier in province.PermanentProvinceModifiers)
            AddModifiersToList(modifier, ModifierType.PermanentProvinceModifier);
      }

      private void debugToolStripMenuItem_Click(object sender, EventArgs e)
      {
      }

      private void MapModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         Globals.MapModeManager.SetCurrentMapMode(Enum.Parse<MapModeType>(MapModeComboBox.SelectedItem?.ToString() ?? string.Empty));
         GC.Collect(); // We force the garbage collector to collect the old bitmap
      }

      private void gCToolStripMenuItem_Click(object sender, EventArgs e)
      {
         GC.Collect();
      }

      private void SaveCurrentMapModeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
         var form = new GetSavingFileForm(pictures, "Where to save the map mode as an image to", ".png");
         form.SetPlaceHolderText(Globals.MapModeManager.CurrentMapMode.GetMapModeName().ToString());
         form.RequireModDirectory = false;
         form.ShowDialog();
         if (form.DialogResult == DialogResult.OK)
         {
            using var bmp = Globals.ZoomControl.Map;
            bmp.Save(form.NewPath, ImageFormat.Png);
         }
      }

      private void openCustomizerToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var toolTipCustomizer = new ToolTipCustomizer();
         toolTipCustomizer.Show();
      }

      private void ShowToolTipMenuItem_Click(object sender, EventArgs e)
      {
         Selection.ShowToolTip = ShowToolTipMenuItem.Checked;
      }

      private void telescopeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var sb = new StringBuilder();
         foreach (var province in Globals.Provinces)
         {
            sb.Append($"{province.GetLocalisation()}: ");
            foreach (var building in province.Buildings)
            {
               sb.Append($"{building}, ");
            }
            sb.AppendLine();
         }
         // download folder
         var downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\";
         File.WriteAllText(Path.Combine(downloadFolder, "buildings.txt"), sb.ToString());
      }

      private void fasdfToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (GuiDrawing.CurrentElements.HasFlag(GuiDrawing.GuiElements.TradeRoutes))
         {
            GuiDrawing.CurrentElements = GuiDrawing.GuiElements.None;
         }
         else
         {
            GuiDrawing.CurrentElements = GuiDrawing.GuiElements.TradeRoutes;
         }
      }

      public void FunnyPaint(object? sender, PaintEventArgs e)
      {
         //using var g = Globals.ZoomControl.CreateGraphics();
         float[] data = new float[]
         {
            3239.500000f, 1153.500000f,
            3180.000000f, 1204.000000f,
            3093.000000f, 1203.000000f,
            3017.500000f, 1244.500000f,
            2977.000000f, 1288.000000f,
            2967.000000f, 1347.000000f,
            2935.000000f, 1348.000000f
         };

         PointF[] points = new PointF[data.Length / 2 + 2];
         for (int i = 0; i < data.Length; i += 2)
         {
            points[i / 2 + 1] = Globals.ZoomControl.ReverseCoordinateFloat(new PointF(data[i], Globals.MapHeight - data[i + 1]));
         }

         points[0] = Globals.ZoomControl.ReverseCoordinateFloat(new PointF((float)Globals.ProvinceIdToProvince[358].Center.X, (float)Globals.ProvinceIdToProvince[358].Center.Y));
         points[^1] = Globals.ZoomControl.ReverseCoordinateFloat(new PointF((float)Globals.ProvinceIdToProvince[1298].Center.X, (float)Globals.ProvinceIdToProvince[1298].Center.Y));

         e.Graphics.DrawCurve(new Pen(Color.Red, 2), points);
         //g.DrawRectangle(Pens.BlueViolet, new RectangleF(points[0], new SizeF(400f, 400f)));
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
               // Tabs
               case Keys.D1:
                  DataTabPanel.SelectedIndex = 0;
                  break;
               case Keys.D2:
                  DataTabPanel.SelectedIndex = 1;
                  break;
               case Keys.D3:
                  DataTabPanel.SelectedIndex = 2;
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

      private void searchToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FormHelper.OpenOrBringToFront(Globals.SearchForm);
      }

      private void bestPointsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var tnsSortedTopological = TradeNodeHelper.TopologicalSort(Globals.TradeNodes.Values.ToList());
         DebugPrints.VerifyTopologicalSort(tnsSortedTopological);
      }

      private void provDiffToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var sb = new StringBuilder();
         foreach (var collision in Globals.LocalisationCollisions)
         {
            sb.AppendLine($"{collision.Key} : {collision.Value}");
         }

         var downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\";
         File.WriteAllText(Path.Combine(downloadFolder, "localisationCollisions.txt"), sb.ToString());
      }

      private void saveAllProvincesToolStripMenuItem_Click(object sender, EventArgs e)
      {
         ProvinceSaver.SaveAllLandProvinces();
      }


      private void saveSelectionToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new SelectionDrawerForm().Show();
      }

      private void SaveCurrentSelectionButton_Click(object sender, EventArgs e)
      {
         ProvinceSaver.SaveSelectedProvinces();
      }

      private void SaveAllModifiedButton_Click(object sender, EventArgs e)
      {
         ProvinceSaver.SaveAllModifiedProvinces();
      }

      private void SaveAllProvincesButton_Click(object sender, EventArgs e)
      {
         ProvinceSaver.SaveAllLandProvinces();
      }

      private void saveEuropeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var inputForm = new GetSavingFileForm(Globals.ModPath, "Please enter your input:", ".txt");
         if (inputForm.ShowDialog() == DialogResult.OK)
         {
            string userInput = inputForm.NewPath;
            Debug.WriteLine($"Selected path: {userInput}");
            MessageBox.Show("You entered: " + userInput);
         }

      }
      private void AddModifierButton_Click(object sender, EventArgs e)
      {
         var error = string.Empty;
         if (!int.TryParse(DurationTextBox.Text, out var duration))
            error += "Duration must be a number\n";
         if (ModifierComboBox.Text == string.Empty)
            error += "Modifier must be selected\n";
         if (_modifierTypeComboBox.Text == string.Empty)
            error += "ModifierType must be selected\n";

         if (error != string.Empty)
         {
            MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }

         var mod = new ApplicableModifier(ModifierComboBox.Text, duration);
         var type = Enum.Parse<ModifierType>(_modifierTypeComboBox.Text);
         ProvinceEditingEvents.OnModifierAdded(mod, type);
         AddModifiersToList(mod, type);
      }

      private void DeleteModifierButton_Click(object sender, EventArgs e)
      {
         if (ModifiersListView.SelectedItems.Count == 0)
            return;
         var item = ModifiersListView.SelectedItems[0];
         var modifierName = item.SubItems[0].Text.Trim();

         if (!int.TryParse(item.SubItems[1].Text, out var duration) || !Enum.TryParse(item.SubItems[2].Text, out ModifierType type))
            return;

         if (!Globals.EventModifiers.TryGetValue(modifierName, out _))
            return;

         ProvinceEditingEvents.OnModifierRemoved(new ApplicableModifier(modifierName, duration), type);
         ModifiersListView.Items.Remove(item);
      }

      private void IsaveableClick(object sender, EventArgs e)
      {
      }
      private void yoloToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var sw = Stopwatch.StartNew();
         for (var i = 0; i < 100; i++)
         {
            LocalisationLoading.Load();
         }
         sw.Stop();
         Debug.WriteLine($"Time: {sw.ElapsedMilliseconds / 100}");
      }

      private void graphicalElementsManagerToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (Globals.GuiDrawings == null || Globals.GuiDrawings.IsDisposed)
            Globals.GuiDrawings = new();
         Globals.GuiDrawings.Show();
      }

      private void OpenAddModifierForm_Click(object sender, EventArgs e)
      {
         new EventModifierForm().ShowDialog();
      }


      private void saveManualToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new ManualSaving().ShowDialog();
      }

      private void checkForCyclesInTradenodesToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var result = TradeNodeHelper.FindCycle(Globals.TradeNodes.Values.ToList());
         if (result.Count == 0)
            MessageBox.Show("TradeNodes does not contain a cycle", "No Cycle Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
         else
         {
            var sb = new StringBuilder();
            foreach (var tradeNode in result)
            {
               sb.AppendLine($"-> {tradeNode.Name}");
            }
            MessageBox.Show(sb.ToString(), "Cycle Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
         }
      }

      private void helpToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new InformationForm().ShowDialog();
      }

      private void randomModifierToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new ModifierSuggestion().ShowDialog();
      }


      // ------------------- COUNTRY EDITING TAB ------------------- \\
      private TagComboBox TagSelectionBox = null!;

      private ComboBox GraphicalCultureBox = null!;
      private ComboBox UnitTypeBox = null!;
      private ComboBox TechGroupBox = null!;
      private ComboBox GovernmentTypeBox = null!;
      private ComboBox GovernmentRankBox = null!;

      private ColorPickerButton CountryColorPickerButton = null!;
      private ColorPickerButton RevolutionColorPickerButton = null!;

      private ItemList GovernmentReforms = null!;


      private void InitializeCountryEditGui()
      {
         Selection.OnCountrySelected += CountryGuiEvents.OnCountrySelected;
         Selection.OnCountryDeselected += CountryGuiEvents.OnCountryDeselected;
         TagSelectionBox = new()
         {
            Margin = new(1),
            Height = 25,
         };
         TagSelectionBox.Items.Add("###");
         TagAndColorTLP.Controls.Add(TagSelectionBox, 1, 0);
         TagSelectionBox.OnTagChanged += CountryGuiEvents.TagSelectionBox_OnTagChanged;
         CountryColorPickerButton = ControlFactory.GetColorPickerButton();
         CountryColorPickerButton.Click += CountryGuiEvents.CountryColorPickerButton_Click;
         GeneralToolTip.SetToolTip(CountryColorPickerButton, "Set the <color> of the selected country");
         TagAndColorTLP.Controls.Add(CountryColorPickerButton, 3, 0);
         RevolutionColorPickerButton = ControlFactory.GetColorPickerButton();
         RevolutionColorPickerButton.Click += CountryGuiEvents.RevolutionColorPickerButton_Click;
         GeneralToolTip.SetToolTip(RevolutionColorPickerButton, "Set the <revolutionary_color> of the selected country");
         GraphicalCultureBox = ControlFactory.GetListComboBox(Globals.GraphicalCultures, new(1));
         UnitTypeBox = ControlFactory.GetListComboBox([.. Globals.TechnologyGroups.Keys], new(1));
         TechGroupBox = ControlFactory.GetListComboBox([.. Globals.TechnologyGroups.Keys], new(1));
         GovernmentTypeBox = ControlFactory.GetListComboBox([.. Globals.GovernmentTypes.Keys], new(1, 1, 6, 1));
         GovernmentRankBox = ControlFactory.GetListComboBox(["1", "2", "3"], new(1)); // TODO read in the defines to determine range
         GovernmentReforms = ControlFactory.GetItemList(ItemTypes.FullWidth, [.. Globals.GovernmentReforms.Keys], "Government Reforms");
         GovernmentReforms.Width = 117;




         TagAndColorTLP.Controls.Add(RevolutionColorPickerButton, 3, 3);
         TagAndColorTLP.Controls.Add(GraphicalCultureBox, 1, 2);
         TagAndColorTLP.Controls.Add(UnitTypeBox, 3, 2);
         TagAndColorTLP.Controls.Add(TechGroupBox, 1, 3);

         GovernmentLayoutPanel.Controls.Add(GovernmentTypeBox, 3, 0);
         GovernmentLayoutPanel.Controls.Add(GovernmentRankBox, 1, 0);
         GovernmentLayoutPanel.Controls.Add(GovernmentReforms, 0, 1);
         GovernmentLayoutPanel.SetColumnSpan(GovernmentReforms, 4);
      }

      public void ClearCountryGui()
      {
         TagSelectionBox.SelectedItem = "###";
         CountryNameLabel.Text = "Country: -";
         CountryColorPickerButton.BackColor = Color.Empty;
         CountryColorPickerButton.Text = "(//)";
         CountryADJLoc.Text = "-";
         CountryLoc.Text = "-";
         RevolutionColorPickerButton.BackColor = Color.Empty;
         GraphicalCultureBox.SelectedIndex = 0;
         UnitTypeBox.SelectedIndex = 0;
         TechGroupBox.SelectedIndex = 0;
      }

      internal void LoadCountryToGui(Country country)
      {
         if (country == Country.Empty)
         {
            ClearCountryGui();
            return;
         }
         TagSelectionBox.SelectedItem = country.Tag.ToString();
         CountryNameLabel.Text = $"{country.GetLocalisation()} ({country.Tag})";
         CountryColorPickerButton.BackColor = country.Color;
         CountryColorPickerButton.Text = $"({country.Color.R}/{country.Color.G}/{country.Color.B})";
         CountryLoc.Text = country.GetLocalisation();
         CountryADJLoc.Text = country.GetAdjectiveLocalisation();
         RevolutionColorPickerButton.BackColor = country.RevolutionaryColor;
         GraphicalCultureBox.SelectedItem = country.Gfx;
         UnitTypeBox.SelectedItem = country.UnitType;
         TechGroupBox.SelectedItem = country.TechnologyGroup;
      }

      private void CreateFilesByDefault_Click(object sender, EventArgs e)
      {
         CreateFilesByDefault.Checked = !CreateFilesByDefault.Checked;
         Globals.Settings.SavingSettings.AlwaysAskBeforeCreatingFiles = CreateFilesByDefault.Checked;
      }

      private void save1ToolStripMenuItem_Click(object sender, EventArgs e)
      {
      }

      private void SaveAllInDefaultFiles_Click(object sender, EventArgs e)
      {

      }

      private void bugReportToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new CrashReporter().ShowDialog();
      }

      private void quickSettingsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (new SettingsWindow(Globals.Settings).ShowDialog() == DialogResult.OK)
         {
            // Settings saved
         }
         else
         {
            // canceled
         }
      }

      private void crashReportToolStripMenuItem_Click(object sender, EventArgs e)
      {
         CrashManager.EnterCrashHandler(new());
      }

      private void loadDDSFilesTestToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var path = @"S:\SteamLibrary\steamapps\common\Europa Universalis IV\gfx\interface\mapmode_military_access.dds";
         var bmp = ImageReader.ReadDDSImage(path);

         bmp.Save(Globals.DownloadsFolder + "\\dds_test.png", ImageFormat.Png);
      }

      private void OpenProvinceFileButton_Click(object sender, EventArgs e)
      {
         foreach (var province in Selection.GetSelectedProvinces)
         {
            var path = province.GetHistoryFilePath();
            if (File.Exists(path))
            {
               Process.Start(new ProcessStartInfo
               {
                  FileName = path,
                  UseShellExecute = true
               });
            }
            else
            {
               MessageBox.Show($"File not found: {path}");
            }
         }
      }

      private void roughEditorToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new RoughEditorForm(Globals.Countries["TUR"]).ShowDialog();
      }

      private void button2_Click(object sender, EventArgs e)
      {
         new RoughEditorForm(Selection.SelectedCountry).ShowDialog();
      }

      private void AdvancedProvinceEditing_Click(object sender, EventArgs e)
      {
         if (Selection.Count != 1)
         {
            MessageBox.Show("Only one province can be selected for advanced editing", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }
         new RoughEditorForm(Selection.GetSelectedProvinces[0], false).ShowDialog();
      }
   }
}
