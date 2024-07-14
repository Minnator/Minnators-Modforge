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
         tableLayoutPanel1 = new TableLayoutPanel();
         tableLayoutPanel2 = new TableLayoutPanel();
         DateLabel = new Label();
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
         MapMenuStrip.SuspendLayout();
         toolStrip1.SuspendLayout();
         tableLayoutPanel1.SuspendLayout();
         tableLayoutPanel2.SuspendLayout();
         toolStripContainer1.ContentPanel.SuspendLayout();
         toolStripContainer1.RightToolStripPanel.SuspendLayout();
         toolStripContainer1.SuspendLayout();
         toolStrip2.SuspendLayout();
         SuspendLayout();
         // 
         // MapMenuStrip
         // 
         MapMenuStrip.Items.AddRange(new ToolStripItem[] { filesToolStripMenuItem, historyToolStripMenuItem, toolTipCustomizerToolStripMenuItem, MapModeComboBox, debugToolStripMenuItem, DateSelector });
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
         testToolStripMenuItem.Size = new Size(125, 22);
         testToolStripMenuItem.Text = "Test";
         testToolStripMenuItem.Click += testToolStripMenuItem_Click;
         // 
         // telescopeToolStripMenuItem
         // 
         telescopeToolStripMenuItem.Name = "telescopeToolStripMenuItem";
         telescopeToolStripMenuItem.Size = new Size(125, 22);
         telescopeToolStripMenuItem.Text = "Telescope";
         telescopeToolStripMenuItem.Click += telescopeToolStripMenuItem_Click;
         // 
         // refStackToolStripMenuItem
         // 
         refStackToolStripMenuItem.Name = "refStackToolStripMenuItem";
         refStackToolStripMenuItem.Size = new Size(125, 22);
         refStackToolStripMenuItem.Click += refStackToolStripMenuItem_Click;
         // 
         // DateSelector
         // 
         DateSelector.Items.AddRange(new object[] { "1444.11.11", "1500.1.1", "1600.1.1" });
         DateSelector.Name = "DateSelector";
         DateSelector.Size = new Size(121, 23);
         DateSelector.SelectedIndexChanged += DateSelector_SelectedIndexChanged;
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
         tableLayoutPanel1.SetColumnSpan(toolStrip1, 2);
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
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Controls.Add(toolStrip1, 0, 2);
         tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 0);
         tableLayoutPanel1.Controls.Add(toolStripContainer1, 1, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 27);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
         tableLayoutPanel1.Size = new Size(1511, 900);
         tableLayoutPanel1.TabIndex = 3;
         // 
         // tableLayoutPanel2
         // 
         tableLayoutPanel2.ColumnCount = 5;
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         tableLayoutPanel2.Controls.Add(DateLabel, 4, 0);
         tableLayoutPanel2.Dock = DockStyle.Fill;
         tableLayoutPanel2.Location = new Point(400, 0);
         tableLayoutPanel2.Margin = new Padding(0);
         tableLayoutPanel2.Name = "tableLayoutPanel2";
         tableLayoutPanel2.RowCount = 1;
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel2.Size = new Size(1111, 26);
         tableLayoutPanel2.TabIndex = 2;
         // 
         // DateLabel
         // 
         DateLabel.AutoSize = true;
         DateLabel.Dock = DockStyle.Fill;
         DateLabel.Location = new Point(891, 3);
         DateLabel.Margin = new Padding(3);
         DateLabel.Name = "DateLabel";
         DateLabel.Size = new Size(217, 20);
         DateLabel.TabIndex = 0;
         DateLabel.Text = "-:-:-";
         DateLabel.TextAlign = ContentAlignment.MiddleCenter;
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
         // MapWindow
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(1511, 927);
         Controls.Add(tableLayoutPanel1);
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
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         tableLayoutPanel2.ResumeLayout(false);
         tableLayoutPanel2.PerformLayout();
         toolStripContainer1.ContentPanel.ResumeLayout(false);
         toolStripContainer1.RightToolStripPanel.ResumeLayout(false);
         toolStripContainer1.RightToolStripPanel.PerformLayout();
         toolStripContainer1.ResumeLayout(false);
         toolStripContainer1.PerformLayout();
         toolStrip2.ResumeLayout(false);
         toolStrip2.PerformLayout();
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
      private TableLayoutPanel tableLayoutPanel1;
      private ToolStripLabel toolStripLabel1;
      private ToolStripSplitButton toolStripSplitButton1;
      private TableLayoutPanel tableLayoutPanel2;
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
      public Label DateLabel;
      private ToolStripComboBox DateSelector;
   }
}

