namespace Editor.Forms.Feature
{
   partial class WikiBrowser
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WikiBrowser));
         tableLayoutPanel1 = new TableLayoutPanel();
         SearchTextBox = new TextBox();
         EffectView = new ListView();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 1;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 83.5403748F));
         tableLayoutPanel1.Controls.Add(SearchTextBox, 0, 0);
         tableLayoutPanel1.Controls.Add(EffectView, 0, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Size = new Size(966, 634);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // SearchTextBox
         // 
         SearchTextBox.Dock = DockStyle.Fill;
         SearchTextBox.Location = new Point(3, 3);
         SearchTextBox.Name = "SearchTextBox";
         SearchTextBox.PlaceholderText = "Search...";
         SearchTextBox.Size = new Size(960, 23);
         SearchTextBox.TabIndex = 0;
         // 
         // EffectView
         // 
         EffectView.Activation = ItemActivation.OneClick;
         EffectView.Dock = DockStyle.Fill;
         EffectView.Location = new Point(3, 33);
         EffectView.Name = "EffectView";
         EffectView.Size = new Size(960, 598);
         EffectView.TabIndex = 2;
         EffectView.UseCompatibleStateImageBehavior = false;
         // 
         // WikiBrowser
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(966, 634);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "WikiBrowser";
         StartPosition = FormStartPosition.CenterScreen;
         Text = "Wiki Browser";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private TextBox SearchTextBox;
      private ListView EffectView;
   }
}