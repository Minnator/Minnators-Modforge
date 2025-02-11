namespace Editor.src.Controls.EXT
{
   partial class EXT_ListViewSearch
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         tableLayoutPanel1 = new TableLayoutPanel();
         SearchInputBox = new Editor.Controls.EXT.EXT_TextBox();
         SearchButton = new Button();
         ItemListView = new EXT_ColorableListView();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
         tableLayoutPanel1.Controls.Add(SearchInputBox, 0, 0);
         tableLayoutPanel1.Controls.Add(SearchButton, 1, 0);
         tableLayoutPanel1.Controls.Add(ItemListView, 0, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Margin = new Padding(0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Size = new Size(374, 325);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // SearchInputBox
         // 
         SearchInputBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
         SearchInputBox.CancelOnEsc = true;
         SearchInputBox.ConfirmOnReturn = true;
         SearchInputBox.Location = new Point(3, 3);
         SearchInputBox.Name = "SearchInputBox";
         SearchInputBox.PlaceholderText = "Search";
         SearchInputBox.Size = new Size(255, 23);
         SearchInputBox.TabIndex = 0;
         SearchInputBox.TimerInterval = 1000;
         SearchInputBox.UseTimer = true;
         // 
         // SearchButton
         // 
         SearchButton.Dock = DockStyle.Fill;
         SearchButton.Image = Properties.Resources.Search;
         SearchButton.ImageAlign = ContentAlignment.MiddleLeft;
         SearchButton.Location = new Point(264, 3);
         SearchButton.Name = "SearchButton";
         SearchButton.Size = new Size(107, 23);
         SearchButton.TabIndex = 1;
         SearchButton.Text = "Search";
         SearchButton.UseVisualStyleBackColor = true;
         // 
         // ItemListView
         // 
         tableLayoutPanel1.SetColumnSpan(ItemListView, 2);
         ItemListView.Dock = DockStyle.Fill;
         ItemListView.FitContentAndHeader = true;
         ItemListView.FullRowSelect = true;
         ItemListView.HeaderBackColor = SystemColors.ControlLight;
         ItemListView.Location = new Point(3, 29);
         ItemListView.Margin = new Padding(3, 0, 3, 3);
         ItemListView.MaxColumnWidth = 200;
         ItemListView.Name = "ItemListView";
         ItemListView.OwnerDraw = true;
         ItemListView.Size = new Size(368, 293);
         ItemListView.TabIndex = 2;
         ItemListView.UseCompatibleStateImageBehavior = false;
         ItemListView.View = View.Details;
         // 
         // EXT_ListViewSearch
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         Controls.Add(tableLayoutPanel1);
         Name = "EXT_ListViewSearch";
         Size = new Size(374, 325);
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      internal Editor.Controls.EXT.EXT_TextBox SearchInputBox;
      internal Button SearchButton;
      internal EXT_ColorableListView ItemListView;
   }
}
