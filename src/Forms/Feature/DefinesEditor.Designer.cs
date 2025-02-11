namespace Editor.src.Forms.Feature
{
   partial class DefinesEditor
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DefinesEditor));
         tableLayoutPanel1 = new TableLayoutPanel();
         DefineNameComboBox = new ComboBox();
         DefineValueTextBox = new TextBox();
         DefinesListBox = new ListBox();
         SaveButton = new Button();
         CancelButton = new Button();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
         tableLayoutPanel1.Controls.Add(DefineNameComboBox, 0, 0);
         tableLayoutPanel1.Controls.Add(DefineValueTextBox, 1, 0);
         tableLayoutPanel1.Controls.Add(DefinesListBox, 0, 1);
         tableLayoutPanel1.Controls.Add(SaveButton, 1, 2);
         tableLayoutPanel1.Controls.Add(CancelButton, 0, 2);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Size = new Size(514, 652);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // DefineNameComboBox
         // 
         DefineNameComboBox.Dock = DockStyle.Fill;
         DefineNameComboBox.FormattingEnabled = true;
         DefineNameComboBox.Location = new Point(3, 3);
         DefineNameComboBox.Name = "DefineNameComboBox";
         DefineNameComboBox.Size = new Size(379, 23);
         DefineNameComboBox.TabIndex = 0;
         // 
         // DefineValueTextBox
         // 
         DefineValueTextBox.Dock = DockStyle.Fill;
         DefineValueTextBox.Location = new Point(388, 3);
         DefineValueTextBox.Name = "DefineValueTextBox";
         DefineValueTextBox.Size = new Size(123, 23);
         DefineValueTextBox.TabIndex = 1;
         // 
         // DefinesListBox
         // 
         tableLayoutPanel1.SetColumnSpan(DefinesListBox, 2);
         DefinesListBox.Dock = DockStyle.Fill;
         DefinesListBox.FormattingEnabled = true;
         DefinesListBox.ItemHeight = 15;
         DefinesListBox.Location = new Point(3, 33);
         DefinesListBox.Name = "DefinesListBox";
         DefinesListBox.Size = new Size(508, 586);
         DefinesListBox.TabIndex = 2;
         // 
         // SaveButton
         // 
         SaveButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
         SaveButton.Location = new Point(436, 625);
         SaveButton.Name = "SaveButton";
         SaveButton.Size = new Size(75, 24);
         SaveButton.TabIndex = 3;
         SaveButton.Text = "Save";
         SaveButton.UseVisualStyleBackColor = true;
         // 
         // CancelButton
         // 
         CancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
         CancelButton.Location = new Point(3, 625);
         CancelButton.Name = "CancelButton";
         CancelButton.Size = new Size(75, 24);
         CancelButton.TabIndex = 4;
         CancelButton.Text = "Cancel";
         CancelButton.UseVisualStyleBackColor = true;
         // 
         // DefinesEditor
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(514, 652);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "DefinesEditor";
         StartPosition = FormStartPosition.CenterScreen;
         Text = "Defines Editor";
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private ComboBox DefineNameComboBox;
      private TextBox DefineValueTextBox;
      private ListBox DefinesListBox;
      private Button SaveButton;
      private Button CancelButton;
   }
}