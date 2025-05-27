namespace Editor.Forms.Feature
{
   partial class ListDeltaSetSelection
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
         DeltaLabel = new Label();
         ConfirmButton = new Button();
         SelectionSearchBox = new ComboBox();
         SourceSearchBox = new ComboBox();
         label1 = new Label();
         label2 = new Label();
         SelectionListView = new ListView();
         SourceListView = new ListView();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 3;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 383F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 301F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
         tableLayoutPanel1.Controls.Add(DeltaLabel, 0, 3);
         tableLayoutPanel1.Controls.Add(ConfirmButton, 2, 3);
         tableLayoutPanel1.Controls.Add(SelectionSearchBox, 1, 1);
         tableLayoutPanel1.Controls.Add(SourceSearchBox, 0, 1);
         tableLayoutPanel1.Controls.Add(label1, 1, 0);
         tableLayoutPanel1.Controls.Add(label2, 0, 0);
         tableLayoutPanel1.Controls.Add(SelectionListView, 1, 2);
         tableLayoutPanel1.Controls.Add(SourceListView, 0, 2);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Size = new Size(784, 461);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // DeltaLabel
         // 
         DeltaLabel.AutoSize = true;
         tableLayoutPanel1.SetColumnSpan(DeltaLabel, 2);
         DeltaLabel.Dock = DockStyle.Fill;
         DeltaLabel.Location = new Point(3, 431);
         DeltaLabel.Name = "DeltaLabel";
         DeltaLabel.Size = new Size(678, 30);
         DeltaLabel.TabIndex = 4;
         DeltaLabel.Text = "Added: 0 | Removed: 0 | Total: 0";
         DeltaLabel.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // ConfirmButton
         // 
         ConfirmButton.Dock = DockStyle.Right;
         ConfirmButton.Location = new Point(706, 434);
         ConfirmButton.Name = "ConfirmButton";
         ConfirmButton.Size = new Size(75, 24);
         ConfirmButton.TabIndex = 3;
         ConfirmButton.Text = "Confirm";
         ConfirmButton.UseVisualStyleBackColor = true;
         ConfirmButton.Click += ConfirmButton_Click;
         // 
         // SelectionSearchBox
         // 
         SelectionSearchBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         SelectionSearchBox.AutoCompleteSource = AutoCompleteSource.ListItems;
         tableLayoutPanel1.SetColumnSpan(SelectionSearchBox, 2);
         SelectionSearchBox.Dock = DockStyle.Fill;
         SelectionSearchBox.Location = new Point(386, 33);
         SelectionSearchBox.Name = "SelectionSearchBox";
         SelectionSearchBox.Size = new Size(395, 23);
         SelectionSearchBox.TabIndex = 5;
         // 
         // SourceSearchBox
         // 
         SourceSearchBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         SourceSearchBox.AutoCompleteSource = AutoCompleteSource.ListItems;
         SourceSearchBox.Dock = DockStyle.Fill;
         SourceSearchBox.Location = new Point(3, 33);
         SourceSearchBox.Name = "SourceSearchBox";
         SourceSearchBox.Size = new Size(377, 23);
         SourceSearchBox.TabIndex = 6;
         // 
         // label1
         // 
         label1.AutoSize = true;
         tableLayoutPanel1.SetColumnSpan(label1, 2);
         label1.Dock = DockStyle.Fill;
         label1.Location = new Point(386, 0);
         label1.Name = "label1";
         label1.Size = new Size(395, 30);
         label1.TabIndex = 5;
         label1.Text = "Selection List";
         label1.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // label2
         // 
         label2.AutoSize = true;
         label2.Dock = DockStyle.Fill;
         label2.Location = new Point(3, 0);
         label2.Name = "label2";
         label2.Size = new Size(377, 30);
         label2.TabIndex = 6;
         label2.Text = "Source List";
         label2.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // SelectionListView
         // 
         SelectionListView.Activation = ItemActivation.OneClick;
         SelectionListView.BorderStyle = BorderStyle.FixedSingle;
         tableLayoutPanel1.SetColumnSpan(SelectionListView, 2);
         SelectionListView.Dock = DockStyle.Fill;
         SelectionListView.HeaderStyle = ColumnHeaderStyle.None;
         SelectionListView.Location = new Point(386, 63);
         SelectionListView.MultiSelect = false;
         SelectionListView.Name = "SelectionListView";
         SelectionListView.ShowItemToolTips = true;
         SelectionListView.Size = new Size(395, 365);
         SelectionListView.Sorting = SortOrder.Ascending;
         SelectionListView.TabIndex = 7;
         SelectionListView.UseCompatibleStateImageBehavior = false;
         SelectionListView.View = View.Details;
         // 
         // SourceListView
         // 
         SourceListView.Activation = ItemActivation.OneClick;
         SourceListView.BorderStyle = BorderStyle.FixedSingle;
         SourceListView.Dock = DockStyle.Fill;
         SourceListView.HeaderStyle = ColumnHeaderStyle.None;
         SourceListView.Location = new Point(3, 63);
         SourceListView.MultiSelect = false;
         SourceListView.Name = "SourceListView";
         SourceListView.ShowItemToolTips = true;
         SourceListView.Size = new Size(377, 365);
         SourceListView.Sorting = SortOrder.Ascending;
         SourceListView.TabIndex = 8;
         SourceListView.UseCompatibleStateImageBehavior = false;
         SourceListView.View = View.Details;
         // 
         // ListDeltaSetSelection
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(784, 461);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.FixedSingle;
         KeyPreview = true;
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "ListDeltaSetSelection";
         Text = "x - ListDeltaSetSelection";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private Button ConfirmButton;
      private ComboBox SelectionSearchBox;
      private ComboBox SourceSearchBox;
      private Label DeltaLabel;
      private Label label1;
      private Label label2;
      private ListView SelectionListView;
      private ListView SourceListView;
   }
}