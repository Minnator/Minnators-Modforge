namespace Editor.Forms
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
         tableLayoutPanel1 = new TableLayoutPanel();
         Output = new RichTextBox();
         Input = new TextBox();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 1;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Controls.Add(Output, 0, 0);
         tableLayoutPanel1.Controls.Add(Input, 0, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle());
         tableLayoutPanel1.Size = new Size(830, 241);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // Output
         // 
         Output.BackColor = SystemColors.ControlDarkDark;
         Output.BorderStyle = BorderStyle.None;
         Output.Dock = DockStyle.Fill;
         Output.Location = new Point(4, 3);
         Output.Margin = new Padding(4, 3, 4, 3);
         Output.Name = "Output";
         Output.ReadOnly = true;
         Output.Size = new Size(822, 206);
         Output.TabIndex = 0;
         Output.TabStop = false;
         Output.Text = "";
         Output.WordWrap = false;
         Output.TextChanged += Output_TextChanged;
         // 
         // Input
         // 
         Input.Dock = DockStyle.Fill;
         Input.Location = new Point(4, 215);
         Input.Margin = new Padding(4, 3, 4, 3);
         Input.Name = "Input";
         Input.ShortcutsEnabled = false;
         Input.Size = new Size(822, 23);
         Input.TabIndex = 0;
         Input.WordWrap = false;
         Input.KeyUp += InputTextBox_KeyUp;
         // 
         // ConsoleForm
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(830, 241);
         Controls.Add(tableLayoutPanel1);
         Margin = new Padding(4, 3, 4, 3);
         Name = "ConsoleForm";
         Text = "Editor Console";
         Load += ConsoleForm_Load;
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      internal System.Windows.Forms.RichTextBox Output;
      private System.Windows.Forms.TextBox Input;
   }
}