using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;
using Editor.Helper;

namespace Editor.Forms.Feature
{
   public partial class CalenderHistoryView : Form
   {
      private Control[] _dayButtons = new Button[31];
      private Control[] _monthButtons = new Button[12];
      private Control[] _yearButtons = new Button[10];

      private bool allowUp = true;
      private bool allowDown = true;

      private CalenderState _state = CalenderState.Days;
      private Date currentDate = Globals.StartDate - Globals.StartDate.Day + 1;

      private EntryPerDate[] entries;

      private int _min;
      private int _max;
      private Color[] _colors;

      private Color _zeroColor = Color.FromArgb((int)(0.32784692303604196 * 255), (int)(0.0066313933705768055 * 255), (int)(0.6402853293744383 * 255));

      private bool noLimitOnDateRange;
      private Date _lowerLimit;
      private Date _upperLimit;

      public enum CalenderState
      {
         Days,
         Months,
         Years,
         Decades,
         Centuries
      }

      public CalenderHistoryView()
      {
         InitializeComponent();

         GenerateDayButtons();
         GenerateMonthButtons();
         GenerateYearButtons();

         Debug.WriteLine(Globals.Provinces.Sum(x => x.History.Count));

         GenerateCache();
         
         //noLimitOnDateRange = Globals.Settings.Rendering.CalendarView.NoLimitOnDateRange;
         noLimitOnDateRange = false;
         if (noLimitOnDateRange && entries!.Length > 0)
         {
            _lowerLimit = new (entries[0].TimeStamp);
            _upperLimit = new(entries[^1].TimeStamp);
         }
         else
         {
            _lowerLimit = Globals.StartDate;
            _upperLimit = Globals.EndDate;
         }
         
         LoadDays(true);

         MouseWheel += OnTitleLabel_MouseWheel;

         MinMaxLabel.Paint += MinMaxLabel_Paint;

         
      }

      private void MinMaxLabel_Paint(object? sender, PaintEventArgs e)
      {
         var (rectWidth, remainder) = Math.DivRem(MinMaxLabel.Width, _colors.Length);
         for (var i = 0; i < _colors.Length; i++) 
            e.Graphics.FillRectangle(new SolidBrush(_colors[i]), i * rectWidth + remainder/2, 0, rectWidth, MinMaxLabel.Height);

         var textSize = MissionView.MeasureTextSize(e.Graphics, $"Min: {_min} - Max: {_max}", MinMaxLabel.Font);

         e.Graphics.DrawString($"Min: {_min} - Max: {_max}", MinMaxLabel.Font, Brushes.Black, (MinMaxLabel.Width - textSize.Width) / 2, (MinMaxLabel.Height - textSize.Height) / 2);
      }


      private void OnTitleLabel_MouseWheel(object? sender, MouseEventArgs e)
      {
         if (e.Delta > 0)
         {
            if(allowUp)
               switch (_state)
               {
                  case CalenderState.Days:
                     currentDate.AddMonths(1);
                     LoadDays(false);
                     break;
                  case CalenderState.Months:
                     currentDate.AddYears(1);
                     LoadMonths(false);
                     break;
                  case CalenderState.Years:
                     currentDate.AddYears(10);
                     LoadYears(false, 1);
                     break;
                  case CalenderState.Decades:
                     currentDate.AddYears(100);
                     LoadYears(false, 10);
                     break;
                  case CalenderState.Centuries:
                     currentDate.AddYears(1000);
                     LoadYears(false, 100);
                     break;
               }
         }
         else if(allowDown)
         {
            switch (_state)
            {
               case CalenderState.Centuries:
                  currentDate.AddYears(-1000);
                  LoadYears(false, 100);
                  break;
               case CalenderState.Decades:
                  currentDate.AddYears(-100);
                  LoadYears(false, 10);
                  break;
               case CalenderState.Years:
                  currentDate.AddYears(-10);
                  LoadYears(false, 1);
                  break;
               case CalenderState.Months:
                  currentDate.AddYears(-1);
                  LoadMonths(false);
                  break;
               case CalenderState.Days:
                  currentDate.AddMonths(-1);
                  LoadDays(false);
                  break;
            }
         }
      }

