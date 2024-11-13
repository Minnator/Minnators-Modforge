namespace Editor.Forms.PopUps
{
   partial class RoughEditorForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoughEditorForm));
         PropGrid = new PropertyGrid();
         SuspendLayout();
         // 
         // PropGrid
         // 
         PropGrid.Dock = DockStyle.Fill;
         PropGrid.Location = new Point(0, 0);
         PropGrid.Name = "PropGrid";
         PropGrid.Size = new Size(445, 482);
         PropGrid.TabIndex = 0;
         // 
         // RoughEditorForm
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(445, 482);
         Controls.Add(PropGrid);
         FormBorderStyle = FormBorderStyle.FixedSingle;
         Icon = (Icon)resources.GetObject("$this.Icon");
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "RoughEditorForm";
         Text = "Advanced Properties Editor";
         FormClosing += RoughEditorForm_FormClosing;
         ResumeLayout(false);
      }

      #endregion

      private PropertyGrid PropGrid;
   }
}