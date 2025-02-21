namespace Editor.Forms.Feature
{
   partial class NameGeneratorForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NameGeneratorForm));
         tableLayoutPanel1 = new TableLayoutPanel();
         GenerateButton = new Button();
         CopyAllButton = new Button();
         ConfigPropertyGrid = new PropertyGrid();
         ResultsListBox = new ListBox();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
         tableLayoutPanel1.Controls.Add(GenerateButton, 1, 1);
         tableLayoutPanel1.Controls.Add(CopyAllButton, 0, 1);
         tableLayoutPanel1.Controls.Add(ConfigPropertyGrid, 1, 0);
         tableLayoutPanel1.Controls.Add(ResultsListBox, 0, 0);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Size = new Size(685, 406);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // GenerateButton
         // 
         GenerateButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
         GenerateButton.Location = new Point(609, 377);
         GenerateButton.Margin = new Padding(1);
         GenerateButton.Name = "GenerateButton";
         GenerateButton.Size = new Size(75, 28);
         GenerateButton.TabIndex = 0;
         GenerateButton.Text = "Generate";
         GenerateButton.UseVisualStyleBackColor = true;
         GenerateButton.Click += GenerateButton_Click;
         // 
         // CopyAllButton
         // 
         CopyAllButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
         CopyAllButton.Location = new Point(1, 377);
         CopyAllButton.Margin = new Padding(1);
         CopyAllButton.Name = "CopyAllButton";
         CopyAllButton.Size = new Size(75, 28);
         CopyAllButton.TabIndex = 1;
         CopyAllButton.Text = "Copy All";
         CopyAllButton.UseVisualStyleBackColor = true;
         // 
         // ConfigPropertyGrid
         // 
         ConfigPropertyGrid.Dock = DockStyle.Fill;
         ConfigPropertyGrid.Location = new Point(206, 1);
         ConfigPropertyGrid.Margin = new Padding(1);
         ConfigPropertyGrid.Name = "ConfigPropertyGrid";
         ConfigPropertyGrid.Size = new Size(478, 374);
         ConfigPropertyGrid.TabIndex = 2;
         // 
         // ResultsListBox
         // 
         ResultsListBox.BorderStyle = BorderStyle.FixedSingle;
         ResultsListBox.Dock = DockStyle.Fill;
         ResultsListBox.FormattingEnabled = true;
         ResultsListBox.IntegralHeight = false;
         ResultsListBox.ItemHeight = 15;
         ResultsListBox.Location = new Point(1, 1);
         ResultsListBox.Margin = new Padding(1);
         ResultsListBox.Name = "ResultsListBox";
         ResultsListBox.ScrollAlwaysVisible = true;
         ResultsListBox.Size = new Size(203, 374);
         ResultsListBox.TabIndex = 3;
         // 
         // NameGeneratorForm
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(685, 406);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "NameGeneratorForm";
         Text = "Name Generator (Stochastic Analysis)";
         tableLayoutPanel1.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private Button GenerateButton;
      private Button CopyAllButton;
      private PropertyGrid ConfigPropertyGrid;
      private ListBox ResultsListBox;
   }
}