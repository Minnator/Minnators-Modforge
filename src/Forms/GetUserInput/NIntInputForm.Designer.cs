namespace Editor.src.Forms.GetUserInput
{
   partial class NIntInputForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NIntInputForm));
         MainTlp = new TableLayoutPanel();
         SuspendLayout();
         // 
         // MainTlp
         // 
         MainTlp.ColumnCount = 2;
         MainTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58.3333321F));
         MainTlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 41.6666679F));
         MainTlp.Dock = DockStyle.Fill;
         MainTlp.Location = new Point(0, 0);
         MainTlp.Name = "MainTlp";
         MainTlp.RowCount = 2;
         MainTlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         MainTlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
         MainTlp.Size = new Size(316, 63);
         MainTlp.TabIndex = 0;
         // 
         // NIntInputForm
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(316, 63);
         Controls.Add(MainTlp);
         FormBorderStyle = FormBorderStyle.FixedSingle;
         Icon = (Icon)resources.GetObject("$this.Icon");
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "NIntInputForm";
         ShowIcon = false;
         ShowInTaskbar = false;
         Text = "NIntInputForm";
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel MainTlp;
   }
}