namespace Editor.src.Forms.Feature
{
   partial class MissionTreeExporter
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MissionTreeExporter));
         tableLayoutPanel1 = new TableLayoutPanel();
         ExportButton = new Button();
         BackgroundCheckBox = new CheckBox();
         SelectMissionFile = new ComboBox();
         MissionNameBox = new CheckBox();
         FullBgBox = new CheckBox();
         PrewViewBox = new PictureBox();
         tableLayoutPanel1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)PrewViewBox).BeginInit();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 73.5F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26.5F));
         tableLayoutPanel1.Controls.Add(ExportButton, 1, 4);
         tableLayoutPanel1.Controls.Add(BackgroundCheckBox, 1, 1);
         tableLayoutPanel1.Controls.Add(SelectMissionFile, 1, 0);
         tableLayoutPanel1.Controls.Add(MissionNameBox, 1, 2);
         tableLayoutPanel1.Controls.Add(FullBgBox, 1, 3);
         tableLayoutPanel1.Controls.Add(PrewViewBox, 0, 0);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 6;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Size = new Size(800, 450);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // ExportButton
         // 
         ExportButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
         ExportButton.Location = new Point(591, 123);
         ExportButton.Name = "ExportButton";
         ExportButton.Size = new Size(75, 24);
         ExportButton.TabIndex = 2;
         ExportButton.Text = "Export";
         ExportButton.UseVisualStyleBackColor = true;
         ExportButton.Click += ExportButton_Click;
         // 
         // BackgroundCheckBox
         // 
         BackgroundCheckBox.AutoSize = true;
         BackgroundCheckBox.Dock = DockStyle.Fill;
         BackgroundCheckBox.Location = new Point(593, 33);
         BackgroundCheckBox.Margin = new Padding(5, 3, 3, 3);
         BackgroundCheckBox.Name = "BackgroundCheckBox";
         BackgroundCheckBox.Size = new Size(204, 24);
         BackgroundCheckBox.TabIndex = 1;
         BackgroundCheckBox.Text = "Background";
         BackgroundCheckBox.UseVisualStyleBackColor = true;
         // 
         // SelectMissionFile
         // 
         SelectMissionFile.Dock = DockStyle.Fill;
         SelectMissionFile.DropDownStyle = ComboBoxStyle.DropDownList;
         SelectMissionFile.FormattingEnabled = true;
         SelectMissionFile.Location = new Point(591, 3);
         SelectMissionFile.Name = "SelectMissionFile";
         SelectMissionFile.Size = new Size(206, 23);
         SelectMissionFile.TabIndex = 0;
         SelectMissionFile.SelectedIndexChanged += SelectMissionFile_SelectedIndexChanged;
         // 
         // MissionNameBox
         // 
         MissionNameBox.AutoSize = true;
         MissionNameBox.Dock = DockStyle.Fill;
         MissionNameBox.Location = new Point(593, 63);
         MissionNameBox.Margin = new Padding(5, 3, 3, 3);
         MissionNameBox.Name = "MissionNameBox";
         MissionNameBox.Size = new Size(204, 24);
         MissionNameBox.TabIndex = 3;
         MissionNameBox.Text = "Mission Names";
         MissionNameBox.UseVisualStyleBackColor = true;
         // 
         // FullBgBox
         // 
         FullBgBox.AutoSize = true;
         FullBgBox.Dock = DockStyle.Fill;
         FullBgBox.Location = new Point(593, 93);
         FullBgBox.Margin = new Padding(5, 3, 3, 3);
         FullBgBox.Name = "FullBgBox";
         FullBgBox.Size = new Size(204, 24);
         FullBgBox.TabIndex = 4;
         FullBgBox.Text = "Full window background";
         FullBgBox.UseVisualStyleBackColor = true;
         // 
         // PrewViewBox
         // 
         PrewViewBox.BackColor = SystemColors.AppWorkspace;
         PrewViewBox.Dock = DockStyle.Fill;
         PrewViewBox.Location = new Point(3, 3);
         PrewViewBox.Name = "PrewViewBox";
         tableLayoutPanel1.SetRowSpan(PrewViewBox, 6);
         PrewViewBox.Size = new Size(582, 444);
         PrewViewBox.SizeMode = PictureBoxSizeMode.Zoom;
         PrewViewBox.TabIndex = 5;
         PrewViewBox.TabStop = false;
         // 
         // MissionTreeExporter
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(800, 450);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "MissionTreeExporter";
         Text = "Mission Tree Exporter";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)PrewViewBox).EndInit();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private ComboBox SelectMissionFile;
      private CheckBox BackgroundCheckBox;
      private Button ExportButton;
      private CheckBox MissionNameBox;
      private CheckBox FullBgBox;
      private PictureBox PrewViewBox;
   }
}