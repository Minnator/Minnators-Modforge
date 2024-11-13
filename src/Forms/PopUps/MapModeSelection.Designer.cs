namespace Editor.Forms.PopUps
{
   partial class MapModeSelection
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
         MMSelectionBox = new ComboBox();
         SuspendLayout();
         // 
         // MMSelectionBox
         // 
         MMSelectionBox.Dock = DockStyle.Fill;
         MMSelectionBox.DropDownStyle = ComboBoxStyle.DropDownList;
         MMSelectionBox.FormattingEnabled = true;
         MMSelectionBox.Location = new Point(0, 0);
         MMSelectionBox.Name = "MMSelectionBox";
         MMSelectionBox.Size = new Size(150, 23);
         MMSelectionBox.TabIndex = 0;
         // 
         // MapModeSelection
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(150, 23);
         Controls.Add(MMSelectionBox);
         FormBorderStyle = FormBorderStyle.None;
         Name = "MapModeSelection";
         Text = "MapModeSelection";
         ResumeLayout(false);
      }

      #endregion

      private ComboBox MMSelectionBox;
   }
}