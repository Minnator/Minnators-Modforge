using Editor.ErrorHandling;
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
            AddLogEntry(a);
            if (ErrorView.Items.Count == 1)
               SizeHeadersAndColumns();
         };


         LoadLogType(Globals.Settings.Logging.LoggingVerbosity);
         AddLogEntries(LogManager.ActiveEntries);

         CustomDrawing();
         
         Timer timer = new Timer();
         timer.Interval = 300;
         timer.Tick += (sender, args) =>
         {
            var rand = Globals.Random.Next(0, 4);
            switch (rand)
            {
               case 0:
                  LogManager.Error("This is an error message");
                  break;
               case 1:
                  LogManager.Warn("This is a warning message");
                  break;
               case 2:
                  LogManager.Inform("This is an information message");
                  break;
               case 3:
                  LogManager.Debug("This is a debug message");
                  break;
            }
         };
         timer.Start();

         new ErrorObject("upsi this is a test error hehehe!", ErrorType.FileNotFound);

         var t2 = new Timer();
         t2.Interval = 6000;
         t2.Tick += (sender, args) =>
         {
            timer.Stop();
            t2.Stop();
         };
         t2.Start();
         SizeHeadersAndColumns();
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

      public void AddLogEntry(LogEntry entry)
      {
         var backColor = entry is IExtendedLogInformationProvider ? Color.DarkKhaki : SystemColors.Window;
         
         var item = new ListViewItem((ErrorView.Items.Count + 1).ToString());
         ListViewItem.ListViewSubItem timeItem = new()
         {
            Text = entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
            BackColor = backColor,
         };
         ListViewItem.ListViewSubItem levelItem = new ()
         {
            BackColor = entry.Level.GetAttributeOfType<LogColorAttribute>()!.Color,
            ForeColor = entry.Level.GetAttributeOfType<LogColorAttribute>()!.BlackColor ? Color.Black : Color.White,
            Text = entry.Level.ToString(),
         };
         ListViewItem.ListViewSubItem messageItem = new ()
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
            AddLogEntry(entry);
      }

      private void OnVerbosityChanged(object? sender, EventArgs e)
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
