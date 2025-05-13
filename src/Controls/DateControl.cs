using System.Diagnostics;
using Editor.DataClasses.Misc;
using Editor.Parser;
using Editor.Saving;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Controls
{
   public class DateTextBox : TextBox
   {
      public DateTextBox()
      {
         PreviewKeyDown += SuppressTab_PreviewKeyDown;
      }

      private bool IsValidInput(int numOfField, string input, string? month = null)
      {
         switch (numOfField)
         {
            case 0: // Year field, must be between 1 and 9999
               if (int.TryParse(input, out var year))
                  return year is >= 1 and <= 9999;
               break;
            case 1: // Month field, must be between 1 and 12
               if (int.TryParse(input, out var intMonth))
                  return intMonth is >= 1 and <= 12;
               break;
            case 2: // Day field, must be between 1 and the Date.DaysInMonth
               if (int.TryParse(input, out var intDay))
               {
                  Debug.Assert(month != null && int.TryParse(month, out _), "month != null && int.TryParse(month, out _)");
                  return intDay >= 1 && intDay <= Date.DaysInMonth(int.Parse(month));
               }
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(numOfField), "Invalid field number");
         }
         return true;
      }

      private int IncreaseByOne(int numOfField, string current, string? month = null)
      {
         switch (numOfField)
         {
            case 0: // Year field
               if (int.TryParse(current, out var year))
                  return Math.Min(9999, year + 1);
               break;
            case 1: // Month field
               if (int.TryParse(current, out var intMonth))
                  return Math.Min(intMonth + 1, 12);
               break;
            case 2: // Day field
               Debug.Assert(month != null && int.TryParse(month, out _), "month != null && int.TryParse(month, out _)");
               if (int.TryParse(current, out var intDay))
                  return intDay + 1 > Date.DaysInMonth(int.Parse(month)) ? intDay : intDay + 1;
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(numOfField), "Invalid field number");
         }
         return -1;
      }

      private int DecreaseByOne(int numOfField, string current, string? month = null)
      {
         switch (numOfField)
         {
            case 0: // Year field
               if (int.TryParse(current, out var year))
                  return Math.Max(1, year - 1);
               break;
            case 1: // Month field
               if (int.TryParse(current, out var intMonth))
                  return Math.Max(intMonth - 1, 1);
               break;
            case 2: // Day field
               Debug.Assert(month != null && int.TryParse(month, out _), "month != null && int.TryParse(month, out _)");
               if (int.TryParse(current, out var intDay))
                  return intDay - 1 < 1 ? intDay : intDay - 1;
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(numOfField), "Invalid field number");
         }
         return -1;
      }

      private (int fieldPos, string content, int start, int end) GetFieldNumber(int pos)
      {
         var upperFieldBound = Text.IndexOf('.', pos);
         upperFieldBound = upperFieldBound == -1 ? Text.Length : upperFieldBound;
         var lowerFieldBound = Text.LastIndexOf('.', SelectionStart) + 1;
         var content = Text[lowerFieldBound.. upperFieldBound].Trim();

         var fieldPos = 0;
         for (var i = 0; i < upperFieldBound; i++)
            if (Text[i] == '.')
               fieldPos++;

         return (fieldPos, content, lowerFieldBound, upperFieldBound);
      }

      private string GetMonth(int start)
      {
         var upperFieldBound = Text.LastIndexOf('.', start);
         var lowerFieldBound = Text.LastIndexOf('.', upperFieldBound - 1) + 1;
         var content = Text[(lowerFieldBound).. (upperFieldBound == -1 ? Text.Length : upperFieldBound)].Trim();
         return content;
      }

      private (int day, int dayLower) GetDay()
      {
         var lowerBound = Text.LastIndexOf('.', Text.Length - 1) + 1;
         var content = Text[(lowerBound)..].Trim();
         if (int.TryParse(content, out var day))
            return (day, lowerBound);
         throw new EvilActions("Only numbers should ever be in this field!");
      }

      private (int day, int dayLower) UpdateDay(int month)
      {
         var (day, lower) = GetDay();
         day = day > Date.DaysInMonth(month) ? Date.DaysInMonth(month) : day;
         return (day, lower);
      }

      protected override void OnKeyDown(KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Up)
         {
            var (fieldPos, content, start, end) = GetFieldNumber(SelectionStart);

            if (fieldPos == 0)
            {
               Text = Text.Remove(start, end - start).Insert(start, IncreaseByOne(fieldPos, content).ToString());
            }
            else if (fieldPos == 1) // Year, Month
            {
               var newMonth = IncreaseByOne(fieldPos, content);
               Text = Text.Remove(start, end - start).Insert(start, newMonth.ToString());
               var (newDay, lower) = UpdateDay(newMonth);
               Text = Text.Remove(lower).Insert(lower, newDay.ToString());
            }
            else // day
            {
               var month = GetMonth(SelectionStart);
               Text = Text.Remove(start, end - start).Insert(start, IncreaseByOne(fieldPos, content, month).ToString());
            }
            SelectionStart = start;
            SelectionLength = end - start;

            e.SuppressKeyPress = true;
            e.Handled = true;
            return;
         }
         if (e.KeyCode == Keys.Down)
         {
            // Down arrow pressed
            e.SuppressKeyPress = true;
            e.Handled = true;
            return;
         }


         if (!char.IsDigit((char)e.KeyValue) && e.KeyCode != Keys.Tab && e.KeyCode != Keys.OemPeriod)
         {
            e.Handled = true;
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
         }
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


   public enum DateControlLayout
   {
      Horizontal,
      Vertical
   }
   public class DateControl : Control
   {
      private TableLayoutPanel _tableLayoutPanel= new ();
      public static event EventHandler<Date> OnDateChanged = delegate { };

      private readonly DateTextBox _dateTextBox = new ()
      {
         Dock = DockStyle.Fill,
         TextAlign = HorizontalAlignment.Center,
         Margin = new(0,1,0,0),
         Padding = new(0),
      };
      
      private readonly DecreaseButton _yearDecreaseButton = new ();
      private readonly DecreaseButton _monthDecreaseButton = new ();
      private readonly DecreaseButton _dayDecreaseButton = new ();

      private readonly IncreaseButton _yearIncreaseButton = new ();
      private readonly IncreaseButton _monthIncreaseButton = new ();
      private readonly IncreaseButton _dayIncreaseButton = new ();

      private readonly Timer _inputTimer = new()
      {
         Interval = Globals.Settings.Gui.TextBoxCommandCreationInterval,
         Enabled = false,
      };

      public DateControl(Date date)
      {
         Date = date;
         Date = Date.MinValue;
         DateControlLayout = DateControlLayout.Horizontal;
         InitHorizontal();
      }

      public DateControl(Date date, DateControlLayout layout) : this(date)
      {
         DateControlLayout = layout;

         if (DateControlLayout == DateControlLayout.Vertical) 
            InitVertical();
         else
            InitHorizontal();

         _dateTextBox.TextChanged += OnDateTextChanged;

         _yearIncreaseButton.Click += OnYearIncrease;
         _monthIncreaseButton.Click += OnMonthIncrease;
         _dayIncreaseButton.Click += OnDayIncrease;
         _yearDecreaseButton.Click += OnYearDecrease;
         _monthDecreaseButton.Click += OnMonthDecrease;
         _dayDecreaseButton.Click += OnDayDecrease;

         Date.OnDateChanged += (sender, d) =>
         {
            SetDate(d);
         };
         
         Size = _tableLayoutPanel.Size;
         Margin = new(0);
         Padding = new(0);
         _tableLayoutPanel.Margin = new(0, 1, 0, 0);
         Controls.Add(_tableLayoutPanel);
      }


      private void InitVertical()
      {
         _tableLayoutPanel = new()
         {
            RowStyles =
            {
               new(SizeType.Absolute, 20),
               new(SizeType.Absolute, 27),
               new(SizeType.Absolute, 20),
            },
            ColumnStyles =
            {
               new(SizeType.Absolute, 40),
               new(SizeType.Absolute, 20),
               new(SizeType.Absolute, 20),
            },
            Size = new(100, 67),
            Padding = new(0),
            Margin = new(0),
         };

         _tableLayoutPanel.Controls.Add(_dateTextBox, 0, 1);
         _tableLayoutPanel.SetColumnSpan(_dateTextBox, 3);

         _tableLayoutPanel.Controls.Add(_yearIncreaseButton, 0, 0);
         _tableLayoutPanel.Controls.Add(_monthIncreaseButton, 1, 0);
         _tableLayoutPanel.Controls.Add(_dayIncreaseButton, 2, 0);

         _tableLayoutPanel.Controls.Add(_yearDecreaseButton, 0, 2);
         _tableLayoutPanel.Controls.Add(_monthDecreaseButton, 1, 2);
         _tableLayoutPanel.Controls.Add(_dayDecreaseButton, 2, 2);
      }

      private void InitHorizontal()
      {
         _tableLayoutPanel = new()
         {
            RowStyles =
            {
               new(SizeType.Absolute, 25),
            },
            ColumnStyles =
            {
               new(SizeType.Absolute, 20),
               new(SizeType.Absolute, 20),
               new(SizeType.Absolute, 20),
               new(SizeType.Absolute, 90), //Date
               new(SizeType.Absolute, 20),
               new(SizeType.Absolute, 20),
               new(SizeType.Absolute, 20),
            },
            Size = new(210, 25),
            Padding = new(0),
            Margin = new(0),
         };

         _tableLayoutPanel.Controls.Add(_monthDecreaseButton, 1, 0);
         _tableLayoutPanel.Controls.Add(_dayDecreaseButton, 0, 0);
         _tableLayoutPanel.Controls.Add(_yearDecreaseButton, 2, 0);

         _tableLayoutPanel.Controls.Add(_dateTextBox, 3, 0);

         _tableLayoutPanel.Controls.Add(_yearIncreaseButton, 4, 0);
         _tableLayoutPanel.Controls.Add(_monthIncreaseButton, 5, 0);
         _tableLayoutPanel.Controls.Add(_dayIncreaseButton, 6, 0);

         _yearDecreaseButton.Text = "<";
         _monthDecreaseButton.Text = "<";
         _dayDecreaseButton.Text = "<";

         _yearIncreaseButton.Text = ">";
         _monthIncreaseButton.Text = ">";
         _dayIncreaseButton.Text = ">";
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

      public DateControlLayout DateControlLayout { get; set; }


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
      }

   }

   public sealed class DecreaseButton : Button
   {
      public DecreaseButton()
      {
         Text = "-";
         Dock = DockStyle.Fill;
         TextAlign = ContentAlignment.MiddleCenter;
         Margin = new (0);
         Padding = new (0);
      }
   }

   public sealed class IncreaseButton : Button
   {
      public IncreaseButton()
      {
         Text = "+";
         Dock = DockStyle.Fill;
         TextAlign = ContentAlignment.MiddleCenter;
         Margin = new(0);
         Padding = new(0);
      }
   }
}