      private void LoadDays(bool add)
      {
         Debug.Assert(currentDate.Day == 1, "Day must be 1st of month for this to work!");
         int start;
         if (currentDate.TimeStamp < Globals.StartDate.TimeStamp)
         {
            allowDown = currentDate >= _lowerLimit;
            start = Globals.StartDate.Day - 1;
         }
         else
         {
            allowDown = true;
            start = 0;
         }
         var days = Date.DaysInMonth(currentDate.Month);
         int end;
         var upperLimit = currentDate.TimeStamp + days - 1;
         if (upperLimit > Globals.EndDate.TimeStamp)
         {
            allowUp = upperLimit >= _upperLimit.TimeStamp;
            end = Globals.EndDate.Day;
         }
         else
         {
            allowUp = true;
            end = days;
         }

         TitleLabel.Text = $"{currentDate.Year} - {currentDate.Month}";
         

         var lowerBound = BinarySearchCache(currentDate.TimeStamp, 0, entries.Length, out _);
         var upperBound = BinarySearchCache(currentDate.TimeStamp + days, lowerBound, entries.Length, out _);

         var counts = new int[days];
         _min = int.MaxValue;
         _max = int.MinValue;

         for (var j = 0; j < days; j++)
         {
            var value = GetEntriesForRange(currentDate.TimeStamp + j, lowerBound, upperBound);
            if (_min > value && value != 0)
               _min = value;
            if (_max < value)
               _max = value;
            counts[j] = value;
         }

         if (_min == int.MaxValue)
            _min = _max;
         
         
         if (add)
         {
            _colors = ColorProviderRgb.GetPlasmaScale(25);
            CalenderPanel.SuspendLayout();
            CalenderPanel.Controls.Clear();
            CalenderPanel.Controls.AddRange(_dayButtons);
            CalenderPanel.ResumeLayout();    
         }
         MinMaxLabel.Invalidate();
         var i = 0;
         for (; i < start; i++)
         {
            UpdateButton(GetButtonColor(counts, i), (Button)_dayButtons[i], true);
            _dayButtons[i].Enabled = noLimitOnDateRange;
            _dayButtons[i].Visible = true;
         }
         for (; i < end; i++)
         {
            UpdateButton(GetButtonColor(counts, i), (Button)_dayButtons[i]);
            _dayButtons[i].Enabled = true;
            _dayButtons[i].Visible = true;
         }
         for (; i < days; i++)
         {
            UpdateButton(GetButtonColor(counts, i), (Button)_dayButtons[i], true);
            _dayButtons[i].Enabled = noLimitOnDateRange;
            _dayButtons[i].Visible = true;
         }
         for (; i < _dayButtons.Length; i++)
         {
            _dayButtons[i].Visible = false;
         }
      }

      private void LoadMonths(bool add)
      {
         Debug.Assert(currentDate.Day == 1 && currentDate.Month == 1, "Month and day must be 1st of january for this to work!");
         int start;
         if (currentDate.Year <= Globals.StartDate.Year)
         {
            allowDown = currentDate.Year > _lowerLimit.Year;
            start = Globals.StartDate.Month - 1;
         }
         else
         {
            allowDown = true;
            start = 0;
         }

         int end;
         if (currentDate.Year >= Globals.EndDate.Year)
         {
            allowUp = currentDate.Year < _upperLimit.Year;
            end = Globals.EndDate.Month;
         }
         else
         {
            allowUp = true;
            end = 12;
         }
         var lowerBound = BinarySearchCache(currentDate.TimeStamp, 0, entries.Length, out _);
         var upperBound = BinarySearchCache(currentDate.TimeStamp + 365, lowerBound, entries.Length, out _);
         TitleLabel.Text = $"{currentDate.Year}"; 

         var days = currentDate.TimeStamp;

         var i = 0;

         var counts = new int[12];
         _min = int.MaxValue;
         _max = int.MinValue;
         for (var j = 0; j < 12; j++)
         {
            var value = GetEntriesForRange(days, days += Date.DaysInMonth(j+1), lowerBound, upperBound);
            Debug.WriteLine(new Date(days));
            if (_min > value && value != 0)
               _min = value;
            if(_max < value)
               _max = value;
            counts[j] = value;
         }

         if (_min == int.MaxValue)
            _min = _max;


         if (add)
         {
            _colors = ColorProviderRgb.GetPlasmaScale(10);
            CalenderPanel.SuspendLayout();
            CalenderPanel.Controls.Clear();
            CalenderPanel.Controls.AddRange(_monthButtons);
            CalenderPanel.ResumeLayout();
         }
         MinMaxLabel.Invalidate();
         for (; i < start; i++)
         {
            UpdateButton(GetButtonColor(counts, i), (Button)_monthButtons[i], true);
            _monthButtons[i].Enabled = noLimitOnDateRange;
         }
         for (; i < end; i++)
         {
            UpdateButton(GetButtonColor(counts, i), (Button)_monthButtons[i]);
            _monthButtons[i].Enabled = true;
         }
         for (; i < 12; i++)
         {
            UpdateButton(GetButtonColor(counts, i), (Button)_monthButtons[i], true);
            _monthButtons[i].Enabled = noLimitOnDateRange;
         }

      }

