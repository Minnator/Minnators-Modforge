namespace Editor.Forms.AdvancedSelections
{
   partial class AdvancedSelectionsForm
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
         this.ConfirmButton = new System.Windows.Forms.Button();
         this.CancelButton = new System.Windows.Forms.Button();
         this.AttributeSelectionComboBox = new System.Windows.Forms.ComboBox();
         this.AttributeValueInput = new System.Windows.Forms.TextBox();
         this.ActionTypeComboBox = new System.Windows.Forms.ComboBox();
         this.tableLayoutPanel1.SuspendLayout();
         this.SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         this.tableLayoutPanel1.ColumnCount = 2;
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
         this.tableLayoutPanel1.Controls.Add(this.ConfirmButton, 1, 3);
         this.tableLayoutPanel1.Controls.Add(this.CancelButton, 0, 3);
         this.tableLayoutPanel1.Controls.Add(this.AttributeSelectionComboBox, 0, 1);
         this.tableLayoutPanel1.Controls.Add(this.AttributeValueInput, 0, 2);
         this.tableLayoutPanel1.Controls.Add(this.ActionTypeComboBox, 0, 0);
         this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
         this.tableLayoutPanel1.Name = "tableLayoutPanel1";
         this.tableLayoutPanel1.RowCount = 4;
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
         this.tableLayoutPanel1.Size = new System.Drawing.Size(242, 104);
         this.tableLayoutPanel1.TabIndex = 0;
         // 
         // ConfirmButton
         // 
         this.ConfirmButton.Dock = System.Windows.Forms.DockStyle.Fill;
         this.ConfirmButton.Location = new System.Drawing.Point(124, 78);
         this.ConfirmButton.Name = "ConfirmButton";
         this.ConfirmButton.Size = new System.Drawing.Size(115, 23);
         this.ConfirmButton.TabIndex = 0;
         this.ConfirmButton.Text = "Confirm";
         this.ConfirmButton.UseVisualStyleBackColor = true;
         this.ConfirmButton.Click += new System.EventHandler(this.ConfirmButton_Click);
         // 
         // CancelButton
         // 
         this.CancelButton.Dock = System.Windows.Forms.DockStyle.Fill;
         this.CancelButton.Location = new System.Drawing.Point(3, 78);
         this.CancelButton.Name = "CancelButton";
         this.CancelButton.Size = new System.Drawing.Size(115, 23);
         this.CancelButton.TabIndex = 1;
         this.CancelButton.Text = "Cancel";
         this.CancelButton.UseVisualStyleBackColor = true;
         this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
         // 
         // AttributeSelectionComboBox
         // 
         this.tableLayoutPanel1.SetColumnSpan(this.AttributeSelectionComboBox, 2);
         this.AttributeSelectionComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
         this.AttributeSelectionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.AttributeSelectionComboBox.FormattingEnabled = true;
         this.AttributeSelectionComboBox.Location = new System.Drawing.Point(3, 28);
         this.AttributeSelectionComboBox.Name = "AttributeSelectionComboBox";
         this.AttributeSelectionComboBox.Size = new System.Drawing.Size(236, 21);
         this.AttributeSelectionComboBox.TabIndex = 2;
         // 
         // AttributeValueInput
         // 
         this.tableLayoutPanel1.SetColumnSpan(this.AttributeValueInput, 2);
         this.AttributeValueInput.Dock = System.Windows.Forms.DockStyle.Fill;
         this.AttributeValueInput.Location = new System.Drawing.Point(3, 53);
         this.AttributeValueInput.Name = "AttributeValueInput";
         this.AttributeValueInput.Size = new System.Drawing.Size(236, 20);
         this.AttributeValueInput.TabIndex = 3;
         // 
         // ActionTypeComboBox
         // 
         this.tableLayoutPanel1.SetColumnSpan(this.ActionTypeComboBox, 2);
         this.ActionTypeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
         this.ActionTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.ActionTypeComboBox.FormattingEnabled = true;
         this.ActionTypeComboBox.Location = new System.Drawing.Point(3, 3);
         this.ActionTypeComboBox.Name = "ActionTypeComboBox";
         this.ActionTypeComboBox.Size = new System.Drawing.Size(236, 21);
         this.ActionTypeComboBox.TabIndex = 4;
         // 
         // AdvancedSelectionsForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(242, 104);
         this.Controls.Add(this.tableLayoutPanel1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.Name = "AdvancedSelectionsForm";
         this.Text = "AdvancedSelection";
         this.tableLayoutPanel1.ResumeLayout(false);
         this.tableLayoutPanel1.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      private System.Windows.Forms.Button ConfirmButton;
      private System.Windows.Forms.Button CancelButton;
      private System.Windows.Forms.ComboBox AttributeSelectionComboBox;
      private System.Windows.Forms.TextBox AttributeValueInput;
      private System.Windows.Forms.ComboBox ActionTypeComboBox;
   }
}