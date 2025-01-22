namespace Editor.src.Forms.Feature
{
   partial class TradeGoodView
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TradeGoodView));
         tableLayoutPanel1 = new TableLayoutPanel();
         TradeGoodListView = new ListView();
         tableLayoutPanel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 1;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.Controls.Add(TradeGoodListView, 0, 0);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
         tableLayoutPanel1.Size = new Size(423, 513);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // TradeGoodListView
         // 
         TradeGoodListView.Dock = DockStyle.Fill;
         TradeGoodListView.FullRowSelect = true;
         TradeGoodListView.LabelWrap = false;
         TradeGoodListView.Location = new Point(3, 3);
         TradeGoodListView.Name = "TradeGoodListView";
         TradeGoodListView.OwnerDraw = true;
         TradeGoodListView.Size = new Size(417, 477);
         TradeGoodListView.Sorting = SortOrder.Ascending;
         TradeGoodListView.TabIndex = 0;
         TradeGoodListView.UseCompatibleStateImageBehavior = false;
         // 
         // TradeGoodView
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(423, 513);
         Controls.Add(tableLayoutPanel1);
         Icon = (Icon)resources.GetObject("$this.Icon");
         Name = "TradeGoodView";
         Text = "TradeGoodView";
         tableLayoutPanel1.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private ListView TradeGoodListView;
   }
}