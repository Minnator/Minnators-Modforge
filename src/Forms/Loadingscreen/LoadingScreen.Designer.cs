namespace Editor.Forms.LoadingScreen
{
   partial class LoadingScreen
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingScreen));
         tableLayoutPanel1 = new TableLayoutPanel();
         LoadingAnimation = new PictureBox();
         tableLayoutPanel1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)LoadingAnimation).BeginInit();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 1;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Controls.Add(LoadingAnimation, 0, 0);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
         tableLayoutPanel1.Size = new Size(512, 585);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // LoadingAnimation
         // 
         LoadingAnimation.Dock = DockStyle.Fill;
         LoadingAnimation.Location = new Point(3, 3);
         LoadingAnimation.Name = "LoadingAnimation";
         LoadingAnimation.Size = new Size(506, 519);
         LoadingAnimation.TabIndex = 3;
         LoadingAnimation.TabStop = false;
         // 
         // LoadingScreen
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(512, 585);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.None;
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "LoadingScreen";
         Text = "LoadingScreen";
         tableLayoutPanel1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)LoadingAnimation).EndInit();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private PictureBox LoadingAnimation;
   }
}