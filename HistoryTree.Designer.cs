namespace Editor
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
         this.RestoreButton = new System.Windows.Forms.Button();
         this.tableLayoutPanel1.SuspendLayout();
         this.SuspendLayout();
         // 
         // HistoryTreeView
         // 
         this.HistoryTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.HistoryTreeView.Location = new System.Drawing.Point(3, 3);
         this.HistoryTreeView.Name = "HistoryTreeView";
         this.HistoryTreeView.Size = new System.Drawing.Size(773, 480);
         this.HistoryTreeView.TabIndex = 0;
         // 
         // tableLayoutPanel1
         // 
         this.tableLayoutPanel1.ColumnCount = 1;
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.tableLayoutPanel1.Controls.Add(this.HistoryTreeView, 0, 0);
         this.tableLayoutPanel1.Controls.Add(this.RestoreButton, 0, 1);
         this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
         this.tableLayoutPanel1.Name = "tableLayoutPanel1";
         this.tableLayoutPanel1.RowCount = 2;
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
         this.tableLayoutPanel1.Size = new System.Drawing.Size(779, 520);
         this.tableLayoutPanel1.TabIndex = 1;
         // 
         // RestoreButton
         // 
         this.RestoreButton.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.RestoreButton.Location = new System.Drawing.Point(3, 489);
         this.RestoreButton.Name = "RestoreButton";
         this.RestoreButton.Size = new System.Drawing.Size(773, 28);
         this.RestoreButton.TabIndex = 1;
         this.RestoreButton.Text = "Restore to selected state";
         this.RestoreButton.UseVisualStyleBackColor = true;
         this.RestoreButton.Click += new System.EventHandler(this.RestoreButton_Click);
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
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.TreeView HistoryTreeView;
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      private System.Windows.Forms.Button RestoreButton;
   }
}