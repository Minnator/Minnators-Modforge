namespace Editor.Forms.GetUserInput
{
   partial class GetSingleString
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
         textBox1 = new TextBox();
         ConfirmButton = new Button();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 1;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
         tableLayoutPanel1.Controls.Add(label1, 0, 0);
         tableLayoutPanel1.Controls.Add(textBox1, 0, 1);
         tableLayoutPanel1.Controls.Add(ConfirmButton, 0, 2);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
         tableLayoutPanel1.Size = new Size(265, 108);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Location = new Point(3, 0);
         label1.Name = "label1";
         label1.Size = new Size(259, 50);
         label1.TabIndex = 0;
         label1.Text = "Description";
         label1.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // textBox1
         // 
         textBox1.Dock = DockStyle.Fill;
         textBox1.Location = new Point(3, 53);
         textBox1.Name = "textBox1";
         textBox1.Size = new Size(259, 23);
         textBox1.TabIndex = 1;
         // 
         // ConfirmButton
         // 
         ConfirmButton.Dock = DockStyle.Fill;
         ConfirmButton.Location = new Point(3, 80);
         ConfirmButton.Name = "ConfirmButton";
         ConfirmButton.Size = new Size(259, 25);
         ConfirmButton.TabIndex = 2;
         ConfirmButton.Text = "Confirm";
         ConfirmButton.UseVisualStyleBackColor = true;
         // 
         // GetSingleString
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(265, 108);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.FixedSingle;
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "GetSingleString";
         Text = "Get unser input";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private Label label1;
      private TextBox textBox1;
      private Button ConfirmButton;
   }
}