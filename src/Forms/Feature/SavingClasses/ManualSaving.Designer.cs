namespace Editor.Forms.Feature.SavingClasses
{
   partial class ManualSaving
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManualSaving));
         tableLayoutPanel1 = new TableLayoutPanel();
         SavingCheckedListBox = new CheckedListBox();
         label1 = new Label();
         SaveSelectedButton = new Button();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 1;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Controls.Add(SavingCheckedListBox, 0, 1);
         tableLayoutPanel1.Controls.Add(label1, 0, 0);
         tableLayoutPanel1.Controls.Add(SaveSelectedButton, 0, 2);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 3;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
         tableLayoutPanel1.Size = new Size(225, 302);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // SavingCheckedListBox
         // 
         SavingCheckedListBox.CheckOnClick = true;
         SavingCheckedListBox.Dock = DockStyle.Fill;
         SavingCheckedListBox.FormattingEnabled = true;
         SavingCheckedListBox.Items.AddRange(new object[] { "Provinces", "Areas", "Regions", "Tradenodes", "Tradecompanies", "Colonial regions", "Super regions", "Continents", "Province groups", "Event Modifiers", "Localisation", "Countries" });
         SavingCheckedListBox.Location = new Point(8, 47);
         SavingCheckedListBox.Margin = new Padding(8, 3, 8, 3);
         SavingCheckedListBox.Name = "SavingCheckedListBox";
         SavingCheckedListBox.Size = new Size(209, 225);
         SavingCheckedListBox.TabIndex = 0;
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
         label1.Location = new Point(4, 0);
         label1.Margin = new Padding(4, 0, 4, 0);
         label1.Name = "label1";
         label1.Size = new Size(217, 44);
         label1.TabIndex = 1;
         label1.Text = "Select which items will be save. All items detected as modified are preselected\r\n";
         label1.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // SaveSelectedButton
         // 
         SaveSelectedButton.Dock = DockStyle.Fill;
         SaveSelectedButton.Location = new Point(8, 277);
         SaveSelectedButton.Margin = new Padding(8, 2, 8, 2);
         SaveSelectedButton.Name = "SaveSelectedButton";
         SaveSelectedButton.Size = new Size(209, 23);
         SaveSelectedButton.TabIndex = 2;
         SaveSelectedButton.Text = "Save";
         SaveSelectedButton.UseVisualStyleBackColor = true;
         SaveSelectedButton.Click += SaveSelectedButton_Click;
         // 
         // ManualSaving
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(225, 302);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         KeyPreview = true;
         Margin = new Padding(4, 3, 4, 3);
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "ManualSaving";
         StartPosition = FormStartPosition.CenterScreen;
         Text = "Manual Saving";
         KeyDown += ManualSaving_KeyDown;
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private CheckedListBox SavingCheckedListBox;
      private Label label1;
      private Button SaveSelectedButton;
   }
}