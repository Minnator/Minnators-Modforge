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
         this.HistoryTreeView = new System.Windows.Forms.TreeView();
         this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
         this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
         this.RestoreButton = new System.Windows.Forms.Button();
         this.ShowAllSelections = new System.Windows.Forms.CheckBox();
         this.tableLayoutPanel1.SuspendLayout();
         this.tableLayoutPanel2.SuspendLayout();
         this.SuspendLayout();
         // 
         // HistoryTreeView
         // 
         this.HistoryTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.HistoryTreeView.Location = new System.Drawing.Point(3, 3);
         this.HistoryTreeView.Name = "HistoryTreeView";
         this.HistoryTreeView.Size = new System.Drawing.Size(773, 481);
         this.HistoryTreeView.TabIndex = 0;
         // 
         // tableLayoutPanel1
         // 
         this.tableLayoutPanel1.ColumnCount = 1;
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.tableLayoutPanel1.Controls.Add(this.HistoryTreeView, 0, 0);
         this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
         this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
         this.tableLayoutPanel1.Name = "tableLayoutPanel1";
         this.tableLayoutPanel1.RowCount = 2;
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
         this.tableLayoutPanel1.Size = new System.Drawing.Size(779, 520);
         this.tableLayoutPanel1.TabIndex = 1;
         // 
         // tableLayoutPanel2
         // 
         this.tableLayoutPanel2.ColumnCount = 2;
         this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.tableLayoutPanel2.Controls.Add(this.RestoreButton, 1, 0);
         this.tableLayoutPanel2.Controls.Add(this.ShowAllSelections, 0, 0);
         this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 487);
         this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
         this.tableLayoutPanel2.Name = "tableLayoutPanel2";
         this.tableLayoutPanel2.RowCount = 1;
         this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.tableLayoutPanel2.Size = new System.Drawing.Size(779, 33);
         this.tableLayoutPanel2.TabIndex = 1;
         // 
         // RestoreButton
         // 
         this.RestoreButton.Dock = System.Windows.Forms.DockStyle.Fill;
         this.RestoreButton.Location = new System.Drawing.Point(392, 3);
         this.RestoreButton.Name = "RestoreButton";
         this.RestoreButton.Size = new System.Drawing.Size(384, 27);
         this.RestoreButton.TabIndex = 1;
         this.RestoreButton.Text = "Restore to selected state";
         this.RestoreButton.UseVisualStyleBackColor = true;
         this.RestoreButton.Click += new System.EventHandler(this.RestoreButton_Click);
         // 
         // ShowAllSelections
         // 
         this.ShowAllSelections.AutoSize = true;
         this.ShowAllSelections.Checked = true;
         this.ShowAllSelections.CheckState = System.Windows.Forms.CheckState.Checked;
         this.ShowAllSelections.Dock = System.Windows.Forms.DockStyle.Fill;
         this.ShowAllSelections.Location = new System.Drawing.Point(3, 3);
         this.ShowAllSelections.Name = "ShowAllSelections";
         this.ShowAllSelections.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
         this.ShowAllSelections.Size = new System.Drawing.Size(383, 27);
         this.ShowAllSelections.TabIndex = 2;
         this.ShowAllSelections.Text = "Schow all selection";
         this.ShowAllSelections.UseVisualStyleBackColor = true;
         this.ShowAllSelections.CheckedChanged += new System.EventHandler(this.ShowAllSelections_CheckedChanged);
         // 
         // HistoryTree
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(779, 520);
         this.Controls.Add(this.tableLayoutPanel1);
         this.Name = "HistoryTree";
         this.Text = "HistoryTree";
         this.tableLayoutPanel1.ResumeLayout(false);
         this.tableLayoutPanel2.ResumeLayout(false);
         this.tableLayoutPanel2.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.TreeView HistoryTreeView;
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      private System.Windows.Forms.Button RestoreButton;
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
      private System.Windows.Forms.CheckBox ShowAllSelections;
   }
}