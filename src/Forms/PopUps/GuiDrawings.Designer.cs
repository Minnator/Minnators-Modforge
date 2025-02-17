﻿namespace Editor.Forms.PopUps
{
   partial class GuiDrawings
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GuiDrawings));
         GuiDrawingsLayoutPanel = new TableLayoutPanel();
         label1 = new Label();
         ShowTradeRoutesCheckBox = new CheckBox();
         StraitsCheckBox = new CheckBox();
         CapitalsCheckBox = new CheckBox();
         RiversCheckBox = new CheckBox();
         EquatorCheckBox = new CheckBox();
         GuiDrawingsToolTip = new ToolTip(components);
         GuiDrawingsLayoutPanel.SuspendLayout();
         SuspendLayout();
         // 
         // GuiDrawingsLayoutPanel
         // 
         GuiDrawingsLayoutPanel.ColumnCount = 1;
         GuiDrawingsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         GuiDrawingsLayoutPanel.Controls.Add(label1, 0, 0);
         GuiDrawingsLayoutPanel.Controls.Add(ShowTradeRoutesCheckBox, 0, 1);
         GuiDrawingsLayoutPanel.Controls.Add(StraitsCheckBox, 0, 2);
         GuiDrawingsLayoutPanel.Controls.Add(CapitalsCheckBox, 0, 3);
         GuiDrawingsLayoutPanel.Controls.Add(RiversCheckBox, 0, 4);
         GuiDrawingsLayoutPanel.Controls.Add(EquatorCheckBox, 0, 5);
         GuiDrawingsLayoutPanel.Dock = DockStyle.Fill;
         GuiDrawingsLayoutPanel.Location = new Point(0, 0);
         GuiDrawingsLayoutPanel.Name = "GuiDrawingsLayoutPanel";
         GuiDrawingsLayoutPanel.RowCount = 7;
         GuiDrawingsLayoutPanel.RowStyles.Add(new RowStyle());
         GuiDrawingsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         GuiDrawingsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         GuiDrawingsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         GuiDrawingsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         GuiDrawingsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         GuiDrawingsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
         GuiDrawingsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
         GuiDrawingsLayoutPanel.Size = new Size(223, 262);
         GuiDrawingsLayoutPanel.TabIndex = 0;
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
         label1.Location = new Point(3, 3);
         label1.Margin = new Padding(3);
         label1.Name = "label1";
         label1.Size = new Size(217, 42);
         label1.TabIndex = 0;
         label1.Text = "Enable or disable graphical elements of the map";
         label1.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // ShowTradeRoutesCheckBox
         // 
         ShowTradeRoutesCheckBox.AutoSize = true;
         ShowTradeRoutesCheckBox.Dock = DockStyle.Fill;
         ShowTradeRoutesCheckBox.Location = new Point(9, 51);
         ShowTradeRoutesCheckBox.Margin = new Padding(9, 3, 3, 3);
         ShowTradeRoutesCheckBox.Name = "ShowTradeRoutesCheckBox";
         ShowTradeRoutesCheckBox.Size = new Size(211, 19);
         ShowTradeRoutesCheckBox.TabIndex = 1;
         ShowTradeRoutesCheckBox.Text = "Show traderoutes";
         ShowTradeRoutesCheckBox.UseVisualStyleBackColor = true;
         ShowTradeRoutesCheckBox.CheckedChanged += ShowTradeRoutesCheckBox_CheckedChanged;
         // 
         // StraitsCheckBox
         // 
         StraitsCheckBox.AutoSize = true;
         StraitsCheckBox.Dock = DockStyle.Fill;
         StraitsCheckBox.Location = new Point(9, 76);
         StraitsCheckBox.Margin = new Padding(9, 3, 3, 3);
         StraitsCheckBox.Name = "StraitsCheckBox";
         StraitsCheckBox.Size = new Size(211, 19);
         StraitsCheckBox.TabIndex = 2;
         StraitsCheckBox.Text = "Straits";
         StraitsCheckBox.UseVisualStyleBackColor = true;
         StraitsCheckBox.CheckedChanged += StraitsCheckBox_CheckedChanged;
         // 
         // CapitalsCheckBox
         // 
         CapitalsCheckBox.AutoSize = true;
         CapitalsCheckBox.Dock = DockStyle.Fill;
         CapitalsCheckBox.Location = new Point(9, 101);
         CapitalsCheckBox.Margin = new Padding(9, 3, 3, 3);
         CapitalsCheckBox.Name = "CapitalsCheckBox";
         CapitalsCheckBox.Size = new Size(211, 19);
         CapitalsCheckBox.TabIndex = 3;
         CapitalsCheckBox.Text = "Capitals";
         CapitalsCheckBox.UseVisualStyleBackColor = true;
         CapitalsCheckBox.CheckedChanged += CapitalsCheckBox_CheckedChanged;
         // 
         // RiversCheckBox
         // 
         RiversCheckBox.AutoSize = true;
         RiversCheckBox.Dock = DockStyle.Fill;
         RiversCheckBox.Location = new Point(9, 126);
         RiversCheckBox.Margin = new Padding(9, 3, 3, 3);
         RiversCheckBox.Name = "RiversCheckBox";
         RiversCheckBox.Size = new Size(211, 19);
         RiversCheckBox.TabIndex = 4;
         RiversCheckBox.Text = "Rivers";
         RiversCheckBox.UseVisualStyleBackColor = true;
         RiversCheckBox.CheckedChanged += RiversCheckBox_CheckedChanged;
         // 
         // EquatorCheckBox
         // 
         EquatorCheckBox.AutoSize = true;
         EquatorCheckBox.Dock = DockStyle.Fill;
         EquatorCheckBox.Location = new Point(9, 151);
         EquatorCheckBox.Margin = new Padding(9, 3, 3, 3);
         EquatorCheckBox.Name = "EquatorCheckBox";
         EquatorCheckBox.Size = new Size(211, 19);
         EquatorCheckBox.TabIndex = 5;
         EquatorCheckBox.Text = "Equator";
         EquatorCheckBox.UseVisualStyleBackColor = true;
         // 
         // GuiDrawings
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(223, 262);
         Controls.Add(GuiDrawingsLayoutPanel);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "GuiDrawings";
         Text = "GuiDrawings";
         GuiDrawingsLayoutPanel.ResumeLayout(false);
         GuiDrawingsLayoutPanel.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel GuiDrawingsLayoutPanel;
      private Label label1;
      private CheckBox ShowTradeRoutesCheckBox;
      private ToolTip GuiDrawingsToolTip;
      private CheckBox StraitsCheckBox;
      private CheckBox CapitalsCheckBox;
      private CheckBox RiversCheckBox;
      private CheckBox EquatorCheckBox;
   }
}