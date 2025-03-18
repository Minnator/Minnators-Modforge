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
         components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalenderHistoryView));
         tableLayoutPanel1 = new TableLayoutPanel();
         CalenderPanel = new FlowLayoutPanel();
         TitleLabel = new Label();
         ExplanationLabel = new Label();
         MinMaxLabel = new Label();
         ExplanationTT = new ToolTip(components);
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.BackColor = SystemColors.ButtonShadow;
         tableLayoutPanel1.ColumnCount = 3;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel1.Controls.Add(CalenderPanel, 0, 1);
         tableLayoutPanel1.Controls.Add(TitleLabel, 1, 0);
         tableLayoutPanel1.Controls.Add(ExplanationLabel, 2, 0);
         tableLayoutPanel1.Controls.Add(MinMaxLabel, 0, 0);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Margin = new Padding(0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Size = new Size(584, 561);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // CalenderPanel
         // 
         tableLayoutPanel1.SetColumnSpan(CalenderPanel, 3);
         CalenderPanel.Dock = DockStyle.Fill;
         CalenderPanel.Location = new Point(0, 30);
         CalenderPanel.Margin = new Padding(0);
         CalenderPanel.Name = "CalenderPanel";
         CalenderPanel.Size = new Size(584, 531);
         CalenderPanel.TabIndex = 1;
         // 
         // TitleLabel
         // 
         TitleLabel.AutoSize = true;
         TitleLabel.BackColor = SystemColors.ScrollBar;
         TitleLabel.Cursor = Cursors.Hand;
         TitleLabel.Dock = DockStyle.Fill;
         TitleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
         TitleLabel.Location = new Point(197, 0);
         TitleLabel.Name = "TitleLabel";
         TitleLabel.Size = new Size(188, 30);
         TitleLabel.TabIndex = 0;
         TitleLabel.Text = "Date";
         TitleLabel.TextAlign = ContentAlignment.MiddleCenter;
         TitleLabel.Click += TitleLabel_Click;
         // 
         // ExplanationLabel
         // 
         ExplanationLabel.AutoSize = true;
         ExplanationLabel.Dock = DockStyle.Fill;
         ExplanationLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
         ExplanationLabel.Location = new Point(391, 0);
         ExplanationLabel.Name = "ExplanationLabel";
         ExplanationLabel.Size = new Size(190, 30);
         ExplanationLabel.TabIndex = 2;
         ExplanationLabel.Text = "?";
         ExplanationLabel.TextAlign = ContentAlignment.MiddleCenter;
         ExplanationTT.SetToolTip(ExplanationLabel, "Scroll to go forward / backwards in time\r\nClick on date label to 'zoom out'\r\nClick on days / years to 'zoom in'\r\nDays < months < years  < decades < centuries");
         // 
         // MinMaxLabel
         // 
         MinMaxLabel.AutoSize = true;
         MinMaxLabel.BackColor = SystemColors.ButtonShadow;
         MinMaxLabel.Dock = DockStyle.Fill;
         MinMaxLabel.Location = new Point(3, 0);
         MinMaxLabel.Name = "MinMaxLabel";
         MinMaxLabel.Size = new Size(188, 30);
         MinMaxLabel.TabIndex = 3;
         MinMaxLabel.Text = "label1";
         MinMaxLabel.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // CalenderHistoryView
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(584, 561);
         Controls.Add(tableLayoutPanel1);
         Cursor = Cursors.SizeNS;
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
      private Label ExplanationLabel;
      private Label MinMaxLabel;
      private ToolTip ExplanationTT;
   }
}