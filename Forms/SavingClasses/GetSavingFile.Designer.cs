namespace Editor.Forms.SavingClasses
{
   partial class GetSavingFile
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
         tableLayoutPanel1 = new TableLayoutPanel();
         tableLayoutPanel2 = new TableLayoutPanel();
         ExistingFile = new CheckBox();
         NewFile = new CheckBox();
         OpenFileDialogButton = new Button();
         PathTextBox = new TextBox();
         ExistingFilePath = new TextBox();
         DescriptionLabel = new Label();
         button1 = new Button();
         tableLayoutPanel1.SuspendLayout();
         tableLayoutPanel2.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 1;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
         tableLayoutPanel1.Controls.Add(DescriptionLabel, 0, 0);
         tableLayoutPanel1.Controls.Add(button1, 0, 2);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Size = new Size(451, 133);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // tableLayoutPanel2
         // 
         tableLayoutPanel2.ColumnCount = 3;
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
         tableLayoutPanel2.Controls.Add(ExistingFile, 0, 0);
         tableLayoutPanel2.Controls.Add(NewFile, 0, 1);
         tableLayoutPanel2.Controls.Add(OpenFileDialogButton, 2, 0);
         tableLayoutPanel2.Controls.Add(PathTextBox, 1, 1);
         tableLayoutPanel2.Controls.Add(ExistingFilePath, 1, 0);
         tableLayoutPanel2.Dock = DockStyle.Fill;
         tableLayoutPanel2.Location = new Point(0, 50);
         tableLayoutPanel2.Margin = new Padding(0);
         tableLayoutPanel2.Name = "tableLayoutPanel2";
         tableLayoutPanel2.RowCount = 2;
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel2.Size = new Size(451, 53);
         tableLayoutPanel2.TabIndex = 0;
         // 
         // ExistingFile
         // 
         ExistingFile.AutoSize = true;
         ExistingFile.Checked = true;
         ExistingFile.CheckState = CheckState.Checked;
         ExistingFile.Dock = DockStyle.Fill;
         ExistingFile.Location = new Point(3, 3);
         ExistingFile.Name = "ExistingFile";
         ExistingFile.Size = new Size(86, 20);
         ExistingFile.TabIndex = 0;
         ExistingFile.Text = "Existing file";
         ExistingFile.UseVisualStyleBackColor = true;
         ExistingFile.CheckedChanged += ExistingFile_CheckedChanged;
         // 
         // NewFile
         // 
         NewFile.AutoSize = true;
         NewFile.Dock = DockStyle.Fill;
         NewFile.Location = new Point(3, 29);
         NewFile.Name = "NewFile";
         NewFile.Size = new Size(86, 21);
         NewFile.TabIndex = 1;
         NewFile.Text = "New file";
         NewFile.UseVisualStyleBackColor = true;
         NewFile.CheckedChanged += ExistingFile_CheckedChanged;
         // 
         // OpenFileDialogButton
         // 
         OpenFileDialogButton.Dock = DockStyle.Fill;
         OpenFileDialogButton.Image = Properties.Resources.FolderIcon;
         OpenFileDialogButton.Location = new Point(422, 1);
         OpenFileDialogButton.Margin = new Padding(1);
         OpenFileDialogButton.Name = "OpenFileDialogButton";
         OpenFileDialogButton.Size = new Size(28, 24);
         OpenFileDialogButton.TabIndex = 2;
         OpenFileDialogButton.UseVisualStyleBackColor = true;
         OpenFileDialogButton.Click += OpenFileDialogButton_Click;
         // 
         // PathTextBox
         // 
         tableLayoutPanel2.SetColumnSpan(PathTextBox, 2);
         PathTextBox.Dock = DockStyle.Fill;
         PathTextBox.Location = new Point(95, 29);
         PathTextBox.Name = "PathTextBox";
         PathTextBox.PlaceholderText = "MOD_event_modifiers";
         PathTextBox.Size = new Size(353, 23);
         PathTextBox.TabIndex = 3;
         // 
         // ExistingFilePath
         // 
         ExistingFilePath.Dock = DockStyle.Fill;
         ExistingFilePath.Location = new Point(95, 3);
         ExistingFilePath.Name = "ExistingFilePath";
         ExistingFilePath.ReadOnly = true;
         ExistingFilePath.Size = new Size(323, 23);
         ExistingFilePath.TabIndex = 4;
         // 
         // DescriptionLabel
         // 
         DescriptionLabel.AutoSize = true;
         DescriptionLabel.Dock = DockStyle.Fill;
         DescriptionLabel.Location = new Point(3, 0);
         DescriptionLabel.Name = "DescriptionLabel";
         DescriptionLabel.Size = new Size(445, 50);
         DescriptionLabel.TabIndex = 1;
         DescriptionLabel.Text = "Select a file to save or provide a new filename";
         DescriptionLabel.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // button1
         // 
         button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
         button1.Location = new Point(373, 106);
         button1.Name = "button1";
         button1.Size = new Size(75, 23);
         button1.TabIndex = 2;
         button1.Text = "Confirm";
         button1.UseVisualStyleBackColor = true;
         button1.Click += button1_Click;
         // 
         // GetSavingFile
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(451, 133);
         Controls.Add(tableLayoutPanel1);
         Name = "GetSavingFile";
         Text = "GetSavingFile";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         tableLayoutPanel2.ResumeLayout(false);
         tableLayoutPanel2.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private TableLayoutPanel tableLayoutPanel2;
      private CheckBox ExistingFile;
      private CheckBox NewFile;
      private Button OpenFileDialogButton;
      private TextBox PathTextBox;
      private TextBox ExistingFilePath;
      private Label DescriptionLabel;
      private Button button1;
   }
}