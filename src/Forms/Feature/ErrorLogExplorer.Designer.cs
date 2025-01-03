namespace Editor.Forms.Feature
{
   partial class ErrorLogExplorer
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorLogExplorer));
         MainTLP = new TableLayoutPanel();
         TopTLP = new TableLayoutPanel();
         tableLayoutPanel1 = new TableLayoutPanel();
         SearchButton = new Button();
         SearchTextBox = new TextBox();
         SearchTypeBox = new ComboBox();
         SearchSource = new CheckBox();
         DebugCheckBox = new CheckBox();
         CriticalCheckbox = new CheckBox();
         ErrorCheckBox = new CheckBox();
         WarningCheckBox = new CheckBox();
         InfoCheckBox = new CheckBox();
         IsVanillaEntryCheckBox = new CheckBox();
         ErrorView = new ListView();
         IndexHeader = new ColumnHeader();
         TimeHeader = new ColumnHeader();
         VerbocityHeader = new ColumnHeader();
         MessageHeader = new ColumnHeader();
         tableLayoutPanel2 = new TableLayoutPanel();
         label1 = new Label();
         SaveLogsButton = new Button();
         ErrorToolTip = new ToolTip(components);
         MainTLP.SuspendLayout();
         TopTLP.SuspendLayout();
         tableLayoutPanel1.SuspendLayout();
         tableLayoutPanel2.SuspendLayout();
         SuspendLayout();
         // 
         // MainTLP
         // 
         MainTLP.ColumnCount = 1;
         MainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         MainTLP.Controls.Add(TopTLP, 0, 0);
         MainTLP.Controls.Add(ErrorView, 0, 1);
         MainTLP.Controls.Add(tableLayoutPanel2, 0, 2);
         MainTLP.Dock = DockStyle.Fill;
         MainTLP.Location = new Point(0, 0);
         MainTLP.Margin = new Padding(0);
         MainTLP.Name = "MainTLP";
         MainTLP.RowCount = 3;
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
         MainTLP.Size = new Size(1129, 450);
         MainTLP.TabIndex = 0;
         // 
         // TopTLP
         // 
         TopTLP.ColumnCount = 8;
         TopTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
         TopTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
         TopTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
         TopTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
         TopTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
         TopTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
         TopTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95F));
         TopTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
         TopTLP.Controls.Add(tableLayoutPanel1, 7, 0);
         TopTLP.Controls.Add(SearchSource, 6, 0);
         TopTLP.Controls.Add(DebugCheckBox, 4, 0);
         TopTLP.Controls.Add(CriticalCheckbox, 0, 0);
         TopTLP.Controls.Add(ErrorCheckBox, 1, 0);
         TopTLP.Controls.Add(WarningCheckBox, 2, 0);
         TopTLP.Controls.Add(InfoCheckBox, 3, 0);
         TopTLP.Controls.Add(IsVanillaEntryCheckBox, 5, 0);
         TopTLP.Dock = DockStyle.Fill;
         TopTLP.Location = new Point(0, 0);
         TopTLP.Margin = new Padding(0);
         TopTLP.Name = "TopTLP";
         TopTLP.RowCount = 1;
         TopTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         TopTLP.Size = new Size(1129, 30);
         TopTLP.TabIndex = 0;
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 3;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Controls.Add(SearchButton, 2, 0);
         tableLayoutPanel1.Controls.Add(SearchTextBox, 1, 0);
         tableLayoutPanel1.Controls.Add(SearchTypeBox, 0, 0);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(875, 0);
         tableLayoutPanel1.Margin = new Padding(0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 1;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Size = new Size(254, 30);
         tableLayoutPanel1.TabIndex = 4;
         // 
         // SearchButton
         // 
         SearchButton.Dock = DockStyle.Fill;
         SearchButton.Image = Properties.Resources.Search;
         SearchButton.Location = new Point(225, 1);
         SearchButton.Margin = new Padding(1);
         SearchButton.Name = "SearchButton";
         SearchButton.Size = new Size(28, 28);
         SearchButton.TabIndex = 0;
         ErrorToolTip.SetToolTip(SearchButton, "RMB to clear all search results");
         SearchButton.UseVisualStyleBackColor = true;
         SearchButton.MouseClick += SearchButton_Click;
         // 
         // SearchTextBox
         // 
         SearchTextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         SearchTextBox.Dock = DockStyle.Fill;
         SearchTextBox.Location = new Point(101, 3);
         SearchTextBox.Margin = new Padding(1, 3, 1, 4);
         SearchTextBox.Name = "SearchTextBox";
         SearchTextBox.PlaceholderText = "Search";
         SearchTextBox.Size = new Size(122, 23);
         SearchTextBox.TabIndex = 1;
         ErrorToolTip.SetToolTip(SearchTextBox, "All items no matter chich checkboxes are checked will be searched.");
         SearchTextBox.KeyPress += SearchTextBox_KeyPress;
         // 
         // SearchTypeBox
         // 
         SearchTypeBox.Dock = DockStyle.Fill;
         SearchTypeBox.DropDownStyle = ComboBoxStyle.DropDownList;
         SearchTypeBox.FormattingEnabled = true;
         SearchTypeBox.Location = new Point(1, 3);
         SearchTypeBox.Margin = new Padding(1, 3, 1, 4);
         SearchTypeBox.Name = "SearchTypeBox";
         SearchTypeBox.Size = new Size(98, 23);
         SearchTypeBox.TabIndex = 2;
         // 
         // SearchSource
         // 
         SearchSource.Anchor = AnchorStyles.None;
         SearchSource.AutoSize = true;
         SearchSource.Location = new Point(788, 5);
         SearchSource.Name = "SearchSource";
         SearchSource.Size = new Size(78, 19);
         SearchSource.TabIndex = 5;
         SearchSource.Text = "Search All";
         SearchSource.UseVisualStyleBackColor = true;
         // 
         // DebugCheckBox
         // 
         DebugCheckBox.Anchor = AnchorStyles.None;
         DebugCheckBox.AutoSize = true;
         DebugCheckBox.Location = new Point(554, 5);
         DebugCheckBox.Name = "DebugCheckBox";
         DebugCheckBox.Size = new Size(61, 19);
         DebugCheckBox.TabIndex = 3;
         DebugCheckBox.Text = "Debug";
         DebugCheckBox.UseVisualStyleBackColor = true;
         // 
         // CriticalCheckbox
         // 
         CriticalCheckbox.Anchor = AnchorStyles.None;
         CriticalCheckbox.AutoSize = true;
         CriticalCheckbox.Location = new Point(33, 5);
         CriticalCheckbox.Name = "CriticalCheckbox";
         CriticalCheckbox.Size = new Size(63, 19);
         CriticalCheckbox.TabIndex = 6;
         CriticalCheckbox.Text = "Critical";
         CriticalCheckbox.UseVisualStyleBackColor = true;
         // 
         // ErrorCheckBox
         // 
         ErrorCheckBox.Anchor = AnchorStyles.None;
         ErrorCheckBox.AutoSize = true;
         ErrorCheckBox.Location = new Point(167, 5);
         ErrorCheckBox.Name = "ErrorCheckBox";
         ErrorCheckBox.Size = new Size(56, 19);
         ErrorCheckBox.TabIndex = 0;
         ErrorCheckBox.Text = "Errors";
         ErrorCheckBox.UseVisualStyleBackColor = true;
         // 
         // WarningCheckBox
         // 
         WarningCheckBox.Anchor = AnchorStyles.None;
         WarningCheckBox.AutoSize = true;
         WarningCheckBox.Location = new Point(287, 5);
         WarningCheckBox.Name = "WarningCheckBox";
         WarningCheckBox.Size = new Size(76, 19);
         WarningCheckBox.TabIndex = 1;
         WarningCheckBox.Text = "Warnings";
         WarningCheckBox.UseVisualStyleBackColor = true;
         // 
         // InfoCheckBox
         // 
         InfoCheckBox.Anchor = AnchorStyles.None;
         InfoCheckBox.AutoSize = true;
         InfoCheckBox.Location = new Point(410, 5);
         InfoCheckBox.Name = "InfoCheckBox";
         InfoCheckBox.Size = new Size(89, 19);
         InfoCheckBox.TabIndex = 2;
         InfoCheckBox.Text = "Information";
         InfoCheckBox.UseVisualStyleBackColor = true;
         // 
         // IsVanillaEntryCheckBox
         // 
         IsVanillaEntryCheckBox.Anchor = AnchorStyles.None;
         IsVanillaEntryCheckBox.AutoSize = true;
         IsVanillaEntryCheckBox.Checked = true;
         IsVanillaEntryCheckBox.CheckState = CheckState.Checked;
         IsVanillaEntryCheckBox.Location = new Point(685, 5);
         IsVanillaEntryCheckBox.Name = "IsVanillaEntryCheckBox";
         IsVanillaEntryCheckBox.Size = new Size(60, 19);
         IsVanillaEntryCheckBox.TabIndex = 7;
         IsVanillaEntryCheckBox.Text = "Vanilla";
         IsVanillaEntryCheckBox.UseVisualStyleBackColor = true;
         // 
         // ErrorView
         // 
         ErrorView.Activation = ItemActivation.OneClick;
         ErrorView.Columns.AddRange(new ColumnHeader[] { IndexHeader, TimeHeader, VerbocityHeader, MessageHeader });
         ErrorView.Dock = DockStyle.Fill;
         ErrorView.FullRowSelect = true;
         ErrorView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
         ErrorView.Location = new Point(3, 33);
         ErrorView.Name = "ErrorView";
         ErrorView.OwnerDraw = true;
         ErrorView.Size = new Size(1123, 384);
         ErrorView.TabIndex = 1;
         ErrorView.UseCompatibleStateImageBehavior = false;
         ErrorView.View = View.Details;
         // 
         // IndexHeader
         // 
         IndexHeader.Text = "Index";
         // 
         // TimeHeader
         // 
         TimeHeader.Text = "Time";
         // 
         // VerbocityHeader
         // 
         VerbocityHeader.Text = "Verbocity";
         // 
         // MessageHeader
         // 
         MessageHeader.Text = "Message";
         // 
         // tableLayoutPanel2
         // 
         tableLayoutPanel2.ColumnCount = 2;
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
         tableLayoutPanel2.Controls.Add(label1, 0, 0);
         tableLayoutPanel2.Controls.Add(SaveLogsButton, 1, 0);
         tableLayoutPanel2.Dock = DockStyle.Fill;
         tableLayoutPanel2.Location = new Point(0, 420);
         tableLayoutPanel2.Margin = new Padding(0);
         tableLayoutPanel2.Name = "tableLayoutPanel2";
         tableLayoutPanel2.RowCount = 1;
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel2.Size = new Size(1129, 30);
         tableLayoutPanel2.TabIndex = 3;
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Location = new Point(3, 0);
         label1.Name = "label1";
         label1.Size = new Size(1043, 30);
         label1.TabIndex = 2;
         label1.Text = "Double click rows marked by light gray background to get more detailed info and tips how to fix the issues.";
         label1.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // SaveLogsButton
         // 
         SaveLogsButton.Location = new Point(1052, 3);
         SaveLogsButton.Name = "SaveLogsButton";
         SaveLogsButton.Size = new Size(74, 23);
         SaveLogsButton.TabIndex = 3;
         SaveLogsButton.Text = "Save Logs";
         SaveLogsButton.UseVisualStyleBackColor = true;
         SaveLogsButton.Click += SaveLogsButton_Click;
         // 
         // ErrorLogExplorer
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(1129, 450);
         Controls.Add(MainTLP);
         Icon = (Icon)resources.GetObject("$this.Icon");
         KeyPreview = true;
         Name = "ErrorLogExplorer";
         Text = "Error Log Explorer";
         KeyPress += ErrorLogExplorer_KeyPress;
         MainTLP.ResumeLayout(false);
         TopTLP.ResumeLayout(false);
         TopTLP.PerformLayout();
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         tableLayoutPanel2.ResumeLayout(false);
         tableLayoutPanel2.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel MainTLP;
      private TableLayoutPanel TopTLP;
      private ListView ErrorView;
      private ColumnHeader TimeHeader;
      private ColumnHeader VerbocityHeader;
      private ColumnHeader MessageHeader;
      private CheckBox ErrorCheckBox;
      private CheckBox WarningCheckBox;
      private CheckBox InfoCheckBox;
      private CheckBox DebugCheckBox;
      private Label label1;
      private ColumnHeader IndexHeader;
      private TableLayoutPanel tableLayoutPanel1;
      private Button SearchButton;
      private TextBox SearchTextBox;
      private ToolTip ErrorToolTip;
      private CheckBox SearchSource;
      private CheckBox CriticalCheckbox;
      private TableLayoutPanel tableLayoutPanel2;
      private Button SaveLogsButton;
      private ComboBox SearchTypeBox;
      private CheckBox IsVanillaEntryCheckBox;
   }
}