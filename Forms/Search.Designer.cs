namespace Editor.Forms
{
   partial class Search
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Search));
         tableLayoutPanel1 = new TableLayoutPanel();
         SearchInput = new TextBox();
         SearchResultsPanel = new FlowLayoutPanel();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 1;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Controls.Add(SearchInput, 0, 0);
         tableLayoutPanel1.Controls.Add(SearchResultsPanel, 0, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle());
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Size = new Size(200, 330);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // SearchInput
         // 
         SearchInput.Dock = DockStyle.Fill;
         SearchInput.Location = new Point(3, 3);
         SearchInput.Name = "SearchInput";
         SearchInput.Size = new Size(194, 23);
         SearchInput.TabIndex = 0;
         SearchInput.TextChanged += SearchInput_TextChanged;
         // 
         // SearchResultsPanel
         // 
         SearchResultsPanel.FlowDirection = FlowDirection.TopDown;
         SearchResultsPanel.Location = new Point(0, 29);
         SearchResultsPanel.Margin = new Padding(0);
         SearchResultsPanel.Name = "SearchResultsPanel";
         SearchResultsPanel.Size = new Size(200, 300);
         SearchResultsPanel.TabIndex = 1;
         // 
         // Search
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(200, 330);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.FixedToolWindow;
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "Search";
         Text = "Search";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private TextBox SearchInput;
      private FlowLayoutPanel SearchResultsPanel;
   }
}