namespace Editor.src.Forms.PopUps
{
   partial class StringPopUp
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StringPopUp));
         StringTextBox = new TextBox();
         SuspendLayout();
         // 
         // StringTextBox
         // 
         StringTextBox.Dock = DockStyle.Fill;
         StringTextBox.Location = new Point(0, 0);
         StringTextBox.Multiline = true;
         StringTextBox.Name = "StringTextBox";
         StringTextBox.ReadOnly = true;
         StringTextBox.ScrollBars = ScrollBars.Both;
         StringTextBox.Size = new Size(494, 289);
         StringTextBox.TabIndex = 0;
         // 
         // StringPopUp
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(494, 289);
         Controls.Add(StringTextBox);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "StringPopUp";
         Text = "String Pop Up";
         ResumeLayout(false);
         PerformLayout();
      }

      #endregion

      private TextBox StringTextBox;
   }
}