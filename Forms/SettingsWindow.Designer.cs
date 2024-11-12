namespace Editor.Forms
{
   partial class SettingsWindow
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsWindow));
         SettingsTabs = new TabControl();
         tableLayoutPanel1 = new TableLayoutPanel();
         ResetButton = new Button();
         CancelButton = new Button();
         SaveButton = new Button();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // SettingsTabs
         // 
         tableLayoutPanel1.SetColumnSpan(SettingsTabs, 3);
         SettingsTabs.Dock = DockStyle.Fill;
         SettingsTabs.Location = new Point(3, 3);
         SettingsTabs.Name = "SettingsTabs";
         SettingsTabs.SelectedIndex = 0;
         SettingsTabs.Size = new Size(442, 494);
         SettingsTabs.TabIndex = 0;
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 3;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel1.Controls.Add(ResetButton, 0, 1);
         tableLayoutPanel1.Controls.Add(SettingsTabs, 0, 0);
         tableLayoutPanel1.Controls.Add(CancelButton, 0, 1);
         tableLayoutPanel1.Controls.Add(SaveButton, 2, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Size = new Size(448, 530);
         tableLayoutPanel1.TabIndex = 1;
         // 
         // ResetButton
         // 
         ResetButton.Dock = DockStyle.Fill;
         ResetButton.Location = new Point(152, 503);
         ResetButton.Name = "ResetButton";
         ResetButton.Size = new Size(143, 24);
         ResetButton.TabIndex = 3;
         ResetButton.Text = "Reset";
         ResetButton.UseVisualStyleBackColor = true;
         ResetButton.Click += ResetButton_Click;
         // 
         // CancelButton
         // 
         CancelButton.Dock = DockStyle.Fill;
         CancelButton.Location = new Point(3, 503);
         CancelButton.Name = "CancelButton";
         CancelButton.Size = new Size(143, 24);
         CancelButton.TabIndex = 1;
         CancelButton.Text = "Reset All";
         CancelButton.UseVisualStyleBackColor = true;
         CancelButton.Click += ResetAll_Click;
         // 
         // SaveButton
         // 
         SaveButton.Dock = DockStyle.Fill;
         SaveButton.Location = new Point(301, 503);
         SaveButton.Name = "SaveButton";
         SaveButton.Size = new Size(144, 24);
         SaveButton.TabIndex = 2;
         SaveButton.Text = "Save";
         SaveButton.UseVisualStyleBackColor = true;
         SaveButton.Click += SaveButton_Click;
         // 
         // SettingsWindow
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(448, 530);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.FixedSingle;
         Icon = (Icon)resources.GetObject("$this.Icon");
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "SettingsWindow";
         Text = "SettingsWindow";
         tableLayoutPanel1.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TabControl SettingsTabs;
      private TableLayoutPanel tableLayoutPanel1;
      private new Button CancelButton;
      private Button SaveButton;
      private Button ResetButton;
   }
}