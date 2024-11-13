namespace Editor.Forms.Feature
{
   partial class ProgressDisplay
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressDisplay));
         tableLayoutPanel1 = new TableLayoutPanel();
         ProgressBar = new ProgressBar();
         Description = new Label();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 1;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Controls.Add(ProgressBar, 0, 1);
         tableLayoutPanel1.Controls.Add(Description, 0, 0);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Size = new Size(395, 71);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // ProgressBar
         // 
         ProgressBar.Dock = DockStyle.Fill;
         ProgressBar.Location = new Point(3, 38);
         ProgressBar.Name = "ProgressBar";
         ProgressBar.Size = new Size(389, 30);
         ProgressBar.Step = 1;
         ProgressBar.TabIndex = 0;
         // 
         // Description
         // 
         Description.AutoSize = true;
         Description.Dock = DockStyle.Fill;
         Description.Location = new Point(3, 0);
         Description.Name = "Description";
         Description.Size = new Size(389, 35);
         Description.TabIndex = 1;
         Description.Text = "loading...";
         Description.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // ProgressDisplay
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(395, 71);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.None;
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "ProgressDisplay";
         Text = "ProgressDisplay";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private ProgressBar ProgressBar;
      private Label Description;
   }
}