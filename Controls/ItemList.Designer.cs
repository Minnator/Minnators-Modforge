namespace Editor.Controls
{
   partial class ItemList
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
         TitleLabel = new Label();
         ItemsComboBox = new ComboBox();
         FlowLayout = new FlowLayoutPanel();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44.4444427F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55.5555573F));
         tableLayoutPanel1.Controls.Add(TitleLabel, 0, 0);
         tableLayoutPanel1.Controls.Add(ItemsComboBox, 1, 0);
         tableLayoutPanel1.Controls.Add(FlowLayout, 0, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Size = new Size(190, 90);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // TitleLabel
         // 
         TitleLabel.AutoSize = true;
         TitleLabel.Dock = DockStyle.Fill;
         TitleLabel.Location = new Point(3, 0);
         TitleLabel.Name = "TitleLabel";
         TitleLabel.Size = new Size(78, 25);
         TitleLabel.TabIndex = 0;
         TitleLabel.Text = "Title";
         TitleLabel.TextAlign = ContentAlignment.MiddleLeft;
         // 
         // ItemsComboBox
         // 
         ItemsComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         ItemsComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
         ItemsComboBox.Dock = DockStyle.Fill;
         ItemsComboBox.FormattingEnabled = true;
         ItemsComboBox.Location = new Point(87, 1);
         ItemsComboBox.Margin = new Padding(3, 1, 3, 3);
         ItemsComboBox.Name = "ItemsComboBox";
         ItemsComboBox.Size = new Size(100, 23);
         ItemsComboBox.TabIndex = 1;
         // 
         // FlowLayout
         // 
         FlowLayout.AutoScroll = true;
         FlowLayout.BorderStyle = BorderStyle.FixedSingle;
         tableLayoutPanel1.SetColumnSpan(FlowLayout, 2);
         FlowLayout.Dock = DockStyle.Fill;
         FlowLayout.Location = new Point(3, 28);
         FlowLayout.Name = "FlowLayout";
         FlowLayout.Size = new Size(184, 59);
         FlowLayout.TabIndex = 2;
         // 
         // ItemList
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         Controls.Add(tableLayoutPanel1);
         Name = "ItemList";
         Size = new Size(190, 90);
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private Label TitleLabel;
      private ComboBox ItemsComboBox;
      private FlowLayoutPanel FlowLayout;
   }
}
