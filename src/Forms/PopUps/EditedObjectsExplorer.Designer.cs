namespace Editor.Forms.Feature
{
   partial class EditedObjectsExplorer
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditedObjectsExplorer));
         MainTLP = new TableLayoutPanel();
         RefreshButton = new Button();
         MainTLP.SuspendLayout();
         SuspendLayout();
         // 
         // MainTLP
         // 
         MainTLP.ColumnCount = 1;
         MainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         MainTLP.Controls.Add(RefreshButton, 0, 1);
         MainTLP.Dock = DockStyle.Fill;
         MainTLP.Location = new Point(0, 0);
         MainTLP.Name = "MainTLP";
         MainTLP.RowCount = 2;
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         MainTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         MainTLP.Size = new Size(372, 567);
         MainTLP.TabIndex = 0;
         // 
         // RefreshButton
         // 
         RefreshButton.Location = new Point(3, 540);
         RefreshButton.Name = "RefreshButton";
         RefreshButton.Size = new Size(75, 22);
         RefreshButton.TabIndex = 0;
         RefreshButton.Text = "Refresh";
         RefreshButton.UseVisualStyleBackColor = true;
         RefreshButton.Click += RefreshButton_Click;
         // 
         // EditedObjectsExplorer
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(372, 567);
         Controls.Add(MainTLP);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "EditedObjectsExplorer";
         Text = "EditedObjectsExplorer";
         MainTLP.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel MainTLP;
      private Button RefreshButton;
   }
}