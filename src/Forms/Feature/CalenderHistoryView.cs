using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Windows.Media.PlayTo;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
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
      private Date currentDate = Globals.StartDate - Globals.StartDate.Day;

      private EntryPerDate[] entries;

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

         LoadDays(true);

         TitleLabel.MouseWheel += OnTitleLabel_MouseWheel;
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
            allowDown = false;
            start = Globals.StartDate.Day - 1;
         }
         else
         {
            allowDown = true;
            start = 0;
         }
         var days = Date.DaysInMonth(currentDate.Month);
         int end;
         if (currentDate.TimeStamp + days - 1 > Globals.EndDate.TimeStamp)
         {
            allowUp = false;
            end = Globals.EndDate.Day;
         }
         else
         {
            allowUp = true;
            end = days;
         }

         TitleLabel.Text = $"{currentDate.Year} - {currentDate.Month}";
         if (add)
         {
            CalenderPanel.SuspendLayout();
            CalenderPanel.Controls.Clear();
            CalenderPanel.Controls.AddRange(_dayButtons);
            CalenderPanel.ResumeLayout();
         }

         var lowerBound = BinarySearchCache(currentDate.TimeStamp, 0, entries.Length);
         var upperBound = BinarySearchCache(currentDate.TimeStamp + days, lowerBound, entries.Length);

         var counts = new int[days];
         var minCount = int.MaxValue;
         var maxCount = int.MinValue;

         for (var j = 0; j < days; j++)
         {
            var value = GetEntriesForRange(currentDate.TimeStamp + j, lowerBound, upperBound);
            if (minCount > value)
               minCount = value;
            if (maxCount < value)
               maxCount = value;
            counts[j] = value;
            Debug.WriteLine($"Value {j}: {value}");
         }

         var colors = ColorProviderRgb.GetGreenBlueScale(25);

         var i = 0;
         for (; i < start; i++)
         {
            UpdateButton(ColorProviderRgb.NormalizeColorToScale(minCount, maxCount, counts[i], colors), (Button)_dayButtons[i]);
            _dayButtons[i].Enabled = false;
            _dayButtons[i].Visible = true;
         }
         for (; i < end; i++)
         {
            UpdateButton(ColorProviderRgb.NormalizeColorToScale(minCount, maxCount, counts[i], colors), (Button)_dayButtons[i]);
            _dayButtons[i].Enabled = true;
            _dayButtons[i].Visible = true;
         }
         for (; i < days; i++)
         {
            UpdateButton(ColorProviderRgb.NormalizeColorToScale(minCount, maxCount, counts[i], colors), (Button)_dayButtons[i]);
            _dayButtons[i].Enabled = false;
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
            allowDown = false;
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
            allowUp = false;
            end = Globals.EndDate.Month;
         }
         else
         {
            allowUp = true;
            end = 12;
         }
         var lowerBound = BinarySearchCache(currentDate.TimeStamp, 0, entries.Length);
         var upperBound = BinarySearchCache(currentDate.TimeStamp + 365, lowerBound, entries.Length);
         TitleLabel.Text = $"{currentDate.Year}";
      
         if (add)
         {
            CalenderPanel.SuspendLayout();
            CalenderPanel.Controls.Clear();
            CalenderPanel.Controls.AddRange(_monthButtons);
            CalenderPanel.ResumeLayout();
         }

         var days = currentDate.TimeStamp;

         var i = 0;

         var counts = new int[12];
         var minCount = int.MaxValue;
         var maxCount = int.MinValue;
         for (var j = 0; j < 12; j++)
         {
            var value = GetEntriesForRange(days, days += Date.DaysInMonth(j), lowerBound, upperBound);
            if (minCount > value)
               minCount = value;
            if(maxCount < value)
               maxCount = value;
            counts[j] = value;
            Debug.WriteLine($"Value {j}: {value}");
         }

         var colors = ColorProviderRgb.GetGreenBlueScale(10);

         for (; i < start; i++)
         {
            UpdateButton(ColorProviderRgb.NormalizeColorToScale(minCount, maxCount, counts[i], colors), (Button)_monthButtons[i]);
            _monthButtons[i].Enabled = false;
         }
         for (; i < end; i++)
         {
            UpdateButton(ColorProviderRgb.NormalizeColorToScale(minCount, maxCount, counts[i], colors), (Button)_monthButtons[i]);
            _monthButtons[i].Enabled = true;
         }
         for (; i < 12; i++)
         {
            UpdateButton(ColorProviderRgb.NormalizeColorToScale(minCount, maxCount, counts[i], colors), (Button)_monthButtons[i]);
            _monthButtons[i].Enabled = false;
         }

      }

      private int BinarySearchCache(int time, int start, int end)
      {
         var fakeObj = new EntryPerDate(-1, time);
         var upperBound = Array.BinarySearch(entries, start, end - start, fakeObj);
         if (upperBound < 0)
         {
            upperBound = ~upperBound;
            if(upperBound >= entries.Length)
               upperBound = entries.Length - 1;
         }
         return upperBound;
      }

      private void LoadYears(bool add, int scale)
      {
         Debug.Assert(currentDate is { Day: 1, Month: 1 }, "Month an day must be 1st of january for this to work!");
         int number_scale = scale * 10;
         int start;
         if (currentDate.Year / number_scale <= Globals.StartDate.Year / number_scale)
         {
            allowDown = false;
            start = (Globals.StartDate.Year % number_scale) / scale;
         }
         else
         {
            allowDown = true;
            start = 0;
         }

         int end;
         if (currentDate.Year / number_scale >= Globals.EndDate.Year / number_scale)
         {
            allowUp = false;
            end = (Globals.EndDate.Year % number_scale) / scale + 1;
         }
         else
         {
            allowUp = true;
            end = 10;
         }

         TitleLabel.Text = $"{currentDate.Year} - {currentDate.Year + 9 * scale}";

         if (add)
         {
            CalenderPanel.SuspendLayout();
            CalenderPanel.Controls.Clear();
            CalenderPanel.Controls.AddRange(_yearButtons);
            CalenderPanel.ResumeLayout();
         }

         var lowerBound = BinarySearchCache(currentDate.TimeStamp, 0, entries.Length);
         var upperBound = BinarySearchCache(currentDate.TimeStamp + 365 * scale, lowerBound, entries.Length);
         var counts = new int[10];
         var minCount = int.MaxValue;
         var maxCount = int.MinValue;

         var year = currentDate.TimeStamp;
         for (var j = 0; j < 10; j++)
         {
            var value = GetEntriesForRange(year, year += 365 * scale, lowerBound, upperBound);
            if (minCount > value)
               minCount = value;
            if (maxCount < value)
               maxCount = value;
            counts[j] = value;
            Debug.WriteLine($"Value {j}: {value}");
         }

         var colors = ColorProviderRgb.GetGreenBlueScale(10);

         var i = 0;
         for (; i < start; i++)
         {
            UpdateButton(ColorProviderRgb.NormalizeColorToScale(minCount, maxCount, counts[i], colors), $"{currentDate.Year + i * scale}", (Button)_yearButtons[i]);
            _yearButtons[i].Enabled = false;
         }
         for (; i < end; i++)
         {
            UpdateButton(ColorProviderRgb.NormalizeColorToScale(minCount, maxCount, counts[i], colors), $"{currentDate.Year + i * scale}", (Button)_yearButtons[i]);
            _yearButtons[i].Enabled = true;
         }
         for (; i < 10; i++)
         {
            UpdateButton(ColorProviderRgb.NormalizeColorToScale(minCount, maxCount, counts[i], colors), $"{currentDate.Year + i * scale}", (Button)_yearButtons[i]);
            _yearButtons[i].Enabled = false;
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
         // TODO load day
      }

      private void UpdateButton(Color backColor, string content, Button button)
      {
         button.BackColor = backColor;
         button.Text = content;
      }

      private void UpdateButton(Color backColor, Button button)
      {
         button.BackColor = backColor;
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

      private int GetEntriesForRange(int dateLower, int dateUpper, int startIndex, int endIndex)
      {
         var resultLower = BinarySearchCache(dateLower, startIndex, endIndex);
         var resultUpper = BinarySearchCache(dateUpper, resultLower, endIndex);
         return entries[resultUpper].Count - entries[resultLower].Count;
      }

      private int GetEntriesForRange(int date, int startIndex, int endIndex)
      {
         var result = BinarySearchCache(date, startIndex, endIndex);
         if (result == 0)
            return entries[0].Count;
         return entries[result].Count - entries[result-1].Count;
      }

      private int GetEntriesForRange(int dateLower, int dateUpper)
      {
         return GetEntriesForRange(dateLower, dateUpper, 0, entries.Length);
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
