using System.Diagnostics;
using System.Drawing;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Parser;
using static System.Net.Mime.MediaTypeNames;
using Font = System.Drawing.Font;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Controls
{
   public class DateTextBox : TextBox
   {
      private object _dateLockObject = new();
      private Timer timer = new()
      {
         Interval = Globals.Settings.Gui.TextBoxCommandCreationInterval / 2,
      };
      private Timer _quickTimer = new()
      {
         Interval = Globals.Settings.Gui.TextBoxCommandCreationInterval / 10,
      };

      public DateTextBox()
      {
         PreviewKeyDown += SuppressTab_PreviewKeyDown;
         timer.Tick += (_, _) => ConfirmDate();
         _quickTimer.Tick += (_, _) => ConfirmDate();
      }


      private void IncreaseByOne(int numOfField, bool increase)
      {
         switch (numOfField)
         {
            case 0: // Year field
               IncreaseYear(increase);
               break;
            case 1: // Month field
               IncreaseMonth(increase);
               break;
            case 2: // Day field
               IncreaseDay(increase);
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(numOfField), "Invalid field number");
         }
      }
      
      private void IncreaseDay(bool increase = true)
      {
         var (_, day, lowerBound, upperBound) = GetFieldContent(2);
         var (_, month, _, _) = GetFieldContent(1);

         day = increase ? day + 1 : day - 1;

         if (day > Date.DaysInMonth(month))
         {
            IncreaseMonth();
            day = 1;
         }
         else if (day < 1)
         {
            day = Date.DaysInMonth(month);
            IncreaseMonth(false);
         }

         (_, _, lowerBound, upperBound) = GetFieldContent(2);
         Text = Text.Remove(lowerBound).Insert(lowerBound, day++.ToString());
         (_, _, lowerBound, upperBound) = GetFieldContent(2);
         SelectionStart = lowerBound;
         SelectionLength = upperBound - lowerBound;
      }

      private void IncreaseMonth(bool increase = true)
      {
         var (_, month, lowerBound, upperBound) = GetFieldContent(1);
         var (_, day, lowerDay, upperDay) = GetFieldContent(2);

         month = increase ? month + 1 : month - 1;

         if (month > 12)
         {
            month = 1;
            IncreaseYear();
         }
         else if (month < 1)
         {
            month = 12;
            IncreaseYear(false);
         }
         var daysInMonth = Date.DaysInMonth(month);
         if (day > daysInMonth)
         {
            day = daysInMonth;
            Text = Text.Remove(lowerDay, upperDay - lowerDay).Insert(lowerDay, day.ToString());
         }
         Text = Text.Remove(lowerBound, upperBound - lowerBound).Insert(lowerBound, month.ToString());
         (_, _, lowerBound, upperBound) = GetFieldContent(1);

         SelectionStart = lowerBound;
         SelectionLength = upperBound - lowerBound;
      }

      private void IncreaseYear(bool increase = true)
      {
         var (_, year, start, end) = GetFieldContent(0);
         year = increase ? year + 1 : year - 1;
         year = Math.Min(9999, Math.Max(2, year));
         Text = Text.Remove(start, end - start).Insert(start, year.ToString());
         (_, _, start, end) = GetFieldContent(0);

         SelectionStart = start;
         SelectionLength = end - start;
      }

      private (int fieldPos, int content, int start, int end) GetFieldContent(int fieldPos)
      {
         var lower = Text.IndexOf('.');
         var upper = Text.LastIndexOf('.');

         switch (fieldPos)
         {
            case 0:
               var content = Text[..lower].Trim();
               Debug.Assert(int.TryParse(content, out _) || !string.IsNullOrEmpty(content), "Year is not in 'int' format!");
               if (string.IsNullOrEmpty(content))
                  content = "2";
               return (fieldPos, int.Parse(content), 0, lower);
            case 1:
               content = Text[(lower + 1)..(upper)].Trim();
               Debug.Assert(int.TryParse(content, out _) || !string.IsNullOrEmpty(content), "Month is not in 'int' format!");
               if (string.IsNullOrEmpty(content))
                  content = "1";
               return (fieldPos, int.Parse(content), lower + 1, upper);
            case 2:
               content = Text[(upper + 1)..].Trim();
               Debug.Assert(int.TryParse(content, out _) || !string.IsNullOrEmpty(content), "Day is not in 'int' format!");
               if (string.IsNullOrEmpty(content))
                  content = "1";
               return (fieldPos, int.Parse(content), upper + 1, Text.Length);
            default:
               throw new ArgumentOutOfRangeException(nameof(fieldPos), "Invalid field number");
         }
      }

      private static (int fieldPos, int content, int start, int end) GetFieldContent(int fieldPos, ref string text)
      {
         var lower = text.IndexOf('.');
         var upper = text.LastIndexOf('.');

         switch (fieldPos)
         {
            case 0:
               var content = text[..lower].Trim();
               Debug.Assert(int.TryParse(content, out _), "Year is not in 'int' format!");
               return (fieldPos, int.Parse(content), 0, lower);
            case 1:
               content = text[(lower + 1)..(upper)].Trim();
               Debug.Assert(int.TryParse(content, out _), "Month is not in 'int' format!");
               return (fieldPos, int.Parse(content), lower + 1, upper);
            case 2:
               content = text[(upper + 1)..].Trim();
               Debug.Assert(int.TryParse(content, out _), "Day is not in 'int' format!");
               return (fieldPos, int.Parse(content), upper + 1, text.Length);
            default:
               throw new ArgumentOutOfRangeException(nameof(fieldPos), "Invalid field number");
         }
      }

      private int GetFieldNumber(int pos)
      {
         var lower = Text.IndexOf('.');
         var upper = Text.LastIndexOf('.');

         var fieldPos = 0;
         if (lower <= pos)
            fieldPos++;
         if (upper <= pos)
            fieldPos++;

         return fieldPos;
      }

      private int GetFieldNumber(int pos, string text)
      {
         var lower = text.IndexOf('.');
         var upper = text.LastIndexOf('.');

         var fieldPos = 0;
         if (lower <= pos)
            fieldPos++;
         if (upper <= pos)
            fieldPos++;

         return fieldPos;
      }

      private void ConfirmDate()
      {
         if (Date.TryParse(Text, out var date).Ignore())
         {
            ProvinceHistoryManager.LoadDate(date);
            timer.Stop();
         }
         else
         {
            Text = Globals.StartDate.ToString();
         }
      }

      private void EnforceValueBounds(KeyEventArgs e)
      {
         var newString = Text.Remove(SelectionStart, SelectionLength).Insert(SelectionStart, ((char)e.KeyValue).ToString());
         var fieldPos = GetFieldNumber(SelectionStart, newString);
         var (_, content, start, end) = GetFieldContent(fieldPos, ref newString);

         switch (fieldPos)
         {
            case 0:
               content = Math.Min(9999, Math.Max(2, content));
               break;
            case 1:
               content = Math.Min(12, Math.Max(1, content));
               break;
            case 2:
               content = Math.Min(Date.DaysInMonth(content), Math.Max(1, content));
               break;
         }
         Text = newString.Remove(start, end - start).Insert(start, content.ToString());
         (_, _, _, end) = GetFieldContent(fieldPos);
         SelectionStart = end;
         SelectionLength = 0;

         e.SuppressKeyPress = true;
         e.Handled = true;
      }

      protected override void OnMouseWheel(MouseEventArgs e)
      {
         if (e.Delta > 0)
         {
            var fieldPos = GetFieldNumber(SelectionStart);
            IncreaseByOne(fieldPos, true);
         }
         else if (e.Delta < 0)
         {
            var fieldPos = GetFieldNumber(SelectionStart);
            IncreaseByOne(fieldPos, false);
         }

         if (Globals.Settings.Rendering.Map.NoDelayMapUpdate)
         {
            ConfirmDate();
            _quickTimer.Stop();
            return;
         }

         _quickTimer.Stop();
         _quickTimer.Start();
      }

      protected override void OnKeyDown(KeyEventArgs e)
      {  
         if (e.KeyCode == Keys.Enter)
         {
            ConfirmDate();

            e.SuppressKeyPress = true;
            e.Handled = true;

            timer.Stop();
         }

         if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
         {
            if (SelectionLength == 0)
               return;

            var selectedText = Text[SelectionStart..SelectionLength];
            var numOfPoints = selectedText.Count(c => c == '.');
            if (numOfPoints > 0)
            {
               Text = Text.Remove(SelectionStart, SelectionLength);
               Text = Text.Insert(SelectionStart, new string('.', numOfPoints));
               SelectionStart = SelectionStart++;
            }
         }

         if (e.KeyCode == Keys.Up)
         {
            var fieldPos = GetFieldNumber(SelectionStart);

            IncreaseByOne(fieldPos, true);
            
            e.SuppressKeyPress = true;
            e.Handled = true;

            timer.Stop();
            timer.Start();
            return;
         }
         if (e.KeyCode == Keys.Down)
         {
            var fieldPos = GetFieldNumber(SelectionStart);

            IncreaseByOne(fieldPos, false);

            e.SuppressKeyPress = true;
            e.Handled = true;
            timer.Stop();
            timer.Start();
            return;
         }

         if (e.KeyCode is Keys.Tab or Keys.OemPeriod)
         {
            e.SuppressKeyPress = true;

            var pos = SelectionStart;

            if (e.Shift)
            {
               // move to previous field
               var prevDot = Text.LastIndexOf('.', pos - 2 >= 0 ? pos - 2 : 0);
               var start = prevDot == -1 ? 0 : prevDot + 1;
               var end = Text.IndexOf('.', start);
               var length = end == -1 ? Text.Length - start : end - start;

               SelectionStart = start;
               SelectionLength = length;
            }
            else
            {
               // move to next field
               var nextDot = Text.IndexOf('.', pos);
               if (nextDot == -1) return;

               var nextFieldStart = nextDot + 1;
               var nextDotEnd = Text.IndexOf('.', nextFieldStart);
               var nextFieldLength = nextDotEnd == -1 ? Text.Length - nextFieldStart : nextDotEnd - nextFieldStart;

               SelectionStart = nextFieldStart;
               SelectionLength = nextFieldLength;
            }
            timer.Stop();
            timer.Start();

            e.Handled = true;
            return;
         }

         if (!char.IsDigit((char)e.KeyValue) && e.KeyCode != Keys.Tab && e.KeyCode != Keys.OemPeriod)
         {
            e.SuppressKeyPress = true;
            e.Handled = true;
            return;
         }

         // Key isDigit
         EnforceValueBounds(e);
      }

      protected override void OnGotFocus(EventArgs e)
      {
         base.OnGotFocus(e);
         // Select the first field (before the first dot)
         BeginInvoke(() =>
         {
            var end = Text.IndexOf('.');
            if (end == -1) end = Text.Length;
            SelectionStart = 0;
            SelectionLength = end;

            timer.Start();
         });
      }

      protected override void WndProc(ref Message m)
      {
         const int wmPaste = 0x0302;

         if (m.Msg == wmPaste)
         {
            if (Clipboard.ContainsText() && Parsing.TryParseDate(Clipboard.GetText(), out _)) 
               base.WndProc(ref m);
            return; 
         }

         base.WndProc(ref m);
      }

      private void SuppressTab_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
      {
         if (e.KeyCode == Keys.Tab)
            e.IsInputKey = true;
      }
   }

   public sealed class ArrowButton : Button
   {
      public enum ArrowOrientation
      {
         Up,
         Down,
         Left,
         Right
      }

      public readonly bool HasBorder;
      public bool FilledArrow { get; set; } = false;
      public ArrowOrientation Orientation { get; set; } = ArrowOrientation.Right;
      public Color BorderColor { get; set; } = Color.Black;
      // Renders either an up, down, left or right arrow depending on the orientation enum
      
      public ArrowButton(ArrowOrientation orientation, bool hasBorder = true)
      {
         HasBorder = hasBorder;
         Orientation = orientation;
         Dock = DockStyle.Fill;
         Margin = new(1);
         Padding = new(0);
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         var g = e.Graphics;
         g.Clear(BackColor);
         g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
         var isHovered = ClientRectangle.Contains(PointToClient(Cursor.Position));
         var arrowColor = isHovered ? Color.DeepSkyBlue : ForeColor;

         var rect = ClientRectangle;
         using var brush = new SolidBrush(arrowColor);

         if (HasBorder)
            ControlPaint.DrawBorder(g, rect, BorderColor, ButtonBorderStyle.Solid);

         var center = new Point(rect.Width / 2, rect.Height / 2);
         var size = Math.Min(rect.Width, rect.Height) / 3;

         Point[] points = Orientation switch
         {
            ArrowOrientation.Up =>
               [new(center.X - size, center.Y + size), center with { Y = center.Y - size }, new(center.X + size, center.Y + size)],
            ArrowOrientation.Down =>
               [new(center.X - size, center.Y - size), center with { Y = center.Y + size }, new(center.X + size, center.Y - size)],
            ArrowOrientation.Left =>
               [new(center.X + size, center.Y + size), center with { X = center.X - size }, new(center.X + size, center.Y - size)],
            ArrowOrientation.Right =>
               [new(center.X - size, center.Y + size), center with { X = center.X + size }, new(center.X - size, center.Y - size)],
            _ => throw new ArgumentOutOfRangeException(nameof(Orientation), "Invalid arrow orientation")
         };
         if (FilledArrow)
            g.FillPolygon(brush, points);
         else
         {
            using var pen = new Pen(arrowColor);
            g.DrawLines(pen, points);
         }
      }

      protected override void OnMouseEnter(EventArgs e)
      {
         base.OnMouseEnter(e);
         Invalidate();
      }

      protected override void OnMouseLeave(EventArgs e)
      {
         base.OnMouseLeave(e);
         Invalidate();
      }
   }

   public sealed class DateControl : Control
   {
      private TableLayoutPanel _tableLayoutPanel;
      public static event EventHandler<Date> OnDateChanged = delegate { };

      private readonly DateTextBox _dateTextBox = new ()
      {
         TextAlign = HorizontalAlignment.Center,
         Margin = new(0,2,0,0),
         Padding = new(0),
         BorderStyle = BorderStyle.None,
         Size = new(78, 25),
         Anchor = AnchorStyles.None,
         // monospaced font
         Font = new Font("Segoe Ui", 11),
         BackColor = DefaultBackColor,
      };
      
      private readonly ArrowButton _yearDecreaseButton = new (ArrowButton.ArrowOrientation.Left, false);
      private readonly ArrowButton _monthDecreaseButton = new (ArrowButton.ArrowOrientation.Left, false);
      private readonly ArrowButton _dayDecreaseButton = new (ArrowButton.ArrowOrientation.Left, false);

      private readonly ArrowButton _yearIncreaseButton = new (ArrowButton.ArrowOrientation.Right, false);
      private readonly ArrowButton _monthIncreaseButton = new (ArrowButton.ArrowOrientation.Right, false);
      private readonly ArrowButton _dayIncreaseButton = new (ArrowButton.ArrowOrientation.Right, false);

      private readonly Timer _inputTimer = new()
      {
         Interval = Globals.Settings.Gui.TextBoxCommandCreationInterval,
         Enabled = false,
      };
      
      public DateControl(Date date)
      {
         Date = date;

         InitHorizontal();

         _dateTextBox.TextChanged += OnDateTextChanged;

         _yearIncreaseButton.Click += OnYearIncrease;
         _monthIncreaseButton.Click += OnMonthIncrease;
         _dayIncreaseButton.Click += OnDayIncrease;
         _yearDecreaseButton.Click += OnYearDecrease;
         _monthDecreaseButton.Click += OnMonthDecrease;
         _dayDecreaseButton.Click += OnDayDecrease;

         Date.OnDateChanged += (_, d) =>
         {
            SetDate(d);
         };
         
         Margin = new(0);
         Padding = new(1);
         Dock = DockStyle.Fill;
         Controls.Add(_tableLayoutPanel);
      }


      private void InitHorizontal()
      {
         _tableLayoutPanel = new()
         {
            RowCount = 1,
            ColumnCount = 9,
            RowStyles =
            {
               new(SizeType.Absolute, 25),
            },
            ColumnStyles =
            {
               new(SizeType.Percent, 50),
               new(SizeType.Absolute, 12),
               new(SizeType.Absolute, 12),
               new(SizeType.Absolute, 12),
               new(SizeType.Absolute, 80), //Date
               new(SizeType.Absolute, 12),
               new(SizeType.Absolute, 12),
               new(SizeType.Absolute, 12),
               new(SizeType.Percent, 50),
            },
            Dock = DockStyle.Fill,
            Padding = new(0),
            Margin = new(0),
         };
         
         _tableLayoutPanel.Controls.Add(_dayDecreaseButton, 1, 0);
         _tableLayoutPanel.Controls.Add(_monthDecreaseButton, 2, 0);
         _tableLayoutPanel.Controls.Add(_yearDecreaseButton, 3, 0);

         _tableLayoutPanel.Controls.Add(_dateTextBox, 4, 0);

         _tableLayoutPanel.Controls.Add(_yearIncreaseButton, 5, 0);
         _tableLayoutPanel.Controls.Add(_monthIncreaseButton, 6, 0);
         _tableLayoutPanel.Controls.Add(_dayIncreaseButton, 7, 0);
      }

      private Date _date = Date.MinValue;
      public Date Date
      {
         get => _date;
         set
         {
            if (_date == value)
               return;
            _date = value;
            _dateTextBox.Text = _date;
         }
      } 

      public void SetDate(Date date)
      {
         Date = date;
         _dateTextBox.Text = Date.ToString();
      }

      private void OnDateTextChanged(object? sender, EventArgs e)
      {
         _inputTimer.Stop();
         _inputTimer.Start();
      }
      public void OnYearIncrease (object? sender, EventArgs e)
      {
         switch (ModifierKeys)
         {
            case Keys.Control:
               Date.AddYears(10);
               break;
            case Keys.Shift:
               Date.AddYears(5);
               break;
            default:
               Date.AddYears(1);
               break;
         }

         _dateTextBox.Text = Date.ToString();
         ProvinceHistoryManager.LoadDate(Date);
      }

      public void OnMonthIncrease (object? sender, EventArgs e)
      {
         switch (ModifierKeys)
         {
            case Keys.Control:
               Date.AddMonths(10);
               break;
            case Keys.Shift:
               Date.AddMonths(5);
               break;
            default:
               Date.AddMonths(1);
               break;
         }

         _dateTextBox.Text = Date.ToString();
         ProvinceHistoryManager.LoadDate(Date);
      }

      public void OnDayIncrease (object? sender, EventArgs e)
      {

         switch (ModifierKeys)
         {
            case Keys.Control:
               Date.AddDays(10);
               break;
            case Keys.Shift:
               Date.AddDays(5);
               break;
            default:
               Date.AddDays(1);
               break;
         }

         _dateTextBox.Text = Date.ToString();
      }

      public void OnYearDecrease (object? sender, EventArgs e)
      {
         switch (ModifierKeys)
         {
            case Keys.Control:
               Date.AddYears(-10);
               break;
            case Keys.Shift:
               Date.AddYears(-5);
               break;
            default:
               Date.AddYears(-1);
               break;
         }
         _dateTextBox.Text = Date.ToString();
         ProvinceHistoryManager.LoadDate(Date);
      }

      public void OnMonthDecrease (object? sender, EventArgs e)
      {
         switch (ModifierKeys)
         {
            case Keys.Control:
               Date.AddMonths(-10);
               break;
            case Keys.Shift:
               Date.AddMonths(-5);
               break;
            default:
               Date.AddMonths(-1);
               break;
         }
         _dateTextBox.Text = Date.ToString();
         ProvinceHistoryManager.LoadDate(Date);
      }

      public void OnDayDecrease (object? sender, EventArgs e)
      {
         switch (ModifierKeys)
         {
            case Keys.Control:
               Date.AddDays(-10);
               break;
            case Keys.Shift:
               Date.AddDays(-5);
               break;
            default:
               Date.AddDays(-1);
               break;
         }
         _dateTextBox.Text = Date.ToString();
         ProvinceHistoryManager.LoadDate(Date);
      }

   }

}