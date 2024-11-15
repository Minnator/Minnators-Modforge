﻿namespace Editor.Forms.Feature.SavingClasses
{
   partial class ManualSaving
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManualSaving));
         tableLayoutPanel1 = new TableLayoutPanel();
         label1 = new Label();
         tableLayoutPanel2 = new TableLayoutPanel();
         SaveSelectedButton = new Button();
         MarkAllModified = new Button();
         UnmarkAllSelected = new Button();
         CheckboxesTLP = new TableLayoutPanel();
         tableLayoutPanel3 = new TableLayoutPanel();
         label2 = new Label();
         label3 = new Label();
         tableLayoutPanel1.SuspendLayout();
         tableLayoutPanel2.SuspendLayout();
         tableLayoutPanel3.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 1;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Controls.Add(label1, 0, 0);
         tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 3);
         tableLayoutPanel1.Controls.Add(CheckboxesTLP, 0, 2);
         tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 0, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 4;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
         tableLayoutPanel1.Size = new Size(303, 396);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
         label1.Location = new Point(4, 0);
         label1.Margin = new Padding(4, 0, 4, 0);
         label1.Name = "label1";
         label1.Size = new Size(295, 44);
         label1.TabIndex = 1;
         label1.Text = "Select which items will be save. All items detected as modified are preselected\r\n";
         label1.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // tableLayoutPanel2
         // 
         tableLayoutPanel2.ColumnCount = 3;
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
         tableLayoutPanel2.Controls.Add(SaveSelectedButton, 2, 0);
         tableLayoutPanel2.Controls.Add(MarkAllModified, 1, 0);
         tableLayoutPanel2.Controls.Add(UnmarkAllSelected, 0, 0);
         tableLayoutPanel2.Dock = DockStyle.Fill;
         tableLayoutPanel2.Location = new Point(0, 369);
         tableLayoutPanel2.Margin = new Padding(0);
         tableLayoutPanel2.Name = "tableLayoutPanel2";
         tableLayoutPanel2.RowCount = 1;
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel2.Size = new Size(303, 27);
         tableLayoutPanel2.TabIndex = 3;
         // 
         // SaveSelectedButton
         // 
         SaveSelectedButton.Dock = DockStyle.Fill;
         SaveSelectedButton.Location = new Point(203, 1);
         SaveSelectedButton.Margin = new Padding(1);
         SaveSelectedButton.Name = "SaveSelectedButton";
         SaveSelectedButton.Size = new Size(99, 25);
         SaveSelectedButton.TabIndex = 2;
         SaveSelectedButton.Text = "Save";
         SaveSelectedButton.UseVisualStyleBackColor = true;
         SaveSelectedButton.Click += SaveSelectedButton_Click;
         // 
         // MarkAllModified
         // 
         MarkAllModified.Dock = DockStyle.Fill;
         MarkAllModified.Location = new Point(102, 1);
         MarkAllModified.Margin = new Padding(1);
         MarkAllModified.Name = "MarkAllModified";
         MarkAllModified.Size = new Size(99, 25);
         MarkAllModified.TabIndex = 3;
         MarkAllModified.Text = "Mark all";
         MarkAllModified.UseVisualStyleBackColor = true;
         MarkAllModified.Click += MarkAllModified_Click;
         // 
         // UnmarkAllSelected
         // 
         UnmarkAllSelected.Dock = DockStyle.Fill;
         UnmarkAllSelected.Location = new Point(1, 1);
         UnmarkAllSelected.Margin = new Padding(1);
         UnmarkAllSelected.Name = "UnmarkAllSelected";
         UnmarkAllSelected.Size = new Size(99, 25);
         UnmarkAllSelected.TabIndex = 4;
         UnmarkAllSelected.Text = "Unmark All";
         UnmarkAllSelected.UseVisualStyleBackColor = true;
         UnmarkAllSelected.Click += UnmarkAllSelected_Click;
         // 
         // CheckboxesTLP
         // 
         CheckboxesTLP.ColumnCount = 2;
         CheckboxesTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 73F));
         CheckboxesTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         CheckboxesTLP.Dock = DockStyle.Fill;
         CheckboxesTLP.Location = new Point(0, 64);
         CheckboxesTLP.Margin = new Padding(0);
         CheckboxesTLP.Name = "CheckboxesTLP";
         CheckboxesTLP.RowCount = 2;
         CheckboxesTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         CheckboxesTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         CheckboxesTLP.Size = new Size(303, 305);
         CheckboxesTLP.TabIndex = 4;
         // 
         // tableLayoutPanel3
         // 
         tableLayoutPanel3.ColumnCount = 2;
         tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 73F));
         tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel3.Controls.Add(label2, 0, 0);
         tableLayoutPanel3.Controls.Add(label3, 1, 0);
         tableLayoutPanel3.Dock = DockStyle.Fill;
         tableLayoutPanel3.Location = new Point(0, 44);
         tableLayoutPanel3.Margin = new Padding(0);
         tableLayoutPanel3.Name = "tableLayoutPanel3";
         tableLayoutPanel3.RowCount = 1;
         tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel3.Size = new Size(303, 20);
         tableLayoutPanel3.TabIndex = 5;
         // 
         // label2
         // 
         label2.AutoSize = true;
         label2.Dock = DockStyle.Fill;
         label2.Location = new Point(3, 0);
         label2.Name = "label2";
         label2.Size = new Size(67, 20);
         label2.TabIndex = 0;
         label2.Text = "Is modified";
         label2.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // label3
         // 
         label3.AutoSize = true;
         label3.Dock = DockStyle.Fill;
         label3.Location = new Point(76, 0);
         label3.Name = "label3";
         label3.Size = new Size(224, 20);
         label3.TabIndex = 1;
         label3.Text = "Which object type";
         label3.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // ManualSaving
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(303, 396);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         KeyPreview = true;
         Margin = new Padding(4, 3, 4, 3);
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "ManualSaving";
         StartPosition = FormStartPosition.CenterScreen;
         Text = "Manual Saving";
         KeyDown += ManualSaving_KeyDown;
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         tableLayoutPanel2.ResumeLayout(false);
         tableLayoutPanel3.ResumeLayout(false);
         tableLayoutPanel3.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private Label label1;
      private Button SaveSelectedButton;
      private TableLayoutPanel tableLayoutPanel2;
      private Button MarkAllModified;
      private Button UnmarkAllSelected;
      private TableLayoutPanel CheckboxesTLP;
      private TableLayoutPanel tableLayoutPanel3;
      private Label label2;
      private Label label3;
   }
}