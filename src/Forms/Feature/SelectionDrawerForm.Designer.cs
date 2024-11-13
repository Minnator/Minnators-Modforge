namespace Editor.Forms.Feature
{
   partial class SelectionDrawerForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectionDrawerForm));
         MainLayoutPanel = new TableLayoutPanel();
         ExportSettingsPropertyGrid = new PropertyGrid();
         tableLayoutPanel2 = new TableLayoutPanel();
         button1 = new Button();
         PathTextBox = new TextBox();
         tableLayoutPanel3 = new TableLayoutPanel();
         CancelButton = new Button();
         SaveButton = new Button();
         MainLayoutPanel.SuspendLayout();
         tableLayoutPanel2.SuspendLayout();
         tableLayoutPanel3.SuspendLayout();
         SuspendLayout();
         // 
         // MainLayoutPanel
         // 
         MainLayoutPanel.ColumnCount = 2;
         MainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
         MainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         MainLayoutPanel.Controls.Add(ExportSettingsPropertyGrid, 0, 0);
         MainLayoutPanel.Controls.Add(tableLayoutPanel2, 1, 1);
         MainLayoutPanel.Controls.Add(tableLayoutPanel3, 0, 1);
         MainLayoutPanel.Dock = DockStyle.Fill;
         MainLayoutPanel.Location = new Point(0, 0);
         MainLayoutPanel.Name = "MainLayoutPanel";
         MainLayoutPanel.RowCount = 2;
         MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         MainLayoutPanel.Size = new Size(889, 447);
         MainLayoutPanel.TabIndex = 0;
         // 
         // ExportSettingsPropertyGrid
         // 
         ExportSettingsPropertyGrid.Dock = DockStyle.Fill;
         ExportSettingsPropertyGrid.Location = new Point(3, 3);
         ExportSettingsPropertyGrid.Name = "ExportSettingsPropertyGrid";
         ExportSettingsPropertyGrid.Size = new Size(244, 411);
         ExportSettingsPropertyGrid.TabIndex = 1;
         // 
         // tableLayoutPanel2
         // 
         tableLayoutPanel2.ColumnCount = 2;
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
         tableLayoutPanel2.Controls.Add(button1, 1, 0);
         tableLayoutPanel2.Controls.Add(PathTextBox, 0, 0);
         tableLayoutPanel2.Dock = DockStyle.Fill;
         tableLayoutPanel2.Location = new Point(250, 417);
         tableLayoutPanel2.Margin = new Padding(0);
         tableLayoutPanel2.Name = "tableLayoutPanel2";
         tableLayoutPanel2.RowCount = 1;
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel2.Size = new Size(639, 30);
         tableLayoutPanel2.TabIndex = 2;
         // 
         // button1
         // 
         button1.Dock = DockStyle.Fill;
         button1.Image = (Image)resources.GetObject("button1.Image");
         button1.Location = new Point(610, 1);
         button1.Margin = new Padding(1);
         button1.Name = "button1";
         button1.Size = new Size(28, 28);
         button1.TabIndex = 0;
         button1.UseVisualStyleBackColor = true;
         button1.Click += SelectFolderButton;
         // 
         // PathTextBox
         // 
         PathTextBox.Dock = DockStyle.Fill;
         PathTextBox.Location = new Point(1, 4);
         PathTextBox.Margin = new Padding(1, 4, 1, 1);
         PathTextBox.Name = "PathTextBox";
         PathTextBox.Size = new Size(607, 23);
         PathTextBox.TabIndex = 1;
         // 
         // tableLayoutPanel3
         // 
         tableLayoutPanel3.ColumnCount = 2;
         tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel3.Controls.Add(CancelButton, 0, 0);
         tableLayoutPanel3.Controls.Add(SaveButton, 1, 0);
         tableLayoutPanel3.Dock = DockStyle.Fill;
         tableLayoutPanel3.Location = new Point(1, 418);
         tableLayoutPanel3.Margin = new Padding(1);
         tableLayoutPanel3.Name = "tableLayoutPanel3";
         tableLayoutPanel3.RowCount = 1;
         tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel3.Size = new Size(248, 28);
         tableLayoutPanel3.TabIndex = 3;
         // 
         // CancelButton
         // 
         CancelButton.Dock = DockStyle.Fill;
         CancelButton.Location = new Point(1, 1);
         CancelButton.Margin = new Padding(1);
         CancelButton.Name = "CancelButton";
         CancelButton.Size = new Size(122, 26);
         CancelButton.TabIndex = 3;
         CancelButton.Text = "Cancel";
         CancelButton.UseVisualStyleBackColor = true;
         // 
         // SaveButton
         // 
         SaveButton.Dock = DockStyle.Fill;
         SaveButton.Location = new Point(125, 1);
         SaveButton.Margin = new Padding(1);
         SaveButton.Name = "SaveButton";
         SaveButton.Size = new Size(122, 26);
         SaveButton.TabIndex = 2;
         SaveButton.Text = "Save";
         SaveButton.UseVisualStyleBackColor = true;
         SaveButton.Click += SaveButton_Click;
         // 
         // SelectionDrawerForm
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(889, 447);
         Controls.Add(MainLayoutPanel);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "SelectionDrawerForm";
         Text = "SelectionDrawerForm";
         FormClosing += SelectionDrawerForm_FormClosing;
         MainLayoutPanel.ResumeLayout(false);
         tableLayoutPanel2.ResumeLayout(false);
         tableLayoutPanel2.PerformLayout();
         tableLayoutPanel3.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel MainLayoutPanel;
      private PropertyGrid ExportSettingsPropertyGrid;
      private TableLayoutPanel tableLayoutPanel2;
      private Button button1;
      private TextBox PathTextBox;
      private TableLayoutPanel tableLayoutPanel3;
      private Button SaveButton;
      private new Button CancelButton;
   }
}