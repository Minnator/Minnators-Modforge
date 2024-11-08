namespace Editor.Forms
{
   partial class ToolTipCustomizer
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolTipCustomizer));
         tableLayoutPanel1 = new TableLayoutPanel();
         ToolTipPreview = new ListView();
         tableLayoutPanel2 = new TableLayoutPanel();
         AddButton = new Button();
         RemoveButton = new Button();
         MoveDownButton = new Button();
         MoveUpButton = new Button();
         ConfirmButton = new Button();
         CancelButton = new Button();
         InputTextBox = new TextBox();
         toolTip1 = new ToolTip(components);
         tableLayoutPanel1.SuspendLayout();
         tableLayoutPanel2.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Controls.Add(ToolTipPreview, 0, 0);
         tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 1);
         tableLayoutPanel1.Controls.Add(ConfirmButton, 1, 2);
         tableLayoutPanel1.Controls.Add(CancelButton, 0, 2);
         tableLayoutPanel1.Controls.Add(InputTextBox, 0, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
         tableLayoutPanel1.Size = new Size(551, 247);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // ToolTipPreview
         // 
         tableLayoutPanel1.SetColumnSpan(ToolTipPreview, 2);
         ToolTipPreview.Dock = DockStyle.Fill;
         ToolTipPreview.FullRowSelect = true;
         ToolTipPreview.GridLines = true;
         ToolTipPreview.Location = new Point(4, 3);
         ToolTipPreview.Margin = new Padding(4, 3, 4, 3);
         ToolTipPreview.Name = "ToolTipPreview";
         ToolTipPreview.Size = new Size(543, 175);
         ToolTipPreview.TabIndex = 3;
         ToolTipPreview.UseCompatibleStateImageBehavior = false;
         // 
         // tableLayoutPanel2
         // 
         tableLayoutPanel2.ColumnCount = 4;
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
         tableLayoutPanel2.Controls.Add(AddButton, 0, 0);
         tableLayoutPanel2.Controls.Add(RemoveButton, 1, 0);
         tableLayoutPanel2.Controls.Add(MoveDownButton, 3, 0);
         tableLayoutPanel2.Controls.Add(MoveUpButton, 2, 0);
         tableLayoutPanel2.Dock = DockStyle.Fill;
         tableLayoutPanel2.Location = new Point(275, 181);
         tableLayoutPanel2.Margin = new Padding(0);
         tableLayoutPanel2.Name = "tableLayoutPanel2";
         tableLayoutPanel2.RowCount = 1;
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel2.Size = new Size(276, 31);
         tableLayoutPanel2.TabIndex = 5;
         // 
         // AddButton
         // 
         AddButton.Dock = DockStyle.Fill;
         AddButton.Location = new Point(4, 3);
         AddButton.Margin = new Padding(4, 3, 4, 3);
         AddButton.Name = "AddButton";
         AddButton.Size = new Size(61, 25);
         AddButton.TabIndex = 6;
         AddButton.Text = "Add";
         toolTip1.SetToolTip(AddButton, "Adds a new line to the tooltip");
         AddButton.UseVisualStyleBackColor = true;
         AddButton.Click += AddButton_Click;
         // 
         // RemoveButton
         // 
         RemoveButton.Dock = DockStyle.Fill;
         RemoveButton.Location = new Point(73, 3);
         RemoveButton.Margin = new Padding(4, 3, 4, 3);
         RemoveButton.Name = "RemoveButton";
         RemoveButton.Size = new Size(61, 25);
         RemoveButton.TabIndex = 4;
         RemoveButton.Text = "Remove";
         toolTip1.SetToolTip(RemoveButton, "Remove the selected line from the  tooltip");
         RemoveButton.UseVisualStyleBackColor = true;
         RemoveButton.Click += RemoveButton_Click;
         // 
         // MoveDownButton
         // 
         MoveDownButton.Dock = DockStyle.Fill;
         MoveDownButton.Location = new Point(211, 3);
         MoveDownButton.Margin = new Padding(4, 3, 4, 3);
         MoveDownButton.Name = "MoveDownButton";
         MoveDownButton.Size = new Size(61, 25);
         MoveDownButton.TabIndex = 1;
         MoveDownButton.Text = "Down";
         toolTip1.SetToolTip(MoveDownButton, "Move the selected one line down in the tooltip");
         MoveDownButton.UseVisualStyleBackColor = true;
         MoveDownButton.Click += MoveDownButton_Click;
         // 
         // MoveUpButton
         // 
         MoveUpButton.Dock = DockStyle.Fill;
         MoveUpButton.Location = new Point(142, 3);
         MoveUpButton.Margin = new Padding(4, 3, 4, 3);
         MoveUpButton.Name = "MoveUpButton";
         MoveUpButton.Size = new Size(61, 25);
         MoveUpButton.TabIndex = 0;
         MoveUpButton.Text = "Up";
         toolTip1.SetToolTip(MoveUpButton, "Move the selected line up in the tooltip");
         MoveUpButton.UseVisualStyleBackColor = true;
         MoveUpButton.Click += MoveUpButton_Click;
         // 
         // ConfirmButton
         // 
         ConfirmButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
         ConfirmButton.Location = new Point(459, 215);
         ConfirmButton.Margin = new Padding(4, 3, 4, 3);
         ConfirmButton.Name = "ConfirmButton";
         ConfirmButton.Size = new Size(88, 27);
         ConfirmButton.TabIndex = 6;
         ConfirmButton.Text = "Confirm";
         toolTip1.SetToolTip(ConfirmButton, "Save and apply the changes");
         ConfirmButton.UseVisualStyleBackColor = true;
         ConfirmButton.Click += ConfirmButton_Click;
         // 
         // CancelButton
         // 
         CancelButton.Location = new Point(4, 215);
         CancelButton.Margin = new Padding(4, 3, 4, 3);
         CancelButton.Name = "CancelButton";
         CancelButton.Size = new Size(88, 27);
         CancelButton.TabIndex = 7;
         CancelButton.Text = "Cancel";
         CancelButton.UseVisualStyleBackColor = true;
         CancelButton.Click += CancelButton_Click;
         // 
         // InputTextBox
         // 
         InputTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
         InputTextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         InputTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
         InputTextBox.Location = new Point(4, 184);
         InputTextBox.Margin = new Padding(4, 3, 4, 3);
         InputTextBox.Name = "InputTextBox";
         InputTextBox.Size = new Size(267, 23);
         InputTextBox.TabIndex = 8;
         toolTip1.SetToolTip(InputTextBox, "Use \"$<province_attribute>$\" to show the given attributes value\r\nUse \"$<province_attribute>%L$\" to show the localisation of the attribute");
         // 
         // ToolTipCustomizer
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(551, 247);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Margin = new Padding(4, 3, 4, 3);
         Name = "ToolTipCustomizer";
         Text = "Tool Tip Customizer";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         tableLayoutPanel2.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      private System.Windows.Forms.Button MoveUpButton;
      private System.Windows.Forms.Button MoveDownButton;
      private System.Windows.Forms.ListView ToolTipPreview;
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
      private System.Windows.Forms.Button AddButton;
      private System.Windows.Forms.Button RemoveButton;
      private System.Windows.Forms.Button ConfirmButton;
      private new System.Windows.Forms.Button CancelButton;
      private System.Windows.Forms.TextBox InputTextBox;
      private ToolTip toolTip1;
   }
}