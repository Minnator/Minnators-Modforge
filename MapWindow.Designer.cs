namespace Editor
{
   partial class MapWindow
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapWindow));
         MapMenuStrip = new MenuStrip();
         filesToolStripMenuItem = new ToolStripMenuItem();
         gCToolStripMenuItem = new ToolStripMenuItem();
         saveCurrentMapModeToolStripMenuItem = new ToolStripMenuItem();
         historyToolStripMenuItem = new ToolStripMenuItem();
         selectionHistoryToolStripMenuItem = new ToolStripMenuItem();
         DeleteHistoryToolStripMenuItem = new ToolStripMenuItem();
         toolTipCustomizerToolStripMenuItem = new ToolStripMenuItem();
         openCustomizerToolStripMenuItem = new ToolStripMenuItem();
         ShowToolTipMenuItem = new ToolStripMenuItem();
         MapModeComboBox = new ToolStripComboBox();
         debugToolStripMenuItem = new ToolStripMenuItem();
         testToolStripMenuItem = new ToolStripMenuItem();
         telescopeToolStripMenuItem = new ToolStripMenuItem();
         refStackToolStripMenuItem = new ToolStripMenuItem();
         DateSelector = new ToolStripComboBox();
         searchToolStripMenuItem = new ToolStripMenuItem();
         MapPanel = new Panel();
         toolStrip1 = new ToolStrip();
         RamUsageStrip = new ToolStripLabel();
         CpuUsageStrip = new ToolStripLabel();
         toolStripSeparator1 = new ToolStripSeparator();
         UndoDepthLabel = new ToolStripLabel();
         RedoDepthLabel = new ToolStripLabel();
         toolStripSeparator2 = new ToolStripSeparator();
         SelectedProvinceSum = new ToolStripLabel();
         toolStripLabel1 = new ToolStripLabel();
         toolStripSplitButton1 = new ToolStripSplitButton();
         MainLayoutPanel = new TableLayoutPanel();
         TopStripLayoutPanel = new TableLayoutPanel();
         OwnerCountryNameLabel = new Label();
         ProvinceNameLabel = new Label();
         toolStripContainer1 = new ToolStripContainer();
         toolStrip2 = new ToolStrip();
         newToolStripButton = new ToolStripButton();
         openToolStripButton = new ToolStripButton();
         saveToolStripButton = new ToolStripButton();
         printToolStripButton = new ToolStripButton();
         toolStripSeparator = new ToolStripSeparator();
         cutToolStripButton = new ToolStripButton();
         copyToolStripButton = new ToolStripButton();
         pasteToolStripButton = new ToolStripButton();
         toolStripSeparator3 = new ToolStripSeparator();
         helpToolStripButton = new ToolStripButton();
         DataTabPanel = new TabControl();
         ProvincePage = new TabPage();
         tableLayoutPanel1 = new TableLayoutPanel();
         groupBox1 = new GroupBox();
         OwnerControllerLayoutPanel = new TableLayoutPanel();
         label1 = new Label();
         label2 = new Label();
         groupBox2 = new GroupBox();
         tableLayoutPanel2 = new TableLayoutPanel();
         CultureComboBox = new ComboBox();
         label5 = new Label();
         label3 = new Label();
         label4 = new Label();
         ReligionComboBox = new ComboBox();
         CapitalNameTextBox = new TextBox();
         groupBox3 = new GroupBox();
         tableLayoutPanel3 = new TableLayoutPanel();
         label6 = new Label();
         label7 = new Label();
         label8 = new Label();
         ManpowerDevTextBox = new TextBox();
         ProductionDevTextBox = new TextBox();
         TaxDevTextBox = new TextBox();
         groupBox4 = new GroupBox();
         tableLayoutPanel4 = new TableLayoutPanel();
         IsCityCheckBox = new CheckBox();
         IsHreCheckBox = new CheckBox();
         IsParlimentSeatCheckbox = new CheckBox();
         HasRevoltCheckBox = new CheckBox();
         COuntryPage = new TabPage();
         ProvinceGroupsPage = new TabPage();
         MapMenuStrip.SuspendLayout();
         toolStrip1.SuspendLayout();
         MainLayoutPanel.SuspendLayout();
         TopStripLayoutPanel.SuspendLayout();
         toolStripContainer1.ContentPanel.SuspendLayout();
         toolStripContainer1.RightToolStripPanel.SuspendLayout();
         toolStripContainer1.SuspendLayout();
         toolStrip2.SuspendLayout();
         DataTabPanel.SuspendLayout();
         ProvincePage.SuspendLayout();
         tableLayoutPanel1.SuspendLayout();
         groupBox1.SuspendLayout();
         OwnerControllerLayoutPanel.SuspendLayout();
         groupBox2.SuspendLayout();
         tableLayoutPanel2.SuspendLayout();
         groupBox3.SuspendLayout();
         tableLayoutPanel3.SuspendLayout();
         groupBox4.SuspendLayout();
         tableLayoutPanel4.SuspendLayout();
         SuspendLayout();
         // 
         // MapMenuStrip
         // 
         MapMenuStrip.Items.AddRange(new ToolStripItem[] { filesToolStripMenuItem, historyToolStripMenuItem, toolTipCustomizerToolStripMenuItem, MapModeComboBox, debugToolStripMenuItem, DateSelector, searchToolStripMenuItem });
         MapMenuStrip.Location = new Point(0, 0);
         MapMenuStrip.Name = "MapMenuStrip";
         MapMenuStrip.Padding = new Padding(7, 2, 0, 2);
         MapMenuStrip.Size = new Size(1511, 27);
         MapMenuStrip.TabIndex = 0;
         MapMenuStrip.Text = "menuStrip1";
         // 
         // filesToolStripMenuItem
         // 
         filesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { gCToolStripMenuItem, saveCurrentMapModeToolStripMenuItem });
         filesToolStripMenuItem.Name = "filesToolStripMenuItem";
         filesToolStripMenuItem.Size = new Size(42, 23);
         filesToolStripMenuItem.Text = "Files";
         // 
         // gCToolStripMenuItem
         // 
         gCToolStripMenuItem.Name = "gCToolStripMenuItem";
         gCToolStripMenuItem.Size = new Size(199, 22);
         gCToolStripMenuItem.Text = "GC";
         gCToolStripMenuItem.Click += gCToolStripMenuItem_Click;
         // 
         // saveCurrentMapModeToolStripMenuItem
         // 
         saveCurrentMapModeToolStripMenuItem.Name = "saveCurrentMapModeToolStripMenuItem";
         saveCurrentMapModeToolStripMenuItem.Size = new Size(199, 22);
         saveCurrentMapModeToolStripMenuItem.Text = "Save Current MapMode";
         saveCurrentMapModeToolStripMenuItem.Click += SaveCurrentMapModeToolStripMenuItem_Click;
         // 
         // historyToolStripMenuItem
         // 
         historyToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { selectionHistoryToolStripMenuItem, DeleteHistoryToolStripMenuItem });
         historyToolStripMenuItem.Name = "historyToolStripMenuItem";
         historyToolStripMenuItem.Size = new Size(57, 23);
         historyToolStripMenuItem.Text = "History";
         // 
         // selectionHistoryToolStripMenuItem
         // 
         selectionHistoryToolStripMenuItem.Name = "selectionHistoryToolStripMenuItem";
         selectionHistoryToolStripMenuItem.Size = new Size(164, 22);
         selectionHistoryToolStripMenuItem.Text = "View History Tree";
         selectionHistoryToolStripMenuItem.Click += RevertInSelectionHistory;
         // 
         // DeleteHistoryToolStripMenuItem
         // 
         DeleteHistoryToolStripMenuItem.Name = "DeleteHistoryToolStripMenuItem";
         DeleteHistoryToolStripMenuItem.Size = new Size(164, 22);
         DeleteHistoryToolStripMenuItem.Text = "Delete History";
         DeleteHistoryToolStripMenuItem.Click += DeleteHistoryToolStripMenuItem_Click;
         // 
         // toolTipCustomizerToolStripMenuItem
         // 
         toolTipCustomizerToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openCustomizerToolStripMenuItem, ShowToolTipMenuItem });
         toolTipCustomizerToolStripMenuItem.Name = "toolTipCustomizerToolStripMenuItem";
         toolTipCustomizerToolStripMenuItem.Size = new Size(120, 23);
         toolTipCustomizerToolStripMenuItem.Text = "ToolTip Customizer";
         // 
         // openCustomizerToolStripMenuItem
         // 
         openCustomizerToolStripMenuItem.Name = "openCustomizerToolStripMenuItem";
         openCustomizerToolStripMenuItem.Size = new Size(166, 22);
         openCustomizerToolStripMenuItem.Text = "Open Customizer";
         openCustomizerToolStripMenuItem.Click += openCustomizerToolStripMenuItem_Click;
         // 
         // ShowToolTipMenuItem
         // 
         ShowToolTipMenuItem.Checked = true;
         ShowToolTipMenuItem.CheckOnClick = true;
         ShowToolTipMenuItem.CheckState = CheckState.Checked;
         ShowToolTipMenuItem.Name = "ShowToolTipMenuItem";
         ShowToolTipMenuItem.Size = new Size(166, 22);
         ShowToolTipMenuItem.Text = "Show ToolTip";
         ShowToolTipMenuItem.Click += ShowToolTipMenuItem_Click;
         // 
         // MapModeComboBox
         // 
         MapModeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
         MapModeComboBox.Name = "MapModeComboBox";
         MapModeComboBox.Size = new Size(140, 23);
         MapModeComboBox.SelectedIndexChanged += MapModeComboBox_SelectedIndexChanged;
         // 
         // debugToolStripMenuItem
         // 
         debugToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { testToolStripMenuItem, telescopeToolStripMenuItem, refStackToolStripMenuItem });
         debugToolStripMenuItem.Name = "debugToolStripMenuItem";
         debugToolStripMenuItem.Size = new Size(54, 23);
         debugToolStripMenuItem.Text = "Debug";
         debugToolStripMenuItem.Click += debugToolStripMenuItem_Click;
         // 
         // testToolStripMenuItem
         // 
         testToolStripMenuItem.Name = "testToolStripMenuItem";
         testToolStripMenuItem.Size = new Size(180, 22);
         testToolStripMenuItem.Text = "Test";
         testToolStripMenuItem.Click += testToolStripMenuItem_Click;
         // 
         // telescopeToolStripMenuItem
         // 
         telescopeToolStripMenuItem.Name = "telescopeToolStripMenuItem";
         telescopeToolStripMenuItem.Size = new Size(180, 22);
         telescopeToolStripMenuItem.Text = "Telescope";
         telescopeToolStripMenuItem.Click += telescopeToolStripMenuItem_Click;
         // 
         // refStackToolStripMenuItem
         // 
         refStackToolStripMenuItem.Name = "refStackToolStripMenuItem";
         refStackToolStripMenuItem.Size = new Size(180, 22);
         refStackToolStripMenuItem.Click += refStackToolStripMenuItem_Click;
         // 
         // DateSelector
         // 
         DateSelector.Items.AddRange(new object[] { "1444.11.11", "1500.1.1", "1600.1.1" });
         DateSelector.Name = "DateSelector";
         DateSelector.Size = new Size(121, 23);
         DateSelector.SelectedIndexChanged += DateSelector_SelectedIndexChanged;
         // 
         // searchToolStripMenuItem
         // 
         searchToolStripMenuItem.Name = "searchToolStripMenuItem";
         searchToolStripMenuItem.Size = new Size(54, 23);
         searchToolStripMenuItem.Text = "Search";
         searchToolStripMenuItem.Click += searchToolStripMenuItem_Click;
         // 
         // MapPanel
         // 
         MapPanel.AutoScroll = true;
         MapPanel.BorderStyle = BorderStyle.FixedSingle;
         MapPanel.Dock = DockStyle.Fill;
         MapPanel.Location = new Point(0, 0);
         MapPanel.Margin = new Padding(4, 3, 4, 3);
         MapPanel.Name = "MapPanel";
         MapPanel.Size = new Size(1081, 844);
         MapPanel.TabIndex = 1;
         // 
         // toolStrip1
         // 
         MainLayoutPanel.SetColumnSpan(toolStrip1, 2);
         toolStrip1.Dock = DockStyle.None;
         toolStrip1.Items.AddRange(new ToolStripItem[] { RamUsageStrip, CpuUsageStrip, toolStripSeparator1, UndoDepthLabel, RedoDepthLabel, toolStripSeparator2, SelectedProvinceSum, toolStripLabel1, toolStripSplitButton1 });
         toolStrip1.Location = new Point(0, 876);
         toolStrip1.Name = "toolStrip1";
         toolStrip1.Size = new Size(420, 24);
         toolStrip1.TabIndex = 0;
         // 
         // RamUsageStrip
         // 
         RamUsageStrip.Name = "RamUsageStrip";
         RamUsageStrip.Size = new Size(48, 21);
         RamUsageStrip.Text = "Ram [0]";
         // 
         // CpuUsageStrip
         // 
         CpuUsageStrip.Name = "CpuUsageStrip";
         CpuUsageStrip.Size = new Size(47, 21);
         CpuUsageStrip.Text = "CPU [0]";
         // 
         // toolStripSeparator1
         // 
         toolStripSeparator1.Name = "toolStripSeparator1";
         toolStripSeparator1.Size = new Size(6, 24);
         // 
         // UndoDepthLabel
         // 
         UndoDepthLabel.Name = "UndoDepthLabel";
         UndoDepthLabel.Size = new Size(68, 21);
         UndoDepthLabel.Text = "UndoDepth";
         // 
         // RedoDepthLabel
         // 
         RedoDepthLabel.Name = "RedoDepthLabel";
         RedoDepthLabel.Size = new Size(66, 21);
         RedoDepthLabel.Text = "RedoDepth";
         // 
         // toolStripSeparator2
         // 
         toolStripSeparator2.Name = "toolStripSeparator2";
         toolStripSeparator2.Size = new Size(6, 24);
         // 
         // SelectedProvinceSum
         // 
         SelectedProvinceSum.Name = "SelectedProvinceSum";
         SelectedProvinceSum.Size = new Size(49, 21);
         SelectedProvinceSum.Text = "ProSum";
         // 
         // toolStripLabel1
         // 
         toolStripLabel1.Name = "toolStripLabel1";
         toolStripLabel1.Size = new Size(86, 21);
         toolStripLabel1.Text = "toolStripLabel1";
         // 
         // toolStripSplitButton1
         // 
         toolStripSplitButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
         toolStripSplitButton1.Image = (Image)resources.GetObject("toolStripSplitButton1.Image");
         toolStripSplitButton1.ImageTransparentColor = Color.Magenta;
         toolStripSplitButton1.Name = "toolStripSplitButton1";
         toolStripSplitButton1.Size = new Size(32, 21);
         toolStripSplitButton1.Text = "toolStripSplitButton1";
         // 
         // MainLayoutPanel
         // 
         MainLayoutPanel.ColumnCount = 2;
         MainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400F));
         MainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         MainLayoutPanel.Controls.Add(toolStrip1, 0, 2);
         MainLayoutPanel.Controls.Add(TopStripLayoutPanel, 1, 0);
         MainLayoutPanel.Controls.Add(toolStripContainer1, 1, 1);
         MainLayoutPanel.Controls.Add(DataTabPanel, 0, 1);
         MainLayoutPanel.Dock = DockStyle.Fill;
         MainLayoutPanel.Location = new Point(0, 27);
         MainLayoutPanel.Name = "MainLayoutPanel";
         MainLayoutPanel.RowCount = 3;
         MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
         MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
         MainLayoutPanel.Size = new Size(1511, 900);
         MainLayoutPanel.TabIndex = 3;
         // 
         // TopStripLayoutPanel
         // 
         TopStripLayoutPanel.ColumnCount = 5;
         TopStripLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         TopStripLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         TopStripLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         TopStripLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         TopStripLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         TopStripLayoutPanel.Controls.Add(OwnerCountryNameLabel, 1, 0);
         TopStripLayoutPanel.Controls.Add(ProvinceNameLabel, 2, 0);
         TopStripLayoutPanel.Dock = DockStyle.Fill;
         TopStripLayoutPanel.Location = new Point(400, 0);
         TopStripLayoutPanel.Margin = new Padding(0);
         TopStripLayoutPanel.Name = "TopStripLayoutPanel";
         TopStripLayoutPanel.RowCount = 1;
         TopStripLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         TopStripLayoutPanel.Size = new Size(1111, 26);
         TopStripLayoutPanel.TabIndex = 2;
         // 
         // OwnerCountryNameLabel
         // 
         OwnerCountryNameLabel.AutoSize = true;
         OwnerCountryNameLabel.Dock = DockStyle.Fill;
         OwnerCountryNameLabel.Location = new Point(225, 0);
         OwnerCountryNameLabel.Name = "OwnerCountryNameLabel";
         OwnerCountryNameLabel.Size = new Size(216, 26);
         OwnerCountryNameLabel.TabIndex = 0;
         OwnerCountryNameLabel.Text = "Owner: -";
         OwnerCountryNameLabel.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // ProvinceNameLabel
         // 
         ProvinceNameLabel.AutoSize = true;
         ProvinceNameLabel.Dock = DockStyle.Fill;
         ProvinceNameLabel.Location = new Point(447, 0);
         ProvinceNameLabel.Name = "ProvinceNameLabel";
         ProvinceNameLabel.Size = new Size(216, 26);
         ProvinceNameLabel.TabIndex = 1;
         ProvinceNameLabel.Text = "Province: -";
         ProvinceNameLabel.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // toolStripContainer1
         // 
         toolStripContainer1.BottomToolStripPanelVisible = false;
         // 
         // toolStripContainer1.ContentPanel
         // 
         toolStripContainer1.ContentPanel.Controls.Add(MapPanel);
         toolStripContainer1.ContentPanel.Margin = new Padding(0);
         toolStripContainer1.ContentPanel.Size = new Size(1081, 844);
         toolStripContainer1.Dock = DockStyle.Fill;
         toolStripContainer1.LeftToolStripPanelVisible = false;
         toolStripContainer1.Location = new Point(403, 29);
         toolStripContainer1.Name = "toolStripContainer1";
         // 
         // toolStripContainer1.RightToolStripPanel
         // 
         toolStripContainer1.RightToolStripPanel.Controls.Add(toolStrip2);
         toolStripContainer1.Size = new Size(1105, 844);
         toolStripContainer1.TabIndex = 3;
         toolStripContainer1.Text = "toolStripContainer1";
         toolStripContainer1.TopToolStripPanelVisible = false;
         // 
         // toolStrip2
         // 
         toolStrip2.Dock = DockStyle.None;
         toolStrip2.Items.AddRange(new ToolStripItem[] { newToolStripButton, openToolStripButton, saveToolStripButton, printToolStripButton, toolStripSeparator, cutToolStripButton, copyToolStripButton, pasteToolStripButton, toolStripSeparator3, helpToolStripButton });
         toolStrip2.Location = new Point(0, 3);
         toolStrip2.Name = "toolStrip2";
         toolStrip2.Size = new Size(24, 207);
         toolStrip2.TabIndex = 0;
         // 
         // newToolStripButton
         // 
         newToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
         newToolStripButton.Image = (Image)resources.GetObject("newToolStripButton.Image");
         newToolStripButton.ImageTransparentColor = Color.Magenta;
         newToolStripButton.Name = "newToolStripButton";
         newToolStripButton.Size = new Size(22, 20);
         newToolStripButton.Text = "&New";
         // 
         // openToolStripButton
         // 
         openToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
         openToolStripButton.Image = (Image)resources.GetObject("openToolStripButton.Image");
         openToolStripButton.ImageTransparentColor = Color.Magenta;
         openToolStripButton.Name = "openToolStripButton";
         openToolStripButton.Size = new Size(22, 20);
         openToolStripButton.Text = "&Open";
         // 
         // saveToolStripButton
         // 
         saveToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
         saveToolStripButton.Image = (Image)resources.GetObject("saveToolStripButton.Image");
         saveToolStripButton.ImageTransparentColor = Color.Magenta;
         saveToolStripButton.Name = "saveToolStripButton";
         saveToolStripButton.Size = new Size(22, 20);
         saveToolStripButton.Text = "&Save";
         // 
         // printToolStripButton
         // 
         printToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
         printToolStripButton.Image = (Image)resources.GetObject("printToolStripButton.Image");
         printToolStripButton.ImageTransparentColor = Color.Magenta;
         printToolStripButton.Name = "printToolStripButton";
         printToolStripButton.Size = new Size(22, 20);
         printToolStripButton.Text = "&Print";
         // 
         // toolStripSeparator
         // 
         toolStripSeparator.Name = "toolStripSeparator";
         toolStripSeparator.Size = new Size(22, 6);
         // 
         // cutToolStripButton
         // 
         cutToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
         cutToolStripButton.Image = (Image)resources.GetObject("cutToolStripButton.Image");
         cutToolStripButton.ImageTransparentColor = Color.Magenta;
         cutToolStripButton.Name = "cutToolStripButton";
         cutToolStripButton.Size = new Size(22, 20);
         cutToolStripButton.Text = "C&ut";
         // 
         // copyToolStripButton
         // 
         copyToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
         copyToolStripButton.Image = (Image)resources.GetObject("copyToolStripButton.Image");
         copyToolStripButton.ImageTransparentColor = Color.Magenta;
         copyToolStripButton.Name = "copyToolStripButton";
         copyToolStripButton.Size = new Size(22, 20);
         copyToolStripButton.Text = "&Copy";
         // 
         // pasteToolStripButton
         // 
         pasteToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
         pasteToolStripButton.Image = (Image)resources.GetObject("pasteToolStripButton.Image");
         pasteToolStripButton.ImageTransparentColor = Color.Magenta;
         pasteToolStripButton.Name = "pasteToolStripButton";
         pasteToolStripButton.Size = new Size(22, 20);
         pasteToolStripButton.Text = "&Paste";
         // 
         // toolStripSeparator3
         // 
         toolStripSeparator3.Name = "toolStripSeparator3";
         toolStripSeparator3.Size = new Size(22, 6);
         // 
         // helpToolStripButton
         // 
         helpToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
         helpToolStripButton.Image = (Image)resources.GetObject("helpToolStripButton.Image");
         helpToolStripButton.ImageTransparentColor = Color.Magenta;
         helpToolStripButton.Name = "helpToolStripButton";
         helpToolStripButton.Size = new Size(22, 20);
         helpToolStripButton.Text = "He&lp";
         // 
         // DataTabPanel
         // 
         DataTabPanel.Controls.Add(ProvincePage);
         DataTabPanel.Controls.Add(COuntryPage);
         DataTabPanel.Controls.Add(ProvinceGroupsPage);
         DataTabPanel.Dock = DockStyle.Fill;
         DataTabPanel.Location = new Point(0, 26);
         DataTabPanel.Margin = new Padding(0);
         DataTabPanel.Name = "DataTabPanel";
         DataTabPanel.SelectedIndex = 0;
         DataTabPanel.Size = new Size(400, 850);
         DataTabPanel.TabIndex = 4;
         // 
         // ProvincePage
         // 
         ProvincePage.Controls.Add(tableLayoutPanel1);
         ProvincePage.Location = new Point(4, 24);
         ProvincePage.Margin = new Padding(0);
         ProvincePage.Name = "ProvincePage";
         ProvincePage.Size = new Size(392, 822);
         ProvincePage.TabIndex = 0;
         ProvincePage.Text = "Province";
         ProvincePage.UseVisualStyleBackColor = true;
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Controls.Add(groupBox1, 0, 0);
         tableLayoutPanel1.Controls.Add(groupBox2, 1, 0);
         tableLayoutPanel1.Controls.Add(groupBox3, 1, 2);
         tableLayoutPanel1.Controls.Add(groupBox4, 0, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Margin = new Padding(0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 4;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 71F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 518F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
         tableLayoutPanel1.Size = new Size(392, 822);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // groupBox1
         // 
         groupBox1.Controls.Add(OwnerControllerLayoutPanel);
         groupBox1.Dock = DockStyle.Fill;
         groupBox1.Location = new Point(3, 0);
         groupBox1.Margin = new Padding(3, 0, 3, 0);
         groupBox1.Name = "groupBox1";
         groupBox1.Padding = new Padding(0);
         groupBox1.Size = new Size(190, 71);
         groupBox1.TabIndex = 0;
         groupBox1.TabStop = false;
         groupBox1.Text = "Country";
         // 
         // OwnerControllerLayoutPanel
         // 
         OwnerControllerLayoutPanel.ColumnCount = 2;
         OwnerControllerLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         OwnerControllerLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         OwnerControllerLayoutPanel.Controls.Add(label1, 0, 0);
         OwnerControllerLayoutPanel.Controls.Add(label2, 0, 1);
         OwnerControllerLayoutPanel.Location = new Point(0, 16);
         OwnerControllerLayoutPanel.Margin = new Padding(0, 3, 0, 0);
         OwnerControllerLayoutPanel.Name = "OwnerControllerLayoutPanel";
         OwnerControllerLayoutPanel.RowCount = 2;
         OwnerControllerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         OwnerControllerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         OwnerControllerLayoutPanel.Size = new Size(190, 50);
         OwnerControllerLayoutPanel.TabIndex = 0;
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Location = new Point(3, 0);
         label1.Name = "label1";
         label1.Size = new Size(89, 25);
         label1.TabIndex = 0;
         label1.Text = "Owner";
         label1.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // label2
         // 
         label2.AutoSize = true;
         label2.Dock = DockStyle.Fill;
         label2.Location = new Point(3, 25);
         label2.Name = "label2";
         label2.Size = new Size(89, 25);
         label2.TabIndex = 1;
         label2.Text = "Controller";
         label2.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // groupBox2
         // 
         groupBox2.Controls.Add(tableLayoutPanel2);
         groupBox2.Location = new Point(199, 0);
         groupBox2.Margin = new Padding(3, 0, 3, 0);
         groupBox2.Name = "groupBox2";
         groupBox2.Padding = new Padding(0);
         tableLayoutPanel1.SetRowSpan(groupBox2, 2);
         groupBox2.Size = new Size(190, 90);
         groupBox2.TabIndex = 1;
         groupBox2.TabStop = false;
         groupBox2.Text = "Religion, Culture, Capital";
         // 
         // tableLayoutPanel2
         // 
         tableLayoutPanel2.ColumnCount = 2;
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel2.Controls.Add(CultureComboBox, 1, 1);
         tableLayoutPanel2.Controls.Add(label5, 0, 2);
         tableLayoutPanel2.Controls.Add(label3, 0, 0);
         tableLayoutPanel2.Controls.Add(label4, 0, 1);
         tableLayoutPanel2.Controls.Add(ReligionComboBox, 1, 0);
         tableLayoutPanel2.Controls.Add(CapitalNameTextBox, 1, 2);
         tableLayoutPanel2.Location = new Point(0, 15);
         tableLayoutPanel2.Margin = new Padding(0, 3, 0, 0);
         tableLayoutPanel2.Name = "tableLayoutPanel2";
         tableLayoutPanel2.RowCount = 3;
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel2.Size = new Size(190, 75);
         tableLayoutPanel2.TabIndex = 1;
         // 
         // CultureComboBox
         // 
         CultureComboBox.FormattingEnabled = true;
         CultureComboBox.Location = new Point(95, 25);
         CultureComboBox.Margin = new Padding(0);
         CultureComboBox.Name = "CultureComboBox";
         CultureComboBox.Size = new Size(95, 23);
         CultureComboBox.TabIndex = 4;
         // 
         // label5
         // 
         label5.AutoSize = true;
         label5.Dock = DockStyle.Fill;
         label5.Location = new Point(3, 50);
         label5.Name = "label5";
         label5.Size = new Size(89, 25);
         label5.TabIndex = 2;
         label5.Text = "Capital";
         label5.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // label3
         // 
         label3.AutoSize = true;
         label3.Dock = DockStyle.Fill;
         label3.Location = new Point(3, 0);
         label3.Name = "label3";
         label3.Size = new Size(89, 25);
         label3.TabIndex = 0;
         label3.Text = "Religion";
         label3.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // label4
         // 
         label4.AutoSize = true;
         label4.Dock = DockStyle.Fill;
         label4.Location = new Point(3, 25);
         label4.Name = "label4";
         label4.Size = new Size(89, 25);
         label4.TabIndex = 1;
         label4.Text = "Culture";
         label4.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // ReligionComboBox
         // 
         ReligionComboBox.Dock = DockStyle.Fill;
         ReligionComboBox.FormattingEnabled = true;
         ReligionComboBox.Location = new Point(95, 0);
         ReligionComboBox.Margin = new Padding(0);
         ReligionComboBox.Name = "ReligionComboBox";
         ReligionComboBox.Size = new Size(95, 23);
         ReligionComboBox.TabIndex = 3;
         // 
         // CapitalNameTextBox
         // 
         CapitalNameTextBox.Dock = DockStyle.Fill;
         CapitalNameTextBox.Location = new Point(95, 50);
         CapitalNameTextBox.Margin = new Padding(0);
         CapitalNameTextBox.Name = "CapitalNameTextBox";
         CapitalNameTextBox.Size = new Size(95, 23);
         CapitalNameTextBox.TabIndex = 5;
         // 
         // groupBox3
         // 
         groupBox3.Controls.Add(tableLayoutPanel3);
         groupBox3.Location = new Point(199, 96);
         groupBox3.Margin = new Padding(3, 0, 3, 0);
         groupBox3.Name = "groupBox3";
         groupBox3.Padding = new Padding(0);
         groupBox3.Size = new Size(190, 90);
         groupBox3.TabIndex = 2;
         groupBox3.TabStop = false;
         groupBox3.Text = "Development";
         // 
         // tableLayoutPanel3
         // 
         tableLayoutPanel3.ColumnCount = 2;
         tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel3.Controls.Add(label6, 0, 2);
         tableLayoutPanel3.Controls.Add(label7, 0, 0);
         tableLayoutPanel3.Controls.Add(label8, 0, 1);
         tableLayoutPanel3.Controls.Add(ManpowerDevTextBox, 1, 2);
         tableLayoutPanel3.Controls.Add(ProductionDevTextBox, 1, 1);
         tableLayoutPanel3.Controls.Add(TaxDevTextBox, 1, 0);
         tableLayoutPanel3.Location = new Point(0, 8);
         tableLayoutPanel3.Margin = new Padding(0, 3, 0, 0);
         tableLayoutPanel3.Name = "tableLayoutPanel3";
         tableLayoutPanel3.RowCount = 3;
         tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
         tableLayoutPanel3.Size = new Size(190, 75);
         tableLayoutPanel3.TabIndex = 2;
         tableLayoutPanel3.Paint += tableLayoutPanel3_Paint;
         // 
         // label6
         // 
         label6.AutoSize = true;
         label6.Dock = DockStyle.Fill;
         label6.Location = new Point(3, 50);
         label6.Name = "label6";
         label6.Size = new Size(89, 25);
         label6.TabIndex = 2;
         label6.Text = "Manpower";
         label6.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // label7
         // 
         label7.AutoSize = true;
         label7.Dock = DockStyle.Fill;
         label7.Location = new Point(3, 0);
         label7.Name = "label7";
         label7.Size = new Size(89, 25);
         label7.TabIndex = 0;
         label7.Text = "Tax";
         label7.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // label8
         // 
         label8.AutoSize = true;
         label8.Dock = DockStyle.Fill;
         label8.Location = new Point(3, 25);
         label8.Name = "label8";
         label8.Size = new Size(89, 25);
         label8.TabIndex = 1;
         label8.Text = "Production";
         label8.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // ManpowerDevTextBox
         // 
         ManpowerDevTextBox.Dock = DockStyle.Fill;
         ManpowerDevTextBox.Location = new Point(95, 50);
         ManpowerDevTextBox.Margin = new Padding(0);
         ManpowerDevTextBox.Name = "ManpowerDevTextBox";
         ManpowerDevTextBox.Size = new Size(95, 23);
         ManpowerDevTextBox.TabIndex = 5;
         // 
         // ProductionDevTextBox
         // 
         ProductionDevTextBox.Dock = DockStyle.Fill;
         ProductionDevTextBox.Location = new Point(95, 25);
         ProductionDevTextBox.Margin = new Padding(0);
         ProductionDevTextBox.Name = "ProductionDevTextBox";
         ProductionDevTextBox.Size = new Size(95, 23);
         ProductionDevTextBox.TabIndex = 6;
         // 
         // TaxDevTextBox
         // 
         TaxDevTextBox.Dock = DockStyle.Fill;
         TaxDevTextBox.Location = new Point(95, 0);
         TaxDevTextBox.Margin = new Padding(0);
         TaxDevTextBox.Name = "TaxDevTextBox";
         TaxDevTextBox.Size = new Size(95, 23);
         TaxDevTextBox.TabIndex = 7;
         // 
         // groupBox4
         // 
         groupBox4.Controls.Add(tableLayoutPanel4);
         groupBox4.Dock = DockStyle.Fill;
         groupBox4.Location = new Point(3, 71);
         groupBox4.Margin = new Padding(3, 0, 3, 0);
         groupBox4.Name = "groupBox4";
         groupBox4.Padding = new Padding(0);
         tableLayoutPanel1.SetRowSpan(groupBox4, 2);
         groupBox4.Size = new Size(190, 115);
         groupBox4.TabIndex = 3;
         groupBox4.TabStop = false;
         groupBox4.Text = "Booleans";
         // 
         // tableLayoutPanel4
         // 
         tableLayoutPanel4.ColumnCount = 1;
         tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel4.Controls.Add(IsCityCheckBox, 0, 0);
         tableLayoutPanel4.Controls.Add(IsHreCheckBox, 0, 1);
         tableLayoutPanel4.Controls.Add(IsParlimentSeatCheckbox, 0, 2);
         tableLayoutPanel4.Controls.Add(HasRevoltCheckBox, 0, 3);
         tableLayoutPanel4.Location = new Point(0, 15);
         tableLayoutPanel4.Margin = new Padding(0, 3, 0, 0);
         tableLayoutPanel4.Name = "tableLayoutPanel4";
         tableLayoutPanel4.RowCount = 4;
         tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel4.Size = new Size(190, 100);
         tableLayoutPanel4.TabIndex = 3;
         // 
         // IsCityCheckBox
         // 
         IsCityCheckBox.AutoSize = true;
         IsCityCheckBox.Cursor = Cursors.Hand;
         IsCityCheckBox.Dock = DockStyle.Fill;
         IsCityCheckBox.Location = new Point(12, 3);
         IsCityCheckBox.Margin = new Padding(12, 3, 3, 3);
         IsCityCheckBox.Name = "IsCityCheckBox";
         IsCityCheckBox.Size = new Size(175, 19);
         IsCityCheckBox.TabIndex = 0;
         IsCityCheckBox.Text = "IsCity";
         IsCityCheckBox.UseVisualStyleBackColor = true;
         // 
         // IsHreCheckBox
         // 
         IsHreCheckBox.AutoSize = true;
         IsHreCheckBox.Cursor = Cursors.Hand;
         IsHreCheckBox.Dock = DockStyle.Fill;
         IsHreCheckBox.Location = new Point(12, 28);
         IsHreCheckBox.Margin = new Padding(12, 3, 3, 3);
         IsHreCheckBox.Name = "IsHreCheckBox";
         IsHreCheckBox.Size = new Size(175, 19);
         IsHreCheckBox.TabIndex = 1;
         IsHreCheckBox.Text = "IsHre";
         IsHreCheckBox.UseVisualStyleBackColor = true;
         // 
         // IsParlimentSeatCheckbox
         // 
         IsParlimentSeatCheckbox.AutoSize = true;
         IsParlimentSeatCheckbox.Cursor = Cursors.Hand;
         IsParlimentSeatCheckbox.Dock = DockStyle.Fill;
         IsParlimentSeatCheckbox.Location = new Point(12, 53);
         IsParlimentSeatCheckbox.Margin = new Padding(12, 3, 3, 3);
         IsParlimentSeatCheckbox.Name = "IsParlimentSeatCheckbox";
         IsParlimentSeatCheckbox.Size = new Size(175, 19);
         IsParlimentSeatCheckbox.TabIndex = 2;
         IsParlimentSeatCheckbox.Text = "IsParlimentSeat";
         IsParlimentSeatCheckbox.UseVisualStyleBackColor = true;
         // 
         // HasRevoltCheckBox
         // 
         HasRevoltCheckBox.AutoSize = true;
         HasRevoltCheckBox.Cursor = Cursors.Hand;
         HasRevoltCheckBox.Dock = DockStyle.Fill;
         HasRevoltCheckBox.Location = new Point(12, 78);
         HasRevoltCheckBox.Margin = new Padding(12, 3, 3, 3);
         HasRevoltCheckBox.Name = "HasRevoltCheckBox";
         HasRevoltCheckBox.Size = new Size(175, 19);
         HasRevoltCheckBox.TabIndex = 3;
         HasRevoltCheckBox.Text = "HasRevolt";
         HasRevoltCheckBox.UseVisualStyleBackColor = true;
         // 
         // COuntryPage
         // 
         COuntryPage.Location = new Point(4, 24);
         COuntryPage.Name = "COuntryPage";
         COuntryPage.Padding = new Padding(3);
         COuntryPage.Size = new Size(392, 822);
         COuntryPage.TabIndex = 1;
         COuntryPage.Text = "Country";
         COuntryPage.UseVisualStyleBackColor = true;
         // 
         // ProvinceGroupsPage
         // 
         ProvinceGroupsPage.Location = new Point(4, 24);
         ProvinceGroupsPage.Name = "ProvinceGroupsPage";
         ProvinceGroupsPage.Padding = new Padding(3);
         ProvinceGroupsPage.Size = new Size(392, 822);
         ProvinceGroupsPage.TabIndex = 2;
         ProvinceGroupsPage.Text = "ProvinceGroups";
         ProvinceGroupsPage.UseVisualStyleBackColor = true;
         // 
         // MapWindow
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(1511, 927);
         Controls.Add(MainLayoutPanel);
         Controls.Add(MapMenuStrip);
         KeyPreview = true;
         MainMenuStrip = MapMenuStrip;
         Margin = new Padding(4, 3, 4, 3);
         Name = "MapWindow";
         SizeGripStyle = SizeGripStyle.Show;
         StartPosition = FormStartPosition.CenterParent;
         Text = "MapWindow";
         FormClosing += MapWindow_FormClosing;
         Load += MapWindow_Load;
         KeyDown += MapWindow_KeyDown;
         MapMenuStrip.ResumeLayout(false);
         MapMenuStrip.PerformLayout();
         toolStrip1.ResumeLayout(false);
         toolStrip1.PerformLayout();
         MainLayoutPanel.ResumeLayout(false);
         MainLayoutPanel.PerformLayout();
         TopStripLayoutPanel.ResumeLayout(false);
         TopStripLayoutPanel.PerformLayout();
         toolStripContainer1.ContentPanel.ResumeLayout(false);
         toolStripContainer1.RightToolStripPanel.ResumeLayout(false);
         toolStripContainer1.RightToolStripPanel.PerformLayout();
         toolStripContainer1.ResumeLayout(false);
         toolStripContainer1.PerformLayout();
         toolStrip2.ResumeLayout(false);
         toolStrip2.PerformLayout();
         DataTabPanel.ResumeLayout(false);
         ProvincePage.ResumeLayout(false);
         tableLayoutPanel1.ResumeLayout(false);
         groupBox1.ResumeLayout(false);
         OwnerControllerLayoutPanel.ResumeLayout(false);
         OwnerControllerLayoutPanel.PerformLayout();
         groupBox2.ResumeLayout(false);
         tableLayoutPanel2.ResumeLayout(false);
         tableLayoutPanel2.PerformLayout();
         groupBox3.ResumeLayout(false);
         tableLayoutPanel3.ResumeLayout(false);
         tableLayoutPanel3.PerformLayout();
         groupBox4.ResumeLayout(false);
         tableLayoutPanel4.ResumeLayout(false);
         tableLayoutPanel4.PerformLayout();
         ResumeLayout(false);
         PerformLayout();
      }

      #endregion

      private System.Windows.Forms.MenuStrip MapMenuStrip;
      private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
      private System.Windows.Forms.Panel MapPanel;
      private System.Windows.Forms.ToolStrip toolStrip1;
      private System.Windows.Forms.ToolStripLabel SelectedProvinceSum;
      private System.Windows.Forms.ToolStripLabel RamUsageStrip;
      private System.Windows.Forms.ToolStripLabel CpuUsageStrip;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
      private System.Windows.Forms.ToolStripLabel UndoDepthLabel;
      private System.Windows.Forms.ToolStripLabel RedoDepthLabel;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
      private System.Windows.Forms.ToolStripMenuItem historyToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem selectionHistoryToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem DeleteHistoryToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
      public System.Windows.Forms.ToolStripComboBox MapModeComboBox;
      private System.Windows.Forms.ToolStripMenuItem gCToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem saveCurrentMapModeToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem toolTipCustomizerToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem openCustomizerToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem ShowToolTipMenuItem;
      private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem telescopeToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem refStackToolStripMenuItem;
      private TableLayoutPanel MainLayoutPanel;
      private ToolStripLabel toolStripLabel1;
      private ToolStripSplitButton toolStripSplitButton1;
      private TableLayoutPanel TopStripLayoutPanel;
      private ToolStripContainer toolStripContainer1;
      private ToolStrip toolStrip2;
      private ToolStripButton newToolStripButton;
      private ToolStripButton openToolStripButton;
      private ToolStripButton saveToolStripButton;
      private ToolStripButton printToolStripButton;
      private ToolStripSeparator toolStripSeparator;
      private ToolStripButton cutToolStripButton;
      private ToolStripButton copyToolStripButton;
      private ToolStripButton pasteToolStripButton;
      private ToolStripSeparator toolStripSeparator3;
      private ToolStripButton helpToolStripButton;
      private ToolStripComboBox DateSelector;
      private ToolStripMenuItem searchToolStripMenuItem;
      private Label OwnerCountryNameLabel;
      private Label ProvinceNameLabel;
      private TabControl DataTabPanel;
      private TabPage ProvincePage;
      private TabPage COuntryPage;
      private TabPage ProvinceGroupsPage;
      private TableLayoutPanel tableLayoutPanel1;
      private GroupBox groupBox1;
      private TableLayoutPanel OwnerControllerLayoutPanel;
      private Label label1;
      private Label label2;
      private GroupBox groupBox2;
      private TableLayoutPanel tableLayoutPanel2;
      private Label label5;
      private Label label3;
      private Label label4;
      private ComboBox ReligionComboBox;
      private ComboBox CultureComboBox;
      private TextBox CapitalNameTextBox;
      private GroupBox groupBox3;
      private TableLayoutPanel tableLayoutPanel3;
      private Label label6;
      private Label label7;
      private Label label8;
      private TextBox ManpowerDevTextBox;
      private TextBox ProductionDevTextBox;
      private TextBox TaxDevTextBox;
      private GroupBox groupBox4;
      private TableLayoutPanel tableLayoutPanel4;
      private CheckBox IsCityCheckBox;
      private CheckBox IsHreCheckBox;
      private CheckBox IsParlimentSeatCheckbox;
      private CheckBox HasRevoltCheckBox;
   }
}

