namespace Editor.src.Forms.Console
{
   partial class ConsoleForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsoleForm));
         MTLP = new TableLayoutPanel();
         OutputBox = new RichTextBox();
         InputBox = new TextBox();
         MTLP.SuspendLayout();
         SuspendLayout();
         // 
         // MTLP
         // 
         MTLP.BackColor = Color.Black;
         MTLP.ColumnCount = 1;
         MTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         MTLP.Controls.Add(OutputBox, 0, 0);
         MTLP.Controls.Add(InputBox, 0, 1);
         MTLP.Dock = DockStyle.Fill;
         MTLP.Location = new Point(0, 0);
         MTLP.Name = "MTLP";
         MTLP.RowCount = 2;
         MTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         MTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
         MTLP.Size = new Size(800, 450);
         MTLP.TabIndex = 0;
         // 
         // OutputBox
         // 
         OutputBox.BackColor = Color.FromArgb(32, 32, 32);
         OutputBox.BorderStyle = BorderStyle.None;
         OutputBox.Dock = DockStyle.Fill;
         OutputBox.Font = new Font("Consolas", 9F);
         OutputBox.ForeColor = SystemColors.InactiveCaption;
         OutputBox.Location = new Point(3, 3);
         OutputBox.Name = "OutputBox";
         OutputBox.ReadOnly = true;
         OutputBox.Size = new Size(794, 420);
         OutputBox.TabIndex = 0;
         OutputBox.Text = "> ";
         // 
         // InputBox
         // 
         InputBox.BackColor = Color.FromArgb(32, 32, 32);
         InputBox.BorderStyle = BorderStyle.None;
         InputBox.Dock = DockStyle.Fill;
         InputBox.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
         InputBox.ForeColor = SystemColors.InactiveCaption;
         InputBox.Location = new Point(3, 429);
         InputBox.Name = "InputBox";
         InputBox.Size = new Size(794, 16);
         InputBox.TabIndex = 1;
         // 
         // ConsoleForm
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         BackColor = Color.FromArgb(32, 32, 32);
         ClientSize = new Size(800, 450);
         Controls.Add(MTLP);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "ConsoleForm";
         StartPosition = FormStartPosition.CenterParent;
         Text = "ConsoleForm";
         MTLP.ResumeLayout(false);
         MTLP.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel MTLP;
      private RichTextBox OutputBox;
      private TextBox InputBox;
   }
}