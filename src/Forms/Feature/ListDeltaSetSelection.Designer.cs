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
         ConfirmButton = new Button();
         SourceSearchBox = new ComboBox();
         label2 = new Label();
         label1 = new Label();
         button1 = new Button();
         button2 = new Button();
         SelectionListBox = new ListBox();
         SourceListBox = new ListBox();
         RemoveCancelsAdd = new CheckBox();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 4;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Controls.Add(ConfirmButton, 3, 4);
         tableLayoutPanel1.Controls.Add(SourceSearchBox, 0, 1);
         tableLayoutPanel1.Controls.Add(label2, 0, 0);
         tableLayoutPanel1.Controls.Add(label1, 3, 0);
         tableLayoutPanel1.Controls.Add(button1, 2, 3);
         tableLayoutPanel1.Controls.Add(button2, 1, 3);
         tableLayoutPanel1.Controls.Add(SelectionListBox, 3, 2);
         tableLayoutPanel1.Controls.Add(SourceListBox, 0, 2);
         tableLayoutPanel1.Controls.Add(RemoveCancelsAdd, 1, 2);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 4;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Size = new Size(784, 461);
         tableLayoutPanel1.TabIndex = 0;
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
         // SourceSearchBox
         // 
         SourceSearchBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         SourceSearchBox.AutoCompleteSource = AutoCompleteSource.ListItems;
         SourceSearchBox.Dock = DockStyle.Fill;
         SourceSearchBox.Location = new Point(3, 33);
         SourceSearchBox.Name = "SourceSearchBox";
         SourceSearchBox.Size = new Size(311, 23);
         SourceSearchBox.TabIndex = 6;
         // 
         // label2
         // 
         label2.AutoSize = true;
         label2.Dock = DockStyle.Fill;
         label2.Location = new Point(3, 0);
         label2.Name = "label2";
         label2.Size = new Size(311, 30);
         label2.TabIndex = 6;
         label2.Text = "Source List";
         label2.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Location = new Point(470, 0);
         label1.Name = "label1";
         tableLayoutPanel1.SetRowSpan(label1, 2);
         label1.Size = new Size(311, 60);
         label1.TabIndex = 5;
         label1.Text = "Selection List";
         label1.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // button1
         // 
         button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
         button1.Location = new Point(393, 407);
         button1.Margin = new Padding(1);
         button1.Name = "button1";
         button1.Size = new Size(73, 23);
         button1.TabIndex = 9;
         button1.Text = "Remove";
         button1.UseVisualStyleBackColor = true;
         button1.Click += button1_Click;
         // 
         // button2
         // 
         button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
         button2.Location = new Point(318, 407);
         button2.Margin = new Padding(1);
         button2.Name = "button2";
         button2.Size = new Size(73, 23);
         button2.TabIndex = 10;
         button2.Text = "Add";
         button2.UseVisualStyleBackColor = true;
         button2.Click += button2_Click;
         // 
         // SelectionListBox
         // 
         SelectionListBox.Dock = DockStyle.Fill;
         SelectionListBox.FormattingEnabled = true;
         SelectionListBox.ItemHeight = 15;
         SelectionListBox.Location = new Point(470, 63);
         SelectionListBox.Name = "SelectionListBox";
         tableLayoutPanel1.SetRowSpan(SelectionListBox, 2);
         SelectionListBox.Size = new Size(311, 365);
         SelectionListBox.Sorted = true;
         SelectionListBox.TabIndex = 11;
         // 
         // SourceListBox
         // 
         SourceListBox.Dock = DockStyle.Fill;
         SourceListBox.FormattingEnabled = true;
         SourceListBox.ItemHeight = 15;
         SourceListBox.Location = new Point(3, 63);
         SourceListBox.Name = "SourceListBox";
         tableLayoutPanel1.SetRowSpan(SourceListBox, 2);
         SourceListBox.Size = new Size(311, 365);
         SourceListBox.Sorted = true;
         SourceListBox.TabIndex = 12;
         // 
         // RemoveCancelsAdd
         // 
         RemoveCancelsAdd.AutoSize = true;
         tableLayoutPanel1.SetColumnSpan(RemoveCancelsAdd, 2);
         RemoveCancelsAdd.Dock = DockStyle.Fill;
         RemoveCancelsAdd.Location = new Point(320, 63);
         RemoveCancelsAdd.Name = "RemoveCancelsAdd";
         RemoveCancelsAdd.Size = new Size(144, 24);
         RemoveCancelsAdd.TabIndex = 13;
         RemoveCancelsAdd.Text = "Remove cancels add";
         RemoveCancelsAdd.UseVisualStyleBackColor = true;
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
      private ComboBox SourceSearchBox;
      private Label label1;
      private Label label2;
      private Button button1;
      private Button button2;
      private ListBox SelectionListBox;
      private ListBox SourceListBox;
      private CheckBox RemoveCancelsAdd;
   }
}