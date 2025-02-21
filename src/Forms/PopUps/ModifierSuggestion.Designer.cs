namespace Editor.Forms.Feature
{
   partial class ModifierSuggestion
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModifierSuggestion));
         tableLayoutPanel1 = new TableLayoutPanel();
         comboBox1 = new ComboBox();
         label1 = new Label();
         button1 = new Button();
         label2 = new Label();
         ValueIdeaBox = new ComboBox();
         ModNameBox = new ComboBox();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 1;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Controls.Add(comboBox1, 0, 2);
         tableLayoutPanel1.Controls.Add(label1, 0, 1);
         tableLayoutPanel1.Controls.Add(button1, 0, 5);
         tableLayoutPanel1.Controls.Add(label2, 0, 3);
         tableLayoutPanel1.Controls.Add(ValueIdeaBox, 0, 4);
         tableLayoutPanel1.Controls.Add(ModNameBox, 0, 0);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 6;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Size = new Size(252, 166);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // comboBox1
         // 
         comboBox1.Dock = DockStyle.Fill;
         comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
         comboBox1.Enabled = false;
         comboBox1.FormattingEnabled = true;
         comboBox1.Location = new Point(3, 56);
         comboBox1.Name = "comboBox1";
         comboBox1.Size = new Size(246, 23);
         comboBox1.TabIndex = 1;
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Dock = DockStyle.Fill;
         label1.Location = new Point(3, 30);
         label1.Name = "label1";
         label1.Size = new Size(246, 23);
         label1.TabIndex = 3;
         label1.Text = "Modifier Category";
         label1.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // button1
         // 
         button1.Dock = DockStyle.Fill;
         button1.Location = new Point(3, 139);
         button1.Name = "button1";
         button1.Size = new Size(246, 24);
         button1.TabIndex = 0;
         button1.Text = "Next random modifier";
         button1.UseVisualStyleBackColor = true;
         button1.Click += button1_Click;
         // 
         // label2
         // 
         label2.AutoSize = true;
         label2.Dock = DockStyle.Fill;
         label2.Location = new Point(3, 83);
         label2.Name = "label2";
         label2.Size = new Size(246, 23);
         label2.TabIndex = 4;
         label2.Text = "Value";
         label2.TextAlign = ContentAlignment.MiddleCenter;
         // 
         // ValueIdeaBox
         // 
         ValueIdeaBox.Dock = DockStyle.Fill;
         ValueIdeaBox.FormattingEnabled = true;
         ValueIdeaBox.Location = new Point(3, 109);
         ValueIdeaBox.Name = "ValueIdeaBox";
         ValueIdeaBox.Size = new Size(246, 23);
         ValueIdeaBox.TabIndex = 6;
         // 
         // ModNameBox
         // 
         ModNameBox.Dock = DockStyle.Fill;
         ModNameBox.FormattingEnabled = true;
         ModNameBox.Location = new Point(3, 3);
         ModNameBox.Name = "ModNameBox";
         ModNameBox.Size = new Size(246, 23);
         ModNameBox.TabIndex = 7;
         // 
         // ModifierSuggestion
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(252, 166);
         Controls.Add(tableLayoutPanel1);
         FormBorderStyle = FormBorderStyle.FixedSingle;
         Icon = (Icon)resources.GetObject("$this.Icon");
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "ModifierSuggestion";
         StartPosition = FormStartPosition.CenterScreen;
         Text = "Random Modifier Form";
         FormClosing += ModifierSuggestion_FormClosing;
         tableLayoutPanel1.ResumeLayout(false);
         tableLayoutPanel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private ComboBox comboBox1;
      private Label label1;
      private Button button1;
      private Label label2;
      private ComboBox ValueIdeaBox;
      private ComboBox ModNameBox;
   }
}