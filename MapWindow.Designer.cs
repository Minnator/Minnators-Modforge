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
         this.MapPanel = new System.Windows.Forms.Panel();
         this.MapMenuStrip.SuspendLayout();
         this.SuspendLayout();
         // 
         // MapMenuStrip
         // 
         this.MapMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesToolStripMenuItem});
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
         // MapPanel
         // 
         this.MapPanel.AutoScroll = true;
         this.MapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
         this.MapPanel.Location = new System.Drawing.Point(0, 24);
         this.MapPanel.Name = "MapPanel";
         this.MapPanel.Size = new System.Drawing.Size(1295, 779);
         this.MapPanel.TabIndex = 1;
         // 
         // MapWindow
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1295, 803);
         this.Controls.Add(this.MapPanel);
         this.Controls.Add(this.MapMenuStrip);
         this.MainMenuStrip = this.MapMenuStrip;
         this.Name = "MapWindow";
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "MapWindow";
         this.MapMenuStrip.ResumeLayout(false);
         this.MapMenuStrip.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.MenuStrip MapMenuStrip;
      private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
      private System.Windows.Forms.Panel MapPanel;
   }
}

