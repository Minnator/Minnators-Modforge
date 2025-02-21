namespace Editor.Forms.PopUps
{
   partial class ImprovedMessageBox
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImprovedMessageBox));
         tableLayoutPanel1 = new TableLayoutPanel();
         DescLabel = new Label();
         IconBox = new PictureBox();
         tableLayoutPanel2 = new TableLayoutPanel();
         PrimaryButton = new Button();
         SecondaryButton = new Button();
         TertiaryButton = new Button();
         DoShowAgainCheckBox = new CheckBox();
         tableLayoutPanel1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)IconBox).BeginInit();
         tableLayoutPanel2.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Controls.Add(DescLabel, 1, 0);
         tableLayoutPanel1.Controls.Add(IconBox, 0, 0);
         tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 2);
         tableLayoutPanel1.Controls.Add(DoShowAgainCheckBox, 1, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Margin = new Padding(0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
         tableLayoutPanel1.Size = new Size(400, 123);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // DescLabel
         // 
         DescLabel.AutoSize = true;
         DescLabel.Dock = DockStyle.Fill;
         DescLabel.Location = new Point(78, 3);
         DescLabel.Margin = new Padding(3);
         DescLabel.Name = "DescLabel";
         DescLabel.Size = new Size(319, 47);
         DescLabel.TabIndex = 0;
         DescLabel.Text = "example";
         DescLabel.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // IconBox
         // 
         IconBox.Dock = DockStyle.Fill;
         IconBox.Location = new Point(3, 3);
         IconBox.Name = "IconBox";
         tableLayoutPanel1.SetRowSpan(IconBox, 2);
         IconBox.Size = new Size(69, 77);
         IconBox.SizeMode = PictureBoxSizeMode.CenterImage;
         IconBox.TabIndex = 1;
         IconBox.TabStop = false;
         // 
         // tableLayoutPanel2
         // 
         tableLayoutPanel2.BackColor = SystemColors.ControlLight;
         tableLayoutPanel2.ColumnCount = 3;
         tableLayoutPanel1.SetColumnSpan(tableLayoutPanel2, 2);
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel2.Controls.Add(PrimaryButton, 0, 0);
         tableLayoutPanel2.Controls.Add(SecondaryButton, 1, 0);
         tableLayoutPanel2.Controls.Add(TertiaryButton, 2, 0);
         tableLayoutPanel2.Dock = DockStyle.Fill;
         tableLayoutPanel2.Location = new Point(0, 83);
         tableLayoutPanel2.Margin = new Padding(0);
         tableLayoutPanel2.Name = "tableLayoutPanel2";
         tableLayoutPanel2.RowCount = 1;
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel2.Size = new Size(400, 40);
         tableLayoutPanel2.TabIndex = 2;
         // 
         // PrimaryButton
         // 
         PrimaryButton.Anchor = AnchorStyles.None;
         PrimaryButton.Location = new Point(29, 8);
         PrimaryButton.Name = "PrimaryButton";
         PrimaryButton.Size = new Size(75, 23);
         PrimaryButton.TabIndex = 0;
         PrimaryButton.Text = "button1";
         PrimaryButton.UseVisualStyleBackColor = true;
         PrimaryButton.Visible = false;
         // 
         // SecondaryButton
         // 
         SecondaryButton.Anchor = AnchorStyles.None;
         SecondaryButton.Location = new Point(162, 8);
         SecondaryButton.Name = "SecondaryButton";
         SecondaryButton.Size = new Size(75, 23);
         SecondaryButton.TabIndex = 1;
         SecondaryButton.Text = "button2";
         SecondaryButton.UseVisualStyleBackColor = true;
         SecondaryButton.Visible = false;
         // 
         // TertiaryButton
         // 
         TertiaryButton.Anchor = AnchorStyles.None;
         TertiaryButton.Location = new Point(295, 8);
         TertiaryButton.Name = "TertiaryButton";
         TertiaryButton.Size = new Size(75, 23);
         TertiaryButton.TabIndex = 2;
         TertiaryButton.Text = "button3";
         TertiaryButton.UseVisualStyleBackColor = true;
         TertiaryButton.Visible = false;
         // 
         // DoShowAgainCheckBox
         // 
         DoShowAgainCheckBox.AutoSize = true;
         DoShowAgainCheckBox.Dock = DockStyle.Fill;
         DoShowAgainCheckBox.Location = new Point(78, 56);
         DoShowAgainCheckBox.Name = "DoShowAgainCheckBox";
         DoShowAgainCheckBox.Size = new Size(319, 24);
         DoShowAgainCheckBox.TabIndex = 3;
         DoShowAgainCheckBox.Text = "Do not ask again";
         DoShowAgainCheckBox.UseVisualStyleBackColor = true;
         // 
         // ImprovedMessageBox
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(400, 123);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.FixedSingle;
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "ImprovedMessageBox";
         Text = "ImpCheckBox";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)IconBox).EndInit();
         tableLayoutPanel2.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private Label DescLabel;
      private PictureBox IconBox;
      private TableLayoutPanel tableLayoutPanel2;
      private CheckBox DoShowAgainCheckBox;
      private Button PrimaryButton;
      private Button SecondaryButton;
      private Button TertiaryButton;
   }
}