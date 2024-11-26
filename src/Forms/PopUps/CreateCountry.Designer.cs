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
         tableLayoutPanel1 = new TableLayoutPanel();
         AvailabilityLabel = new Label();
         TagBox = new ComboBox();
         CancelButton = new Button();
         CreateButton = new Button();
         InfoLabel = new Label();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 3;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Controls.Add(AvailabilityLabel, 2, 0);
         tableLayoutPanel1.Controls.Add(TagBox, 0, 0);
         tableLayoutPanel1.Controls.Add(CancelButton, 0, 2);
         tableLayoutPanel1.Controls.Add(CreateButton, 1, 2);
         tableLayoutPanel1.Controls.Add(InfoLabel, 0, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Margin = new Padding(0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Size = new Size(298, 88);
         tableLayoutPanel1.TabIndex = 0;
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
         tableLayoutPanel1.SetColumnSpan(TagBox, 2);
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
         CancelButton.Location = new Point(1, 64);
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
         tableLayoutPanel1.SetColumnSpan(CreateButton, 2);
         CreateButton.Location = new Point(222, 64);
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
         // CreateCountryForm
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(298, 88);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "CreateCountryForm";
         Text = "Create Country";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private Label AvailabilityLabel;
      private ComboBox TagBox;
      private Button CancelButton;
      private Button CreateButton;
      private Label InfoLabel;
   }
}