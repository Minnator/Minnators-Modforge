namespace Editor.Forms.PopUps
{
   partial class ProgressBarPopUp
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressBarPopUp));
         LayoutPanel = new TableLayoutPanel();
         DescriptionLabel = new Label();
         LayoutPanel.SuspendLayout();
         SuspendLayout();
         // 
         // LayoutPanel
         // 
         LayoutPanel.ColumnCount = 1;
         LayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         LayoutPanel.Controls.Add(DescriptionLabel, 0, 0);
         LayoutPanel.Dock = DockStyle.Fill;
         LayoutPanel.Location = new Point(0, 0);
         LayoutPanel.Name = "LayoutPanel";
         LayoutPanel.RowCount = 2;
         LayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         LayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
         LayoutPanel.Size = new Size(394, 97);
         LayoutPanel.TabIndex = 0;
         // 
         // DescriptionLabel
         // 
         DescriptionLabel.AutoSize = true;
         DescriptionLabel.Dock = DockStyle.Fill;
         DescriptionLabel.Location = new Point(3, 0);
         DescriptionLabel.Name = "DescriptionLabel";
         DescriptionLabel.Size = new Size(388, 57);
         DescriptionLabel.TabIndex = 0;
         DescriptionLabel.Text = "operation x";
         DescriptionLabel.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // ProgressBarPopUp
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(394, 97);
         Controls.Add(LayoutPanel);
         FormBorderStyle = FormBorderStyle.None;
         Icon = (Icon)resources.GetObject("$this.Icon");
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "ProgressBarPopUp";
         Text = "title";
         LayoutPanel.ResumeLayout(false);
         LayoutPanel.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel LayoutPanel;
      internal Label DescriptionLabel;
   }
}