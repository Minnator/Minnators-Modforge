namespace Editor.Forms.PopUps
{
   partial class LogEntryViewer
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogEntryViewer));
         MainTLP = new TableLayoutPanel();
         MessageLabel = new Label();
         DescriptionLabel = new Label();
         ResolutionLabel = new Label();
         MainTLP.SuspendLayout();
         SuspendLayout();
         // 
         // MainTLP
         // 
         MainTLP.ColumnCount = 1;
         MainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         MainTLP.Controls.Add(MessageLabel, 0, 0);
         MainTLP.Controls.Add(DescriptionLabel, 0, 1);
         MainTLP.Controls.Add(ResolutionLabel, 0, 2);
         MainTLP.Dock = DockStyle.Fill;
         MainTLP.Location = new Point(0, 0);
         MainTLP.Margin = new Padding(0);
         MainTLP.Name = "MainTLP";
         MainTLP.RowCount = 3;
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
         MainTLP.Size = new Size(455, 238);
         MainTLP.TabIndex = 0;
         // 
         // MessageLabel
         // 
         MessageLabel.AutoSize = true;
         MessageLabel.BackColor = SystemColors.Control;
         MessageLabel.Dock = DockStyle.Fill;
         MessageLabel.Location = new Point(2, 2);
         MessageLabel.Margin = new Padding(2);
         MessageLabel.Name = "MessageLabel";
         MessageLabel.Size = new Size(451, 138);
         MessageLabel.TabIndex = 0;
         MessageLabel.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // DescriptionLabel
         // 
         DescriptionLabel.AutoSize = true;
         DescriptionLabel.BackColor = SystemColors.Control;
         DescriptionLabel.Dock = DockStyle.Fill;
         DescriptionLabel.Location = new Point(2, 144);
         DescriptionLabel.Margin = new Padding(2);
         DescriptionLabel.Name = "DescriptionLabel";
         DescriptionLabel.Size = new Size(451, 43);
         DescriptionLabel.TabIndex = 1;
         DescriptionLabel.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // ResolutionLabel
         // 
         ResolutionLabel.AutoSize = true;
         ResolutionLabel.BackColor = SystemColors.Control;
         ResolutionLabel.Dock = DockStyle.Fill;
         ResolutionLabel.Location = new Point(2, 191);
         ResolutionLabel.Margin = new Padding(2);
         ResolutionLabel.Name = "ResolutionLabel";
         ResolutionLabel.Size = new Size(451, 45);
         ResolutionLabel.TabIndex = 2;
         ResolutionLabel.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // LogEntryViewer
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         BackColor = SystemColors.AppWorkspace;
         ClientSize = new Size(455, 238);
         Controls.Add(MainTLP);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "LogEntryViewer";
         Text = "LogEntryViewer";
         MainTLP.ResumeLayout(false);
         MainTLP.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel MainTLP;
      private Label MessageLabel;
      private Label DescriptionLabel;
      private Label ResolutionLabel;
   }
}