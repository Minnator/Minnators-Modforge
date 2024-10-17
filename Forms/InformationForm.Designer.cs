namespace Editor.Forms
{
   partial class InformationForm
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
         label1 = new Label();
         label2 = new Label();
         label3 = new Label();
         textBox1 = new TextBox();
         textBox2 = new TextBox();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
         tableLayoutPanel1.Controls.Add(label1, 0, 0);
         tableLayoutPanel1.Controls.Add(label2, 0, 1);
         tableLayoutPanel1.Controls.Add(label3, 0, 2);
         tableLayoutPanel1.Controls.Add(textBox1, 1, 1);
         tableLayoutPanel1.Controls.Add(textBox2, 1, 2);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Size = new Size(395, 152);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // label1
         // 
         label1.AutoSize = true;
         tableLayoutPanel1.SetColumnSpan(label1, 2);
         label1.Dock = DockStyle.Fill;
         label1.Location = new Point(3, 0);
         label1.Name = "label1";
         label1.Size = new Size(389, 40);
         label1.TabIndex = 0;
         label1.Text = "Contact Information";
         label1.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // label2
         // 
         label2.AutoSize = true;
         label2.Dock = DockStyle.Fill;
         label2.Location = new Point(3, 40);
         label2.Name = "label2";
         label2.Size = new Size(112, 56);
         label2.TabIndex = 1;
         label2.Text = "GitHub Repository / Source Code";
         label2.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // label3
         // 
         label3.AutoSize = true;
         label3.Dock = DockStyle.Fill;
         label3.Location = new Point(3, 96);
         label3.Name = "label3";
         label3.Size = new Size(112, 56);
         label3.TabIndex = 2;
         label3.Text = "Official Discord Server";
         label3.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // textBox1
         // 
         textBox1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
         textBox1.Location = new Point(121, 56);
         textBox1.Name = "textBox1";
         textBox1.ReadOnly = true;
         textBox1.Size = new Size(271, 23);
         textBox1.TabIndex = 3;
         textBox1.Text = "https://github.com/Minnator/Editor.git";
         // 
         // textBox2
         // 
         textBox2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
         textBox2.Location = new Point(121, 112);
         textBox2.Name = "textBox2";
         textBox2.ReadOnly = true;
         textBox2.Size = new Size(271, 23);
         textBox2.TabIndex = 4;
         textBox2.Text = "https://discord.gg/22AhD5qkme";
         // 
         // InformationForm
         // 
         AutoScaleMode = AutoScaleMode.None;
         ClientSize = new Size(395, 152);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.FixedToolWindow;
         Name = "InformationForm";
         StartPosition = FormStartPosition.CenterScreen;
         Text = "Information";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private Label label1;
      private Label label2;
      private Label label3;
      private TextBox textBox1;
      private TextBox textBox2;
   }
}