      private Color GetButtonColor(int[] counts, int i)
      {
         if (counts[i] == 0)
            return _zeroColor;
         return ColorProviderRgb.NormalizeColorToScale(_min, _max, counts[i], _colors, false);
      }

      private int BinarySearchCache(int time, int start, int end, out bool exact)
      {
         var fakeObj = new EntryPerDate(-1, time);
         var upperBound = Array.BinarySearch(entries, start, end - start, fakeObj);
         if (upperBound < 0)
         {
            exact = false;
            upperBound = ~upperBound;
            if (upperBound >= entries.Length)
               upperBound = entries.Length - 1;
         }
         else
            exact = true;
         return upperBound;
      }

      private void LoadYears(bool add, int scale)
      {
         Debug.Assert(currentDate is { Day: 1, Month: 1 }, "Month an day must be 1st of january for this to work!");
         int number_scale = scale * 10;
         int start;
         int limit = currentDate.Year / number_scale;
         if (limit <= Globals.StartDate.Year / number_scale)
         {
            allowDown = limit > _lowerLimit.Year / number_scale;
            start = (Globals.StartDate.Year % number_scale) / scale;
         }
         else
         {
            allowDown = true;
            start = 0;
         }

         int end;
         if (limit >= Globals.EndDate.Year / number_scale)
         {
            allowUp = limit < _lowerLimit.Year / number_scale;
            end = (Globals.EndDate.Year % number_scale) / scale + 1;
         }
         else
         {
            allowUp = true;
            end = 10;
         }

         TitleLabel.Text = $"{currentDate.Year} - {currentDate.Year + 9 * scale}";

         var lowerBound = BinarySearchCache(currentDate.TimeStamp, 0, entries.Length, out _);
         var upperBound = BinarySearchCache(currentDate.TimeStamp + 365 * number_scale, lowerBound, entries.Length, out _);
         var counts = new int[10];
         _min = int.MaxValue;
         _max = int.MinValue;

         var year = currentDate.TimeStamp;
         for (var j = 0; j < 10; j++)
         {
            var value = GetEntriesForRange(year, year += 365 * scale, lowerBound, upperBound);
            if (_min > value && value != 0)
               _min = value;
            if (_max < value)
               _max = value;
            counts[j] = value;
         }


         if (_min == int.MaxValue)
            _min = _max;

         if (add)
         {
            _colors = ColorProviderRgb.GetPlasmaScale(10);
            CalenderPanel.SuspendLayout();
            CalenderPanel.Controls.Clear();
            CalenderPanel.Controls.AddRange(_yearButtons);
            CalenderPanel.ResumeLayout();
         }
         MinMaxLabel.Invalidate();

         var i = 0;
         for (; i < start; i++)
         {
            UpdateButton(GetButtonColor(counts, i), $"{currentDate.Year + i * scale}", (Button)_yearButtons[i], true);
            _yearButtons[i].Enabled = noLimitOnDateRange;
         }
         for (; i < end; i++)
         {
            UpdateButton(GetButtonColor(counts, i), $"{currentDate.Year + i * scale}", (Button)_yearButtons[i]);
            _yearButtons[i].Enabled = true;
         }
         for (; i < 10; i++)
         {
            UpdateButton(GetButtonColor(counts, i), $"{currentDate.Year + i * scale}", (Button)_yearButtons[i], true);
            _yearButtons[i].Enabled = noLimitOnDateRange;
         }
      }
      
