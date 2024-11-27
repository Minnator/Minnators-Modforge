namespace Editor.Forms.PopUps
{
   partial class CreateCountryForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateCountryForm));
         MTLP = new TableLayoutPanel();
         AvailabilityLabel = new Label();
         TagBox = new ComboBox();
         CancelButton = new Button();
         CreateButton = new Button();
         InfoLabel = new Label();
         label1 = new Label();
         label2 = new Label();
         CountryNameTextBox = new TextBox();
         CountryAdjTextBox = new TextBox();
         label3 = new Label();
         MTLP.SuspendLayout();
         SuspendLayout();
         // 
         // MTLP
         // 
         MTLP.ColumnCount = 3;
         MTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         MTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         MTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
         MTLP.Controls.Add(AvailabilityLabel, 2, 0);
         MTLP.Controls.Add(TagBox, 0, 0);
         MTLP.Controls.Add(CancelButton, 0, 5);
         MTLP.Controls.Add(CreateButton, 1, 5);
         MTLP.Controls.Add(InfoLabel, 0, 1);
         MTLP.Controls.Add(label1, 0, 2);
         MTLP.Controls.Add(label2, 0, 3);
         MTLP.Controls.Add(CountryNameTextBox, 1, 2);
         MTLP.Controls.Add(CountryAdjTextBox, 1, 3);
         MTLP.Controls.Add(label3, 0, 4);
         MTLP.Dock = DockStyle.Fill;
         MTLP.Location = new Point(0, 0);
         MTLP.Margin = new Padding(0);
         MTLP.Name = "MTLP";
         MTLP.RowCount = 6;
         MTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         MTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         MTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
         MTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
         MTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
         MTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         MTLP.Size = new Size(298, 168);
         MTLP.TabIndex = 0;
         // 
         // AvailabilityLabel
         // 
         AvailabilityLabel.AutoSize = true;
         AvailabilityLabel.Dock = DockStyle.Fill;
         AvailabilityLabel.Location = new Point(269, 1);
         AvailabilityLabel.Margin = new Padding(1);
         AvailabilityLabel.Name = "AvailabilityLabel";
         AvailabilityLabel.Size = new Size(28, 28);
         AvailabilityLabel.TabIndex = 0;
         // 
         // TagBox
         // 
         MTLP.SetColumnSpan(TagBox, 2);
         TagBox.Dock = DockStyle.Fill;
         TagBox.FormattingEnabled = true;
         TagBox.Location = new Point(3, 3);
         TagBox.Name = "TagBox";
         TagBox.Size = new Size(262, 23);
         TagBox.TabIndex = 1;
         TagBox.SelectedIndexChanged += TagBox_SelectedIndexChanged;
         TagBox.TextUpdate += TagBox_TextUpdate;
         TagBox.KeyPress += TagBox_KeyPress;
         // 
         // CancelButton
         // 
         CancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
         CancelButton.Location = new Point(1, 144);
         CancelButton.Margin = new Padding(1);
         CancelButton.Name = "CancelButton";
         CancelButton.Size = new Size(75, 23);
         CancelButton.TabIndex = 2;
         CancelButton.Text = "Cancel";
         CancelButton.UseVisualStyleBackColor = true;
         CancelButton.Click += CancelButton_Click;
         // 
         // CreateButton
         // 
         CreateButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
         MTLP.SetColumnSpan(CreateButton, 2);
         CreateButton.Location = new Point(222, 144);
         CreateButton.Margin = new Padding(1);
         CreateButton.Name = "CreateButton";
         CreateButton.Size = new Size(75, 23);
         CreateButton.TabIndex = 3;
         CreateButton.Text = "Create";
         CreateButton.UseVisualStyleBackColor = true;
         CreateButton.Click += CreateButton_Click;
         // 
         // InfoLabel
         // 
         InfoLabel.AutoSize = true;
         InfoLabel.Dock = DockStyle.Fill;
         InfoLabel.ImageAlign = ContentAlignment.MiddleLeft;
         InfoLabel.Location = new Point(1, 31);
         InfoLabel.Margin = new Padding(1);
         InfoLabel.Name = "InfoLabel";
         InfoLabel.Size = new Size(132, 23);
         InfoLabel.TabIndex = 4;
         InfoLabel.Text = "Ok";
         InfoLabel.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Location = new Point(3, 55);
         label1.Name = "label1";
         label1.Size = new Size(128, 28);
         label1.TabIndex = 5;
         label1.Text = "Country Name";
         label1.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // label2
         // 
         label2.AutoSize = true;
         label2.Dock = DockStyle.Fill;
         label2.Location = new Point(3, 83);
         label2.Name = "label2";
         label2.Size = new Size(128, 28);
         label2.TabIndex = 6;
         label2.Text = "Country Adjective";
         label2.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // CountryNameTextBox
         // 
         CountryNameTextBox.Dock = DockStyle.Fill;
         CountryNameTextBox.Location = new Point(137, 58);
         CountryNameTextBox.Name = "CountryNameTextBox";
         CountryNameTextBox.Size = new Size(128, 23);
         CountryNameTextBox.TabIndex = 7;
         // 
         // CountryAdjTextBox
         // 
         CountryAdjTextBox.Dock = DockStyle.Fill;
         CountryAdjTextBox.Location = new Point(137, 86);
         CountryAdjTextBox.Name = "CountryAdjTextBox";
         CountryAdjTextBox.Size = new Size(128, 23);
         CountryAdjTextBox.TabIndex = 8;
         // 
         // label3
         // 
         label3.AutoSize = true;
         label3.Dock = DockStyle.Fill;
         label3.Location = new Point(3, 111);
         label3.Name = "label3";
         label3.Size = new Size(128, 28);
         label3.TabIndex = 9;
         label3.Text = "Color";
         label3.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // CreateCountryForm
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(298, 168);
         Controls.Add(MTLP);
         FormBorderStyle = FormBorderStyle.FixedSingle;
         Icon = (Icon)resources.GetObject("$this.Icon");
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "CreateCountryForm";
         Text = "Create Country";
         MTLP.ResumeLayout(false);
         MTLP.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel MTLP;
      private Label AvailabilityLabel;
      private ComboBox TagBox;
      private Button CancelButton;
      private Button CreateButton;
      private Label InfoLabel;
      private Label label1;
      private Label label2;
      private TextBox CountryNameTextBox;
      private TextBox CountryAdjTextBox;
      private Label label3;
   }
}