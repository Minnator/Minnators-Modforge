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
         this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
         this.ToolTipPreview = new System.Windows.Forms.ListView();
         this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
         this.AddButton = new System.Windows.Forms.Button();
         this.RemoveButton = new System.Windows.Forms.Button();
         this.MoveDownButton = new System.Windows.Forms.Button();
         this.MoveUpButton = new System.Windows.Forms.Button();
         this.ConfirmButton = new System.Windows.Forms.Button();
         this.CancelButton = new System.Windows.Forms.Button();
         this.InputTextBox = new System.Windows.Forms.TextBox();
         this.tableLayoutPanel1.SuspendLayout();
         this.tableLayoutPanel2.SuspendLayout();
         this.SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         this.tableLayoutPanel1.ColumnCount = 2;
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.tableLayoutPanel1.Controls.Add(this.ToolTipPreview, 0, 0);
         this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
         this.tableLayoutPanel1.Controls.Add(this.ConfirmButton, 1, 2);
         this.tableLayoutPanel1.Controls.Add(this.CancelButton, 0, 2);
         this.tableLayoutPanel1.Controls.Add(this.InputTextBox, 0, 1);
         this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
         this.tableLayoutPanel1.Name = "tableLayoutPanel1";
         this.tableLayoutPanel1.RowCount = 3;
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
         this.tableLayoutPanel1.Size = new System.Drawing.Size(472, 214);
         this.tableLayoutPanel1.TabIndex = 0;
         // 
         // ToolTipPreview
         // 
         this.tableLayoutPanel1.SetColumnSpan(this.ToolTipPreview, 2);
         this.ToolTipPreview.Dock = System.Windows.Forms.DockStyle.Fill;
         this.ToolTipPreview.FullRowSelect = true;
         this.ToolTipPreview.GridLines = true;
         this.ToolTipPreview.HideSelection = false;
         this.ToolTipPreview.Location = new System.Drawing.Point(3, 3);
         this.ToolTipPreview.Name = "ToolTipPreview";
         this.ToolTipPreview.Size = new System.Drawing.Size(466, 151);
         this.ToolTipPreview.TabIndex = 3;
         this.ToolTipPreview.UseCompatibleStateImageBehavior = false;
         // 
         // tableLayoutPanel2
         // 
         this.tableLayoutPanel2.ColumnCount = 4;
         this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
         this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
         this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
         this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
         this.tableLayoutPanel2.Controls.Add(this.AddButton, 0, 0);
         this.tableLayoutPanel2.Controls.Add(this.RemoveButton, 1, 0);
         this.tableLayoutPanel2.Controls.Add(this.MoveDownButton, 3, 0);
         this.tableLayoutPanel2.Controls.Add(this.MoveUpButton, 2, 0);
         this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tableLayoutPanel2.Location = new System.Drawing.Point(236, 157);
         this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
         this.tableLayoutPanel2.Name = "tableLayoutPanel2";
         this.tableLayoutPanel2.RowCount = 1;
         this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
         this.tableLayoutPanel2.Size = new System.Drawing.Size(236, 27);
         this.tableLayoutPanel2.TabIndex = 5;
         // 
         // AddButton
         // 
         this.AddButton.Dock = System.Windows.Forms.DockStyle.Fill;
         this.AddButton.Location = new System.Drawing.Point(3, 3);
         this.AddButton.Name = "AddButton";
         this.AddButton.Size = new System.Drawing.Size(53, 21);
         this.AddButton.TabIndex = 6;
         this.AddButton.Text = "Add";
         this.AddButton.UseVisualStyleBackColor = true;
         this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
         // 
         // RemoveButton
         // 
         this.RemoveButton.Dock = System.Windows.Forms.DockStyle.Fill;
         this.RemoveButton.Location = new System.Drawing.Point(62, 3);
         this.RemoveButton.Name = "RemoveButton";
         this.RemoveButton.Size = new System.Drawing.Size(53, 21);
         this.RemoveButton.TabIndex = 4;
         this.RemoveButton.Text = "Remove";
         this.RemoveButton.UseVisualStyleBackColor = true;
         this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
         // 
         // MoveDownButton
         // 
         this.MoveDownButton.Dock = System.Windows.Forms.DockStyle.Fill;
         this.MoveDownButton.Location = new System.Drawing.Point(180, 3);
         this.MoveDownButton.Name = "MoveDownButton";
         this.MoveDownButton.Size = new System.Drawing.Size(53, 21);
         this.MoveDownButton.TabIndex = 1;
         this.MoveDownButton.Text = "Down";
         this.MoveDownButton.UseVisualStyleBackColor = true;
         this.MoveDownButton.Click += new System.EventHandler(this.MoveDownButton_Click);
         // 
         // MoveUpButton
         // 
         this.MoveUpButton.Dock = System.Windows.Forms.DockStyle.Fill;
         this.MoveUpButton.Location = new System.Drawing.Point(121, 3);
         this.MoveUpButton.Name = "MoveUpButton";
         this.MoveUpButton.Size = new System.Drawing.Size(53, 21);
         this.MoveUpButton.TabIndex = 0;
         this.MoveUpButton.Text = "Up";
         this.MoveUpButton.UseVisualStyleBackColor = true;
         this.MoveUpButton.Click += new System.EventHandler(this.MoveUpButton_Click);
         // 
         // ConfirmButton
         // 
         this.ConfirmButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.ConfirmButton.Location = new System.Drawing.Point(394, 187);
         this.ConfirmButton.Name = "ConfirmButton";
         this.ConfirmButton.Size = new System.Drawing.Size(75, 23);
         this.ConfirmButton.TabIndex = 6;
         this.ConfirmButton.Text = "Confirm";
         this.ConfirmButton.UseVisualStyleBackColor = true;
         this.ConfirmButton.Click += new System.EventHandler(this.ConfirmButton_Click);
         // 
         // CancelButton
         // 
         this.CancelButton.Location = new System.Drawing.Point(3, 187);
         this.CancelButton.Name = "CancelButton";
         this.CancelButton.Size = new System.Drawing.Size(75, 23);
         this.CancelButton.TabIndex = 7;
         this.CancelButton.Text = "Cancel";
         this.CancelButton.UseVisualStyleBackColor = true;
         this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
         // 
         // InputTextBox
         // 
         this.InputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.InputTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
         this.InputTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
         this.InputTextBox.Location = new System.Drawing.Point(3, 160);
         this.InputTextBox.Name = "InputTextBox";
         this.InputTextBox.Size = new System.Drawing.Size(230, 20);
         this.InputTextBox.TabIndex = 8;
         // 
         // ToolTipCustomizer
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(472, 214);
         this.Controls.Add(this.tableLayoutPanel1);
         this.Name = "ToolTipCustomizer";
         this.Text = "Tool Tip Customizer";
         this.tableLayoutPanel1.ResumeLayout(false);
         this.tableLayoutPanel1.PerformLayout();
         this.tableLayoutPanel2.ResumeLayout(false);
         this.ResumeLayout(false);

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
      private System.Windows.Forms.Button CancelButton;
      private System.Windows.Forms.TextBox InputTextBox;
   }
}