namespace Editor.src.Forms.Feature
{
   partial class CollectionSelectorBase
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectionSelectorBase));
         tableLayoutPanel1 = new TableLayoutPanel();
         label1 = new Label();
         label2 = new Label();
         SourceListView = new ListView();
         Items = new ColumnHeader();
         tableLayoutPanel2 = new TableLayoutPanel();
         MoveUpButton = new Button();
         MoveDownButton = new Button();
         AddButton = new Button();
         RemoveButton = new Button();
         tableLayoutPanel3 = new TableLayoutPanel();
         SearchButton = new Button();
         SearchTextBox = new TextBox();
         ConfirmButton = new Button();
         CancelButton = new Button();
         SelectedListView = new ListView();
         columnHeader1 = new ColumnHeader();
         tableLayoutPanel4 = new TableLayoutPanel();
         SelectedSearchButton = new Button();
         SelectedSearchTextBox = new TextBox();
         tableLayoutPanel1.SuspendLayout();
         tableLayoutPanel2.SuspendLayout();
         tableLayoutPanel3.SuspendLayout();
         tableLayoutPanel4.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 3;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Controls.Add(label1, 0, 0);
         tableLayoutPanel1.Controls.Add(label2, 2, 0);
         tableLayoutPanel1.Controls.Add(SourceListView, 2, 2);
         tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 2);
         tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 2, 1);
         tableLayoutPanel1.Controls.Add(ConfirmButton, 2, 3);
         tableLayoutPanel1.Controls.Add(CancelButton, 0, 3);
         tableLayoutPanel1.Controls.Add(SelectedListView, 0, 2);
         tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 0, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Margin = new Padding(0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 4;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Size = new Size(519, 326);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Font = new Font("Segoe UI", 11.25F);
         label1.Location = new Point(3, 0);
         label1.Name = "label1";
         label1.Size = new Size(238, 20);
         label1.TabIndex = 0;
         label1.Text = "Selected Items";
         label1.TextAlign = ContentAlignment.TopCenter;
         // 
         // label2
         // 
         label2.AutoSize = true;
         label2.Dock = DockStyle.Fill;
         label2.Font = new Font("Segoe UI", 11.25F);
         label2.Location = new Point(277, 0);
         label2.Name = "label2";
         label2.Size = new Size(239, 20);
         label2.TabIndex = 1;
         label2.Text = "Available Items";
         label2.TextAlign = ContentAlignment.TopCenter;
         // 
         // SourceListView
         // 
         SourceListView.Activation = ItemActivation.OneClick;
         SourceListView.BorderStyle = BorderStyle.FixedSingle;
         SourceListView.Columns.AddRange(new ColumnHeader[] { Items });
         SourceListView.Dock = DockStyle.Fill;
         SourceListView.FullRowSelect = true;
         SourceListView.HeaderStyle = ColumnHeaderStyle.None;
         SourceListView.HoverSelection = true;
         SourceListView.Location = new Point(277, 52);
         SourceListView.Name = "SourceListView";
         SourceListView.ShowItemToolTips = true;
         SourceListView.Size = new Size(239, 241);
         SourceListView.Sorting = SortOrder.Ascending;
         SourceListView.TabIndex = 4;
         SourceListView.UseCompatibleStateImageBehavior = false;
         SourceListView.View = View.Details;
         // 
         // Items
         // 
         Items.Text = "Items";
         // 
         // tableLayoutPanel2
         // 
         tableLayoutPanel2.ColumnCount = 1;
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel2.Controls.Add(MoveUpButton, 0, 0);
         tableLayoutPanel2.Controls.Add(MoveDownButton, 0, 1);
         tableLayoutPanel2.Controls.Add(AddButton, 0, 2);
         tableLayoutPanel2.Controls.Add(RemoveButton, 0, 3);
         tableLayoutPanel2.Dock = DockStyle.Fill;
         tableLayoutPanel2.Location = new Point(244, 49);
         tableLayoutPanel2.Margin = new Padding(0);
         tableLayoutPanel2.Name = "tableLayoutPanel2";
         tableLayoutPanel2.RowCount = 5;
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel2.Size = new Size(30, 247);
         tableLayoutPanel2.TabIndex = 5;
         // 
         // MoveUpButton
         // 
         MoveUpButton.Dock = DockStyle.Fill;
         MoveUpButton.Image = Properties.Resources.Up;
         MoveUpButton.Location = new Point(0, 2);
         MoveUpButton.Margin = new Padding(0, 2, 0, 0);
         MoveUpButton.Name = "MoveUpButton";
         MoveUpButton.Size = new Size(30, 30);
         MoveUpButton.TabIndex = 0;
         MoveUpButton.Text = " ";
         MoveUpButton.UseVisualStyleBackColor = true;
         MoveUpButton.Click += MoveUpButton_Click;
         // 
         // MoveDownButton
         // 
         MoveDownButton.Dock = DockStyle.Fill;
         MoveDownButton.Image = Properties.Resources.Down;
         MoveDownButton.Location = new Point(0, 32);
         MoveDownButton.Margin = new Padding(0);
         MoveDownButton.Name = "MoveDownButton";
         MoveDownButton.Size = new Size(30, 30);
         MoveDownButton.TabIndex = 1;
         MoveDownButton.Text = " ";
         MoveDownButton.UseVisualStyleBackColor = true;
         MoveDownButton.Click += MoveDownButton_Click;
         // 
         // AddButton
         // 
         AddButton.Dock = DockStyle.Fill;
         AddButton.Image = Properties.Resources.GreenPlusBg;
         AddButton.Location = new Point(0, 62);
         AddButton.Margin = new Padding(0);
         AddButton.Name = "AddButton";
         AddButton.Size = new Size(30, 30);
         AddButton.TabIndex = 2;
         AddButton.Text = " ";
         AddButton.UseVisualStyleBackColor = true;
         AddButton.Click += AddButton_Click;
         // 
         // RemoveButton
         // 
         RemoveButton.Dock = DockStyle.Fill;
         RemoveButton.Image = Properties.Resources.RedMinus;
         RemoveButton.Location = new Point(0, 92);
         RemoveButton.Margin = new Padding(0);
         RemoveButton.Name = "RemoveButton";
         RemoveButton.Size = new Size(30, 30);
         RemoveButton.TabIndex = 3;
         RemoveButton.Text = " ";
         RemoveButton.UseVisualStyleBackColor = true;
         RemoveButton.Click += RemoveButton_Click;
         // 
         // tableLayoutPanel3
         // 
         tableLayoutPanel3.ColumnCount = 2;
         tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 86.6666641F));
         tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13.333333F));
         tableLayoutPanel3.Controls.Add(SearchButton, 1, 0);
         tableLayoutPanel3.Controls.Add(SearchTextBox, 0, 0);
         tableLayoutPanel3.Dock = DockStyle.Fill;
         tableLayoutPanel3.Location = new Point(274, 20);
         tableLayoutPanel3.Margin = new Padding(0);
         tableLayoutPanel3.Name = "tableLayoutPanel3";
         tableLayoutPanel3.RowCount = 1;
         tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel3.Size = new Size(245, 29);
         tableLayoutPanel3.TabIndex = 6;
         // 
         // SearchButton
         // 
         SearchButton.Dock = DockStyle.Fill;
         SearchButton.Image = Properties.Resources.Search;
         SearchButton.Location = new Point(213, 1);
         SearchButton.Margin = new Padding(1);
         SearchButton.Name = "SearchButton";
         SearchButton.Size = new Size(31, 27);
         SearchButton.TabIndex = 0;
         SearchButton.Text = " ";
         SearchButton.UseVisualStyleBackColor = true;
         SearchButton.MouseDown += SearchButton_MouseDown;
         // 
         // SearchTextBox
         // 
         SearchTextBox.Dock = DockStyle.Fill;
         SearchTextBox.Location = new Point(3, 3);
         SearchTextBox.Name = "SearchTextBox";
         SearchTextBox.Size = new Size(206, 23);
         SearchTextBox.TabIndex = 1;
         SearchTextBox.TextChanged += AnySearchTextBox_TextChanged;
         SearchTextBox.KeyDown += SearchTextBox_KeyDown;
         // 
         // ConfirmButton
         // 
         ConfirmButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
         ConfirmButton.Location = new Point(441, 299);
         ConfirmButton.Name = "ConfirmButton";
         ConfirmButton.Size = new Size(75, 23);
         ConfirmButton.TabIndex = 7;
         ConfirmButton.Text = "Confirm";
         ConfirmButton.UseVisualStyleBackColor = true;
         ConfirmButton.Click += OkButton_Click;
         // 
         // CancelButton
         // 
         CancelButton.Location = new Point(3, 299);
         CancelButton.Name = "CancelButton";
         CancelButton.Size = new Size(75, 23);
         CancelButton.TabIndex = 8;
         CancelButton.Text = "Cancel";
         CancelButton.UseVisualStyleBackColor = true;
         CancelButton.Click += CancelButton_Click;
         // 
         // SelectedListView
         // 
         SelectedListView.Activation = ItemActivation.OneClick;
         SelectedListView.BorderStyle = BorderStyle.FixedSingle;
         SelectedListView.Columns.AddRange(new ColumnHeader[] { columnHeader1 });
         SelectedListView.Dock = DockStyle.Fill;
         SelectedListView.FullRowSelect = true;
         SelectedListView.HeaderStyle = ColumnHeaderStyle.None;
         SelectedListView.HoverSelection = true;
         SelectedListView.Location = new Point(3, 52);
         SelectedListView.Name = "SelectedListView";
         SelectedListView.ShowItemToolTips = true;
         SelectedListView.Size = new Size(238, 241);
         SelectedListView.TabIndex = 3;
         SelectedListView.UseCompatibleStateImageBehavior = false;
         SelectedListView.View = View.Details;
         // 
         // columnHeader1
         // 
         columnHeader1.Text = "Items";
         // 
         // tableLayoutPanel4
         // 
         tableLayoutPanel4.ColumnCount = 2;
         tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 86.6666641F));
         tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13.333333F));
         tableLayoutPanel4.Controls.Add(SelectedSearchButton, 1, 0);
         tableLayoutPanel4.Controls.Add(SelectedSearchTextBox, 0, 0);
         tableLayoutPanel4.Dock = DockStyle.Fill;
         tableLayoutPanel4.Location = new Point(0, 20);
         tableLayoutPanel4.Margin = new Padding(0);
         tableLayoutPanel4.Name = "tableLayoutPanel4";
         tableLayoutPanel4.RowCount = 1;
         tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel4.Size = new Size(244, 29);
         tableLayoutPanel4.TabIndex = 9;
         // 
         // SelectedSearchButton
         // 
         SelectedSearchButton.Dock = DockStyle.Fill;
         SelectedSearchButton.Image = Properties.Resources.Search;
         SelectedSearchButton.Location = new Point(212, 1);
         SelectedSearchButton.Margin = new Padding(1);
         SelectedSearchButton.Name = "SelectedSearchButton";
         SelectedSearchButton.Size = new Size(31, 27);
         SelectedSearchButton.TabIndex = 0;
         SelectedSearchButton.Text = " ";
         SelectedSearchButton.UseVisualStyleBackColor = true;
         SelectedSearchButton.MouseDown += SelectedSearchButton_MouseDown;
         // 
         // SelectedSearchTextBox
         // 
         SelectedSearchTextBox.Dock = DockStyle.Fill;
         SelectedSearchTextBox.Location = new Point(3, 3);
         SelectedSearchTextBox.Name = "SelectedSearchTextBox";
         SelectedSearchTextBox.Size = new Size(205, 23);
         SelectedSearchTextBox.TabIndex = 1;
         SelectedSearchTextBox.TextChanged += AnySearchTextBox_TextChanged;
         SelectedSearchTextBox.KeyDown += SelectedSearchTextBox_KeyDown;
         // 
         // CollectionSelectorBase
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(519, 326);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.FixedSingle;
         Icon = (Icon)resources.GetObject("$this.Icon");
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "CollectionSelectorBase";
         Text = "Collection Items Selector";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         tableLayoutPanel2.ResumeLayout(false);
         tableLayoutPanel3.ResumeLayout(false);
         tableLayoutPanel3.PerformLayout();
         tableLayoutPanel4.ResumeLayout(false);
         tableLayoutPanel4.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private Label label1;
      private Label label2;
      private ListView SourceListView;
      private ListView SelectedListView;
      private TableLayoutPanel tableLayoutPanel2;
      private TableLayoutPanel tableLayoutPanel3;
      private Button MoveUpButton;
      private Button MoveDownButton;
      private Button AddButton;
      private Button RemoveButton;
      private Button SearchButton;
      private TextBox SearchTextBox;
      private Button ConfirmButton;
      private Button CancelButton;
      private ColumnHeader Items;
      private ColumnHeader columnHeader1;
      private TableLayoutPanel tableLayoutPanel4;
      private Button SelectedSearchButton;
      private TextBox SelectedSearchTextBox;
   }
}