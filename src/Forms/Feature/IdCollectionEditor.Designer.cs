namespace Editor.Forms.Feature
{
   partial class IdCollectionEditor
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IdCollectionEditor));
         tableLayoutPanel1 = new TableLayoutPanel();
         tableLayoutPanel2 = new TableLayoutPanel();
         UnionButton = new Button();
         IntersectButton = new Button();
         ExceptButton = new Button();
         SortDown = new Button();
         SortUpButton = new Button();
         CopyButton = new Button();
         PaddingCheckBox = new CheckBox();
         RemoveButton = new Button();
         InputTB = new TextBox();
         OutputTB = new TextBox();
         SourceTB = new TextBox();
         ToolTipMain = new ToolTip(components);
         tableLayoutPanel1.SuspendLayout();
         tableLayoutPanel2.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 2);
         tableLayoutPanel1.Controls.Add(InputTB, 1, 0);
         tableLayoutPanel1.Controls.Add(OutputTB, 0, 1);
         tableLayoutPanel1.Controls.Add(SourceTB, 0, 0);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
         tableLayoutPanel1.Size = new Size(800, 450);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // tableLayoutPanel2
         // 
         tableLayoutPanel2.ColumnCount = 8;
         tableLayoutPanel1.SetColumnSpan(tableLayoutPanel2, 2);
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
         tableLayoutPanel2.Controls.Add(UnionButton, 6, 0);
         tableLayoutPanel2.Controls.Add(IntersectButton, 5, 0);
         tableLayoutPanel2.Controls.Add(ExceptButton, 4, 0);
         tableLayoutPanel2.Controls.Add(SortDown, 3, 0);
         tableLayoutPanel2.Controls.Add(SortUpButton, 2, 0);
         tableLayoutPanel2.Controls.Add(CopyButton, 7, 0);
         tableLayoutPanel2.Controls.Add(PaddingCheckBox, 0, 0);
         tableLayoutPanel2.Controls.Add(RemoveButton, 1, 0);
         tableLayoutPanel2.Dock = DockStyle.Fill;
         tableLayoutPanel2.Location = new Point(0, 420);
         tableLayoutPanel2.Margin = new Padding(0);
         tableLayoutPanel2.Name = "tableLayoutPanel2";
         tableLayoutPanel2.RowCount = 1;
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel2.Size = new Size(800, 30);
         tableLayoutPanel2.TabIndex = 0;
         // 
         // UnionButton
         // 
         UnionButton.Dock = DockStyle.Fill;
         UnionButton.Location = new Point(603, 3);
         UnionButton.Name = "UnionButton";
         UnionButton.Size = new Size(94, 24);
         UnionButton.TabIndex = 0;
         UnionButton.Text = "Union";
         UnionButton.UseVisualStyleBackColor = true;
         UnionButton.Click += UnionButton_Click;
         // 
         // IntersectButton
         // 
         IntersectButton.Dock = DockStyle.Fill;
         IntersectButton.Location = new Point(503, 3);
         IntersectButton.Name = "IntersectButton";
         IntersectButton.Size = new Size(94, 24);
         IntersectButton.TabIndex = 1;
         IntersectButton.Text = "Intersect";
         IntersectButton.UseVisualStyleBackColor = true;
         IntersectButton.Click += IntersectButton_Click;
         // 
         // ExceptButton
         // 
         ExceptButton.Dock = DockStyle.Fill;
         ExceptButton.Location = new Point(403, 3);
         ExceptButton.Name = "ExceptButton";
         ExceptButton.Size = new Size(94, 24);
         ExceptButton.TabIndex = 2;
         ExceptButton.Text = "Except";
         ExceptButton.UseVisualStyleBackColor = true;
         ExceptButton.Click += ExceptButton_Click;
         // 
         // SortDown
         // 
         SortDown.Dock = DockStyle.Fill;
         SortDown.Location = new Point(303, 3);
         SortDown.Name = "SortDown";
         SortDown.Size = new Size(94, 24);
         SortDown.TabIndex = 3;
         SortDown.Text = "Decending";
         SortDown.UseVisualStyleBackColor = true;
         SortDown.Click += SortDown_Click;
         // 
         // SortUpButton
         // 
         SortUpButton.Dock = DockStyle.Fill;
         SortUpButton.Location = new Point(203, 3);
         SortUpButton.Name = "SortUpButton";
         SortUpButton.Size = new Size(94, 24);
         SortUpButton.TabIndex = 4;
         SortUpButton.Text = "Acending";
         SortUpButton.UseVisualStyleBackColor = true;
         SortUpButton.Click += SortUpButton_Click;
         // 
         // CopyButton
         // 
         CopyButton.Dock = DockStyle.Fill;
         CopyButton.Location = new Point(703, 3);
         CopyButton.Name = "CopyButton";
         CopyButton.Size = new Size(94, 24);
         CopyButton.TabIndex = 7;
         CopyButton.Text = "Copy";
         CopyButton.UseVisualStyleBackColor = true;
         CopyButton.Click += CopyButton_Click;
         // 
         // PaddingCheckBox
         // 
         PaddingCheckBox.AutoSize = true;
         PaddingCheckBox.Dock = DockStyle.Fill;
         PaddingCheckBox.Location = new Point(3, 3);
         PaddingCheckBox.Name = "PaddingCheckBox";
         PaddingCheckBox.Size = new Size(94, 24);
         PaddingCheckBox.TabIndex = 5;
         PaddingCheckBox.Text = "Padding";
         PaddingCheckBox.UseVisualStyleBackColor = true;
         PaddingCheckBox.CheckedChanged += checkBox1_CheckedChanged;
         // 
         // RemoveButton
         // 
         RemoveButton.Dock = DockStyle.Fill;
         RemoveButton.Location = new Point(103, 3);
         RemoveButton.Name = "RemoveButton";
         RemoveButton.Size = new Size(94, 24);
         RemoveButton.TabIndex = 8;
         RemoveButton.Text = "Remove";
         ToolTipMain.SetToolTip(RemoveButton, "This also works for whole files, not only for list of ints");
         RemoveButton.UseVisualStyleBackColor = true;
         RemoveButton.Click += RemoveButton_Click;
         // 
         // InputTB
         // 
         InputTB.Dock = DockStyle.Fill;
         InputTB.Location = new Point(403, 3);
         InputTB.Multiline = true;
         InputTB.Name = "InputTB";
         InputTB.PlaceholderText = "Input two";
         InputTB.ScrollBars = ScrollBars.Vertical;
         InputTB.Size = new Size(394, 288);
         InputTB.TabIndex = 1;
         // 
         // OutputTB
         // 
         tableLayoutPanel1.SetColumnSpan(OutputTB, 2);
         OutputTB.Dock = DockStyle.Fill;
         OutputTB.Location = new Point(3, 297);
         OutputTB.Multiline = true;
         OutputTB.Name = "OutputTB";
         OutputTB.PlaceholderText = "Output";
         OutputTB.ReadOnly = true;
         OutputTB.ScrollBars = ScrollBars.Vertical;
         OutputTB.Size = new Size(794, 120);
         OutputTB.TabIndex = 2;
         // 
         // SourceTB
         // 
         SourceTB.Dock = DockStyle.Fill;
         SourceTB.Location = new Point(3, 3);
         SourceTB.Multiline = true;
         SourceTB.Name = "SourceTB";
         SourceTB.PlaceholderText = "Input one";
         SourceTB.ScrollBars = ScrollBars.Vertical;
         SourceTB.Size = new Size(394, 288);
         SourceTB.TabIndex = 3;
         // 
         // IdCollectionEditor
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(800, 450);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "IdCollectionEditor";
         Text = "ID Collection Editor";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         tableLayoutPanel2.ResumeLayout(false);
         tableLayoutPanel2.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private TableLayoutPanel tableLayoutPanel2;
      private Button UnionButton;
      private Button IntersectButton;
      private Button ExceptButton;
      private Button SortDown;
      private Button SortUpButton;
      private CheckBox PaddingCheckBox;
      private Button CopyButton;
      private TextBox InputTB;
      private TextBox OutputTB;
      private TextBox SourceTB;
      private Button RemoveButton;
      private ToolTip ToolTipMain;
   }
}