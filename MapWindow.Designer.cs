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
         MapPanel = new Panel();
         toolStripContainer1 = new ToolStripContainer();
         toolStrip1 = new ToolStrip();
         RamUsageStrip = new ToolStripLabel();
         CpuUsageStrip = new ToolStripLabel();
         toolStripSeparator1 = new ToolStripSeparator();
         UndoDepthLabel = new ToolStripLabel();
         RedoDepthLabel = new ToolStripLabel();
         toolStripSeparator2 = new ToolStripSeparator();
         SelectedProvinceSum = new ToolStripLabel();
         MapMenuStrip.SuspendLayout();
         toolStripContainer1.BottomToolStripPanel.SuspendLayout();
         toolStripContainer1.ContentPanel.SuspendLayout();
         toolStripContainer1.SuspendLayout();
         toolStrip1.SuspendLayout();
         SuspendLayout();
         // 
         // MapMenuStrip
         // 
         MapMenuStrip.Items.AddRange(new ToolStripItem[] { filesToolStripMenuItem, historyToolStripMenuItem, toolTipCustomizerToolStripMenuItem, MapModeComboBox, debugToolStripMenuItem });
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
         // MapPanel
         // 
         MapPanel.AutoScroll = true;
         MapPanel.Dock = DockStyle.Fill;
         MapPanel.Location = new Point(0, 0);
         MapPanel.Margin = new Padding(4, 3, 4, 3);
         MapPanel.Name = "MapPanel";
         MapPanel.Size = new Size(1511, 875);
         MapPanel.TabIndex = 1;
         // 
         // toolStripContainer1
         // 
         // 
         // toolStripContainer1.BottomToolStripPanel
         // 
         toolStripContainer1.BottomToolStripPanel.Controls.Add(toolStrip1);
         // 
         // toolStripContainer1.ContentPanel
         // 
         toolStripContainer1.ContentPanel.Controls.Add(MapPanel);
         toolStripContainer1.ContentPanel.Margin = new Padding(4, 3, 4, 3);
         toolStripContainer1.ContentPanel.Size = new Size(1511, 875);
         toolStripContainer1.Dock = DockStyle.Fill;
         toolStripContainer1.LeftToolStripPanelVisible = false;
         toolStripContainer1.Location = new Point(0, 27);
         toolStripContainer1.Margin = new Padding(4, 3, 4, 3);
         toolStripContainer1.Name = "toolStripContainer1";
         toolStripContainer1.RightToolStripPanelVisible = false;
         toolStripContainer1.Size = new Size(1511, 900);
         toolStripContainer1.TabIndex = 2;
         toolStripContainer1.Text = "toolStripContainer1";
         toolStripContainer1.TopToolStripPanelVisible = false;
         // 
         // toolStrip1
         // 
         toolStrip1.Dock = DockStyle.None;
         toolStrip1.Items.AddRange(new ToolStripItem[] { RamUsageStrip, CpuUsageStrip, toolStripSeparator1, UndoDepthLabel, RedoDepthLabel, toolStripSeparator2, SelectedProvinceSum });
         toolStrip1.Location = new Point(3, 0);
         toolStrip1.Name = "toolStrip1";
         toolStrip1.Size = new Size(302, 25);
         toolStrip1.TabIndex = 0;
         // 
         // RamUsageStrip
         // 
         RamUsageStrip.Name = "RamUsageStrip";
         RamUsageStrip.Size = new Size(48, 22);
         RamUsageStrip.Text = "Ram [0]";
         // 
         // CpuUsageStrip
         // 
         CpuUsageStrip.Name = "CpuUsageStrip";
         CpuUsageStrip.Size = new Size(47, 22);
         CpuUsageStrip.Text = "CPU [0]";
         // 
         // toolStripSeparator1
         // 
         toolStripSeparator1.Name = "toolStripSeparator1";
         toolStripSeparator1.Size = new Size(6, 25);
         // 
         // UndoDepthLabel
         // 
         UndoDepthLabel.Name = "UndoDepthLabel";
         UndoDepthLabel.Size = new Size(68, 22);
         UndoDepthLabel.Text = "UndoDepth";
         // 
         // RedoDepthLabel
         // 
         RedoDepthLabel.Name = "RedoDepthLabel";
         RedoDepthLabel.Size = new Size(66, 22);
         RedoDepthLabel.Text = "RedoDepth";
         // 
         // toolStripSeparator2
         // 
         toolStripSeparator2.Name = "toolStripSeparator2";
         toolStripSeparator2.Size = new Size(6, 25);
         // 
         // SelectedProvinceSum
         // 
         SelectedProvinceSum.Name = "SelectedProvinceSum";
         SelectedProvinceSum.Size = new Size(49, 22);
         SelectedProvinceSum.Text = "ProSum";
         // 
         // MapWindow
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(1511, 927);
         Controls.Add(toolStripContainer1);
         Controls.Add(MapMenuStrip);
         KeyPreview = true;
         MainMenuStrip = MapMenuStrip;
         Margin = new Padding(4, 3, 4, 3);
         Name = "MapWindow";
         SizeGripStyle = SizeGripStyle.Show;
         StartPosition = FormStartPosition.CenterParent;
         Text = "MapWindow";
         FormClosing += MapWindow_FormClosing;
         KeyDown += MapWindow_KeyDown;
         MapMenuStrip.ResumeLayout(false);
         MapMenuStrip.PerformLayout();
         toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
         toolStripContainer1.BottomToolStripPanel.PerformLayout();
         toolStripContainer1.ContentPanel.ResumeLayout(false);
         toolStripContainer1.ResumeLayout(false);
         toolStripContainer1.PerformLayout();
         toolStrip1.ResumeLayout(false);
         toolStrip1.PerformLayout();
         ResumeLayout(false);
         PerformLayout();
      }

      #endregion

      private System.Windows.Forms.MenuStrip MapMenuStrip;
      private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
      private System.Windows.Forms.Panel MapPanel;
      private System.Windows.Forms.ToolStripContainer toolStripContainer1;
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
   }
}

