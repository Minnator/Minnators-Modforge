namespace Editor.Forms.Feature
{
   partial class CalenderHistoryView
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalenderHistoryView));
         tableLayoutPanel1 = new TableLayoutPanel();
         TitleLabel = new Label();
         CalenderPanel = new FlowLayoutPanel();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 1;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Controls.Add(TitleLabel, 0, 0);
         tableLayoutPanel1.Controls.Add(CalenderPanel, 0, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Margin = new Padding(0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100.000008F));
         tableLayoutPanel1.Size = new Size(584, 561);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // TitleLabel
         // 
         TitleLabel.AutoSize = true;
         TitleLabel.Dock = DockStyle.Fill;
         TitleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
         TitleLabel.Location = new Point(3, 0);
         TitleLabel.Name = "TitleLabel";
         TitleLabel.Size = new Size(578, 30);
         TitleLabel.TabIndex = 0;
         TitleLabel.Text = "Date";
         TitleLabel.TextAlign = ContentAlignment.MiddleCenter;
         TitleLabel.Click += TitleLabel_Click;
         // 
         // CalenderPanel
         // 
         CalenderPanel.Dock = DockStyle.Fill;
         CalenderPanel.Location = new Point(0, 30);
         CalenderPanel.Margin = new Padding(0);
         CalenderPanel.Name = "CalenderPanel";
         CalenderPanel.Size = new Size(584, 531);
         CalenderPanel.TabIndex = 1;
         // 
         // CalenderHistoryView
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(584, 561);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.FixedSingle;
         Icon = (Icon)resources.GetObject("$this.Icon");
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "CalenderHistoryView";
         Text = "Calender History View";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private Label TitleLabel;
      private FlowLayoutPanel CalenderPanel;
   }
}