using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace Editor.src.Controls.EXT
{
   public class EXT_ColorableListView : ListView
   {
      private Color _headerBackColor = SystemColors.ControlLight;
      private bool _fitContentAndHeader = true;
      private int _maxColumnWidth = 200;

      [Category("MMF Appearance")]
      public Color HeaderBackColor
      {
         get => _headerBackColor;
         set => _headerBackColor = value;
      }

      [Category("MMF Behavior")]
      public bool FitContentAndHeader
      {
         get => _fitContentAndHeader;
         set
         {
            _fitContentAndHeader = value;
            if (_fitContentAndHeader)
               ResizeToFitContentAndHeader();
         }
      }

      [Category("MMF Behavior")]
      public int MaxColumnWidth
      {
         get => _maxColumnWidth;
         set => _maxColumnWidth = value;
      }

      private bool _isResizing = false;


      public EXT_ColorableListView()
      {
         OwnerDraw = true;
         FullRowSelect = true;
         View = View.Details;
         base.DoubleBuffered = true;


         DrawColumnHeader += (sender, e) =>
         {
            e.Graphics.FillRectangle(new SolidBrush(SystemColors.ControlLight), e.Bounds);
            e.DrawText();
         };

         DrawItem += (sender, e) =>
         {

         };


         DrawSubItem += (sender, e) =>
         {
            // Draw the background for the subitem
            e.Graphics.FillRectangle(new SolidBrush(e.SubItem?.BackColor ?? SystemColors.Window), e.Bounds);

            // Draw subitem text
            e.Graphics.DrawString(e.SubItem?.Text ?? string.Empty,
                                  e.SubItem?.Font ?? SystemFonts.DefaultFont,
                                  new SolidBrush(e.SubItem?.ForeColor ?? SystemColors.ControlText),
                                  e.Bounds
                                 );

            // Draw focus rectangle for selected state
            if ((e.ItemState & ListViewItemStates.Focused) != 0)
               ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds);
         };

         Resize += (sender, e) =>
         {
            if (_fitContentAndHeader)
               ResizeToFitContentAndHeader();
         };
         if (_fitContentAndHeader)
            ResizeToFitContentAndHeader();
      }

      protected override void OnResize(EventArgs e)
      {
         if (_isResizing)
            return;
         base.OnResize(e);
      }

      public void ResizeToFitContentAndHeader()
      {
         if (_isResizing)
            return;
         _isResizing = true;
         foreach (ColumnHeader column in Columns)
            column.Width = -1;

         foreach (ColumnHeader column in Columns)
         {
            if (column.Width > _maxColumnWidth)
               column.Width = _maxColumnWidth;
            else
               column.Width += 14;
         }
         _isResizing = false;
      }
   }
}