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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListDeltaSetSelection));
         tableLayoutPanel1 = new TableLayoutPanel();
         SourceListBox = new ListBox();
         DeltaLabel = new Label();
         ConfirmButton = new Button();
         IsSetCheckBox = new CheckBox();
         SelectionListBox = new ListBox();
         label1 = new Label();
         label2 = new Label();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 3;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 383F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 301F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
         tableLayoutPanel1.Controls.Add(SourceListBox, 0, 1);
         tableLayoutPanel1.Controls.Add(DeltaLabel, 0, 2);
         tableLayoutPanel1.Controls.Add(ConfirmButton, 2, 2);
         tableLayoutPanel1.Controls.Add(IsSetCheckBox, 1, 2);
         tableLayoutPanel1.Controls.Add(SelectionListBox, 1, 1);
         tableLayoutPanel1.Controls.Add(label1, 1, 0);
         tableLayoutPanel1.Controls.Add(label2, 0, 0);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Size = new Size(784, 461);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // SourceListBox
         // 
         SourceListBox.Dock = DockStyle.Fill;
         SourceListBox.FormattingEnabled = true;
         SourceListBox.ItemHeight = 15;
         SourceListBox.Location = new Point(3, 33);
         SourceListBox.Name = "SourceListBox";
         SourceListBox.Size = new Size(377, 395);
         SourceListBox.TabIndex = 1;
         // 
         // DeltaLabel
         // 
         DeltaLabel.AutoSize = true;
         DeltaLabel.Dock = DockStyle.Fill;
         DeltaLabel.Location = new Point(3, 431);
         DeltaLabel.Name = "DeltaLabel";
         DeltaLabel.Size = new Size(377, 30);
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
         // IsSetCheckBox
         // 
         IsSetCheckBox.AutoSize = true;
         IsSetCheckBox.Dock = DockStyle.Right;
         IsSetCheckBox.Location = new Point(572, 434);
         IsSetCheckBox.Name = "IsSetCheckBox";
         IsSetCheckBox.Size = new Size(109, 24);
         IsSetCheckBox.TabIndex = 2;
         IsSetCheckBox.Text = "Set/Delta(Delta)";
         IsSetCheckBox.UseVisualStyleBackColor = true;
         // 
         // SelectionListBox
         // 
         tableLayoutPanel1.SetColumnSpan(SelectionListBox, 2);
         SelectionListBox.Dock = DockStyle.Fill;
         SelectionListBox.FormattingEnabled = true;
         SelectionListBox.ItemHeight = 15;
         SelectionListBox.Location = new Point(386, 33);
         SelectionListBox.Name = "SelectionListBox";
         SelectionListBox.Size = new Size(395, 395);
         SelectionListBox.TabIndex = 0;
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
         // ListDeltaSetSelection
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(784, 461);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.FixedSingle;
         Icon = (Icon)resources.GetObject("$this.Icon");
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
      private ListBox SourceListBox;
      private ListBox SelectionListBox;
      private CheckBox IsSetCheckBox;
      private Button ConfirmButton;
      private Label DeltaLabel;
      private Label label1;
      private Label label2;
   }
}