      private void GenerateDayButtons()
      {
         GetButtons(7, _dayButtons, CalenderState.Days);
         for (var i = 0; i < _dayButtons.Length; i++)
         {
            UpdateButton(i % 2 == 0 ? Color.Gray : Color.DarkBlue, (i + 1).ToString(), (Button)_dayButtons[i]);
         }
      }

      private void GenerateMonthButtons()
      {
         GetButtons(3, _monthButtons, CalenderState.Months);
         for (var i = 0; i < _monthButtons.Length; i++)
         {
            UpdateButton(i % 2 == 0 ? Color.Gray : Color.DarkRed, currentDate.GetNameOfMonth(i + 1), (Button)_monthButtons[i]);
         }
      }

      private void GenerateYearButtons()
      {
         GetButtons(3, _yearButtons, CalenderState.Years);
      }

      private void GetButtons(int columnCount, Control[] buttons, CalenderState state, int margin = 2)
      {
         var rowCount = (buttons.Length - 1) / columnCount + 1;
         var width = (CalenderPanel.Width - margin * columnCount * 2) / columnCount;
         var height = (CalenderPanel.Height - margin * rowCount * 2) / rowCount;

         for (var i = 0; i < buttons.Length; i++)
         {
            buttons[i] = new Button()
            {
               Width = width,
               Height = height,
               Tag = i,
               Margin = new(margin),
               Padding = new(0),
            };

            switch (state)
            {
               case CalenderState.Days:
                  buttons[i].Click += OnDaysButton_Click;
                  break;
               case CalenderState.Months:
                  buttons[i].Click += OnMonthsButton_Click;
                  break;
               case CalenderState.Years:
                  buttons[i].Click += OnYearsButton_Click;
                  break;
            }
         }
      }

      private void OnDaysButton_Click(object? sender, EventArgs e)
      {
         var date = currentDate.Copy();
         date.AddDays((int)((Button)sender!).Tag!);
         var entries = GetEntries(date);

         var contextMenu = ConstructContextMenu(entries);

         var button = (Button)sender;
         contextMenu.Show(button, button.PointToClient(Cursor.Position));
      }

      private void UpdateButton(Color backColor, string content, Button button, bool altForeGround = false)
      {
         button.BackColor = backColor;
         button.Text = content;
         button.ForeColor = altForeGround ? Color.Red : Color.Black;
      }

      private void UpdateButton(Color backColor, Button button, bool altForeGround = false)
      {
         button.BackColor = backColor;
         button.ForeColor = altForeGround ? Color.Red : Color.Black;
      }

      private void OnMonthsButton_Click(object? sender, EventArgs e)
      {
         _state = CalenderState.Days;
         currentDate.AddMonths((int)((Button)sender!).Tag!);
         LoadDays(true);
      }

      private void OnYearsButton_Click(object? sender, EventArgs e)
      {
         switch (_state)
         {
            case CalenderState.Centuries:
               _state = CalenderState.Decades;
               currentDate.AddYears((int)((Button)sender!).Tag! * 100);
               LoadYears(false, 10);
               break;
            case CalenderState.Decades:
               _state = CalenderState.Years;
               currentDate.AddYears((int)((Button)sender!).Tag! * 10);
               LoadYears(false, 1);
               break;
            case CalenderState.Years:
               _state = CalenderState.Months;
               currentDate.AddYears((int)((Button)sender!).Tag!);
               LoadMonths(true);
               break;
         }
      }

