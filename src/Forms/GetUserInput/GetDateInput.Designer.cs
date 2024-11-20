namespace Editor.src.Forms.GetUserInput
{
   partial class GetDateInput
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetDateInput));
         tableLayoutPanel1 = new TableLayoutPanel();
         DescLabel = new Label();
         label1 = new Label();
         label23 = new Label();
         labe24 = new Label();
         DayBox = new ComboBox();
         MonthBox = new ComboBox();
         YearBox = new ComboBox();
         ConfirmButton = new Button();
         CancelButton = new Button();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 3;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 49.03846F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.96154F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 117F));
         tableLayoutPanel1.Controls.Add(DescLabel, 0, 0);
         tableLayoutPanel1.Controls.Add(label1, 0, 1);
         tableLayoutPanel1.Controls.Add(label23, 1, 1);
         tableLayoutPanel1.Controls.Add(labe24, 2, 1);
         tableLayoutPanel1.Controls.Add(DayBox, 0, 2);
         tableLayoutPanel1.Controls.Add(MonthBox, 1, 2);
         tableLayoutPanel1.Controls.Add(YearBox, 2, 2);
         tableLayoutPanel1.Controls.Add(ConfirmButton, 2, 3);
         tableLayoutPanel1.Controls.Add(CancelButton, 0, 3);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 4;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 19F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Size = new Size(326, 105);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // DescLabel
         // 
         DescLabel.AutoSize = true;
         tableLayoutPanel1.SetColumnSpan(DescLabel, 3);
         DescLabel.Dock = DockStyle.Fill;
         DescLabel.Location = new Point(3, 0);
         DescLabel.Name = "DescLabel";
         DescLabel.Size = new Size(320, 28);
         DescLabel.TabIndex = 0;
         DescLabel.Text = "label1";
         DescLabel.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Location = new Point(3, 28);
         label1.Name = "label1";
         label1.Size = new Size(96, 19);
         label1.TabIndex = 1;
         label1.Text = "Day";
         label1.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // label23
         // 
         label23.AutoSize = true;
         label23.Dock = DockStyle.Fill;
         label23.Location = new Point(105, 28);
         label23.Name = "label23";
         label23.Size = new Size(100, 19);
         label23.TabIndex = 2;
         label23.Text = "Month";
         label23.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // labe24
         // 
         labe24.AutoSize = true;
         labe24.Dock = DockStyle.Fill;
         labe24.Location = new Point(211, 28);
         labe24.Name = "labe24";
         labe24.Size = new Size(112, 19);
         labe24.TabIndex = 3;
         labe24.Text = "Year";
         labe24.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // DayBox
         // 
         DayBox.Dock = DockStyle.Fill;
         DayBox.DropDownStyle = ComboBoxStyle.DropDownList;
         DayBox.FormattingEnabled = true;
         DayBox.Location = new Point(3, 48);
         DayBox.Margin = new Padding(3, 1, 1, 1);
         DayBox.Name = "DayBox";
         DayBox.Size = new Size(98, 23);
         DayBox.TabIndex = 4;
         // 
         // MonthBox
         // 
         MonthBox.Dock = DockStyle.Fill;
         MonthBox.DropDownStyle = ComboBoxStyle.DropDownList;
         MonthBox.FormattingEnabled = true;
         MonthBox.Location = new Point(103, 48);
         MonthBox.Margin = new Padding(1);
         MonthBox.Name = "MonthBox";
         MonthBox.Size = new Size(104, 23);
         MonthBox.TabIndex = 5;
         // 
         // YearBox
         // 
         YearBox.Dock = DockStyle.Fill;
         YearBox.FormattingEnabled = true;
         YearBox.Location = new Point(209, 48);
         YearBox.Margin = new Padding(1, 1, 3, 1);
         YearBox.Name = "YearBox";
         YearBox.Size = new Size(114, 23);
         YearBox.TabIndex = 6;
         // 
         // ConfirmButton
         // 
         ConfirmButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
         ConfirmButton.Location = new Point(248, 79);
         ConfirmButton.Name = "ConfirmButton";
         ConfirmButton.Size = new Size(75, 23);
         ConfirmButton.TabIndex = 7;
         ConfirmButton.Text = "Confirm";
         ConfirmButton.UseVisualStyleBackColor = true;
         ConfirmButton.Click += ConfirmButton_Click;
         // 
         // CancelButton
         // 
         CancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
         CancelButton.Location = new Point(3, 79);
         CancelButton.Name = "CancelButton";
         CancelButton.Size = new Size(75, 23);
         CancelButton.TabIndex = 8;
         CancelButton.Text = "Cancel";
         CancelButton.UseVisualStyleBackColor = true;
         CancelButton.Click += CancelButton_Click;
         // 
         // GetDateInput
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(326, 105);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "GetDateInput";
         Text = "Input Date Provider";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private Label DescLabel;
      private Label label1;
      private Label label23;
      private Label labe24;
      private ComboBox DayBox;
      private ComboBox MonthBox;
      private ComboBox YearBox;
      private Button ConfirmButton;
      private Button CancelButton;
   }
}