namespace Editor.Forms.Feature
{
   partial class ErrorLogExplorer
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorLogExplorer));
         MainTLP = new TableLayoutPanel();
         TopTLP = new TableLayoutPanel();
         ErrorCheckBox = new CheckBox();
         WarningCheckBox = new CheckBox();
         InfoCheckBox = new CheckBox();
         DebugCheckBox = new CheckBox();
         ErrorView = new ListView();
         TimeHeader = new ColumnHeader();
         VerbocityHeader = new ColumnHeader();
         MessageHeader = new ColumnHeader();
         label1 = new Label();
         IndexHeader = new ColumnHeader();
         MainTLP.SuspendLayout();
         TopTLP.SuspendLayout();
         SuspendLayout();
         // 
         // MainTLP
         // 
         MainTLP.ColumnCount = 1;
         MainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
         MainTLP.Controls.Add(TopTLP, 0, 0);
         MainTLP.Controls.Add(ErrorView, 0, 1);
         MainTLP.Controls.Add(label1, 0, 2);
         MainTLP.Dock = DockStyle.Fill;
         MainTLP.Location = new Point(0, 0);
         MainTLP.Margin = new Padding(0);
         MainTLP.Name = "MainTLP";
         MainTLP.RowCount = 3;
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         MainTLP.Size = new Size(800, 450);
         MainTLP.TabIndex = 0;
         // 
         // TopTLP
         // 
         TopTLP.ColumnCount = 4;
         TopTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
         TopTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
         TopTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
         TopTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
         TopTLP.Controls.Add(ErrorCheckBox, 0, 0);
         TopTLP.Controls.Add(WarningCheckBox, 1, 0);
         TopTLP.Controls.Add(InfoCheckBox, 2, 0);
         TopTLP.Controls.Add(DebugCheckBox, 3, 0);
         TopTLP.Dock = DockStyle.Fill;
         TopTLP.Location = new Point(0, 0);
         TopTLP.Margin = new Padding(0);
         TopTLP.Name = "TopTLP";
         TopTLP.RowCount = 1;
         TopTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         TopTLP.Size = new Size(800, 30);
         TopTLP.TabIndex = 0;
         // 
         // ErrorCheckBox
         // 
         ErrorCheckBox.Anchor = AnchorStyles.None;
         ErrorCheckBox.AutoSize = true;
         ErrorCheckBox.Location = new Point(72, 5);
         ErrorCheckBox.Name = "ErrorCheckBox";
         ErrorCheckBox.Size = new Size(56, 19);
         ErrorCheckBox.TabIndex = 0;
         ErrorCheckBox.Text = "Errors";
         ErrorCheckBox.UseVisualStyleBackColor = true;
         // 
         // WarningCheckBox
         // 
         WarningCheckBox.Anchor = AnchorStyles.None;
         WarningCheckBox.AutoSize = true;
         WarningCheckBox.Location = new Point(262, 5);
         WarningCheckBox.Name = "WarningCheckBox";
         WarningCheckBox.Size = new Size(76, 19);
         WarningCheckBox.TabIndex = 1;
         WarningCheckBox.Text = "Warnings";
         WarningCheckBox.UseVisualStyleBackColor = true;
         // 
         // InfoCheckBox
         // 
         InfoCheckBox.Anchor = AnchorStyles.None;
         InfoCheckBox.AutoSize = true;
         InfoCheckBox.Location = new Point(455, 5);
         InfoCheckBox.Name = "InfoCheckBox";
         InfoCheckBox.Size = new Size(89, 19);
         InfoCheckBox.TabIndex = 2;
         InfoCheckBox.Text = "Information";
         InfoCheckBox.UseVisualStyleBackColor = true;
         // 
         // DebugCheckBox
         // 
         DebugCheckBox.Anchor = AnchorStyles.None;
         DebugCheckBox.AutoSize = true;
         DebugCheckBox.Location = new Point(669, 5);
         DebugCheckBox.Name = "DebugCheckBox";
         DebugCheckBox.Size = new Size(61, 19);
         DebugCheckBox.TabIndex = 3;
         DebugCheckBox.Text = "Debug";
         DebugCheckBox.UseVisualStyleBackColor = true;
         // 
         // ErrorView
         // 
         ErrorView.Columns.AddRange(new ColumnHeader[] { IndexHeader, TimeHeader, VerbocityHeader, MessageHeader });
         ErrorView.Dock = DockStyle.Fill;
         ErrorView.FullRowSelect = true;
         ErrorView.Location = new Point(3, 33);
         ErrorView.Name = "ErrorView";
         ErrorView.OwnerDraw = true;
         ErrorView.Size = new Size(794, 384);
         ErrorView.TabIndex = 1;
         ErrorView.UseCompatibleStateImageBehavior = false;
         ErrorView.View = View.Details;
         // 
         // TimeHeader
         // 
         TimeHeader.Text = "Time";
         // 
         // VerbocityHeader
         // 
         VerbocityHeader.Text = "Verbocity";
         // 
         // MessageHeader
         // 
         MessageHeader.Text = "Message";
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Location = new Point(3, 420);
         label1.Name = "label1";
         label1.Size = new Size(794, 30);
         label1.TabIndex = 2;
         label1.Text = "Double click rows marked by yellow background to get more detailed info and tips how to fix the issues.";
         label1.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // IndexHeader
         // 
         IndexHeader.Text = "Index";
         // 
         // ErrorLogExplorer
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(800, 450);
         Controls.Add(MainTLP);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "ErrorLogExplorer";
         Text = "Error Log Explorer";
         MainTLP.ResumeLayout(false);
         MainTLP.PerformLayout();
         TopTLP.ResumeLayout(false);
         TopTLP.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel MainTLP;
      private TableLayoutPanel TopTLP;
      private ListView ErrorView;
      private ColumnHeader TimeHeader;
      private ColumnHeader VerbocityHeader;
      private ColumnHeader MessageHeader;
      private CheckBox ErrorCheckBox;
      private CheckBox WarningCheckBox;
      private CheckBox InfoCheckBox;
      private CheckBox DebugCheckBox;
      private Label label1;
      private ColumnHeader IndexHeader;
   }
}