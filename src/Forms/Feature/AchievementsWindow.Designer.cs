using Editor.Controls.MMF_DARK;

namespace Editor.Forms.Feature
{
   partial class AchievementsWindow
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AchievementsWindow));
         MainTLP = new TableLayoutPanel();
         AchievementFlowPanel = new FlowLayoutPanel();
         SearchTextBox = new MmfTextBox();
         TitleLabel = new Label();
         MainTLP.SuspendLayout();
         SuspendLayout();
         // 
         // MainTLP
         // 
         MainTLP.ColumnCount = 3;
         MainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
         MainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
         MainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
         MainTLP.Controls.Add(AchievementFlowPanel, 0, 1);
         MainTLP.Controls.Add(SearchTextBox, 1, 0);
         MainTLP.Controls.Add(TitleLabel, 0, 0);
         MainTLP.Dock = DockStyle.Fill;
         MainTLP.Location = new Point(0, 0);
         MainTLP.Margin = new Padding(0, 3, 0, 0);
         MainTLP.Name = "MainTLP";
         MainTLP.Padding = new Padding(0, 3, 0, 0);
         MainTLP.RowCount = 2;
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         MainTLP.Size = new Size(769, 457);
         MainTLP.TabIndex = 0;
         // 
         // AchievementFlowPanel
         // 
         AchievementFlowPanel.AutoScroll = true;
         MainTLP.SetColumnSpan(AchievementFlowPanel, 3);
         AchievementFlowPanel.Dock = DockStyle.Fill;
         AchievementFlowPanel.Location = new Point(3, 38);
         AchievementFlowPanel.Name = "AchievementFlowPanel";
         AchievementFlowPanel.Size = new Size(763, 416);
         AchievementFlowPanel.TabIndex = 0;
         // 
         // SearchTextBox
         // 
         SearchTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
         SearchTextBox.BackColor = SystemColors.Window;
         SearchTextBox.BorderColor = Color.FromArgb(14, 20, 27);
         SearchTextBox.BorderFocusColor = Color.FromArgb(26, 159, 255);
         SearchTextBox.BorderWidth = 1;
         SearchTextBox.CornerRadius = 3;
         SearchTextBox.FocusMode = MmfTextBox.FocusModeEnum.Fade;
         SearchTextBox.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
         SearchTextBox.ForeColor = Color.DimGray;
         SearchTextBox.Location = new Point(261, 4);
         SearchTextBox.Margin = new Padding(3, 1, 3, 1);
         SearchTextBox.Multiline = false;
         SearchTextBox.Name = "SearchTextBox";
         SearchTextBox.Padding = new Padding(7);
         SearchTextBox.PasswordChar = false;
         SearchTextBox.PlaceHolderText = "Search";
         SearchTextBox.Size = new Size(248, 31);
         SearchTextBox.TabIndex = 1;
         SearchTextBox.Texts = "";
         // 
         // TitleLabel
         // 
         TitleLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
         TitleLabel.AutoSize = true;
         TitleLabel.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
         TitleLabel.Location = new Point(7, 3);
         TitleLabel.Margin = new Padding(7, 0, 3, 8);
         TitleLabel.Name = "TitleLabel";
         TitleLabel.Size = new Size(134, 24);
         TitleLabel.TabIndex = 2;
         TitleLabel.Text = "Achievements";
         TitleLabel.TextAlign = ContentAlignment.BottomLeft;
         // 
         // AchievementsWindow
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(769, 457);
         Controls.Add(MainTLP);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "AchievementsWindow";
         Text = "AchievementsWindow";
         MainTLP.ResumeLayout(false);
         MainTLP.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel MainTLP;
      private FlowLayoutPanel AchievementFlowPanel;
      private MmfTextBox SearchTextBox;
      private Label TitleLabel;
   }
}