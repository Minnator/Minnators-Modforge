namespace Editor.Forms
{
   partial class EnterPathForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnterPathForm));
         tableLayoutPanel1 = new TableLayoutPanel();
         SelectVanillaPathButton = new Button();
         SelectModPathButton = new Button();
         label1 = new Label();
         label2 = new Label();
         VanillaPathTextBox = new TextBox();
         ModPathTextBox = new TextBox();
         ContinueButton = new Button();
         CancelButton = new Button();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 3;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15.98878F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 84.01122F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 37F));
         tableLayoutPanel1.Controls.Add(SelectVanillaPathButton, 2, 0);
         tableLayoutPanel1.Controls.Add(SelectModPathButton, 2, 1);
         tableLayoutPanel1.Controls.Add(label1, 0, 0);
         tableLayoutPanel1.Controls.Add(label2, 0, 1);
         tableLayoutPanel1.Controls.Add(VanillaPathTextBox, 1, 0);
         tableLayoutPanel1.Controls.Add(ModPathTextBox, 1, 1);
         tableLayoutPanel1.Controls.Add(ContinueButton, 1, 2);
         tableLayoutPanel1.Controls.Add(CancelButton, 0, 2);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel1.Size = new Size(751, 84);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // SelectVanillaPathButton
         // 
         SelectVanillaPathButton.Dock = DockStyle.Fill;
         SelectVanillaPathButton.Image = (Image)resources.GetObject("SelectVanillaPathButton.Image");
         SelectVanillaPathButton.Location = new Point(714, 1);
         SelectVanillaPathButton.Margin = new Padding(1);
         SelectVanillaPathButton.Name = "SelectVanillaPathButton";
         SelectVanillaPathButton.Size = new Size(36, 26);
         SelectVanillaPathButton.TabIndex = 0;
         SelectVanillaPathButton.UseVisualStyleBackColor = true;
         SelectVanillaPathButton.Click += SelectVanillaPathButton_Click;
         // 
         // SelectModPathButton
         // 
         SelectModPathButton.Dock = DockStyle.Fill;
         SelectModPathButton.Image = (Image)resources.GetObject("SelectModPathButton.Image");
         SelectModPathButton.Location = new Point(714, 29);
         SelectModPathButton.Margin = new Padding(1);
         SelectModPathButton.Name = "SelectModPathButton";
         SelectModPathButton.Size = new Size(36, 26);
         SelectModPathButton.TabIndex = 1;
         SelectModPathButton.UseVisualStyleBackColor = true;
         SelectModPathButton.Click += SelectModPathButton_Click;
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Location = new Point(3, 0);
         label1.Name = "label1";
         label1.Size = new Size(108, 28);
         label1.TabIndex = 2;
         label1.Text = "VanillaPath";
         label1.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // label2
         // 
         label2.AutoSize = true;
         label2.Dock = DockStyle.Fill;
         label2.Location = new Point(3, 28);
         label2.Name = "label2";
         label2.Size = new Size(108, 28);
         label2.TabIndex = 3;
         label2.Text = "ModPath";
         label2.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // VanillaPathTextBox
         // 
         VanillaPathTextBox.Dock = DockStyle.Fill;
         VanillaPathTextBox.Location = new Point(117, 3);
         VanillaPathTextBox.Name = "VanillaPathTextBox";
         VanillaPathTextBox.Size = new Size(593, 23);
         VanillaPathTextBox.TabIndex = 4;
         // 
         // ModPathTextBox
         // 
         ModPathTextBox.Dock = DockStyle.Fill;
         ModPathTextBox.Location = new Point(117, 31);
         ModPathTextBox.Name = "ModPathTextBox";
         ModPathTextBox.Size = new Size(593, 23);
         ModPathTextBox.TabIndex = 5;
         // 
         // ContinueButton
         // 
         ContinueButton.Dock = DockStyle.Fill;
         ContinueButton.Location = new Point(115, 57);
         ContinueButton.Margin = new Padding(1);
         ContinueButton.Name = "ContinueButton";
         ContinueButton.Size = new Size(597, 26);
         ContinueButton.TabIndex = 6;
         ContinueButton.Text = "Continue";
         ContinueButton.UseVisualStyleBackColor = true;
         ContinueButton.Click += ContinueButton_Click;
         // 
         // CancelButton
         // 
         CancelButton.Dock = DockStyle.Fill;
         CancelButton.Location = new Point(1, 57);
         CancelButton.Margin = new Padding(1);
         CancelButton.Name = "CancelButton";
         CancelButton.Size = new Size(112, 26);
         CancelButton.TabIndex = 7;
         CancelButton.Text = "Cancel";
         CancelButton.UseVisualStyleBackColor = true;
         CancelButton.Click += CancelButton_Click;
         // 
         // EnterPathForm
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(751, 84);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.None;
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "EnterPathForm";
         Text = "EnterPathForm";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private Button SelectVanillaPathButton;
      private Button SelectModPathButton;
      private Label label1;
      private Label label2;
      private TextBox VanillaPathTextBox;
      private TextBox ModPathTextBox;
      private Button ContinueButton;
      private Button CancelButton;
   }
}