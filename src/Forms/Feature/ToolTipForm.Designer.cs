namespace Editor.Forms.Feature
{
   partial class ToolTipForm
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
         ToolTipContent = new Label();
         SuspendLayout();
         // 
         // ToolTipContent
         // 
         ToolTipContent.AutoSize = true;
         ToolTipContent.Dock = DockStyle.Fill;
         ToolTipContent.Location = new Point(0, 0);
         ToolTipContent.Name = "ToolTipContent";
         ToolTipContent.Size = new Size(68, 15);
         ToolTipContent.TabIndex = 0;
         ToolTipContent.Text = "NO TT TEXT";
         // 
         // ToolTipForm
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(292, 107);
         Controls.Add(ToolTipContent);
         FormBorderStyle = FormBorderStyle.None;
         Name = "ToolTipForm";
         Text = "ToolTipForm";
         ResumeLayout(false);
         PerformLayout();
      }

      #endregion

      private Label ToolTipContent;
   }
}