namespace Editor.Forms.Feature.AdvancedSelections
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedSelectionsForm));
         tableLayoutPanel1 = new TableLayoutPanel();
         ConfirmButton = new Button();
         CancelButton = new Button();
         ActionTypeComboBox = new ComboBox();
         AttributeValueInput = new TextBox();
         OperationComboBox = new ComboBox();
         AttributeSelectionComboBox = new ComboBox();
         SelectionSource = new ComboBox();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Controls.Add(ConfirmButton, 1, 5);
         tableLayoutPanel1.Controls.Add(CancelButton, 0, 5);
         tableLayoutPanel1.Controls.Add(ActionTypeComboBox, 0, 1);
         tableLayoutPanel1.Controls.Add(AttributeValueInput, 0, 4);
         tableLayoutPanel1.Controls.Add(OperationComboBox, 0, 2);
         tableLayoutPanel1.Controls.Add(AttributeSelectionComboBox, 0, 3);
         tableLayoutPanel1.Controls.Add(SelectionSource, 0, 0);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 6;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
         tableLayoutPanel1.Size = new Size(282, 176);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // ConfirmButton
         // 
         ConfirmButton.Dock = DockStyle.Fill;
         ConfirmButton.Location = new Point(145, 148);
         ConfirmButton.Margin = new Padding(4, 3, 4, 3);
         ConfirmButton.Name = "ConfirmButton";
         ConfirmButton.Size = new Size(133, 25);
         ConfirmButton.TabIndex = 0;
         ConfirmButton.Text = "Confirm";
         ConfirmButton.UseVisualStyleBackColor = true;
         ConfirmButton.Click += ConfirmButton_Click;
         // 
         // CancelButton
         // 
         CancelButton.Dock = DockStyle.Fill;
         CancelButton.Location = new Point(4, 148);
         CancelButton.Margin = new Padding(4, 3, 4, 3);
         CancelButton.Name = "CancelButton";
         CancelButton.Size = new Size(133, 25);
         CancelButton.TabIndex = 1;
         CancelButton.Text = "Cancel";
         CancelButton.UseVisualStyleBackColor = true;
         CancelButton.Click += CancelButton_Click;
         // 
         // ActionTypeComboBox
         // 
         tableLayoutPanel1.SetColumnSpan(ActionTypeComboBox, 2);
         ActionTypeComboBox.Dock = DockStyle.Fill;
         ActionTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
         ActionTypeComboBox.FormattingEnabled = true;
         ActionTypeComboBox.Location = new Point(4, 32);
         ActionTypeComboBox.Margin = new Padding(4, 3, 4, 3);
         ActionTypeComboBox.Name = "ActionTypeComboBox";
         ActionTypeComboBox.Size = new Size(274, 23);
         ActionTypeComboBox.TabIndex = 4;
         // 
         // AttributeValueInput
         // 
         tableLayoutPanel1.SetColumnSpan(AttributeValueInput, 2);
         AttributeValueInput.Dock = DockStyle.Fill;
         AttributeValueInput.Location = new Point(4, 119);
         AttributeValueInput.Margin = new Padding(4, 3, 4, 3);
         AttributeValueInput.Name = "AttributeValueInput";
         AttributeValueInput.Size = new Size(274, 23);
         AttributeValueInput.TabIndex = 3;
         // 
         // OperationComboBox
         // 
         tableLayoutPanel1.SetColumnSpan(OperationComboBox, 2);
         OperationComboBox.Dock = DockStyle.Fill;
         OperationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
         OperationComboBox.FormattingEnabled = true;
         OperationComboBox.Location = new Point(3, 61);
         OperationComboBox.Name = "OperationComboBox";
         OperationComboBox.Size = new Size(276, 23);
         OperationComboBox.TabIndex = 5;
         // 
         // AttributeSelectionComboBox
         // 
         tableLayoutPanel1.SetColumnSpan(AttributeSelectionComboBox, 2);
         AttributeSelectionComboBox.Dock = DockStyle.Fill;
         AttributeSelectionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
         AttributeSelectionComboBox.FormattingEnabled = true;
         AttributeSelectionComboBox.Location = new Point(4, 90);
         AttributeSelectionComboBox.Margin = new Padding(4, 3, 4, 3);
         AttributeSelectionComboBox.Name = "AttributeSelectionComboBox";
         AttributeSelectionComboBox.Size = new Size(274, 23);
         AttributeSelectionComboBox.TabIndex = 2;
         // 
         // SelectionSource
         // 
         tableLayoutPanel1.SetColumnSpan(SelectionSource, 2);
         SelectionSource.Dock = DockStyle.Fill;
         SelectionSource.DropDownStyle = ComboBoxStyle.DropDownList;
         SelectionSource.FormattingEnabled = true;
         SelectionSource.Location = new Point(3, 3);
         SelectionSource.Name = "SelectionSource";
         SelectionSource.Size = new Size(276, 23);
         SelectionSource.TabIndex = 6;
         // 
         // AdvancedSelectionsForm
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(282, 176);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.FixedSingle;
         Icon = (Icon)resources.GetObject("$this.Icon");
         Margin = new Padding(4, 3, 4, 3);
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "AdvancedSelectionsForm";
         Text = "AdvancedSelection";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      private System.Windows.Forms.Button ConfirmButton;
      private System.Windows.Forms.Button CancelButton;
      private System.Windows.Forms.ComboBox AttributeSelectionComboBox;
      private System.Windows.Forms.TextBox AttributeValueInput;
      private System.Windows.Forms.ComboBox ActionTypeComboBox;
      private ComboBox OperationComboBox;
      private ComboBox SelectionSource;
   }
}