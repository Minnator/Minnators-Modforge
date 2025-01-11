using System.Diagnostics;
using System.Drawing.Imaging;
using System.Media;
using System.Runtime;
using System.Text;
using Editor.Controls;
using Editor.Controls.NewControls;
using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Misc;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Forms.Feature;
using Editor.Forms.Feature.Crash_Reporter;
using Editor.Forms.Feature.SavingClasses;
using Editor.Forms.PopUps;
using Editor.Helper;
using Editor.Loading;
using Editor.NameGenerator;
using Editor.Saving;
using Editor.src.Forms.GetUserInput;
using Editor.src.Forms.PopUps;
using Editor.Testing;
using static Editor.Helper.ProvinceEnumHelper;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MapLoading = Editor.Loading.Enhanced.MapLoading;
using MethodInvoker = System.Windows.Forms.MethodInvoker;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor.Forms
{
   public partial class MapWindow : Form
   {
      #region CustomEditingControls

      private ItemList _claims = null!;
      private ItemList _permanentClaims = null!;
      private ItemList _cores = null!;
      private ItemList _buildings = null!;
      private ItemList _discoveredBy = null!;

      private BindablePropertyComboBox<Province, Religion, string> _religionComboBox = null!;
      private PropertyComboBox<Province, int> _tradeCenterComboBox = null!;
      private BindablePropertyComboBox<Province, TradeGood, string> _tradeGoodsComboBox = null!;
      private BindablePropertyComboBox<Province, Culture, string> _cultureComboBox = null!;
      public ExtendedComboBox ModifierComboBox = null!;
      private ExtendedComboBox _modifierTypeComboBox = null!;

      private PropertyNumeric<Province, int> _taxNumeric = null!;
      private PropertyNumeric<Province, int> _prdNumeric = null!;
      private PropertyNumeric<Province, int> _mnpNumeric = null!;
      private PropertyNumeric<Province, float> _autonomyNumeric = null!;
      private PropertyNumeric<Province, float> _devastationNumeric = null!;
      private PropertyNumeric<Province, float> _prosperityNumeric = null!;
      private PropertyNumeric<Province, int> _extraCostNumeric = null!;

      private PropertyCheckBox<Province> _isCityCheckBox = null!;
      private PropertyCheckBox<Province> _isHreCheckBox = null!;
      private PropertyCheckBox<Province> _isParliamentSeatCheckbox = null!;
      private PropertyCheckBox<Province> _hasRevoltCheckBox = null!;

      private BindablePropertyComboBox<Province, Country, Tag> _tribalOwner = null!;
      private ExtendedComboBox _tradeCompanyInvestments = null!;
      private BindableFakePropertyComboBox<Province, Terrain, string> _terrainComboBox = null!;

      private PropertyNumeric<Province, int> _nativesSizeTextBox = null!;
      private PropertyNumeric<Province, float> _nativeFerocityTextBox = null!;
      private PropertyNumeric<Province, int> _nativeHostilityTextBox = null!;

      private PropertyTextBox<Province> _localisationTextBox = null!;
      private PropertyTextBox<Province> _provAdjTextBox = null!;
      private PropertyTextBox<Province> _capitalNameTextBox = null!;

      private ToolTip _savingButtonsToolTip = null!;

      private CollectionEditor2<Area, Province> _areaEditingGui = null!;
      private CollectionEditor2<Region, Area> _regionEditingGui = null!;
      private CollectionEditor2<SuperRegion, Region> _superRegionEditingGui = null!;
      private CollectionEditor2<TradeCompany, Province> _tradeCompanyEditingGui = null!;
      private CollectionEditor2<Country, Province> _countryEditingGui = null!;
      private CollectionEditor2<TradeNode, Province> _tradeNodeEditingGui = null!;
      private CollectionEditor2<ProvinceGroup, Province> _provinceGroupsEditingGui = null!;
      private CollectionEditor2<ColonialRegion, Province> _colonialRegionEditingGui = null!;

      private BindablePropertyComboBox<Province, Country, Tag> _ownerTagBox = null!;
      private BindablePropertyComboBox<Province, Country, Tag> _controllerTagBox = null!;

      private PropertyLabel<Province> LocalisationLabel = null!;
      private PropertyLabel<Province> ProvAdjLabel = null!;

      public Screen? StartScreen = null;

      #endregion

      public readonly DateControl DateControl = new(Date.MinValue, DateControlLayout.Horizontal);
      private LoadingScreen.LoadingScreen _ls = null!;

      public MapWindow()
      {
         Globals.State = State.Loading;
         Globals.MapWindow = this;
         Globals.Random = new(Globals.Settings.Misc.RandomSeed);

         // Load the game data
         RunLoadingScreen();
         ThreadHelper.StartBWHeapThread();

         Eu4Cursors.SetEu4CursorIfEnabled(Eu4CursorTypes.Normal, Cursors.Default, this);

         if (StartScreen != null)
         {
            var screen = Globals.MapWindow.StartScreen;
            if (screen != null)
               Location = new(screen.Bounds.X + (screen.Bounds.Width - Width) / 2, screen.Bounds.Y + (screen.Bounds.Height - Height) / 2);
         }

         //TODO: solve all the issues regarding this
         //DiscordActivityManager.ActivateActivity();

#if DEBUG
         debugToolStripMenuItem.Enabled = true;
         debugToolStripMenuItem.Visible = true;
#endif
      }

      #region Initialize Application and Loadingscreen

      public void Initialize()
      {
         Hide();
         //pause gui updates
         SuspendLayout();

         InitGui();
         Text = $"{Text} | {Globals.DescriptorData.Name}";

         // MUST BE LAST in the loading sequence
         InitMapModes();
         HistoryManager.UndoDepthChanged += UpdateUndoDepth!;
         HistoryManager.RedoDepthChanged += UpdateRedoDepth!;

         //Needs to be after loading the game data to populate the gui with it
         InitializeEditGui();
         //resume gui updates
         // Enable the Application
         Globals.LoadingLog.Close();
         ResourceUsageHelper.Initialize(this);

         // ALL LOADING COMPLETE - Set the application to running

         DateControl.Date = new(1444, 11, 11);
         CountryLoading.AssignProvinces();
         Globals.State = State.Running;
         MapModeManager.SetCurrentMapMode(MapModeType.Country);
         ResumeLayout();

         StartPosition = FormStartPosition.CenterScreen;
         Globals.ZoomControl.FocusOn(new(3100, 600), 1f);
         Show();

         // Activate this window
         Activate();
         Globals.ZoomControl.Invalidate();
      }


      private void RunLoadingScreen()
      {
         _ls = new();
         Eu4Cursors.SetEu4CursorIfEnabled(Eu4CursorTypes.Loading, Cursors.AppStarting, _ls);
         _ls.ShowDialog();
      }

      #endregion
      #region MapWindow GUI Init
      private void InitGui()
      {
         InitializeComponent();

         HistoryManager.UndoEvent += (sender, args) => UpdateGui();
         HistoryManager.RedoEvent += (sender, args) => UpdateGui();

         Globals.ZoomControl = new(new(Globals.MapWidth, Globals.MapHeight))
         {
            BorderWidth = Globals.Settings.Rendering.MapBorderWidth,
            Border = Globals.Settings.Rendering.ShowMapBorder,
            BorderColor = Globals.Settings.Rendering.MapBorderColor,
            MinVisiblePixels = Globals.Settings.Rendering.MinVisiblePixels,
            Dock = DockStyle.Fill,
            Margin = new(0),
         };

         MapLayoutPanel.Controls.Add(Globals.ZoomControl, 0, 0);
         Selection.Initialize();
         GuiDrawing.Initialize();

         TopStripLayoutPanel.Controls.Add(DateControl, 4, 0);
         DateControl.OnDateChanged += OnDateChanged;

         SelectionTypeBox.Items.AddRange([.. Enum.GetNames<SelectionType>()]);
         SelectionTypeBox.SelectedIndex = 0;

         BookMarkComboBox.Items.AddRange([.. Globals.Bookmarks]);
         BookMarkComboBox.SelectedIndexChanged += OnBookMarkChanged;


         // Initalize Settings Events and Listeners
         SettingsHelper.InitializeEvent();
      }

      private void OnBookMarkChanged(object? sender, EventArgs e)
      {
         if (BookMarkComboBox.SelectedIndex == -1)
            return;

         var bookmark = Globals.Bookmarks[BookMarkComboBox.SelectedIndex];
         DateControl.Date = bookmark.Date;
      }
      #endregion
      #region GUI Updates


      private void UpdateGui()
      {
         UpdateTabIfSelected(DataTabPanel.SelectedIndex);
      }

      private void UpdateTabIfSelected(int i)
      {
         if (DataTabPanel.SelectedIndex != i)
            return;
         switch (i)
         {
            case 0:
               UpdateProvinceTab();
               break;
            case 1:
               UpdateCountryTab();
               break;
            case 2:
               //UpdateProvinceCollectionTab();
               break;
         }
      }

      private void UpdateProvinceTab()
      {
         if (DataTabPanel.SelectedIndex != 0)
            return;

         if (Selection.Count == 0)
         {
            DataTabPanel.TabPages[0].SuspendLayout();
            Globals.State = State.Loading;
            ClearProvinceGui();
            Globals.State = State.Running;
            DataTabPanel.TabPages[0].Enabled = false;
            DataTabPanel.TabPages[0].ResumeLayout();
         }
         else
         {
            DataTabPanel.TabPages[0].Enabled = true;
            //LoadSelectedProvincesToGui();
            LoadGuiEvents.ReloadProvinces();
         }
      }

      private void UpdateCountryTab()
      {
         if (DataTabPanel.SelectedIndex != 1)
            return;

         if (Selection.SelectedCountry == Country.Empty)
         {
            DataTabPanel.TabPages[1].SuspendLayout();
            Globals.State = State.Loading;
            ClearCountryGui();
            Globals.State = State.Running;
            DataTabPanel.TabPages[1].Enabled = false;
            DataTabPanel.TabPages[1].ResumeLayout();
         }
         else
         {
            DataTabPanel.TabPages[1].Enabled = true;
            //LoadCountryToGui(Selection.SelectedCountry);
            LoadGuiEvents.ReloadCountry();
         }
      }

      #endregion
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
         MapModeManager.MapModeChanged += (sender, mode) =>
         {
            MapModeTimesInfo.Text = $"MapMode times [ms]: last: {MapModeManager.LasMapModeTime} | Min: {MapModeManager.MinMapModeTime} | Max: {MapModeManager.MaxMapModeTime} | Avg: {MapModeManager.AverageMapModeTime}";
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
         var button01 = ControlFactory.GetMapModeButton('q', 0);
         button01.Margin = new(0, 0, 3, 0);
         var button21 = ControlFactory.GetMapModeButton('w', 1);
         var button31 = ControlFactory.GetMapModeButton('e', 2);
         var button41 = ControlFactory.GetMapModeButton('r', 3);
         var button5 = ControlFactory.GetMapModeButton('t', 4);
         var button6 = ControlFactory.GetMapModeButton('y', 5);
         var button7 = ControlFactory.GetMapModeButton('u', 6);
         var button8 = ControlFactory.GetMapModeButton('i', 7);
         var button9 = ControlFactory.GetMapModeButton('o', 8);
         var button10 = ControlFactory.GetMapModeButton('p', 9);
         button10.Margin = new(3, 0, 0, 0);
         MMButtonsTLPanel.Controls.Add(button01, 0, 0);
         MMButtonsTLPanel.Controls.Add(button21, 1, 0);
         MMButtonsTLPanel.Controls.Add(button31, 2, 0);
         MMButtonsTLPanel.Controls.Add(button41, 3, 0);
         MMButtonsTLPanel.Controls.Add(button5, 4, 0);
         MMButtonsTLPanel.Controls.Add(button6, 5, 0);
         MMButtonsTLPanel.Controls.Add(button7, 6, 0);
         MMButtonsTLPanel.Controls.Add(button8, 7, 0);
         MMButtonsTLPanel.Controls.Add(button9, 8, 0);
         MMButtonsTLPanel.Controls.Add(button10, 9, 0);

      }

      private void InitializeProvinceCollectionEditGui()
      {
         _countryEditingGui = new(ItemTypes.Id, SaveableType.Country, SaveableType.Province, MapModeType.Country);
         Country.ItemsModified += _countryEditingGui.OnCorrespondingDataChange;
         _countryEditingGui._extendedComboBox.DataSource = new BindingSource
         {
            DataSource = Globals.Countries
         };
         _countryEditingGui._extendedComboBox.DisplayMember = "Key";
         _countryEditingGui._extendedComboBox.ValueMember = "Value";

         _areaEditingGui = new(ItemTypes.Id, SaveableType.Area, SaveableType.Province, MapModeType.Area);
         Area.ItemsModified += _areaEditingGui.OnCorrespondingDataChange;

         _regionEditingGui = new(ItemTypes.String, SaveableType.Region, SaveableType.Area, MapModeType.Regions);
         DataClasses.GameDataClasses.Region.ItemsModified += _regionEditingGui.OnCorrespondingDataChange;

         _superRegionEditingGui = new(ItemTypes.String, SaveableType.SuperRegion, SaveableType.Region, MapModeType.SuperRegion);
         SuperRegion.ItemsModified += _superRegionEditingGui.OnCorrespondingDataChange;

         _tradeCompanyEditingGui = new(ItemTypes.Id, SaveableType.TradeCompany, SaveableType.Province, MapModeType.TradeCompany);
         TradeCompany.ItemsModified += _tradeCompanyEditingGui.OnCorrespondingDataChange;

         _tradeNodeEditingGui = new(ItemTypes.Id, SaveableType.TradeNode, SaveableType.Province, MapModeType.TradeNode);
         TradeNode.ItemsModified += _tradeNodeEditingGui.OnCorrespondingDataChange;

         _provinceGroupsEditingGui = new(ItemTypes.Id, SaveableType.ProvinceGroup, SaveableType.Province, MapModeType.ProvinceGroup);
         ProvinceGroup.ItemsModified += _provinceGroupsEditingGui.OnCorrespondingDataChange;

         _colonialRegionEditingGui = new(ItemTypes.Id, SaveableType.ColonialRegion, SaveableType.Province, MapModeType.ColonialRegions);
         ColonialRegion.ItemsModified += _colonialRegionEditingGui.OnCorrespondingDataChange;

         ProvinceCollectionsLayoutPanel.Controls.Add(_countryEditingGui, 0, 0);
         ProvinceCollectionsLayoutPanel.Controls.Add(_regionEditingGui, 0, 2);
         ProvinceCollectionsLayoutPanel.Controls.Add(_areaEditingGui, 0, 1);
         ProvinceCollectionsLayoutPanel.Controls.Add(_superRegionEditingGui, 0, 3);
         ProvinceCollectionsLayoutPanel.Controls.Add(_tradeCompanyEditingGui, 0, 5);
         ProvinceCollectionsLayoutPanel.Controls.Add(_tradeNodeEditingGui, 0, 4);
         ProvinceCollectionsLayoutPanel.Controls.Add(_provinceGroupsEditingGui, 0, 6);
         ProvinceCollectionsLayoutPanel.Controls.Add(_colonialRegionEditingGui, 0, 7);

         FocusSelectionCheckBox.Checked = Globals.Settings.Gui.JumpToSelectedProvinceCollection;
      }

      private void InitializeProvinceEditGui()
      {
         DataTabPanel.TabPages[0].Enabled = false;
         DataTabPanel.TabPages[1].Enabled = false;
         Selection.OnProvinceSelectionChange += (sender, i) => UpdateTabIfSelected(0);
         Selection.OnCountrySelectionChange += (sender, i) => UpdateTabIfSelected(1);

         DataTabPanel.SelectedIndexChanged += (sender, args) => UpdateGui();


         // CustomTextBox
         _localisationTextBox = ControlFactory.GetPropertyTextBox(typeof(Province).GetProperty(nameof(Province.TitleLocalisation)));
         LocTableLayoutPanel.Controls.Add(_localisationTextBox, 1, 1);

         _provAdjTextBox = ControlFactory.GetPropertyTextBox(typeof(Province).GetProperty(nameof(Province.AdjectiveLocalisation)));
         LocTableLayoutPanel.Controls.Add(_provAdjTextBox, 1, 3);

         LocalisationLabel = ControlFactory.GetPropertyLabel(typeof(Province).GetProperty(nameof(Province.TitleKey)));
         LocTableLayoutPanel.Controls.Add(LocalisationLabel, 1, 0);

         ProvAdjLabel = ControlFactory.GetPropertyLabel(typeof(Province).GetProperty(nameof(Province.AdjectiveKey)));
         LocTableLayoutPanel.Controls.Add(ProvAdjLabel, 1, 2);

         // Tooltips for saving Buttons
         _savingButtonsToolTip = new();
         _savingButtonsToolTip.AutoPopDelay = 10000;
         _savingButtonsToolTip.InitialDelay = 100;
         _savingButtonsToolTip.ShowAlways = true;

         SaveAllProvincesButton.MouseEnter += OnSavingAllEnter;
         SaveCurrentSelectionButton.MouseEnter += OnSavingSelectionEnter;
         OpenProvinceFile.MouseEnter += OnOpenProvinceFileEnter;

         _ownerTagBox = ControlFactory.GetTagComboBox(typeof(Province).GetProperty(nameof(Province.Owner))!, Globals.Countries);
         MisProvinceData.Controls.Add(_ownerTagBox, 1, 0);
         _controllerTagBox = ControlFactory.GetTagComboBox(typeof(Province).GetProperty(nameof(Province.Controller))!, Globals.Countries);
         MisProvinceData.Controls.Add(_controllerTagBox, 1, 1);

         _cores = ControlFactory.GetItemList(nameof(Province.Cores), ItemTypes.Tag, [], "Cores");
         _cores.OnItemAdded += ProvinceEditingEvents.OnItemAddedModified;
         _cores.OnItemRemoved += ProvinceEditingEvents.OnItemRemoveModified;
         _claims = ControlFactory.GetItemList(nameof(Province.Claims), ItemTypes.Tag, [], "Regular");
         _claims.OnItemAdded += ProvinceEditingEvents.OnItemAddedModified;
         _claims.OnItemRemoved += ProvinceEditingEvents.OnItemRemoveModified;
         _permanentClaims = ControlFactory.GetItemList(nameof(Province.PermanentClaims), ItemTypes.Tag, [], "Permanent");
         _permanentClaims.OnItemAdded += ProvinceEditingEvents.OnItemAddedModified;
         _permanentClaims.OnItemRemoved += ProvinceEditingEvents.OnItemRemoveModified;

         _buildings = ControlFactory.GetItemListObjects(nameof(Province.Buildings), ItemTypes.String, [.. Globals.Buildings], "Building");
         _buildings.OnItemAdded += ProvinceEditingEvents.OnItemAddedModified;
         _buildings.OnItemRemoved += ProvinceEditingEvents.OnItemRemoveModified;
         _discoveredBy = ControlFactory.GetItemList(nameof(Province.DiscoveredBy), ItemTypes.String, [.. Globals.TechnologyGroups.Keys], "TechGroup");
         _discoveredBy.OnItemAdded += ProvinceEditingEvents.OnItemAddedModified;
         _discoveredBy.OnItemRemoved += ProvinceEditingEvents.OnItemRemoveModified;

         CoresAndClaimLayoutPanel.Controls.Add(_permanentClaims, 0, 0);
         CoresAndClaimLayoutPanel.Controls.Add(_claims, 1, 0);
         CoresGroupBox.Controls.Add(_cores);
         _cores.Location = new(0, 18);
         BuildingsGroupBox.Controls.Add(_buildings);
         _buildings.Location = new(0, 18);
         DiscoveredByGroupBox.Controls.Add(_discoveredBy);
         _discoveredBy.Location = new(0, 18);

         _tradeCenterComboBox = ControlFactory.SimpleComboBoxProvince<int>(typeof(Province).GetProperty(nameof(Province.CenterOfTrade))!);
         _tradeCenterComboBox.Items.AddRange(["0", "1", "2", "3"]);
         _tradeCenterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
         TradePanel.Controls.Add(_tradeCenterComboBox, 1, 0);

         _tradeGoodsComboBox = ControlFactory.GetBindablePropertyComboBox(typeof(Province).GetProperty(nameof(Province.TradeGood))!, Globals.TradeGoods);
         TradePanel.Controls.Add(_tradeGoodsComboBox, 1, 1);

         _cultureComboBox = ControlFactory.GetBindablePropertyComboBox(typeof(Province).GetProperty(nameof(Province.Culture))!, Globals.Cultures);
         MisProvinceData.Controls.Add(_cultureComboBox, 1, 3);

         _religionComboBox = ControlFactory.GetBindablePropertyComboBox(typeof(Province).GetProperty(nameof(Province.Religion))!, Globals.Religions);
         MisProvinceData.Controls.Add(_religionComboBox, 1, 2);

         // TODO GEDANKEN MACHEN Wie man das hier am besten macht
         _terrainComboBox = ControlFactory.GetBindableFakePropertyComboBox(typeof(Province).GetProperty(nameof(Province.Terrain))!, Globals.Terrains);
         _terrainComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
         MisProvinceData.Controls.Add(_terrainComboBox, 1, 5);


         _taxNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.BaseTax)), 0);
         _taxNumeric.Minimum = 0;
         _taxNumeric.Maximum = 1000;
         MisProvinceData.Controls.Add(_taxNumeric, 3, 0);

         _prdNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.BaseProduction)), 0);
         _prdNumeric.Minimum = 0;
         _prdNumeric.Maximum = 1000;
         MisProvinceData.Controls.Add(_prdNumeric, 3, 1);

         _mnpNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.BaseManpower)), 0);
         _mnpNumeric.Minimum = 0;
         _mnpNumeric.Maximum = 1000;
         MisProvinceData.Controls.Add(_mnpNumeric, 3, 2);

         _autonomyNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.LocalAutonomy)), 0.0f);
         _autonomyNumeric.Minimum = 0;
         _autonomyNumeric.Maximum = 100;
         FloatLayoutPanel.Controls.Add(_autonomyNumeric, 1, 0);

         _devastationNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.Devastation)), 0.0f);
         _devastationNumeric.Minimum = 0;
         _devastationNumeric.Maximum = 100;
         FloatLayoutPanel.Controls.Add(_devastationNumeric, 1, 1);

         _prosperityNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.Prosperity)), 0.0f);
         _prosperityNumeric.Minimum = 0;
         _prosperityNumeric.Maximum = 100;
         FloatLayoutPanel.Controls.Add(_prosperityNumeric, 1, 2);

         _extraCostNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.ExtraCost)), 0);
         _extraCostNumeric.Minimum = 0;
         _extraCostNumeric.Maximum = 1000;
         TradePanel.Controls.Add(_extraCostNumeric, 1, 2);

         _capitalNameTextBox = ControlFactory.GetPropertyTextBox(typeof(Province).GetProperty(nameof(Province.Capital)));
         MisProvinceData.Controls.Add(_capitalNameTextBox, 1, 4);

         _isCityCheckBox = ControlFactory.GetExtendedCheckBoxProvince(typeof(Province).GetProperty(nameof(Province.IsCity))!);
         _isCityCheckBox.CheckedChanged += ProvinceEditingEvents.OnExtendedCheckBoxCheckedChanged;
         MisProvinceData.Controls.Add(_isCityCheckBox, 3, 3);

         _isHreCheckBox = ControlFactory.GetExtendedCheckBoxProvince(typeof(Province).GetProperty(nameof(Province.IsHre))!);
         _isHreCheckBox.CheckedChanged += ProvinceEditingEvents.OnExtendedCheckBoxCheckedChanged;
         MisProvinceData.Controls.Add(_isHreCheckBox, 3, 4);

         _isParliamentSeatCheckbox = ControlFactory.GetExtendedCheckBoxProvince(typeof(Province).GetProperty(nameof(Province.IsSeatInParliament))!);
         _isParliamentSeatCheckbox.CheckedChanged += ProvinceEditingEvents.OnExtendedCheckBoxCheckedChanged;
         MisProvinceData.Controls.Add(_isParliamentSeatCheckbox, 3, 5);

         _hasRevoltCheckBox = ControlFactory.GetExtendedCheckBoxProvince(typeof(Province).GetProperty(nameof(Province.HasRevolt))!);
         _hasRevoltCheckBox.CheckedChanged += ProvinceEditingEvents.OnExtendedCheckBoxCheckedChanged;
         MisProvinceData.Controls.Add(_hasRevoltCheckBox, 3, 6);

         MWAttirbuteCombobox.Items.AddRange([.. Enum.GetNames<ProvAttrGet>()]);

         // NATIVES TAB
         _tribalOwner = ControlFactory.GetTagComboBox(typeof(Province).GetProperty(nameof(Province.TribalOwner))!, Globals.Countries);
         NativesLayoutPanel.Controls.Add(_tribalOwner, 1, 0);

         _nativesSizeTextBox = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.NativeSize)), 0, 100);
         _nativesSizeTextBox.Maximum = 10000;
         NativesLayoutPanel.Controls.Add(_nativesSizeTextBox, 1, 1);

         _nativeFerocityTextBox = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.NativeFerocity)), 0f, 100);
         _nativeFerocityTextBox.Maximum = 10000;
         NativesLayoutPanel.Controls.Add(_nativeFerocityTextBox, 1, 3);

         _nativeHostilityTextBox = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.NativeHostileness)), 0, 100);
         _nativeHostilityTextBox.Maximum = 10000;
         NativesLayoutPanel.Controls.Add(_nativeHostilityTextBox, 1, 2);

         // TRADE_COMPANIES TAB
         _tradeCompanyInvestments = ControlFactory.GetExtendedComboBox(nameof(Province.TradeCompanyInvestments));
         _tradeCompanyInvestments.Items.AddRange([.. Globals.TradeCompanyInvestments.Keys]);
         _tradeCompanyInvestments.OnDataChanged += ProvinceEditingEvents.OnExtendedComboBoxSelectedIndexChanged;
         TradeCompaniesLayoutPanel.Controls.Add(_tradeCompanyInvestments, 1, 0);

         // MODIFIERS TAB
         InitializeModifierTab();


         ProvinceCustomToolStripLayoutPanel.Paint += TableLayoutBorder_Paint;
      }


      private void InitializeModifierTab()
      {
         ModifierComboBox = ControlFactory.GetExtendedComboBox(nameof(Province.ProvinceModifiers));
         ModifierComboBox.Items.AddRange([.. Globals.EventModifiers.Keys]);
         // No data changed here as they are added via the "Add" button
         ModifiersLayoutPanel.Controls.Add(ModifierComboBox, 1, 1);

         _modifierTypeComboBox = ControlFactory.GetExtendedComboBox("", ["CountryModifier", "ProvinceModifier"]);
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


      #endregion
      #region Province Gui
      public void ClearProvinceGui()
      {
         ExtendedComboBox.AllowEvents = false;
         _ownerTagBox.SetDefault();
         _controllerTagBox.SetDefault();
         _religionComboBox.SetDefault();
         _cultureComboBox.SetDefault();
         _capitalNameTextBox.SetDefault();
         _isCityCheckBox.Checked = false;
         _isHreCheckBox.Checked = false;
         _isParliamentSeatCheckbox.Checked = false;
         _hasRevoltCheckBox.Checked = false;
         _taxNumeric.SetDefault();
         _prdNumeric.SetDefault();
         _mnpNumeric.SetDefault();
         _claims.Clear();
         _permanentClaims.Clear();
         _cores.Clear();
         _buildings.Clear();
         _discoveredBy.Clear();
         _autonomyNumeric.SetDefault();
         _devastationNumeric.SetDefault();
         _prosperityNumeric.SetDefault();
         _tradeGoodsComboBox.SetDefault();
         _tradeCenterComboBox.SetDefault();
         _extraCostNumeric.SetDefault();
         _tribalOwner.Clear();
         _nativesSizeTextBox.SetDefault();
         _nativeFerocityTextBox.SetDefault();
         _nativeHostilityTextBox.SetDefault();
         ModifierComboBox.Text = string.Empty;
         ModifiersListView.Items.Clear();
         DurationTextBox.Text = string.Empty;
         _tradeCompanyInvestments.Text = string.Empty;
         _localisationTextBox.SetDefault();
         LocalisationLabel.SetDefault();
         _provAdjTextBox.SetDefault();
         ProvAdjLabel.SetDefault();
         _terrainComboBox.SelectedItem = string.Empty;
         ExtendedComboBox.AllowEvents = true;
      }
      #endregion
      #region MapMode Init / Update

      public void UpdateMapModeButtons(bool buttonInv = true)
      {
         foreach (var mapModeButton in MMButtonsTLPanel.Controls)
         {
            if (mapModeButton is not MapModeButton button)
               continue;

            button.UpdateMapMode(buttonInv);
         }
      }

      private void InitMapModes()
      {
         MapModeComboBox.Items.Clear();
         MapModeComboBox.Items.AddRange([.. Enum.GetNames<MapModeType>()]);
      }
      private void MapModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         MapModeManager.SetCurrentMapMode(Enum.Parse<MapModeType>(MapModeComboBox.SelectedItem?.ToString() ?? string.Empty));
      }

      #endregion
      #region Resource Updater MethodInvokation
      public void SetSelectedProvinceSum(int sum)
      {
         SelectedProvinceSum.Text = $"ProvSum: [{sum}]";
      }


      public void UpdateMemoryUsage(float memoryUsage)
      {
         if (IsDisposed || Disposing) return;
         if (InvokeRequired)
            Invoke(new MethodInvoker(delegate
            {
               RamUsageStrip.Text = memoryUsage > 1024 ? $"RAM: [{Math.Round(memoryUsage / 1024, 2):F2} GB]" : $"RAM: [{Math.Round(memoryUsage)} MB]";
            }));
      }

      public void UpdateCpuUsage(float cpuUsage)
      {
         if (IsDisposed || Disposing) return;
         if (InvokeRequired) Invoke(new MethodInvoker(delegate { CpuUsageStrip.Text = $"CPU: [{Math.Round(cpuUsage, 2):F2}%]"; }));
      }

      #endregion
      #region HistoryManager Event Handlers

      private void UpdateRedoDepth(object sender, int e) => RedoDepthLabel.Text = $"Redos [{e}]";
      private void UpdateUndoDepth(object sender, int e) => UndoDepthLabel.Text = $"Undos [{e}]";
      #endregion
      #region History interface interactions

      private void RevertInSelectionHistory(object sender, EventArgs e)
      {
         var historyTreeView = new HistoryTree(HistoryManager.RevertTo);
         historyTreeView.VisualizeFull(HistoryManager.Root);
         historyTreeView.ShowDialog();
      }

      private void DeleteHistoryToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var result = MessageBox.Show("Are you sure you want to delete the history?", "Delete History", MessageBoxButtons.OKCancel);
         if (result == DialogResult.OK)
            HistoryManager.Clear();
      }

      #endregion
      #region Toolstrip Interactions

      private void OneGBRAM(object sender, EventArgs e)
      {
         Theory.OneGBRamUsage();
      }
      private void searchToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FormsHelper.ShowIfAnyOpen<Search>();
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

      private void gCToolStripMenuItem_Click(object sender, EventArgs e)
      {
         GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
         GC.Collect();
      }

      private void SaveCurrentMapModeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
         var form = new GetSavingFileForm(pictures, "Where to save the map mode as an image to", ".png");
         form.SetPlaceHolderText(MapModeManager.CurrentMapMode.MapModeType.ToString());
         form.RequireModDirectory = false;
         form.ShowDialog();
         if (form.DialogResult == DialogResult.OK)
         {
            using var bmp = Globals.ZoomControl.Map;
            bmp.Save(form.NewPath, ImageFormat.Png);
         }
      }
      private void graphicalElementsManagerToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FormsHelper.ShowIfAnyOpen<GuiDrawings>();
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
      private void randomModifierToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new ModifierSuggestion().ShowDialog();
      }


      #endregion
      #region Modifiers

      private void OpenAddModifierForm_Click(object sender, EventArgs e)
      {
         new EventModifierForm().ShowDialog();
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
         //TODO  ProvinceEditingEvents.OnModifierAdded(mod, type);
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

         // TODO  ProvinceEditingEvents.OnModifierRemoved(new ApplicableModifier(modifierName, duration), type);
         ModifiersListView.Items.Remove(item);
      }

      #endregion

      private void OnSavingAllEnter(object? sender, EventArgs e) => _savingButtonsToolTip.SetToolTip(SaveAllProvincesButton, $"Save all provinces ({Globals.Provinces.Count})");
      private void OnSavingSelectionEnter(object? sender, EventArgs e) => _savingButtonsToolTip.SetToolTip(SaveCurrentSelectionButton, $"Save selection ({Selection.Count})");

      private void OnOpenProvinceFileEnter(object? sender, EventArgs e)
      {
         _savingButtonsToolTip.SetToolTip(OpenProvinceFile,
            Selection.Count == 1
               ? $"Open the selected province file ({Selection.GetSelectedProvinces[0].TitleLocalisation})"
               : $"Open the selected province files ({Selection.Count})");
      }
      
      private static void OnDateChanged(object? sender, Date date)
      {
         //ProvinceHistoryManager.LoadDate(date);
      }

      public void UpdateHoveredInfo(Province? province)
      {
         if (province == null)
         {
            ProvinceNameLabel.Text = "Province: -";
            OwnerCountryNameLabel.Text = "Owner: -";
            return;
         }
         ProvinceNameLabel.Text = $"Province: {province.TitleLocalisation}";
         OwnerCountryNameLabel.Text = $"Owner: {Localisation.GetLoc(province.Owner.TitleKey)}";
      }

      private void MapWindow_KeyDown(object sender, KeyEventArgs e)
      {
         if (ModifierKeys == Keys.Control)
         {
            switch (e.KeyCode)
            {
               case Keys.F:
                  FormsHelper.ShowIfAnyOpen<Search>();
                  break;
               case Keys.Z:
                  HistoryManager.Undo();
                  break;
               case Keys.Y:
                  HistoryManager.Redo();
                  e.SuppressKeyPress = true;
                  e.Handled = true;
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
               FormsHelper.ShowIfAnyOpen<ConsoleForm>();
               break;
         }
      }
      private void MapWindow_FormClosing(object sender, FormClosingEventArgs e)
      {
         if (!ShutDownMaster.DoWeShutDown())
            e.Cancel = true;
      }



      // ------------------- COUNTRY EDITING TAB ------------------- \\
      public TagComboBox _tagSelectionBox = null!;

      internal FlagLabel CountryFlagLabel = null!;

      private ComboBox _graphicalCultureBox = null!;
      private ComboBox _unitTypeBox = null!;
      private ComboBox _techGroupBox = null!;
      private ComboBox _governmentTypeBox = null!;
      private ComboBox _governmentRankBox = null!;
      private ComboBox _primaryCultureBox = null!;
      private ComboBox _focusComboBox = null!;

      private PropertyColorButton<CommonCountry> _countryColorPickerButton = null!;
      public ThreeColorStripesButton RevolutionColorPickerButton = null!;

      private ItemList _governmentReforms = null!;
      private ItemList _acceptedCultures = null!;

      private NamesEditor _leaderEditor = null!;
      private NamesEditor _shipEditor = null!;
      private NamesEditor _armyEditor = null!;
      private NamesEditor _fleetEditor = null!;

      private PropertyLabel<HistoryCountry> _capitalTextBox = null!;

      private ExtendedNumeric _countryDevelopmentNumeric = null!;

      private QuickAssignControl<string> _historicalUnits = null!;
      private QuickAssignControl<string> _historicalIdeas = null!;
      private QuickAssignControl<Tag> _historicRivals = null!;
      private QuickAssignControl<Tag> _historicFriends = null!;
      private QuickAssignControl<string> _estatePrivileges = null!;

      private TextSaveableTextBox<string, CModifyLocalisation> CountryLoc = null!;
      private TextSaveableTextBox<string, CModifyLocalisation> CountryADJLoc = null!;

      private void InitializeCountryEditGui()
      {
         Selection.OnCountrySelected += CountryGuiEvents.OnCountrySelected;
         Selection.OnCountryDeselected += CountryGuiEvents.OnCountryDeselected;
         _tagSelectionBox = new(null!)
         {
            Margin = new(1),
            Height = 25,
         };
         CountryFlagLabel = ControlFactory.GetFlagLabel();
         _tagSelectionBox.OnTagChanged += CountryGuiEvents.TagSelectionBox_OnTagChanged;
         _countryColorPickerButton = ControlFactory.GetColorPickerButtonCommonCountry(typeof(CommonCountry).GetProperty(nameof(CommonCountry.Color)));
         _countryColorPickerButton.Click += CountryGuiEvents.CountryColorPickerButton_Click;
         GeneralToolTip.SetToolTip(_countryColorPickerButton, "Set the <color> of the selected country");
         RevolutionColorPickerButton = ControlFactory.GetThreeColorsButton();
         RevolutionColorPickerButton.Margin = new(0);

         GeneralToolTip.SetToolTip(RevolutionColorPickerButton, "LMB: Set the <revolutionary_color> of the selected country\nRMB: randomize");
         _graphicalCultureBox = ControlFactory.GetListComboBox(Globals.GraphicalCultures, new(1));
         _graphicalCultureBox.SelectedIndexChanged += CountryGuiEvents.GraphicalCultureBox_SelectedIndexChanged;
         _unitTypeBox = ControlFactory.GetListComboBox([.. Globals.TechnologyGroups.Keys], new(1));
         _unitTypeBox.SelectedIndexChanged += CountryGuiEvents.UnitTypeBox_SelectedIndexChanged;
         _techGroupBox = ControlFactory.GetListComboBox([.. Globals.TechnologyGroups.Keys], new(1));
         _techGroupBox.SelectedIndexChanged += CountryGuiEvents.TechGroupBox_SelectedIndexChanged;
         _governmentTypeBox = ControlFactory.GetListComboBox([.. Globals.GovernmentTypes.Keys], new(1, 1, 6, 1));
         _governmentTypeBox.SelectedIndexChanged += GovernmentTypeBox_SelectedIndexChanged;
         List<string> ranks = [];
         for (var i = 1; i <= Globals.MaxGovRank; i++)
            ranks.Add(i.ToString());
         _governmentRankBox = ControlFactory.GetListComboBox(ranks, new(1)); // TODO read in the defines to determine range
         _governmentRankBox.SelectedIndexChanged += CountryGuiEvents.GovernmentRankBox_SelectedIndexChanged;
         _governmentReforms = ControlFactory.GetItemList(nameof(Country.HistoryCountry.GovernmentReforms), ItemTypes.FullWidth, [], "Government Reforms");
         _governmentReforms.Width = 117;
         _governmentReforms.OnItemAdded += CountryGuiEvents.GovernmentReforms_OnItemAdded;
         _governmentReforms.OnItemRemoved += CountryGuiEvents.GovernmentReforms_OnItemRemoved;
         _capitalTextBox = ControlFactory.GetPropertyLabelHistoryCountry(typeof(HistoryCountry).GetProperty(nameof(HistoryCountry.GetCapitalLoc)));

         _focusComboBox = ControlFactory.GetListComboBox([.. Enum.GetNames<Mana>()], new(1), false);
         _focusComboBox.SelectedIndexChanged += CountryGuiEvents.FocusComboBox_SelectedIndexChanged;

         TagAndColorTLP.Controls.Add(_tagSelectionBox, 1, 0);
         TagAndColorTLP.Controls.Add(_countryColorPickerButton, 3, 0);
         TagAndColorTLP.Controls.Add(RevolutionColorPickerButton, 3, 3);
         TagAndColorTLP.Controls.Add(_graphicalCultureBox, 1, 2);
         TagAndColorTLP.Controls.Add(_unitTypeBox, 3, 2);
         TagAndColorTLP.Controls.Add(_techGroupBox, 1, 3);
         CapitalTLP.Controls.Add(_capitalTextBox, 0, 0);
         TagAndColorTLP.Controls.Add(_focusComboBox, 1, 4);
         CountryHeaderTLP.Controls.Add(CountryFlagLabel, 0, 0);

         GovernmentLayoutPanel.Controls.Add(_governmentTypeBox, 3, 0);
         GovernmentLayoutPanel.Controls.Add(_governmentRankBox, 1, 0);
         GovernmentLayoutPanel.Controls.Add(_governmentReforms, 0, 1);
         GovernmentLayoutPanel.SetColumnSpan(_governmentReforms, 4);

         // Names
         _leaderEditor = new([], "Add / Remove any names, separate with \",\"");
         _shipEditor = new([], "Add / Remove any names, separate with \",\"  $PROVINCE$ can be used here.");
         _armyEditor = new([], "Add / Remove any names, separate with \",\"");
         _fleetEditor = new([], "Add / Remove any names, separate with \",\"  $PROVINCE$ can be used here.");

         LeaderNamesTab.Controls.Add(_leaderEditor);
         ShipNamesTab.Controls.Add(_shipEditor);
         ArmyNamesTab.Controls.Add(_armyEditor);
         FleetNamesTab.Controls.Add(_fleetEditor);

         // Cultures
         _primaryCultureBox = new() { Dock = DockStyle.Fill };
         _primaryCultureBox.Items.AddRange([.. Globals.Cultures.Keys]);
         _primaryCultureBox.Margin = new(1, 1, 6, 1);
         _acceptedCultures = ControlFactory.GetItemList(nameof(HistoryCountry.AcceptedCultures), ItemTypes.FullWidth, [.. Globals.Cultures.Keys], "Accepted Cultures");
         _acceptedCultures.OnItemAdded += CountryGuiEvents.AcceptedCultures_OnItemAdded;
         _acceptedCultures.OnItemRemoved += CountryGuiEvents.AcceptedCultures_OnItemRemoved;

         CulturesTLP.Controls.Add(_primaryCultureBox, 1, 0);
         CulturesTLP.Controls.Add(_acceptedCultures, 0, 1);
         CulturesTLP.SetColumnSpan(_acceptedCultures, 2);

         // Development
         _countryDevelopmentNumeric = ControlFactory.GetExtendedNumeric("");
         _countryDevelopmentNumeric.Minimum = 0;
         _countryDevelopmentNumeric.Maximum = 100000;
         _countryDevelopmentNumeric.UpButtonPressedSmall += (_, _) => AddDevToSelectedCountryIfValid(1);
         _countryDevelopmentNumeric.UpButtonPressedMedium += (_, _) => AddDevToSelectedCountryIfValid(5);
         _countryDevelopmentNumeric.UpButtonPressedLarge += (_, _) => AddDevToSelectedCountryIfValid(10);
         _countryDevelopmentNumeric.DownButtonPressedSmall += (_, _) => AddDevToSelectedCountryIfValid(-1);
         _countryDevelopmentNumeric.DownButtonPressedMedium += (_, _) => AddDevToSelectedCountryIfValid(-5);
         _countryDevelopmentNumeric.DownButtonPressedLarge += (_, _) => AddDevToSelectedCountryIfValid(-10);
         _countryDevelopmentNumeric.OnValueChanged += (_, _) => SpreadDevInSelectedCountryIfValid((int)_countryDevelopmentNumeric.Value);
         DevelopmenTLP.Controls.Add(_countryDevelopmentNumeric, 1, 0);
         GeneralToolTip.SetToolTip(_countryDevelopmentNumeric, "LMB = +- 1\nSHIFT + LMB = +- 5\nCTRL + LMB = +-10\nThe development is only added to one random province in the selected country per click.");

         // Quick Assign
         List<string> unitsList = [];
         foreach (var unit in Globals.Units)
            unitsList.Add(unit.UnitName);
         _historicalUnits = new(unitsList, CommonCountry.Empty, nameof(CommonCountry.HistoricUnits), "Historic Units", -1);
         _historicalUnits.SetAutoSelectFunc(LandUnit.AutoSelectFuncUnits);

         List<string> ideaGroups = [];
         foreach (var idea in Globals.Ideas)
            if (idea.Type == IdeaType.IdeaGroup)
               ideaGroups.Add(idea.Name);
         _historicalIdeas = new(ideaGroups, CommonCountry.Empty, nameof(CommonCountry.HistoricIdeas), "Historic Ideas", 8);
         _historicalIdeas.SetAutoSelectFunc(IdeaGroup.GetAutoAssignment);

         _historicRivals = new([.. Globals.Countries.Keys], HistoryCountry.Empty, nameof(HistoryCountry.HistoricalRivals), "Historic Rivals", 8);
         _historicRivals.SetAutoSelectFunc(Country.GetHistoricRivals);
         _historicFriends = new([.. Globals.Countries.Keys], HistoryCountry.Empty, nameof(HistoryCountry.HistoricalFriends), "Historic Friends", 8);
         _historicFriends.SetAutoSelectFunc(Country.GetHistoricFriends);
         _estatePrivileges = new([], HistoryCountry.Empty, nameof(HistoryCountry.EstatePrivileges), "Estate Privileges", 8);
         _estatePrivileges.Enabled = false;

         MiscTLP.Controls.Add(_historicalUnits, 0, 1);
         MiscTLP.Controls.Add(_historicalIdeas, 0, 2);
         MiscTLP.Controls.Add(_historicRivals, 0, 3);
         MiscTLP.Controls.Add(_historicFriends, 0, 4);
         MiscTLP.Controls.Add(_estatePrivileges, 0, 5);

         CountryLoc = new(Selection.GetCountryAsList, new CLocObjFactory(true))
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
         };

         TagAndColorTLP.Controls.Add(CountryLoc, 1, 1);

         CountryADJLoc = new(Selection.GetCountryAsList, new CLocObjFactory(false))
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
         };
         TagAndColorTLP.Controls.Add(CountryADJLoc, 3, 1);

         AddNewMonarchNameButton.Click += CountryGuiEvents.AddMonarchName_Click;

         CountryCustomToolStripLayoutPanel.Paint += TableLayoutBorder_Paint;
         OpenCountryFileButton.Enter += SetSavingToolTipCountryFileButton;

      }

      private void SetSavingToolTipCountryFileButton(object? sender, EventArgs e)
      {
         var countries = Selection.GetSelectedProvinceOwners();
         _savingButtonsToolTip.SetToolTip(OpenCountryFileButton,
            countries.Count == 1
               ? $"Open the file of {countries.First().Tag} ({countries.First().TitleLocalisation})"
               : $"Open the files of {countries.Count} countries");
      }

      public void ClearCountryGui()
      {
         Globals.State = State.Loading;
         // Flag
         CountryFlagLabel.SetCountry(Country.Empty);

         // Misc
         _tagSelectionBox.SelectedItem = DataClasses.GameDataClasses.Tag.Empty;
         CountryNameLabel.Text = "Country: -";
         _countryColorPickerButton.BackColor = Color.Empty;
         _countryColorPickerButton.Text = "(//)";
         CountryADJLoc.Clear();
         CountryLoc.Clear();
         RevolutionColorPickerButton.SetDefault();
         _graphicalCultureBox.SelectedIndex = 0;
         _unitTypeBox.SelectedIndex = 0;
         _techGroupBox.SelectedIndex = 0;
         _focusComboBox.SelectedIndex = 0;
         //_capitalTextBox.Clear();

         // Names
         _leaderEditor.Clear();
         _shipEditor.Clear();
         _armyEditor.Clear();
         _fleetEditor.Clear();
         MonarchNamesFlowPanel.Controls.Clear();

         // Cultures
         _primaryCultureBox.SelectedIndex = -1;
         _acceptedCultures.Clear();

         // Government
         _governmentTypeBox.SelectedIndex = 0;
         _governmentRankBox.SelectedIndex = 0;
         _governmentReforms.Clear();

         // Development
         _countryDevelopmentNumeric.Value = 3;

         // Quick Assign
         _historicalUnits.Clear();
         _historicalIdeas.Clear();
         _historicRivals.Clear();
         _historicFriends.Clear();
         _estatePrivileges.Clear();

         Globals.State = State.Running;
      }

      internal void LoadCountryToGui(Country country)
      {
         Globals.State = State.Loading;
         if (country == Country.Empty)
            return;
         SuspendLayout();
         _tagSelectionBox.SelectedItem = country.Tag;
         if (Globals.Settings.Gui.ShowCountryFlagInCE)
         {
            CountryFlagLabel.SetCountry(country);
            GeneralToolTip.SetToolTip(CountryFlagLabel, $"{country.TitleLocalisation} ({country.Tag})");
         }
         CountryNameLabel.Text = $"{country.TitleLocalisation} ({country.Tag}) | (Total Dev: {country.GetDevelopment()})";
         _countryColorPickerButton.BackColor = country.Color;
         _countryColorPickerButton.Text = $"({country.Color.R}/{country.Color.G}/{country.Color.B})";
         CountryLoc.Text = country.TitleLocalisation;
         CountryADJLoc.Text = country.AdjectiveLocalisation;
         RevolutionColorPickerButton.SetColorIndexes(country.CommonCountry.RevolutionaryColor.R, country.CommonCountry.RevolutionaryColor.G, country.CommonCountry.RevolutionaryColor.B);
         _graphicalCultureBox.Text = country.CommonCountry.GraphicalCulture;
         _unitTypeBox.Text = country.HistoryCountry.UnitType;
         _techGroupBox.Text = country.HistoryCountry.TechnologyGroup.Name;
         _focusComboBox.Text = country.HistoryCountry.NationalFocus.ToString();

         // Names
         _leaderEditor.SetNames(country.CommonCountry.LeaderNames);
         _leaderEditor.TextBox.ContentModified += CountryGuiEvents.LeaderNames_ContentModified;
         _shipEditor.SetNames(country.CommonCountry.ShipNames);
         _shipEditor.TextBox.ContentModified += CountryGuiEvents.ShipNames_ContentModified;
         _armyEditor.SetNames(country.CommonCountry.ArmyNames);
         _armyEditor.TextBox.ContentModified += CountryGuiEvents.ArmyNames_ContentModified;
         _fleetEditor.SetNames(country.CommonCountry.FleetNames);
         _fleetEditor.TextBox.ContentModified += CountryGuiEvents.FleetNames_ContentModified;
         if (ShowMonachrNamesCB.Checked)
            AddMonarchNamesToGui(country.CommonCountry.MonarchNames);
         ResumeLayout();

         // Government
         _governmentTypeBox.SelectedItem = country.HistoryCountry.Government;
         _governmentRankBox.SelectedItem = country.HistoryCountry.GovernmentRank.ToString();
         if (Globals.GovernmentTypes.TryGetValue(country.HistoryCountry.Government, out var government))
            _governmentReforms.InitializeItems([.. government.AllReformNames]);

         _governmentReforms.Clear();
         _governmentReforms.Initializing = true;// we dont want to fire events that modify this collection when adding stuff as we are not editing it just loading.
         _governmentReforms.AddItemsUnique(country.HistoryCountry.GovernmentReforms);
         _governmentReforms.Initializing = false;

         // Cultures
         _primaryCultureBox.SelectedItem = country.HistoryCountry.PrimaryCulture;
         _acceptedCultures.Clear();
         foreach (var cult in country.HistoryCountry.AcceptedCultures)
            _acceptedCultures.AddItem(cult);

         // Development
         _countryDevelopmentNumeric.Value = country.GetDevelopment();

         _historicalUnits.UpdateCountry(country.CommonCountry);
         _historicalIdeas.UpdateCountry(country.CommonCountry);
         _historicRivals.UpdateCountry(country.HistoryCountry);
         _historicFriends.UpdateCountry(country.HistoryCountry);
         _estatePrivileges.UpdateCountry(country.HistoryCountry);


         Globals.State = State.Running;
      }

      private void GovernmentTypeBox_SelectedIndexChanged(object? sender, EventArgs e)
      {
         if (_governmentTypeBox.SelectedItem == null)
            return;
         if (Globals.GovernmentTypes.TryGetValue(_governmentTypeBox.SelectedItem?.ToString()!, out var government))
         {
            _governmentReforms.InitializeItems([.. government.AllReformNames]);
            if (Selection.SelectedCountry != Country.Empty)
               Selection.SelectedCountry.HistoryCountry.Government = _governmentTypeBox.SelectedItem!.ToString()!;
         }
      }

      private void AddDevToSelectedCountryIfValid(int dev)
      {
         if (Selection.SelectedCountry == Country.Empty)
            return;
         Selection.SelectedCountry.AddDevToRandomProvince(dev);
      }

      private void SpreadDevInSelectedCountryIfValid(int value)
      {
         if (Selection.SelectedCountry == Country.Empty)
            return;
         var provinces = Selection.SelectedCountry.GetProvinces();
         var pieces = MathHelper.SplitIntoNRandomPieces(provinces.Count, value, Globals.Settings.Misc.MinDevelopmentInGeneration, Globals.Settings.Misc.MaxDevelopmentInGeneration);
         if (pieces.Count != provinces.Count)
            return;
         var index = 0;
         foreach (var province in provinces)
         {
            var devParts = MathHelper.SplitIntoNRandomPieces(3, pieces[index], 1,
               Globals.Settings.Misc.MaxDevelopmentInGeneration);

            province.BaseTax = devParts[0];
            province.BaseProduction = devParts[1];
            province.BaseManpower = devParts[2];

            index++;
         }
      }

      private void CreateFilesByDefault_Click(object sender, EventArgs e)
      {
         CreateFilesByDefault.Checked = !CreateFilesByDefault.Checked;
         Globals.Settings.Saving.AlwaysAskBeforeCreatingFiles = CreateFilesByDefault.Checked;
      }

      private void save1ToolStripMenuItem_Click(object sender, EventArgs e)
      {
      }

      private void bugReportToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new CrashReporter().ShowDialog();
      }

      private void quickSettingsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FormsHelper.ShowIfAnyOpen<SettingsWindow>();
      }

      private void CountryAdvancedEditorButton_Click(object sender, EventArgs e)
      {
         var editor = new RoughEditorForm(Selection.SelectedCountry, false);
         editor.ExpandObjectsOfType(nameof(Country.HistoryCountry), nameof(Country.CommonCountry));
         editor.ShowDialog();
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

      private void AddMonarchNamesToGui(List<MonarchName> names)
      {
         MonarchNamesFlowPanel.FlowDirection = FlowDirection.TopDown;
         MonarchNamesFlowPanel.AutoScroll = true;
         MonarchNamesFlowPanel.WrapContents = false;

         MonarchNamesFlowPanel.Controls.Clear();
         SuspendLayout();
         foreach (var monName in names)
            AddMonarchNameToGui(monName);
         ResumeLayout();
      }

      internal void AddMonarchNameToGui(MonarchName monarchName)
      {
         MonarchNameControl monarchNameControl = new(monarchName, new(MonarchNamesFlowPanel.Width - 28, 29));
         MonarchNamesFlowPanel.Controls.Add(monarchNameControl);
      }

      private void ShowMonarchNamesCB_CheckedChanged(object sender, EventArgs e)
      {
         if (Selection.SelectedCountry == Country.Empty)
            return;
         if (ShowMonachrNamesCB.Checked)
         {
            AddMonarchNamesToGui(Selection.SelectedCountry.CommonCountry.MonarchNames);
            NameTextBox.Enabled = true;
            AddNewMonarchNameButton.Enabled = true;
            ChanceTextBox.Enabled = true;
         }
         else
         {
            MonarchNamesFlowPanel.Controls.Clear();
            NameTextBox.Enabled = false;
            AddNewMonarchNameButton.Enabled = false;
            ChanceTextBox.Enabled = false;
         }
      }

      private void SetCapitalToSelected_Click(object sender, EventArgs e)
      {
         if (Selection.Count != 1)
            return;

         _capitalTextBox.Text = Selection.GetSelectedProvinces[0].Id.ToString();
         _capitalTextBox.Focus();
      }


      private void RedistributeDev_Click(object sender, EventArgs e)
      {
         SpreadDevInSelectedCountryIfValid((int)_countryDevelopmentNumeric.Value);
      }

      private void newSavingToolStripMenuItem_Click(object sender, EventArgs e)
      {
         SaveMaster.SaveAllChanges();
      }

      private void refStackToolStripMenuItem_Click(object sender, EventArgs e)
      {

      }

      private void infoToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new InformationForm().ShowDialog();
      }

      private void toolStripMenuItem4_Click(object sender, EventArgs e)
      {
#if DEBUG
         var sb = new StringBuilder();
         foreach (var terrain in Globals.Terrains.Values)
         {
            sb.AppendLine($"{terrain.Name} : {terrain.SubCollection.Count}");
            foreach (var or in terrain.SubCollection)
            {
               sb.Append($"{or.Id}, ");
            }
            sb.AppendLine();
         }

         File.WriteAllText(Path.Combine(Globals.DebugPath, "terrainOverrides.txt"), sb.ToString());
#endif
      }

      private void deleteProvinceHistoryEntriesToolStripMenuItem_Click(object sender, EventArgs e)
      {
         GetDateInput dateInputForm = new("Select the date:");
         dateInputForm.ShowDialog();
         if (dateInputForm.DialogResult == DialogResult.OK)
         {
            if (!dateInputForm.GetDate(out var date))
               return;

            foreach (var province in Globals.Provinces)
               for (var i = province.History.Count - 1; i >= 0; i--)
                  if (province.History[i].Date >= date)
                     province.History.RemoveAt(i);
         }

      }

      private void FocusSelectionCheckBox_CheckedChanged(object sender, EventArgs e)
      {

      }

      private void AddNewCountryButton_Click(object sender, EventArgs e)
      {
         new CreateCountryForm().ShowDialog();
      }

      private void fileNamesToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var sb = new StringBuilder();
         foreach (var country in Globals.Countries.Values)
         {
            sb.AppendLine($"{country.Tag} : {country.CountryFilePath.FilePathArr.ToString()}");
         }


      }

      private void emptyCOlorInCountryToolStripMenuItem_Click(object sender, EventArgs e)
      {
         foreach (var country in Globals.Countries.Values)
         {
            if (country.Color == Color.Empty)
               Debug.WriteLine(country.Tag);
         }
      }

      private void iMBTESTToolStripMenuItem_Click(object sender, EventArgs e)
      {
         SystemSounds.Question.Play();
      }

      private void browseEditedObjectsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new EditedObjectsExplorer().Show();
      }

      private void provinceToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Globals.State = State.Loading;
         ClearProvinceGui();
         Globals.State = State.Running;
      }

      private void countryToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Globals.State = State.Loading;
         ClearCountryGui();
         Globals.State = State.Running;
      }


      private void AddNewMonarchNameButton_Click_1(object sender, EventArgs e)
      {
         CountryGuiEvents.AddMonarchName_Click(sender, e);
         NameTextBox.Clear();
         ChanceTextBox.Clear();
      }

      private void gameOfLiveToolStripMenuItem_Click(object sender, EventArgs e)
      {
         GameOfLive.RunGameOfLive(100);
      }

      private void benchmarkMapModesToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var sw = Stopwatch.StartNew();
         for (var i = 0; i < 1000; i++)
         {
            MapModeManager.SetCurrentMapMode(MapModeType.Area);
            MapModeManager.SetCurrentMapMode(MapModeType.Country);
         }
         sw.Stop();
         System.Diagnostics.Debug.WriteLine($"Time: {sw.ElapsedMilliseconds / 1000 * 2}");
      }

      private void ViewErrorLogToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new ErrorLogExplorer()
         {
            StartPosition = FormStartPosition.CenterParent
         }.Show();
      }

      private void OpenProvinceFile_Click(object sender, EventArgs e)
      {
         ProcessHelper.OpenSaveableFiles(Selection.GetSelectedProvincesAsSaveable());
      }

      private void OpenProvinceFolder_Click(object sender, EventArgs e)
      {
         ProcessHelper.OpenSaveableFolders(Selection.GetSelectedProvincesAsSaveable());
      }

      private void TableLayoutBorder_Paint(object? sender, PaintEventArgs e)
      {
         var tableLayout = (TableLayoutPanel)sender!;
         var pen = new Pen(Color.Black, 1);
         e.Graphics.DrawRectangle(pen, 0, 0, tableLayout.Width - 1, tableLayout.Height - 1);
      }

      private void OpenCountryFolder_Click(object sender, EventArgs e)
      {
         ProcessHelper.OpenSaveableFolders(Selection.GetSelectedProvinceOwners().Select(c => c.CommonCountry).Cast<Saveable>().ToList());
      }

      private void OpenCountryHistoryFolder(object? sender, EventArgs e)
      {
         ProcessHelper.OpenSaveableFolders(Selection.GetSelectedProvinceOwners().Select(c => c.HistoryCountry).Cast<Saveable>().ToList());
      }

      private void OpenCountryFileButton_Click(object sender, MouseEventArgs e)
      {
         ProcessHelper.OpenSaveableFiles(e.Button == MouseButtons.Left
            ? Selection.GetSelectedProvinceOwners().Select(c => c.CommonCountry).Cast<Saveable>().ToList()
            : Selection.GetSelectedProvinceOwners().Select(c => c.HistoryCountry).Cast<Saveable>().ToList());
      }

      private void SaveSelectedCountriesButton_Click(object sender, EventArgs e)
      {
         SaveMaster.SaveSaveables(Selection.GetSelectedProvinceOwnersAsSaveable(), onlyModified: false);
      }

      private void SaveAllCountries_Click(object sender, EventArgs e)
      {
         SaveMaster.SaveAllChanges(false, SaveableType.Country);
      }

      private void testToolStripMenuItem_Click(object sender, EventArgs e)
      {
         _ = new RoughEditorForm(new GlobalsDynamicWrapper(), false).ShowDialog();
      }

      private void CountryHistoryEntry_Toggle(object sender, EventArgs e)
      {
         if (Globals.Settings.Gui.EnableDisableHistoryEntryCreationGlobally)
            ProvinceHistoryEntryToggleButton.State = CountryHistoryEntryToggleButton.State;
      }

      private void ProvinceHistoryEntryToggleButton_Click(object sender, EventArgs e)
      {
         if (Globals.Settings.Gui.EnableDisableHistoryEntryCreationGlobally)
            CountryHistoryEntryToggleButton.State = ProvinceHistoryEntryToggleButton.State;
      }

      private void runNameGenToolStripMenuItem_Click(object sender, EventArgs e)
      {
         NameGenStarter.RunNameGen();
      }

      private void loadingToolStripMenuItem_Click(object sender, EventArgs e)
      {
         MapLoading.Load();
      }

      private void saveErrorLogsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         LogManager.SaveLogToFile();
      }

      private void saveErrorLogAscsvToolStripMenuItem_Click(object sender, EventArgs e)
      {
         LogManager.SaveLogAsCsv();
      }

      private void clearCrashLogsToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         CrashManager.ClearCrashLogs();
      }

      private void openCrashLogFolderToolStripMenuItem_Click(object sender, EventArgs e)
      {
         ProcessHelper.OpenFolder(CrashManager.LogFolder);
      }

      private void openLastCrashLogsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         CrashManager.OpenLastCrashLogs();
      }
   }
}
