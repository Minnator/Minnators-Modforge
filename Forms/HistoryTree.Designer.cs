namespace Editor.Forms
{
   partial class HistoryTree
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryTree));
         HistoryTreeView = new TreeView();
         tableLayoutPanel1 = new TableLayoutPanel();
         tableLayoutPanel2 = new TableLayoutPanel();
         RestoreButton = new Button();
         ShowAllSelections = new CheckBox();
         tableLayoutPanel1.SuspendLayout();
         tableLayoutPanel2.SuspendLayout();
         SuspendLayout();
         // 
         // HistoryTreeView
         // 
         HistoryTreeView.Dock = DockStyle.Fill;
         HistoryTreeView.Location = new Point(4, 3);
         HistoryTreeView.Margin = new Padding(4, 3, 4, 3);
         HistoryTreeView.Name = "HistoryTreeView";
         HistoryTreeView.Size = new Size(901, 556);
         HistoryTreeView.TabIndex = 0;
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 1;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Controls.Add(HistoryTreeView, 0, 0);
         tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
         tableLayoutPanel1.Size = new Size(909, 600);
         tableLayoutPanel1.TabIndex = 1;
         // 
         // tableLayoutPanel2
         // 
         tableLayoutPanel2.ColumnCount = 2;
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel2.Controls.Add(RestoreButton, 1, 0);
         tableLayoutPanel2.Controls.Add(ShowAllSelections, 0, 0);
         tableLayoutPanel2.Dock = DockStyle.Fill;
         tableLayoutPanel2.Location = new Point(0, 562);
         tableLayoutPanel2.Margin = new Padding(0);
         tableLayoutPanel2.Name = "tableLayoutPanel2";
         tableLayoutPanel2.RowCount = 1;
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel2.Size = new Size(909, 38);
         tableLayoutPanel2.TabIndex = 1;
         // 
         // RestoreButton
         // 
         RestoreButton.Dock = DockStyle.Fill;
         RestoreButton.Location = new Point(458, 3);
         RestoreButton.Margin = new Padding(4, 3, 4, 3);
         RestoreButton.Name = "RestoreButton";
         RestoreButton.Size = new Size(447, 32);
         RestoreButton.TabIndex = 1;
         RestoreButton.Text = "Restore to selected state";
         RestoreButton.UseVisualStyleBackColor = true;
         RestoreButton.Click += RestoreButton_Click;
         // 
         // ShowAllSelections
         // 
         ShowAllSelections.AutoSize = true;
         ShowAllSelections.Checked = true;
         ShowAllSelections.CheckState = CheckState.Checked;
         ShowAllSelections.Dock = DockStyle.Fill;
         ShowAllSelections.Location = new Point(4, 3);
         ShowAllSelections.Margin = new Padding(4, 3, 4, 3);
         ShowAllSelections.Name = "ShowAllSelections";
         ShowAllSelections.Padding = new Padding(12, 0, 0, 0);
         ShowAllSelections.Size = new Size(446, 32);
         ShowAllSelections.TabIndex = 2;
         ShowAllSelections.Text = "Schow all selection";
         ShowAllSelections.UseVisualStyleBackColor = true;
         ShowAllSelections.CheckedChanged += ShowAllSelections_CheckedChanged;
         // 
         // HistoryTree
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(909, 600);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Margin = new Padding(4, 3, 4, 3);
         Name = "HistoryTree";
         Text = "HistoryTree";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel2.ResumeLayout(false);
         tableLayoutPanel2.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private System.Windows.Forms.TreeView HistoryTreeView;
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      private System.Windows.Forms.Button RestoreButton;
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
      private System.Windows.Forms.CheckBox ShowAllSelections;
   }
}