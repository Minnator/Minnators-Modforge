using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text.Json;
using Editor.Controls;
using Editor.DataClasses;
using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Forms;
using Editor.Forms.Loadingscreen;
using Editor.Helper;
using Editor.Savers;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

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
      private ExtendedComboBox _modifierComboBox = null!;
      private ExtendedComboBox _modifierTypeComboBox = null!;

      private ExtendedNumeric _taxNumeric = null!;
      private ExtendedNumeric _prodNumeric = null!;
      private ExtendedNumeric _manpNumeric = null!;

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

      private CollectionEditor _areaEditingGui = null!;
      private CollectionEditor _regionEditingGui = null!;

      #endregion

      public PannablePictureBox MapPictureBox = null!;
      public readonly DateControl DateControl = new(DateTime.MinValue, DateControlLayout.Horizontal);
      private LoadingScreen ls = null!;
      private EnterPathForm epf = null!;
      public bool SHUT_DOWN = false;

      public ModProject Project = new()
      {
         ModPath = Consts.MOD_PATH,
         VanillaPath = Consts.VANILLA_PATH
      };

      public MapWindow()
      {
         Project.ModPath = Globals.modPath;
         Project.VanillaPath = Globals.vanillaPath;

         Globals.State = State.Loading;
         Globals.MapWindow = this;

         //RunEnterPathForm();

         if (SHUT_DOWN)
         {
            Dispose();
            return;
         }
         RunLoadingScreen();
      }

      public void Initialize()
      {
         Hide();
         //pause gui updates
         SuspendLayout();
         InitGui();


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
         Globals.State = State.Running;
         Globals.LoadingStage++;
         MapModeComboBox.SelectedIndex = 11;
         ResumeLayout();
         Globals.LoadingStage++;

         StartPosition = FormStartPosition.CenterScreen;
         Show();
         MapPictureBox.FocusOn(new(3100, 600));

         AfterLoad();
      }

      private void AfterLoad()
      {
      }

      public void InitMapModes()
      {
         var sw = Stopwatch.StartNew();
         Globals.MapModeManager = new(MapPictureBox); // Initialize the MapModeManager
         Globals.MapModeManager.InitializeAllMapModes(); // Initialize all map modes
         MapModeComboBox.Items.Clear();
         MapModeComboBox.Items.AddRange([.. Globals.MapModeManager.GetMapModeNames()]);
         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Initializing MapModes", sw.ElapsedMilliseconds);
      }

      private void RunEnterPathForm()
      {
         epf = new();
         epf.ShowDialog();
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

         // TODO figure out how to make this work
         //toolStripContainer1.ContentPanel.MouseEnter += OnMouseEnter!;
         //toolStripContainer1.ContentPanel.MouseLeave += OnMouseLeave!;
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
      }

      private void InitializeProvinceCollectionEditGui()
      {
         _areaEditingGui = ControlFactory.GetCollectionEditor("Area", ItemTypes.Id, [.. Globals.Areas.Keys],
            s => // An Area is selected
            {
               List<string> provName = [];
               Globals.Selection.Clear();
               if (Globals.Areas.TryGetValue(s, out var area))
               {
                  for (var i = 0; i < area.Provinces.Length; i++)
                     provName.Add(area.Provinces[i].ToString());
                  Globals.Selection.AddRange(area.Provinces);
               }
               return provName;
            },
            (s, b) =>
            {
               if (!Globals.Areas.TryGetValue(s, out var area))
                  return [];

               Globals.HistoryManager.AddCommand(new CModifyExitingArea(s, Globals.Selection.SelectedProvinces, b));

               List<string> provName = [];
               for (var i = 0; i < area.Provinces.Length; i++)
                  provName.Add(area.Provinces[i].ToString());
               return provName;
            },
            s => // A new Area is created
            {
               Globals.HistoryManager.AddCommand(new CCreateNewArea(s, Globals.Selection.SelectedProvinces));

               List<string> provName = [];
               for (var i = 0; i < Globals.Selection.GetSelectedProvinces.Count; i++)
                  provName.Add(Globals.Selection.GetSelectedProvinces[i].ToString());
               return provName;
            },
            s => // An Area is deleted
            {
               if (!Globals.Areas.TryGetValue(s, out _))
                  return;

               Globals.HistoryManager.AddCommand(new CRemoveArea(s));
            },
            (s, idStr) => // A single province is removed from an area
            {
               if (!Globals.Areas.TryGetValue(s, out var area) || !int.TryParse(idStr, out var id))
                  return;

               if (!Globals.Provinces.TryGetValue(id, out var prov))
                  return;

               Globals.HistoryManager.AddCommand(new CModifyExitingArea(s, [id], false));
            }
         );

         ProvinceCollectionsMainLayoutPanel.Controls.Add(_areaEditingGui, 0, 0);


      }

      private void InitializeCountryEditGui()
      {

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
         _discoveredBy = ControlFactory.GetItemList(ItemTypes.String, [.. Globals.TechnologyGroups], "TechGroup");
         _discoveredBy.OnItemAdded += ProvinceEditingEvents.OnDiscoveredByAdded;
         _discoveredBy.OnItemRemoved += ProvinceEditingEvents.OnDiscoveredByRemoved;

         TradeCenterComboBox.SelectedIndexChanged += ProvinceEditingEvents.OnTradeCenterChanged;

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

         _prodNumeric = ControlFactory.GetExtendedNumeric();
         _prodNumeric.Minimum = 0;
         _prodNumeric.Maximum = 1000;
         _prodNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextBaseProductionChanged;
         _prodNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpBaseProductionChanged;
         _prodNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumProduction;
         _prodNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeProduction;
         _prodNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownBaseProductionChanged;
         _prodNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumProduction;
         _prodNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeProduction;
         DevelopmentLayoutPanel.Controls.Add(_prodNumeric, 1, 1);

         _manpNumeric = ControlFactory.GetExtendedNumeric();
         _manpNumeric.Minimum = 0;
         _manpNumeric.Maximum = 1000;
         _manpNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextBaseManpowerChanged;
         _manpNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpBaseManpowerChanged;
         _manpNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumManpower;
         _manpNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeManpower;
         _manpNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownBaseManpowerChanged;
         _manpNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumManpower;
         _manpNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeManpower;
         DevelopmentLayoutPanel.Controls.Add(_manpNumeric, 1, 2);

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

         AttirbuteCombobox.Items.AddRange([.. Enum.GetNames(typeof(ProvAttrGet))]);

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
      }

      #endregion



      private void InitializeModifierTab()
      {
         _modifierComboBox = ControlFactory.GetExtendedComboBox();
         _modifierComboBox.Items.AddRange([.. Globals.Modifiers.Keys]);
         // No data changed here as they are added via the "Add" button
         ModifiersLayoutPanel.Controls.Add(_modifierComboBox, 1, 1);

         _modifierTypeComboBox = ControlFactory.GetExtendedComboBox(["CountryModifier", "ProvinceModifier"]);
         _modifierTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
         ModifiersLayoutPanel.Controls.Add(_modifierTypeComboBox, 1, 0);

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
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.claims, out var result) && result is List<Tag> tags)
            _claims.AddItemsUnique([.. tags]);
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.permanent_claims, out result) && result is List<Tag> permanentTags)
            _permanentClaims.AddItemsUnique([.. permanentTags]);
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.cores, out result) && result is List<Tag> coreTags)
            _cores.AddItemsUnique([.. coreTags]);
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.buildings, out result) && result is List<string> buildings)
            _buildings.AddItemsUnique(buildings);
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.discovered_by, out result) && result is List<string> techGroups)
            _discoveredBy.AddItemsUnique(techGroups);
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.owner, out result) && result is Tag owner)
            OwnerTagBox.Text = owner;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.controller, out result) && result is Tag controller)
            ControllerTagBox.Text = controller;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.religion, out result) && result is string religion)
            _religionComboBox.Text = religion;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.culture, out result) && result is string culture)
            _cultureComboBox.Text = culture;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.capital, out result) && result is string capital)
            CapitalNameTextBox.Text = capital;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.is_city, out result) && result is bool isCity)
            IsCityCheckBox.Checked = isCity;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.hre, out result) && result is bool isHre)
            IsHreCheckBox.Checked = isHre;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.seat_in_parliament, out result) && result is bool isSeatInParliament)
            IsParlimentSeatCheckbox.Checked = isSeatInParliament;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.revolt, out result) && result is bool hasRevolt)
            HasRevoltCheckBox.Checked = hasRevolt;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.base_tax, out result) && result is int baseTax)
            _taxNumeric.Value = baseTax;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.base_production, out result) && result is int baseProduction)
            _prodNumeric.Value = baseProduction;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.base_manpower, out result) && result is int baseManpower)
            _manpNumeric.Value = baseManpower;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.local_autonomy, out result) && result is float localAutonomy)
            _autonomyNumeric.Value = (int)localAutonomy;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.devastation, out result) && result is float devastation)
            _devastationNumeric.Value = (int)devastation;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.prosperity, out result) && result is float prosperity)
            _prosperityNumeric.Value = (int)prosperity;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.trade_good, out result) && result is string tradeGood)
            TradeGoodsComboBox.Text = tradeGood;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.center_of_trade, out result) && result is int centerOfTrade)
            TradeCenterComboBox.Text = centerOfTrade.ToString();
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.extra_cost, out result) && result is int extraCost)
            _extraCostNumeric.Value = extraCost;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.tribal_owner, out result) && result is Tag tribalOwner)
            _tribalOwner.Text = tribalOwner;
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.native_size, out result) && result is int nativeSize)
            _nativesSizeTextBox.Text = nativeSize.ToString();
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.native_ferocity, out result) && result is float nativeFerocity)
            _nativeFerocityTextBox.Text = nativeFerocity.ToString(CultureInfo.InvariantCulture);
         if (Globals.Selection.GetSharedAttribute(ProvAttrGet.native_hostileness, out result) && result is float nativeHostileness)
            _nativeHostilityTextBox.Text = nativeHostileness.ToString(CultureInfo.InvariantCulture);
         // TODO The Gui needs to be able to represent several trade company investments
         //if (Globals.Selection.GetSharedAttribute(ProvAttrGet.trade_company_investment, out result) && result is string tradeCompanyInvestments)
         //   _tradeCompanyInvestments.Text = tradeCompanyInvestments;
         if (Globals.Selection.GetSelectedProvinces.Count == 1)
            AddAllModifiersToListView(Globals.Selection.GetSelectedProvinces[0]);
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
         _prodNumeric.Value = province.BaseProduction;
         _manpNumeric.Value = province.BaseManpower;
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
         _prodNumeric.Value = 1;
         _manpNumeric.Value = 1;
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
         _modifierComboBox.Text = string.Empty;
         ModifiersListView.Items.Clear();
         DurationTextBox.Text = string.Empty;
         _tradeCompanyInvestments.Text = string.Empty;
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

      private void OnSavingAllEnter(object? sender, EventArgs e)
      {
         _savingButtonsToolTip.SetToolTip(SaveAllProvincesButton, $"Save all provinces ({Globals.LandProvinceIds.Length})");
      }

      private void OnSavingModifiedEnter(object? sender, EventArgs e)
      {
         _savingButtonsToolTip.SetToolTip(SaveAllModifiedButton, $"Save modified provinces ({EditingManager.GetModifiedProvinces().Count})");
      }

      private void OnSavingSelectionEnter(object? sender, EventArgs e)
      {
         _savingButtonsToolTip.SetToolTip(SaveCurrentSelectionButton, $"Save selection ({Globals.Selection.Count})");
      }

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

      private void OnMouseEnter(object sender, EventArgs e)
      {
         Cursor = Cursors.Default;
      }

      private void OnMouseLeave(object sender, EventArgs e)
      {
         Cursor = Globals.Selection.State switch
         {
            SelectionState.MagicWand => Cursors.Cross,
            _ => Cursors.Default
         };
      }

      private void debugToolStripMenuItem_Click(object sender, EventArgs e)
      {
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
      }

      private void telescopeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         //DebugMaps.TelescopeImageBenchmark();
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
         //DebugMaps.GridMap();
      }

      private void DateSelector_SelectedIndexChanged(object sender, EventArgs e)
      {

      }

      private void MapWindow_Load(object sender, EventArgs e)
      {
         if (SHUT_DOWN)
         {
            Close();
         }
      }

      private void searchToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FormHelper.OpenOrBringToFront(Globals.SearchForm);
      }

      private void bestPointsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         //DebugMaps.TestCenterPoints();
      }

      private void provDiffToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var res = EditingManager.GetModifiedProvinces();

         Debug.WriteLine($"Modified Provinces: {res.Count}");
         foreach (var province in res)
         {
            Debug.WriteLine(province.Id);
         }
      }

      private void ProvincePreviewMode_SelectedIndexChanged(object sender, EventArgs e)
      {
         Globals.ProvinceEditingStatus = (ProvinceEditingStatus)ProvincePreviewMode.SelectedIndex;
         // Close the menu strip// Close the menu when an item is selected
         filesToolStripMenuItem.DropDown.Close();
      }

      private void MagicWandToolButton_Click(object sender, EventArgs e)
      {
         if (Globals.Selection.State == SelectionState.Single)
         {
            Globals.Selection.State = SelectionState.MagicWand;
            // change the cursor to the magic wand
            Cursor = Cursors.Cross;
         }
         else if (Globals.Selection.State == SelectionState.MagicWand)
         {
            Globals.Selection.State = SelectionState.Single;
            // change the cursor to the normal cursor
            Cursor = Cursors.Default;
         }

      }

      private void saveAllProvincesToolStripMenuItem_Click(object sender, EventArgs e)
      {
         ProvinceSaver.SaveAllLandProvinces();
      }

      private void save1ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Globals.Provinces[269].SaveToHistoryFile();
      }

      private void saveEuropeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         foreach (var id in Globals.Continents["europe"].Provinces)
         {
            Globals.Provinces[id].SaveToHistoryFile();
         }
      }

      private void saveSelectionToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var selectionDrawer = new SelectionDrawerForm();
         selectionDrawer.Show();
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

      private void AddModifierButton_Click(object sender, EventArgs e)
      {
         var error = string.Empty;
         if (!int.TryParse(DurationTextBox.Text, out var duration))
            error += "Duration must be a number\n";
         if (_modifierComboBox.Text == string.Empty)
            error += "Modifier must be selected\n";
         if (_modifierTypeComboBox.Text == string.Empty)
            error += "ModifierType must be selected\n";

         if (error != string.Empty)
         {
            MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }

         var mod = new ApplicableModifier(_modifierComboBox.Text, duration);
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

         if (!Globals.Modifiers.TryGetValue(modifierName, out _))
            return;

         ProvinceEditingEvents.OnModifierRemoved(new ApplicableModifier(modifierName, duration), type);
         ModifiersListView.Items.Remove(item);
      }

      private void jsonToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Province province = new(123);
         province.Owner = "ENG";
         province.Controller = "ENG";
         province.Religion = "catholic";

         var jsonSettings = new JsonSerializerOptions { WriteIndented = true };
         string jsonString = JsonSerializer.Serialize(province, jsonSettings);


         IO.WriteToFile(Path.Combine(Globals.DownloadsFolder, "jsonTest.json"), jsonString, false);
      }
      private void yoloToolStripMenuItem_Click(object sender, EventArgs e)
      {

         SettingsManager.CurrentSettings.Setting1 = "Custom Value";
         SettingsManager.SaveSettings();
         SettingsManager.LoadSettings();
      }


   }
}
