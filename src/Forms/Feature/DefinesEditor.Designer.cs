using Editor.Controls.EXT;

namespace Editor.Forms.Feature
{
   partial class DefinesEditor
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DefinesEditor));
         tableLayoutPanel1 = new TableLayoutPanel();
         DefineNameComboBox = new ComboBox();
         DefineValueTextBox = new TextBox();
         SaveButton = new Button();
         CancelButton = new Button();
         DefinesListView = new EXT_ListViewSearch();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
         tableLayoutPanel1.Controls.Add(DefineNameComboBox, 0, 0);
         tableLayoutPanel1.Controls.Add(DefineValueTextBox, 1, 0);
         tableLayoutPanel1.Controls.Add(SaveButton, 1, 2);
         tableLayoutPanel1.Controls.Add(CancelButton, 0, 2);
         tableLayoutPanel1.Controls.Add(DefinesListView, 0, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Margin = new Padding(0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Size = new Size(694, 652);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // DefineNameComboBox
         // 
         DefineNameComboBox.Dock = DockStyle.Fill;
         DefineNameComboBox.FormattingEnabled = true;
         DefineNameComboBox.Location = new Point(3, 3);
         DefineNameComboBox.Name = "DefineNameComboBox";
         DefineNameComboBox.Size = new Size(479, 23);
         DefineNameComboBox.TabIndex = 0;
         // 
         // DefineValueTextBox
         // 
         DefineValueTextBox.Dock = DockStyle.Fill;
         DefineValueTextBox.Location = new Point(488, 3);
         DefineValueTextBox.Name = "DefineValueTextBox";
         DefineValueTextBox.Size = new Size(203, 23);
         DefineValueTextBox.TabIndex = 1;
         DefineValueTextBox.KeyDown += DefineValueTextBox_KeyDown;
         // 
         // SaveButton
         // 
         SaveButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
         SaveButton.Location = new Point(616, 625);
         SaveButton.Name = "SaveButton";
         SaveButton.Size = new Size(75, 24);
         SaveButton.TabIndex = 3;
         SaveButton.Text = "Save";
         SaveButton.UseVisualStyleBackColor = true;
         SaveButton.Click += SaveButton_Click;
         // 
         // CancelButton
         // 
         CancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
         CancelButton.Location = new Point(3, 625);
         CancelButton.Name = "CancelButton";
         CancelButton.Size = new Size(75, 24);
         CancelButton.TabIndex = 4;
         CancelButton.Text = "Cancel";
         CancelButton.UseVisualStyleBackColor = true;
         CancelButton.Click += CancelButton_Click;
         // 
         // DefinesListView
         // 
         tableLayoutPanel1.SetColumnSpan(DefinesListView, 2);
         DefinesListView.Dock = DockStyle.Fill;
         DefinesListView.Location = new Point(0, 30);
         DefinesListView.Margin = new Padding(0);
         DefinesListView.Name = "DefinesListView";
         DefinesListView.SearchBarLocation = EXT_ListViewSearch.SearchBarLocationEnum.Top;
         DefinesListView.Size = new Size(694, 592);
         DefinesListView.TabIndex = 5;
         // 
         // DefinesEditor
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(694, 652);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "DefinesEditor";
         StartPosition = FormStartPosition.CenterScreen;
         Text = "Defines Editor";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private ComboBox DefineNameComboBox;
      private TextBox DefineValueTextBox;
      private Button SaveButton;
      private Button CancelButton;
      private EXT_ListViewSearch DefinesListView;
   }
}