namespace Editor.Forms.PopUps
{
   partial class UsageWarningForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UsageWarningForm));
         tableLayoutPanel1 = new TableLayoutPanel();
         RichTextBox = new RichTextBox();
         CancelButton = new Button();
         AgreeButton = new Button();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Controls.Add(RichTextBox, 0, 0);
         tableLayoutPanel1.Controls.Add(CancelButton, 0, 1);
         tableLayoutPanel1.Controls.Add(AgreeButton, 1, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Size = new Size(691, 362);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // RichTextBox
         // 
         tableLayoutPanel1.SetColumnSpan(RichTextBox, 2);
         RichTextBox.Dock = DockStyle.Fill;
         RichTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
         RichTextBox.Location = new Point(3, 3);
         RichTextBox.Name = "RichTextBox";
         RichTextBox.ReadOnly = true;
         RichTextBox.Size = new Size(685, 326);
         RichTextBox.TabIndex = 0;
         RichTextBox.Text = "";
         // 
         // CancelButton
         // 
         CancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
         CancelButton.Location = new Point(3, 336);
         CancelButton.Name = "CancelButton";
         CancelButton.Size = new Size(100, 23);
         CancelButton.TabIndex = 1;
         CancelButton.Text = "Cancel";
         CancelButton.UseVisualStyleBackColor = true;
         CancelButton.Click += CancelButton_Click;
         // 
         // AgreeButton
         // 
         AgreeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
         AgreeButton.Location = new Point(588, 336);
         AgreeButton.Name = "AgreeButton";
         AgreeButton.Size = new Size(100, 23);
         AgreeButton.TabIndex = 2;
         AgreeButton.Text = "I Understand";
         AgreeButton.UseVisualStyleBackColor = true;
         AgreeButton.Click += AgreeButton_Click;
         // 
         // UsageWarningForm
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(691, 362);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "UsageWarningForm";
         StartPosition = FormStartPosition.CenterParent;
         Text = "Experimental Software Warning";
         tableLayoutPanel1.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private RichTextBox RichTextBox;
      private Button CancelButton;
      private Button AgreeButton;
   }
}