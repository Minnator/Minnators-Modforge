namespace Editor.Forms.Crash_Reporter
{
   partial class CrashReporter
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrashReporter));
         MainLayoutPanel = new TableLayoutPanel();
         ModLinkBox = new RichTextBox();
         label1 = new Label();
         label2 = new Label();
         label3 = new Label();
         tableLayoutPanel1 = new TableLayoutPanel();
         CancelButton = new Button();
         SaveButton = new Button();
         linkLabel1 = new LinkLabel();
         label4 = new Label();
         textBox1 = new TextBox();
         MainLayoutPanel.SuspendLayout();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // MainLayoutPanel
         // 
         MainLayoutPanel.ColumnCount = 1;
         MainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         MainLayoutPanel.Controls.Add(ModLinkBox, 0, 5);
         MainLayoutPanel.Controls.Add(label1, 0, 4);
         MainLayoutPanel.Controls.Add(label2, 0, 1);
         MainLayoutPanel.Controls.Add(label3, 0, 2);
         MainLayoutPanel.Controls.Add(tableLayoutPanel1, 0, 5);
         MainLayoutPanel.Controls.Add(label4, 0, 0);
         MainLayoutPanel.Controls.Add(textBox1, 0, 3);
         MainLayoutPanel.Dock = DockStyle.Fill;
         MainLayoutPanel.Location = new Point(0, 0);
         MainLayoutPanel.Margin = new Padding(3, 8, 3, 3);
         MainLayoutPanel.Name = "MainLayoutPanel";
         MainLayoutPanel.RowCount = 7;
         MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
         MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
         MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
         MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
         MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         MainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         MainLayoutPanel.Size = new Size(604, 354);
         MainLayoutPanel.TabIndex = 0;
         // 
         // ModLinkBox
         // 
         ModLinkBox.Dock = DockStyle.Fill;
         ModLinkBox.Location = new Point(3, 297);
         ModLinkBox.Multiline = false;
         ModLinkBox.Name = "ModLinkBox";
         ModLinkBox.Size = new Size(598, 24);
         ModLinkBox.TabIndex = 0;
         ModLinkBox.Text = "";
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Location = new Point(3, 274);
         label1.Name = "label1";
         label1.Size = new Size(598, 20);
         label1.TabIndex = 1;
         label1.Text = "Please provide a link to the mod used (Steam workshop / PDX plaza)";
         label1.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // label2
         // 
         label2.AutoSize = true;
         label2.Dock = DockStyle.Fill;
         label2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
         label2.Location = new Point(3, 20);
         label2.Name = "label2";
         label2.Size = new Size(598, 40);
         label2.TabIndex = 2;
         label2.Text = "Remember this program is still being developed and some known or unknown bugs can and will happen.\r\nPS. You can disable the crash sound in the Settings!";
         label2.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // label3
         // 
         label3.AutoSize = true;
         label3.Dock = DockStyle.Fill;
         label3.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
         label3.Location = new Point(3, 60);
         label3.Name = "label3";
         label3.Size = new Size(598, 20);
         label3.TabIndex = 3;
         label3.Text = "Please describe shortly what you tried to do?";
         label3.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 3;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel1.Controls.Add(CancelButton, 0, 0);
         tableLayoutPanel1.Controls.Add(SaveButton, 2, 0);
         tableLayoutPanel1.Controls.Add(linkLabel1, 1, 0);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 324);
         tableLayoutPanel1.Margin = new Padding(0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 1;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Size = new Size(604, 30);
         tableLayoutPanel1.TabIndex = 4;
         // 
         // CancelButton
         // 
         CancelButton.Dock = DockStyle.Fill;
         CancelButton.Location = new Point(3, 3);
         CancelButton.Name = "CancelButton";
         CancelButton.Size = new Size(195, 24);
         CancelButton.TabIndex = 4;
         CancelButton.Text = "Cancel";
         CancelButton.UseVisualStyleBackColor = true;
         CancelButton.Click += CancelButton_Click;
         // 
         // SaveButton
         // 
         SaveButton.Dock = DockStyle.Fill;
         SaveButton.Location = new Point(405, 3);
         SaveButton.Name = "SaveButton";
         SaveButton.Size = new Size(196, 24);
         SaveButton.TabIndex = 2;
         SaveButton.Text = "Save";
         SaveButton.UseVisualStyleBackColor = true;
         SaveButton.Click += SaveButton_Click;
         // 
         // linkLabel1
         // 
         linkLabel1.AutoSize = true;
         linkLabel1.BorderStyle = BorderStyle.FixedSingle;
         linkLabel1.Dock = DockStyle.Fill;
         linkLabel1.LinkBehavior = LinkBehavior.HoverUnderline;
         linkLabel1.Location = new Point(204, 3);
         linkLabel1.Margin = new Padding(3);
         linkLabel1.Name = "linkLabel1";
         linkLabel1.Size = new Size(195, 24);
         linkLabel1.TabIndex = 3;
         linkLabel1.TabStop = true;
         linkLabel1.Text = "Bug Report Forum";
         linkLabel1.TextAlign = ContentAlignment.MiddleCenter;
         linkLabel1.LinkClicked += linkLabel1_LinkClicked;
         // 
         // label4
         // 
         label4.AutoSize = true;
         label4.Dock = DockStyle.Fill;
         label4.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
         label4.Location = new Point(3, 0);
         label4.Name = "label4";
         label4.Size = new Size(598, 20);
         label4.TabIndex = 6;
         label4.Text = "Oh no....! Something went wrong. ";
         label4.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // textBox1
         // 
         textBox1.Dock = DockStyle.Fill;
         textBox1.Location = new Point(3, 83);
         textBox1.Multiline = true;
         textBox1.Name = "textBox1";
         textBox1.Size = new Size(598, 188);
         textBox1.TabIndex = 5;
         // 
         // CrashReporter
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(604, 354);
         Controls.Add(MainLayoutPanel);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "CrashReporter";
         Text = "Crash Reporter";
         MainLayoutPanel.ResumeLayout(false);
         MainLayoutPanel.PerformLayout();
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel MainLayoutPanel;
      private RichTextBox ModLinkBox;
      private Label label1;
      private Label label2;
      private TableLayoutPanel tableLayoutPanel1;
      private Button button1;
      private Button button2;
      private Button SaveButton;
      private TextBox textBox1;
      private Label label3;
      private Label label4;
      private LinkLabel linkLabel1;
      private Button CancelButton;
   }
}