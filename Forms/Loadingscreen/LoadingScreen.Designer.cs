namespace Editor.Forms.Loadingscreen
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
         tableLayoutPanel1 = new TableLayoutPanel();
         ContinueButton = new Button();
         LoadButton = new Button();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 1;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Controls.Add(ContinueButton, 0, 3);
         tableLayoutPanel1.Controls.Add(LoadButton, 0, 0);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 4;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Size = new Size(800, 450);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // ContinueButton
         // 
         ContinueButton.Dock = DockStyle.Fill;
         ContinueButton.Location = new Point(3, 423);
         ContinueButton.Name = "ContinueButton";
         ContinueButton.Size = new Size(794, 24);
         ContinueButton.TabIndex = 1;
         ContinueButton.Text = "Venture Forth";
         ContinueButton.UseVisualStyleBackColor = true;
         ContinueButton.Click += ContinueButton_Click;
         // 
         // LoadButton
         // 
         LoadButton.Dock = DockStyle.Fill;
         LoadButton.Location = new Point(3, 3);
         LoadButton.Name = "LoadButton";
         LoadButton.Size = new Size(794, 24);
         LoadButton.TabIndex = 2;
         LoadButton.Text = "Load";
         LoadButton.UseVisualStyleBackColor = true;
         LoadButton.Click += LoadButton_Click;
         // 
         // LoadingScreen
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(800, 450);
         Controls.Add(tableLayoutPanel1);
         Name = "LoadingScreen";
         Text = "LoadingScreen";
         tableLayoutPanel1.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      public Button ContinueButton;
      private Button LoadButton;
   }
}