      private void TitleLabel_Click(object sender, EventArgs e)
      {
         switch (_state)
         {
            case CalenderState.Days:
               _state = CalenderState.Months;
               currentDate.AddMonths(-currentDate.Month + 1);
               LoadMonths(true);
               break;
            case CalenderState.Months:
               _state = CalenderState.Years;
               currentDate.AddYears(-currentDate.Year % 10);
               LoadYears(true, 1);
               break;
            case CalenderState.Years:
               _state = CalenderState.Decades;
               currentDate.AddYears(-currentDate.Year % 100);
               LoadYears(false, 10);
               break;
            case CalenderState.Decades:
               _state = CalenderState.Centuries;
               currentDate.AddYears(-currentDate.Year % 1000);
               LoadYears(false, 100);
               break;
            case CalenderState.Centuries:
               break;
         }
      }

      private ContextMenuStrip ConstructContextMenu(List<(Province,HistoryEntry)> entries)
      {
         var contextMenu = new ContextMenuStrip();
         foreach (var (province, entry) in entries)
         {
            var item = new ToolStripMenuItem($"({province.Id}, '{province.TitleLocalisation}'): {entry.ToString()?.Replace('\n', ' ')}");
            contextMenu.Items.Add(item);
         }
         return contextMenu;
      }

      private int GetEntriesForRange(int dateLower, int dateUpper, int startIndex, int endIndex)
      {
         var resultLower = BinarySearchCache(dateLower, startIndex, endIndex, out _);
         var resultUpper = BinarySearchCache(dateUpper, resultLower, endIndex, out _);
         if (resultLower == 0)
            if(resultUpper == 0)
               return 0;
            else 
               return entries[resultUpper - 1].Count;
         return entries[resultUpper-1].Count - entries[resultLower-1].Count;
      }

      private int GetEntriesForRange(int date, int startIndex, int endIndex)
      {
         var result = BinarySearchCache(date, startIndex, endIndex, out var exact);
         if (!exact)
            return 0;
         if (result == 0)
            return entries[0].Count;
         
         return entries[result].Count - entries[result-1].Count;
      }

      private int GetEntriesForRange(int dateLower, int dateUpper)
      {
         return GetEntriesForRange(dateLower, dateUpper, 0, entries.Length);
      }

      private List<(Province, HistoryEntry)> GetEntries(Date date)
      {
         var result = new List<(Province, HistoryEntry)>();
         foreach (var province in Globals.Provinces)
         {
            foreach (var entry in province.History)
            {
               if (entry.Date > date)
                  break;
               if(entry.Date == date)
                  result.Add((province,entry));
            }
         }

         return result;
      }

      private void GenerateCache()
      {
         var tempSet = new HashSet<EntryPerDate>();
         EntryPerDate fakeEntry = new(-1, 0);

         foreach (var province in Globals.Provinces)
         {
            foreach (var entry in province.History)
            {
               fakeEntry.TimeStamp = entry.Date.TimeStamp;
               if (tempSet.TryGetValue(fakeEntry, out var set))
                  set.Count++;
               else
                  tempSet.Add(new (1, entry.Date.TimeStamp));
            }
         }

         var list = tempSet.ToList();
         list.Sort((a, b) => a.TimeStamp.CompareTo(b.TimeStamp));
         entries = list.ToArray();


         var count = 0;

         foreach (var entry in entries)
         {
            entry.Count += count;
            count = entry.Count;
         }

      }
   }

   public class EntryPerDate(int count, int timeStamp) : IEquatable<EntryPerDate>, IComparable<int>, IComparable<EntryPerDate>
   {
      public int Count { get; set; } = count;
      public int TimeStamp { get; set; } = timeStamp;

      public override int GetHashCode()
      {
         return TimeStamp;
      }

      public int CompareTo(object? obj)
      {
         if (obj is not EntryPerDate date)
            return 0;

         return TimeStamp.CompareTo(date.TimeStamp);
      }

      public int CompareTo(int other)
      {
         return TimeStamp.CompareTo(other);
      }

      public int CompareTo(EntryPerDate? other)
      {
         return TimeStamp.CompareTo(other?.TimeStamp);
      }

      public override bool Equals(object? obj)
      {
         if(obj is not EntryPerDate date)
            return false;

         return date.TimeStamp == TimeStamp;
      }

      public bool Equals(EntryPerDate? other)
      {
         return TimeStamp == other?.TimeStamp;
      }

   }
}
