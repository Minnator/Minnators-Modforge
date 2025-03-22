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
         components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MissionTreeExporter));
         tableLayoutPanel1 = new TableLayoutPanel();
         BackgroundCheckBox = new CheckBox();
         SelectMissionFile = new ComboBox();
         MissionNameBox = new CheckBox();
         FullBgBox = new CheckBox();
         PrewViewBox = new PictureBox();
         MissionEffectIcon = new ComboBox();
         MissionFrameIcon = new ComboBox();
         CountrySelection = new ComboBox();
         SaveTextLabel = new Label();
         FileNameTextBox = new TextBox();
         ExportButton = new Button();
         CopyButton = new Button();
         toolTip1 = new ToolTip(components);
         tableLayoutPanel1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)PrewViewBox).BeginInit();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 73.5F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26.5F));
         tableLayoutPanel1.Controls.Add(BackgroundCheckBox, 1, 1);
         tableLayoutPanel1.Controls.Add(SelectMissionFile, 1, 0);
         tableLayoutPanel1.Controls.Add(MissionNameBox, 1, 2);
         tableLayoutPanel1.Controls.Add(FullBgBox, 1, 3);
         tableLayoutPanel1.Controls.Add(PrewViewBox, 0, 0);
         tableLayoutPanel1.Controls.Add(MissionEffectIcon, 1, 4);
         tableLayoutPanel1.Controls.Add(MissionFrameIcon, 1, 5);
         tableLayoutPanel1.Controls.Add(CountrySelection, 1, 6);
         tableLayoutPanel1.Controls.Add(SaveTextLabel, 1, 11);
         tableLayoutPanel1.Controls.Add(FileNameTextBox, 1, 7);
         tableLayoutPanel1.Controls.Add(ExportButton, 1, 9);
         tableLayoutPanel1.Controls.Add(CopyButton, 1, 8);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 12;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
         tableLayoutPanel1.Size = new Size(800, 450);
         tableLayoutPanel1.TabIndex = 0;
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
         BackgroundCheckBox.CheckedChanged += BackgroundCheckBox_CheckedChanged;
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
         MissionNameBox.Enabled = false;
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
         FullBgBox.Enabled = false;
         FullBgBox.Location = new Point(593, 93);
         FullBgBox.Margin = new Padding(5, 3, 3, 3);
         FullBgBox.Name = "FullBgBox";
         FullBgBox.Size = new Size(204, 24);
         FullBgBox.TabIndex = 4;
         FullBgBox.Text = "Full window background";
         FullBgBox.UseVisualStyleBackColor = true;
         FullBgBox.CheckedChanged += FullBgBox_CheckedChanged;
         // 
         // PrewViewBox
         // 
         PrewViewBox.BackColor = SystemColors.AppWorkspace;
         PrewViewBox.Dock = DockStyle.Fill;
         PrewViewBox.Location = new Point(3, 3);
         PrewViewBox.Name = "PrewViewBox";
         tableLayoutPanel1.SetRowSpan(PrewViewBox, 12);
         PrewViewBox.Size = new Size(582, 444);
         PrewViewBox.SizeMode = PictureBoxSizeMode.Zoom;
         PrewViewBox.TabIndex = 5;
         PrewViewBox.TabStop = false;
         // 
         // MissionEffectIcon
         // 
         MissionEffectIcon.Dock = DockStyle.Fill;
         MissionEffectIcon.DropDownStyle = ComboBoxStyle.DropDownList;
         MissionEffectIcon.FormattingEnabled = true;
         MissionEffectIcon.Location = new Point(591, 123);
         MissionEffectIcon.Name = "MissionEffectIcon";
         MissionEffectIcon.Size = new Size(206, 23);
         MissionEffectIcon.TabIndex = 6;
         // 
         // MissionFrameIcon
         // 
         MissionFrameIcon.Dock = DockStyle.Fill;
         MissionFrameIcon.DropDownStyle = ComboBoxStyle.DropDownList;
         MissionFrameIcon.FormattingEnabled = true;
         MissionFrameIcon.Location = new Point(591, 153);
         MissionFrameIcon.Name = "MissionFrameIcon";
         MissionFrameIcon.Size = new Size(206, 23);
         MissionFrameIcon.TabIndex = 8;
         // 
         // CountrySelection
         // 
         CountrySelection.Dock = DockStyle.Fill;
         CountrySelection.DropDownStyle = ComboBoxStyle.DropDownList;
         CountrySelection.FormattingEnabled = true;
         CountrySelection.Location = new Point(591, 183);
         CountrySelection.Name = "CountrySelection";
         CountrySelection.Size = new Size(206, 23);
         CountrySelection.TabIndex = 9;
         // 
         // SaveTextLabel
         // 
         SaveTextLabel.AutoSize = true;
         SaveTextLabel.Dock = DockStyle.Fill;
         SaveTextLabel.Location = new Point(591, 330);
         SaveTextLabel.Name = "SaveTextLabel";
         SaveTextLabel.Size = new Size(206, 120);
         SaveTextLabel.TabIndex = 10;
         SaveTextLabel.Text = "SaveText";
         SaveTextLabel.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // FileNameTextBox
         // 
         FileNameTextBox.Dock = DockStyle.Fill;
         FileNameTextBox.Location = new Point(591, 213);
         FileNameTextBox.Name = "FileNameTextBox";
         FileNameTextBox.PlaceholderText = "Optional filename";
         FileNameTextBox.Size = new Size(206, 23);
         FileNameTextBox.TabIndex = 11;
         // 
         // ExportButton
         // 
         ExportButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
         ExportButton.Location = new Point(591, 273);
         ExportButton.Name = "ExportButton";
         ExportButton.Size = new Size(75, 24);
         ExportButton.TabIndex = 2;
         ExportButton.Text = "Export";
         ExportButton.UseVisualStyleBackColor = true;
         ExportButton.Click += ExportButton_Click;
         // 
         // CopyButton
         // 
         CopyButton.Location = new Point(591, 243);
         CopyButton.Name = "CopyButton";
         CopyButton.Size = new Size(75, 23);
         CopyButton.TabIndex = 12;
         CopyButton.Text = "Copy";
         toolTip1.SetToolTip(CopyButton, "The copied image does not have transparency");
         CopyButton.UseVisualStyleBackColor = true;
         CopyButton.Click += CopyButton_Click;
         // 
         // MissionTreeExporter
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(800, 450);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "MissionTreeExporter";
         StartPosition = FormStartPosition.CenterScreen;
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
      private ComboBox CountrySelection;
      private ComboBox MissionEffectIcon;
      private ComboBox MissionFrameIcon;
      private Label SaveTextLabel;
      private TextBox FileNameTextBox;
      private Button CopyButton;
      private ToolTip toolTip1;
   }
}