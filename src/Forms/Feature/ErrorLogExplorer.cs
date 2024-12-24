﻿using Editor.ErrorHandling;
using Editor.Forms.PopUps;
using Editor.Helper;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Forms.Feature
{
   public partial class ErrorLogExplorer : Form
   {
      private bool _loading = false;

      public ErrorLogExplorer()
      {
         InitializeComponent();
         ErrorCheckBox.CheckedChanged += OnVerbosityChanged;
         WarningCheckBox.CheckedChanged += OnVerbosityChanged;
         InfoCheckBox.CheckedChanged += OnVerbosityChanged;
         DebugCheckBox.CheckedChanged += OnVerbosityChanged;
         ErrorView.MouseClick += BuildContextMenu;

         ErrorView.DoubleClick += (sender, args) =>
         {
            var convCoords = ErrorView.PointToClient(Cursor.Position);
            var item = ErrorView.GetItemAt(convCoords.X, convCoords.Y);
            if (item == null)
               return;

            var entry = (LogEntry)item.Tag!;

            if (entry is not IExtendedLogInformationProvider provider)
               return;

            var form = new LogEntryViewer(provider);
            form.ShowDialog();
         };
         
         LogManager.LogEntryAdded += (_, a) =>
         {
            AddLogEntry(a, true);
            if (ErrorView.Items.Count == 1)
               SizeHeadersAndColumns();
         };


         LoadLogType(Globals.Settings.Logging.LoggingVerbosity);

         CustomDrawing();
         SizeHeadersAndColumns();
      }

      private void BuildContextMenu(object? sender, MouseEventArgs e)
      {
         // if its not RMB return
         if (e is not { Button: MouseButtons.Right })
            return;

         var convPoint = ErrorView.PointToClient(Cursor.Position);
         var clickedElement = ErrorView.GetItemAt(convPoint.X, convPoint.Y);
         if (clickedElement == null)
            return;

         if (clickedElement.Tag is not FileRefLogEntry entry)
            return;

         var contextMenu = new ContextMenuStrip();
         var openFile = new ToolStripMenuItem("Open File");
         var openFolderOfFile = new ToolStripMenuItem("Open Folder of File");
         var openFolder = new ToolStripMenuItem("Open Folder");

         openFile.Click += (_, args) => ProcessHelper.OpenFile(entry.Path);
         openFolder.Click += (_, args) => ProcessHelper.OpenFolder(entry.Path);
         openFolderOfFile.Click += (_, args) =>
         {
            var fPath = Path.GetDirectoryName(entry.Path);
            if (fPath != null)
               ProcessHelper.OpenFolder(fPath);
         };

         contextMenu.Items.Add(openFile);
         contextMenu.Items.Add(openFolderOfFile);
         contextMenu.Items.Add(openFolder);

         contextMenu.Show(ErrorView, convPoint);
      }

      public void LoadLogType(LogType type)
      {
         _loading = true;
         ErrorCheckBox.Checked = type.HasFlag(LogType.Error);
         ErrorCheckBox.Enabled = ErrorCheckBox.Checked;
         WarningCheckBox.Checked = type.HasFlag(LogType.Warning);
         WarningCheckBox.Enabled = WarningCheckBox.Checked;
         InfoCheckBox.Checked = type.HasFlag(LogType.Information);
         InfoCheckBox.Enabled = InfoCheckBox.Checked;
         DebugCheckBox.Checked = type.HasFlag(LogType.Debug);
         DebugCheckBox.Enabled = DebugCheckBox.Checked;
         _loading = false;
         UpdateListView();
      }

      public void SizeHeadersAndColumns()
      {
         for (var i = 0; i < ErrorView.Columns.Count; i++)
         {
            ErrorView.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
            var headerWidth = ErrorView.Columns[i].Width;
            ErrorView.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
            var contentWidth = ErrorView.Columns[i].Width;
            ErrorView.Columns[i].Width = Math.Max(headerWidth, contentWidth) + 10;
         }
      }

      public void AddLogEntry(LogEntry entry, bool events)
      {
         var backColor = entry is IExtendedLogInformationProvider ? Color.DarkKhaki : SystemColors.Window;

         var item = new ListViewItem((ErrorView.Items.Count + 1).ToString());
         ListViewItem.ListViewSubItem timeItem = new()
         {
            Text = entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
            BackColor = backColor,
         };
         ListViewItem.ListViewSubItem levelItem = new()
         {
            BackColor = entry.Level.GetAttributeOfType<LogColorAttribute>()!.Color,
            ForeColor = entry.Level.GetAttributeOfType<LogColorAttribute>()!.BlackColor ? Color.Black : Color.White,
            Text = entry.Level.ToString(),
         };
         ListViewItem.ListViewSubItem messageItem = new()
         {
            Text = entry.Message,
            BackColor = backColor,
         };
         item.SubItems.Add(timeItem);
         item.SubItems.Add(levelItem);
         item.SubItems.Add(messageItem);
         item.Tag = entry;
         ErrorView.Items.Add(item);
      }

      public void AddLogEntries(ICollection<LogEntry> entries)
      {
         foreach (var entry in entries) 
            AddLogEntry(entry, false);
      }


      private void OnVerbosityChanged(object? sender, EventArgs e)
      {
         UpdateListView();
      }

      private void UpdateListView()
      {
         if (_loading)
            return;
         LogManager.ChangeVerbosity(GetLogType());
         ErrorView.Items.Clear();
         AddLogEntries(LogManager.ActiveEntries);
      }

      public LogType GetLogType()
      {
         var type = LogType.None;
         if (ErrorCheckBox.Checked)
            type |= LogType.Error;
         if (WarningCheckBox.Checked)
            type |= LogType.Warning;
         if (InfoCheckBox.Checked)
            type |= LogType.Information;
         if (DebugCheckBox.Checked)
            type |= LogType.Debug;
         return type;
      }

      private void CustomDrawing()
      {
         // Handle DrawColumnHeader event to draw headers
         ErrorView.DrawColumnHeader += (sender, e) =>
         {
            e.Graphics.FillRectangle(
               new SolidBrush(SystemColors.ControlLight),
               e.Bounds
            );

            e.DrawText();       // Default text
         };

         ErrorView.DrawItem += (sender, e) =>
         {

         };


         ErrorView.DrawSubItem += (sender, e) =>
         {
            // Draw the background for the subitem
            e.Graphics.FillRectangle(
               new SolidBrush(e.SubItem?.BackColor ?? SystemColors.Window),
               e.Bounds
            );

            // Draw subitem text
            using var textBrush = new SolidBrush(e.SubItem?.ForeColor ?? SystemColors.ControlText);
            e.Graphics.DrawString(
               e.SubItem?.Text ?? string.Empty,
               e.SubItem?.Font ?? SystemFonts.DefaultFont,
               textBrush,
               e.Bounds
            );

            // Draw focus rectangle for selected state
            if ((e.ItemState & ListViewItemStates.Focused) != 0)
            {
               ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds);
            }
         };
      }
   }
}
