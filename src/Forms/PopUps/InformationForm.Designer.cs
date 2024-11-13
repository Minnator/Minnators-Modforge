namespace Editor.Forms.PopUps
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
         label7 = new Label();
         label6 = new Label();
         label1 = new Label();
         label2 = new Label();
         label3 = new Label();
         textBox1 = new TextBox();
         textBox2 = new TextBox();
         label4 = new Label();
         label5 = new Label();
         OpenGitButton = new Button();
         OpenDiscordButton = new Button();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 3;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
         tableLayoutPanel1.Controls.Add(label7, 1, 4);
         tableLayoutPanel1.Controls.Add(label6, 1, 3);
         tableLayoutPanel1.Controls.Add(label1, 0, 0);
         tableLayoutPanel1.Controls.Add(label2, 0, 1);
         tableLayoutPanel1.Controls.Add(label3, 0, 2);
         tableLayoutPanel1.Controls.Add(textBox1, 1, 1);
         tableLayoutPanel1.Controls.Add(textBox2, 1, 2);
         tableLayoutPanel1.Controls.Add(label4, 0, 3);
         tableLayoutPanel1.Controls.Add(label5, 0, 4);
         tableLayoutPanel1.Controls.Add(OpenGitButton, 2, 1);
         tableLayoutPanel1.Controls.Add(OpenDiscordButton, 2, 2);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.Padding = new Padding(0, 0, 0, 10);
         tableLayoutPanel1.RowCount = 5;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel1.Size = new Size(484, 221);
         tableLayoutPanel1.TabIndex = 0;
         tableLayoutPanel1.Paint += tableLayoutPanel1_Paint;
         // 
         // label7
         // 
         label7.AutoSize = true;
         label7.Dock = DockStyle.Fill;
         label7.Location = new Point(118, 185);
         label7.Name = "label7";
         label7.Size = new Size(262, 26);
         label7.TabIndex = 8;
         label7.Text = "@melon_coaster (Discord)";
         label7.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // label6
         // 
         label6.AutoSize = true;
         label6.Dock = DockStyle.Fill;
         label6.Location = new Point(118, 160);
         label6.Name = "label6";
         label6.Size = new Size(262, 25);
         label6.TabIndex = 7;
         label6.Text = "@minnator (Discord)";
         label6.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // label1
         // 
         label1.AutoSize = true;
         tableLayoutPanel1.SetColumnSpan(label1, 3);
         label1.Dock = DockStyle.Fill;
         label1.Location = new Point(3, 0);
         label1.Name = "label1";
         label1.Size = new Size(478, 40);
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
         label2.Size = new Size(109, 60);
         label2.TabIndex = 1;
         label2.Text = "GitHub Repository / Source Code";
         label2.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // label3
         // 
         label3.AutoSize = true;
         label3.Dock = DockStyle.Fill;
         label3.Location = new Point(3, 100);
         label3.Name = "label3";
         label3.Size = new Size(109, 60);
         label3.TabIndex = 2;
         label3.Text = "Official Discord Server";
         label3.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // textBox1
         // 
         textBox1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
         textBox1.Location = new Point(118, 58);
         textBox1.Name = "textBox1";
         textBox1.ReadOnly = true;
         textBox1.Size = new Size(262, 23);
         textBox1.TabIndex = 3;
         // 
         // textBox2
         // 
         textBox2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
         textBox2.Location = new Point(118, 118);
         textBox2.Name = "textBox2";
         textBox2.ReadOnly = true;
         textBox2.Size = new Size(262, 23);
         textBox2.TabIndex = 4;
         // 
         // label4
         // 
         label4.AutoSize = true;
         label4.Dock = DockStyle.Fill;
         label4.Location = new Point(3, 160);
         label4.Name = "label4";
         label4.Size = new Size(109, 25);
         label4.TabIndex = 5;
         label4.Text = "Main Programmer";
         label4.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // label5
         // 
         label5.AutoSize = true;
         label5.Dock = DockStyle.Fill;
         label5.Location = new Point(3, 185);
         label5.Name = "label5";
         label5.Size = new Size(109, 26);
         label5.TabIndex = 6;
         label5.Text = "Chief Overthinker";
         label5.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // OpenGitButton
         // 
         OpenGitButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
         OpenGitButton.Location = new Point(386, 58);
         OpenGitButton.Name = "OpenGitButton";
         OpenGitButton.Size = new Size(95, 23);
         OpenGitButton.TabIndex = 9;
         OpenGitButton.Text = "Open";
         OpenGitButton.UseVisualStyleBackColor = true;
         OpenGitButton.Click += OpenGitButton_Click;
         // 
         // OpenDiscordButton
         // 
         OpenDiscordButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
         OpenDiscordButton.Location = new Point(386, 118);
         OpenDiscordButton.Name = "OpenDiscordButton";
         OpenDiscordButton.Size = new Size(95, 23);
         OpenDiscordButton.TabIndex = 10;
         OpenDiscordButton.Text = "Open";
         OpenDiscordButton.UseVisualStyleBackColor = true;
         OpenDiscordButton.Click += OpenDiscordButton_Click;
         // 
         // InformationForm
         // 
         AutoScaleMode = AutoScaleMode.None;
         ClientSize = new Size(484, 221);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.FixedSingle;
         MaximizeBox = false;
         MinimizeBox = false;
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
      private Label label7;
      private Label label6;
      private Label label4;
      private Label label5;
      private Button OpenGitButton;
      private Button OpenDiscordButton;
   }
}