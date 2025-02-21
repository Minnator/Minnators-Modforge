namespace Editor.Controls.MMF_DARK
{
   partial class MmfTextBox
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         BaseTextBox = new TextBox();
         SuspendLayout();
         // 
         // BaseTextBox
         // 
         BaseTextBox.BorderStyle = BorderStyle.None;
         BaseTextBox.Dock = DockStyle.Fill;
         BaseTextBox.Location = new Point(7, 7);
         BaseTextBox.Name = "BaseTextBox";
         BaseTextBox.Size = new Size(236, 15);
         BaseTextBox.TabIndex = 0;
         BaseTextBox.Click += textBox1_Click;
         BaseTextBox.TextChanged += textBox1_TextChanged;
         BaseTextBox.Enter += textBox1_Enter;
         BaseTextBox.KeyPress += textBox1_KeyPress;
         BaseTextBox.Leave += textBox1_Leave;
         BaseTextBox.MouseEnter += textBox1_MouseEnter;
         BaseTextBox.MouseLeave += textBox1_MouseLeave;
         // 
         // MmfTextBox
         // 
         AutoScaleMode = AutoScaleMode.None;
         BackColor = SystemColors.Window;
         Controls.Add(BaseTextBox);
         Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
         ForeColor = Color.DimGray;
         Margin = new Padding(4);
         Name = "MmfTextBox";
         Padding = new Padding(7);
         Size = new Size(250, 30);
         ResumeLayout(false);
         PerformLayout();
      }

      #endregion
      private System.Windows.Forms.TextBox BaseTextBox;
   }
}
