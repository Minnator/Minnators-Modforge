namespace Editor.Forms
{
   partial class EventModifierForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventModifierForm));
         EventModifiersLayoutPanel = new TableLayoutPanel();
         ModifiersLP = new TableLayoutPanel();
         label6 = new Label();
         CustomAttributesLP = new TableLayoutPanel();
         label5 = new Label();
         tableLayoutPanel3 = new TableLayoutPanel();
         label4 = new Label();
         DescriptionTextBox = new TextBox();
         tableLayoutPanel2 = new TableLayoutPanel();
         label2 = new Label();
         LocalisationTextBox = new TextBox();
         EventModNameTL = new TableLayoutPanel();
         label3 = new Label();
         label1 = new Label();
         tableLayoutPanel7 = new TableLayoutPanel();
         SaveButton = new Button();
         CancelButton = new Button();
         ModifyButton = new Button();
         toolTip1 = new ToolTip(components);
         EventModifiersLayoutPanel.SuspendLayout();
         ModifiersLP.SuspendLayout();
         CustomAttributesLP.SuspendLayout();
         tableLayoutPanel3.SuspendLayout();
         tableLayoutPanel2.SuspendLayout();
         EventModNameTL.SuspendLayout();
         tableLayoutPanel7.SuspendLayout();
         SuspendLayout();
         // 
         // EventModifiersLayoutPanel
         // 
         EventModifiersLayoutPanel.ColumnCount = 1;
         EventModifiersLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         EventModifiersLayoutPanel.Controls.Add(ModifiersLP, 0, 5);
         EventModifiersLayoutPanel.Controls.Add(CustomAttributesLP, 0, 4);
         EventModifiersLayoutPanel.Controls.Add(tableLayoutPanel3, 0, 3);
         EventModifiersLayoutPanel.Controls.Add(tableLayoutPanel2, 0, 1);
         EventModifiersLayoutPanel.Controls.Add(EventModNameTL, 0, 1);
         EventModifiersLayoutPanel.Controls.Add(label1, 0, 0);
         EventModifiersLayoutPanel.Controls.Add(tableLayoutPanel7, 0, 6);
         EventModifiersLayoutPanel.Dock = DockStyle.Fill;
         EventModifiersLayoutPanel.Location = new Point(0, 0);
         EventModifiersLayoutPanel.Name = "EventModifiersLayoutPanel";
         EventModifiersLayoutPanel.RowCount = 7;
         EventModifiersLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
         EventModifiersLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         EventModifiersLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         EventModifiersLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
         EventModifiersLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
         EventModifiersLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 125F));
         EventModifiersLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
         EventModifiersLayoutPanel.Size = new Size(541, 369);
         EventModifiersLayoutPanel.TabIndex = 0;
         // 
         // ModifiersLP
         // 
         ModifiersLP.ColumnCount = 2;
         ModifiersLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         ModifiersLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
         ModifiersLP.Controls.Add(label6, 0, 0);
         ModifiersLP.Dock = DockStyle.Fill;
         ModifiersLP.Location = new Point(0, 218);
         ModifiersLP.Margin = new Padding(0);
         ModifiersLP.Name = "ModifiersLP";
         ModifiersLP.RowCount = 1;
         ModifiersLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         ModifiersLP.Size = new Size(541, 125);
         ModifiersLP.TabIndex = 5;
         // 
         // label6
         // 
         label6.AutoSize = true;
         label6.Dock = DockStyle.Fill;
         label6.Location = new Point(6, 0);
         label6.Margin = new Padding(6, 0, 3, 0);
         label6.Name = "label6";
         label6.Size = new Size(99, 125);
         label6.TabIndex = 0;
         label6.Text = "Modifiers";
         label6.TextAlign = ContentAlignment.MiddleLeft;
         toolTip1.SetToolTip(label6, "The individual modifiers");
         // 
         // CustomAttributesLP
         // 
         CustomAttributesLP.ColumnCount = 2;
         CustomAttributesLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         CustomAttributesLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
         CustomAttributesLP.Controls.Add(label5, 0, 0);
         CustomAttributesLP.Dock = DockStyle.Fill;
         CustomAttributesLP.Location = new Point(0, 140);
         CustomAttributesLP.Margin = new Padding(0);
         CustomAttributesLP.Name = "CustomAttributesLP";
         CustomAttributesLP.RowCount = 1;
         CustomAttributesLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         CustomAttributesLP.Size = new Size(541, 78);
         CustomAttributesLP.TabIndex = 4;
         // 
         // label5
         // 
         label5.AutoSize = true;
         label5.Dock = DockStyle.Fill;
         label5.Location = new Point(2, 0);
         label5.Margin = new Padding(2, 0, 3, 0);
         label5.Name = "label5";
         label5.Size = new Size(103, 78);
         label5.TabIndex = 0;
         label5.Text = "Custom attributes";
         label5.TextAlign = ContentAlignment.MiddleLeft;
         toolTip1.SetToolTip(label5, "Custom attributes; e.g. religion = yes");
         // 
         // tableLayoutPanel3
         // 
         tableLayoutPanel3.ColumnCount = 2;
         tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
         tableLayoutPanel3.Controls.Add(label4, 0, 0);
         tableLayoutPanel3.Controls.Add(DescriptionTextBox, 1, 0);
         tableLayoutPanel3.Dock = DockStyle.Fill;
         tableLayoutPanel3.Location = new Point(0, 90);
         tableLayoutPanel3.Margin = new Padding(0);
         tableLayoutPanel3.Name = "tableLayoutPanel3";
         tableLayoutPanel3.RowCount = 1;
         tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel3.Size = new Size(541, 50);
         tableLayoutPanel3.TabIndex = 3;
         // 
         // label4
         // 
         label4.AutoSize = true;
         label4.Dock = DockStyle.Fill;
         label4.Location = new Point(2, 0);
         label4.Margin = new Padding(2, 0, 3, 0);
         label4.Name = "label4";
         label4.Size = new Size(103, 50);
         label4.TabIndex = 0;
         label4.Text = "Description";
         label4.TextAlign = ContentAlignment.MiddleLeft;
         toolTip1.SetToolTip(label4, "The Description of the Modifier");
         // 
         // DescriptionTextBox
         // 
         DescriptionTextBox.Dock = DockStyle.Fill;
         DescriptionTextBox.Location = new Point(111, 1);
         DescriptionTextBox.Margin = new Padding(3, 1, 3, 1);
         DescriptionTextBox.Multiline = true;
         DescriptionTextBox.Name = "DescriptionTextBox";
         DescriptionTextBox.Size = new Size(427, 48);
         DescriptionTextBox.TabIndex = 1;
         // 
         // tableLayoutPanel2
         // 
         tableLayoutPanel2.ColumnCount = 2;
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
         tableLayoutPanel2.Controls.Add(label2, 0, 0);
         tableLayoutPanel2.Controls.Add(LocalisationTextBox, 1, 0);
         tableLayoutPanel2.Dock = DockStyle.Fill;
         tableLayoutPanel2.Location = new Point(0, 65);
         tableLayoutPanel2.Margin = new Padding(0);
         tableLayoutPanel2.Name = "tableLayoutPanel2";
         tableLayoutPanel2.RowCount = 1;
         tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel2.Size = new Size(541, 25);
         tableLayoutPanel2.TabIndex = 1;
         // 
         // label2
         // 
         label2.AutoSize = true;
         label2.Dock = DockStyle.Fill;
         label2.Location = new Point(2, 0);
         label2.Margin = new Padding(2, 0, 3, 0);
         label2.Name = "label2";
         label2.Size = new Size(103, 25);
         label2.TabIndex = 0;
         label2.Text = "Title";
         label2.TextAlign = ContentAlignment.MiddleLeft;
         toolTip1.SetToolTip(label2, "The localisation for the modifier");
         // 
         // LocalisationTextBox
         // 
         LocalisationTextBox.Dock = DockStyle.Fill;
         LocalisationTextBox.Location = new Point(111, 1);
         LocalisationTextBox.Margin = new Padding(3, 1, 3, 1);
         LocalisationTextBox.Name = "LocalisationTextBox";
         LocalisationTextBox.Size = new Size(427, 23);
         LocalisationTextBox.TabIndex = 1;
         // 
         // EventModNameTL
         // 
         EventModNameTL.ColumnCount = 2;
         EventModNameTL.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         EventModNameTL.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
         EventModNameTL.Controls.Add(label3, 0, 0);
         EventModNameTL.Dock = DockStyle.Fill;
         EventModNameTL.Location = new Point(0, 40);
         EventModNameTL.Margin = new Padding(0);
         EventModNameTL.Name = "EventModNameTL";
         EventModNameTL.RowCount = 1;
         EventModNameTL.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         EventModNameTL.Size = new Size(541, 25);
         EventModNameTL.TabIndex = 0;
         // 
         // label3
         // 
         label3.AutoSize = true;
         label3.Dock = DockStyle.Fill;
         label3.Location = new Point(2, 0);
         label3.Margin = new Padding(2, 0, 3, 0);
         label3.Name = "label3";
         label3.Size = new Size(103, 25);
         label3.TabIndex = 0;
         label3.Text = "code_name";
         label3.TextAlign = ContentAlignment.MiddleLeft;
         toolTip1.SetToolTip(label3, "The name of the modifier how it will be called in the mod files.");
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
         label1.Location = new Point(3, 0);
         label1.Name = "label1";
         label1.Size = new Size(535, 40);
         label1.TabIndex = 2;
         label1.Text = "Event Modifier creation and modification menu";
         label1.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // tableLayoutPanel7
         // 
         tableLayoutPanel7.ColumnCount = 5;
         tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
         tableLayoutPanel7.Controls.Add(SaveButton, 4, 0);
         tableLayoutPanel7.Controls.Add(CancelButton, 0, 0);
         tableLayoutPanel7.Controls.Add(ModifyButton, 2, 0);
         tableLayoutPanel7.Dock = DockStyle.Fill;
         tableLayoutPanel7.Location = new Point(0, 343);
         tableLayoutPanel7.Margin = new Padding(0);
         tableLayoutPanel7.Name = "tableLayoutPanel7";
         tableLayoutPanel7.RowCount = 1;
         tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel7.Size = new Size(541, 26);
         tableLayoutPanel7.TabIndex = 7;
         // 
         // SaveButton
         // 
         SaveButton.Dock = DockStyle.Fill;
         SaveButton.Location = new Point(433, 0);
         SaveButton.Margin = new Padding(1, 0, 1, 0);
         SaveButton.Name = "SaveButton";
         SaveButton.Size = new Size(107, 26);
         SaveButton.TabIndex = 0;
         SaveButton.Text = "Add";
         toolTip1.SetToolTip(SaveButton, "Adds the new event_modifer\r\n(Ctrl + S)");
         SaveButton.UseVisualStyleBackColor = true;
         SaveButton.Click += SaveButton_Click;
         // 
         // CancelButton
         // 
         CancelButton.Dock = DockStyle.Fill;
         CancelButton.Location = new Point(1, 0);
         CancelButton.Margin = new Padding(1, 0, 1, 0);
         CancelButton.Name = "CancelButton";
         CancelButton.Size = new Size(106, 26);
         CancelButton.TabIndex = 1;
         CancelButton.Text = "Close";
         toolTip1.SetToolTip(CancelButton, "All progress that has not been added or modified will be lost.\r\n(Ctrl + X)");
         CancelButton.UseVisualStyleBackColor = true;
         CancelButton.Click += CancelButton_Click;
         // 
         // ModifyButton
         // 
         ModifyButton.Dock = DockStyle.Fill;
         ModifyButton.Location = new Point(217, 0);
         ModifyButton.Margin = new Padding(1, 0, 1, 0);
         ModifyButton.Name = "ModifyButton";
         ModifyButton.Size = new Size(106, 26);
         ModifyButton.TabIndex = 2;
         ModifyButton.Text = "Modify";
         toolTip1.SetToolTip(ModifyButton, "Modifies the selected modifier if it already exists\r\n(Ctrl + D)");
         ModifyButton.UseVisualStyleBackColor = true;
         // 
         // EventModifierForm
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(541, 369);
         ControlBox = false;
         Controls.Add(EventModifiersLayoutPanel);
         FormBorderStyle = FormBorderStyle.None;
         Icon = (Icon)resources.GetObject("$this.Icon");
         KeyPreview = true;
         Name = "EventModifierForm";
         StartPosition = FormStartPosition.CenterScreen;
         Text = "EventModifierForm";
         KeyDown += EventModifierForm_KeyDown;
         EventModifiersLayoutPanel.ResumeLayout(false);
         EventModifiersLayoutPanel.PerformLayout();
         ModifiersLP.ResumeLayout(false);
         ModifiersLP.PerformLayout();
         CustomAttributesLP.ResumeLayout(false);
         CustomAttributesLP.PerformLayout();
         tableLayoutPanel3.ResumeLayout(false);
         tableLayoutPanel3.PerformLayout();
         tableLayoutPanel2.ResumeLayout(false);
         tableLayoutPanel2.PerformLayout();
         EventModNameTL.ResumeLayout(false);
         EventModNameTL.PerformLayout();
         tableLayoutPanel7.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel EventModifiersLayoutPanel;
      private TableLayoutPanel EventModNameTL;
      private TableLayoutPanel tableLayoutPanel2;
      private Label label1;
      private Label label2;
      private Label label3;
      private TableLayoutPanel CustomAttributesLP;
      private Label label5;
      private TableLayoutPanel tableLayoutPanel3;
      private Label label4;
      private TableLayoutPanel ModifiersLP;
      private Label label6;
      private ComboBox comboBox1;
      private TableLayoutPanel tableLayoutPanel7;
      private Button SaveButton;
      private Button CancelButton;
      public TextBox DescriptionTextBox;
      public TextBox LocalisationTextBox;
      private ToolTip toolTip1;
      private Button ModifyButton;
   }
}