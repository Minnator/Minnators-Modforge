namespace Editor.Forms.Feature
{
   partial class RevolutionaryColorPicker
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RevolutionaryColorPicker));
         MainTLP = new TableLayoutPanel();
         FlagTLP = new TableLayoutPanel();
         OkButton = new Button();
         MainTLP.SuspendLayout();
         FlagTLP.SuspendLayout();
         SuspendLayout();
         // 
         // MainTLP
         // 
         MainTLP.ColumnCount = 1;
         MainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         MainTLP.Controls.Add(FlagTLP, 0, 1);
         MainTLP.Dock = DockStyle.Fill;
         MainTLP.Location = new Point(0, 0);
         MainTLP.Margin = new Padding(0);
         MainTLP.Name = "MainTLP";
         MainTLP.RowCount = 2;
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
         MainTLP.Size = new Size(724, 192);
         MainTLP.TabIndex = 0;
         // 
         // FlagTLP
         // 
         FlagTLP.ColumnCount = 5;
         FlagTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         FlagTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
         FlagTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
         FlagTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
         FlagTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         FlagTLP.Controls.Add(OkButton, 4, 0);
         FlagTLP.Dock = DockStyle.Fill;
         FlagTLP.Location = new Point(3, 145);
         FlagTLP.Name = "FlagTLP";
         FlagTLP.RowCount = 1;
         FlagTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         FlagTLP.Size = new Size(718, 44);
         FlagTLP.TabIndex = 0;
         // 
         // OkButton
         // 
         OkButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
         OkButton.Location = new Point(640, 18);
         OkButton.Name = "OkButton";
         OkButton.Size = new Size(75, 23);
         OkButton.TabIndex = 0;
         OkButton.Text = "Ok";
         OkButton.UseVisualStyleBackColor = true;
         OkButton.Click += OkButton_Click;
         // 
         // RevolutionaryColorPicker
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(724, 192);
         Controls.Add(MainTLP);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "RevolutionaryColorPicker";
         Text = "Revolutionary Color Picker";
         MainTLP.ResumeLayout(false);
         FlagTLP.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel MainTLP;
      private TableLayoutPanel FlagTLP;
      private Button OkButton;
   }
}