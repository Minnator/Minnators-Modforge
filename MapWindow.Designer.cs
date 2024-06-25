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
         this.MapMenuStrip = new System.Windows.Forms.MenuStrip();
         this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.historyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.selectionHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.DeleteHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.MapPanel = new System.Windows.Forms.Panel();
         this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
         this.toolStrip1 = new System.Windows.Forms.ToolStrip();
         this.RamUsageStrip = new System.Windows.Forms.ToolStripLabel();
         this.CpuUsageStrip = new System.Windows.Forms.ToolStripLabel();
         this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
         this.UndoDepthLabel = new System.Windows.Forms.ToolStripLabel();
         this.RedoDepthLabel = new System.Windows.Forms.ToolStripLabel();
         this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
         this.SelectedProvinceSum = new System.Windows.Forms.ToolStripLabel();
         this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.MapMenuStrip.SuspendLayout();
         this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
         this.toolStripContainer1.ContentPanel.SuspendLayout();
         this.toolStripContainer1.SuspendLayout();
         this.toolStrip1.SuspendLayout();
         this.SuspendLayout();
         // 
         // MapMenuStrip
         // 
         this.MapMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesToolStripMenuItem,
            this.historyToolStripMenuItem,
            this.debugToolStripMenuItem});
         this.MapMenuStrip.Location = new System.Drawing.Point(0, 0);
         this.MapMenuStrip.Name = "MapMenuStrip";
         this.MapMenuStrip.Size = new System.Drawing.Size(1295, 24);
         this.MapMenuStrip.TabIndex = 0;
         this.MapMenuStrip.Text = "menuStrip1";
         // 
         // filesToolStripMenuItem
         // 
         this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
         this.filesToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
         this.filesToolStripMenuItem.Text = "Files";
         // 
         // historyToolStripMenuItem
         // 
         this.historyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectionHistoryToolStripMenuItem,
            this.DeleteHistoryToolStripMenuItem});
         this.historyToolStripMenuItem.Name = "historyToolStripMenuItem";
         this.historyToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
         this.historyToolStripMenuItem.Text = "History";
         // 
         // selectionHistoryToolStripMenuItem
         // 
         this.selectionHistoryToolStripMenuItem.Name = "selectionHistoryToolStripMenuItem";
         this.selectionHistoryToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
         this.selectionHistoryToolStripMenuItem.Text = "View History Tree";
         this.selectionHistoryToolStripMenuItem.Click += new System.EventHandler(this.RevertInSelectionHistory);
         // 
         // DeleteHistoryToolStripMenuItem
         // 
         this.DeleteHistoryToolStripMenuItem.Name = "DeleteHistoryToolStripMenuItem";
         this.DeleteHistoryToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
         this.DeleteHistoryToolStripMenuItem.Text = "Delete History";
         this.DeleteHistoryToolStripMenuItem.Click += new System.EventHandler(this.DeleteHistoryToolStripMenuItem_Click);
         // 
         // MapPanel
         // 
         this.MapPanel.AutoScroll = true;
         this.MapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
         this.MapPanel.Location = new System.Drawing.Point(0, 0);
         this.MapPanel.Name = "MapPanel";
         this.MapPanel.Size = new System.Drawing.Size(1295, 754);
         this.MapPanel.TabIndex = 1;
         // 
         // toolStripContainer1
         // 
         // 
         // toolStripContainer1.BottomToolStripPanel
         // 
         this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.toolStrip1);
         // 
         // toolStripContainer1.ContentPanel
         // 
         this.toolStripContainer1.ContentPanel.Controls.Add(this.MapPanel);
         this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1295, 754);
         this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.toolStripContainer1.LeftToolStripPanelVisible = false;
         this.toolStripContainer1.Location = new System.Drawing.Point(0, 24);
         this.toolStripContainer1.Name = "toolStripContainer1";
         this.toolStripContainer1.RightToolStripPanelVisible = false;
         this.toolStripContainer1.Size = new System.Drawing.Size(1295, 779);
         this.toolStripContainer1.TabIndex = 2;
         this.toolStripContainer1.Text = "toolStripContainer1";
         this.toolStripContainer1.TopToolStripPanelVisible = false;
         // 
         // toolStrip1
         // 
         this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
         this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RamUsageStrip,
            this.CpuUsageStrip,
            this.toolStripSeparator1,
            this.UndoDepthLabel,
            this.RedoDepthLabel,
            this.toolStripSeparator2,
            this.SelectedProvinceSum});
         this.toolStrip1.Location = new System.Drawing.Point(3, 0);
         this.toolStrip1.Name = "toolStrip1";
         this.toolStrip1.Size = new System.Drawing.Size(302, 25);
         this.toolStrip1.TabIndex = 0;
         // 
         // RamUsageStrip
         // 
         this.RamUsageStrip.Name = "RamUsageStrip";
         this.RamUsageStrip.Size = new System.Drawing.Size(48, 22);
         this.RamUsageStrip.Text = "Ram [0]";
         // 
         // CpuUsageStrip
         // 
         this.CpuUsageStrip.Name = "CpuUsageStrip";
         this.CpuUsageStrip.Size = new System.Drawing.Size(47, 22);
         this.CpuUsageStrip.Text = "CPU [0]";
         // 
         // toolStripSeparator1
         // 
         this.toolStripSeparator1.Name = "toolStripSeparator1";
         this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
         // 
         // UndoDepthLabel
         // 
         this.UndoDepthLabel.Name = "UndoDepthLabel";
         this.UndoDepthLabel.Size = new System.Drawing.Size(68, 22);
         this.UndoDepthLabel.Text = "UndoDepth";
         // 
         // RedoDepthLabel
         // 
         this.RedoDepthLabel.Name = "RedoDepthLabel";
         this.RedoDepthLabel.Size = new System.Drawing.Size(66, 22);
         this.RedoDepthLabel.Text = "RedoDepth";
         // 
         // toolStripSeparator2
         // 
         this.toolStripSeparator2.Name = "toolStripSeparator2";
         this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
         // 
         // SelectedProvinceSum
         // 
         this.SelectedProvinceSum.Name = "SelectedProvinceSum";
         this.SelectedProvinceSum.Size = new System.Drawing.Size(49, 22);
         this.SelectedProvinceSum.Text = "ProSum";
         // 
         // debugToolStripMenuItem
         // 
         this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
         this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
         this.debugToolStripMenuItem.Text = "Debug";
         this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem_Click);
         // 
         // MapWindow
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1295, 803);
         this.Controls.Add(this.toolStripContainer1);
         this.Controls.Add(this.MapMenuStrip);
         this.MainMenuStrip = this.MapMenuStrip;
         this.Name = "MapWindow";
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "MapWindow";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MapWindow_FormClosing);
         this.MapMenuStrip.ResumeLayout(false);
         this.MapMenuStrip.PerformLayout();
         this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
         this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
         this.toolStripContainer1.ContentPanel.ResumeLayout(false);
         this.toolStripContainer1.ResumeLayout(false);
         this.toolStripContainer1.PerformLayout();
         this.toolStrip1.ResumeLayout(false);
         this.toolStrip1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

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
   }
}

