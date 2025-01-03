using Editor.ErrorHandling;
using Editor.Forms.PopUps;
using Editor.Helper;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Forms.Feature
{
   public partial class ErrorLogExplorer : Form
   {
      public enum SearchType
      {
         TextSearch,
         ErrorType,
         ErrorLevel,
      }

      private bool _loading = false;

      public ErrorLogExplorer()
      {
         InitializeComponent();

#if DEBUG
         ErrorView.Columns.Add("Source");
#endif

         ErrorCheckBox.CheckedChanged += OnVerbosityChanged;
         WarningCheckBox.CheckedChanged += OnVerbosityChanged;
         InfoCheckBox.CheckedChanged += OnVerbosityChanged;
         DebugCheckBox.CheckedChanged += OnVerbosityChanged;
         CriticalCheckbox.CheckedChanged += OnVerbosityChanged;
         IsVanillaEntryCheckBox.CheckedChanged += OnVerbosityChanged;
         ErrorView.MouseClick += BuildContextMenu;
         SearchButton.MouseDown += SearchButton_Click;


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

         SearchTypeBox.DataSource = Enum.GetValues(typeof(SearchType));

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

         if (clickedElement.Tag is not LoadingError entry)
            return;

         var contextMenu = new ContextMenuStrip();
         var openFile = new ToolStripMenuItem("Open file");
         var openFolderOfFile = new ToolStripMenuItem("Open folder of file");
         var openFolder = new ToolStripMenuItem("Open folder");
         var openFileAtLocation = new ToolStripMenuItem("Open File at error location");

         openFile.Click += (_, args) => ProcessHelper.OpenFile(entry.Path);
         openFileAtLocation.Click += (_, args) =>
         {
            if (entry is { } lError) 
               ProcessHelper.OpenFileAtLine(lError.Path, lError.Line - 1, lError.CharPos);
         };
         openFolder.Click += (_, args) => ProcessHelper.OpenFolder(entry.Path);
         openFolderOfFile.Click += (_, args) =>
         {
            var fPath = Path.GetDirectoryName(entry.Path);
            if (fPath != null)
               ProcessHelper.OpenFolder(fPath);
         };

         contextMenu.Items.Add(openFile);
         contextMenu.Items.Add(openFileAtLocation);
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
         CriticalCheckbox.Checked = type.HasFlag(LogType.Critical);
         CriticalCheckbox.Enabled = CriticalCheckbox.Checked;
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
         var backColor = entry is IExtendedLogInformationProvider ? Color.Silver : Color.Gray;

         var item = new ListViewItem((ErrorView.Items.Count + 1).ToString()){BackColor = backColor};
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
#if DEBUG
         var sourceItem = new ListViewItem.ListViewSubItem
         {
            Text = entry.DebugInformation,
            BackColor = Color.CadetBlue,
         };
         item.SubItems.Add(sourceItem);
#endif
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
         if (IsVanillaEntryCheckBox.Checked)
            AddLogEntries(LogManager.ActiveEntries);
         else
            AddLogEntries(LogManager.ActiveEntries.Where(x => x.IsVanilla == false).ToList());
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
         if (CriticalCheckbox.Checked)
            type |= LogType.Critical;
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

      private void ErrorLogExplorer_KeyPress(object sender, KeyPressEventArgs e)
      {
         // if Ctrl + F is pressed focus SearchTextBox
         if (e.KeyChar == (char)6)
         {
            SearchTextBox.Focus();
            SearchTextBox.SelectAll();
            e.Handled = true;
         }
      }

      private void SearchTextBox_KeyPress(object sender, KeyPressEventArgs e)
      {
         // if Esc is pressed clear SearchTextBox 
         if (e.KeyChar == (char)27)
         {
            SearchTextBox.Clear();
            SearchTypeBox.Focus();
            e.Handled = true;
         }
         // Search if Enter is pressed
         else if (e.KeyChar == (char)13)
         {
            SearchFor((SearchType?)SearchTypeBox.SelectedItem, SearchTextBox.Text);
            e.Handled = true;
         }
      }

      private void SearchButton_Click(object? sender, MouseEventArgs e)
      {
         // if RMB clear all previous search results
         if (e.Button == MouseButtons.Right)
         {
            SearchTextBox.Clear();
            ClearSearchResults();
            return;
         }

         SearchFor((SearchType?)SearchTypeBox.SelectedItem, SearchTextBox.Text);
      }

      private void ClearSearchResults()
      {
         ErrorView.Items.Clear();
         LoadLogType(Globals.Settings.Logging.LoggingVerbosity);
      }

      private void SearchFor(SearchType? searchType, string searchString)
      {
         if (searchType == null || string.IsNullOrWhiteSpace(searchString))
            return;

         var entries = SearchSource.Checked ? LogManager.GetAlLogEntries : LogManager.ActiveEntries;

         switch (searchType.Value)
         {
            case SearchType.ErrorType:
               entries = entries.Where(e => e is ErrorObject er && er.ErrorType.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
               break;
            case SearchType.ErrorLevel:
               entries = entries.Where(e => e.Level.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
               break;
            case SearchType.TextSearch:
               entries = entries.Where(e => e.Message.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
               break;
         }

         ErrorView.Items.Clear();
         AddLogEntries(entries);
      }

      private void SaveLogsButton_Click(object sender, EventArgs e)
      {
          LogManager.SaveLogAsCsv();
          LogManager.SaveLogToFile();
      }
   }
}
