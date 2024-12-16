using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Editor.DataClasses.Misc
{
   public partial class Date
   {
      public short Year
      {
         get => _year;
         set
         {
            if (_year == value)
               return;
            _year = value;
            OnYearChanged(this, value);
            OnDateChanged.Invoke(this, this);
         }
      }

      public byte Month
      {
         get => _month;
         set
         {
            if (_month == value)
               return;
            _month = value;
            OnMonthChanged(this, value);
            OnDateChanged.Invoke(this, this);
         }
      }

      public byte Day
      {
         get => _day;
         set
         {
            if (_day == value)
               return;
            _day = value;
            OnDayChanged(this, value);
            OnDateChanged.Invoke(this, this);
         }
      }

      private static readonly Regex DateRegex = DateRegexGeneration();
      private short _year;
      private byte _month;
      private byte _day;

      public const int DAYS_IN_YEAR = 365;

      public EventHandler<int> OnYearChanged = delegate { };
      public EventHandler<int> OnMonthChanged = delegate { };
      public EventHandler<int> OnDayChanged = delegate { };
      public EventHandler<Date> OnDateChanged = delegate { };

      [GeneratedRegex(@"(?<year>-?\d{1,4}).(?<month>\d{1,2}).(?<day>\d{1,2})")]
      private static partial Regex DateRegexGeneration();

      public Date(short year, byte month, byte day)
      {
         _year = year;
         _month = month;
         _day = day;
      }

      [Browsable(false)]
      public static Date MinValue { get; } = new(short.MinValue, 1, 1);

      [Browsable(false)]
      public static Date MaxValue { get; } = new(short.MaxValue, 12, 31);
      [Browsable(false)]
      public static Date Empty  { get; } = new(0, 0, 0);

      public void CopyDate(Date date2)
      {
         Year = date2.Year;
         Month = date2.Month;
         Day = date2.Day;
      }


      public static bool TryParseDate(string str, out Date date)
      {
         date = MinValue;
         if (string.IsNullOrWhiteSpace(str))
            return false;
         var match = DateRegex.Match(str);
         if (!match.Success)
            return false;

         if (!short.TryParse(match.Groups["year"].Value, out var year) ||
         !byte.TryParse(match.Groups["month"].Value, out var month) ||
         !byte.TryParse(match.Groups["day"].Value, out var day))
            return false;

         if (month < 1 || month > 12 || day < 1 || day > DaysInMonth(month))
            return false;

         date = new (year, month, day);
         return true;
      }

      public static int DaysInMonth(byte month)
      {
         return month switch
         {
            2 => 28,
            4 or 6 or 9 or 11 => 30,
            _ => 31
         };
      }

      public Date AddDays(int days)
      {
         while (days > 0)
         {
            var daysInMonth = DaysInMonth(Month);
            if (Day + days <= daysInMonth)
            {
               Day += (byte)days;
               return this;
            }

            days -= daysInMonth - Day - 1;
            Day = 1;
            if (Month == 12)
            {
               Month = 1;
               Year++;
            }
            else
            {
               Month++;
            }
         }
         return this;
      }

      public Date AddMonths(int months)
      {
         while (months > 0)
         {
            if (Month + months <= 12)
            {
               Month += (byte)months;
               return this;
            }

            months -= 12 - Month;
            Month = 1;
            Year++;
         }
         return this;
      }

      public Date AddYears(int years)
      {
         Year += (short)years;
         return this;
      }

      public int DaysBetween(Date d1, Date d2)
      {
         var diff = Math.Abs(d1.Day - d2.Day);
         diff += d1.Year > d2.Year ? Math.Abs(d1.Year - d2.Year) : Math.Abs(d2.Year - d1.Year) * DAYS_IN_YEAR;
         diff += GetDaysForMonths(d1.Month, d2.Month);
         return diff;
      }

      /// <summary>
      /// Does not include days for the start month
      /// </summary>
      /// <param name="m1"></param>
      /// <param name="m2"></param>
      /// <returns></returns>
      public int GetDaysForMonths(int m1, int m2)
      {
         var days = 0;
         for (var i = 1; i < Math.Abs(m1 - m2); i++)
         {
            var month = m1 + i;
            if (month > 12)
               month -= 12;
            days += DaysInMonth((byte)(month));
         }
         return days;
      }

      public bool IsValid()
      {
         if (Month < 1 || Month > 12 || Day < 1 || Day > 31)
            return false;

         if (Month == 2 && Day > 28)
            return false;

         if (Day > DaysInMonth(Month))
            return false;

         return true;
      }

      public override string ToString()
      {
         return $"{Year}.{Month}.{Day}";
      }

      public override bool Equals(object? obj)
      {
         if (obj is Date other)
            return Year == other.Year && Month == other.Month && Day == other.Day;
         return false;
      }
      public bool Equals(Date other)
      {
         return Year == other.Year && Month == other.Month && Day == other.Day;
      }

      public int CompareTo(Date other)
      {
         if (Year > other.Year)
            return 1;
         if (Year < other.Year)
            return -1;

         if (Month > other.Month)
            return 1;
         if (Month < other.Month)
            return -1;

         if (Day > other.Day)
            return 1;
         if (Day < other.Day)
            return -1;

         return 0;
      }

      public override int GetHashCode()
      {
         return HashCode.Combine(Year, Month, Day);
      }

      public static bool operator ==(Date? left, Date? right)
      {
         if (left is null && right is null)
            return true;
         if (left is null || right is null)
            return false;
         return left.Equals(right);
      }

      public static bool operator !=(Date? left, Date? right)
      {
         if (left is null && right is null)
            return true;
         if (left is null || right is null)
            return false;
         return !(left == right);
      }

      // >, <, >=, <= operators
      public static bool operator >(Date? left, Date? right)
      {
         if (left is null && right is null)
            return true;
         if (left is null || right is null)
            return false;
         if (left.Year > right.Year)
            return true;
         if (left.Year < right.Year)
            return false;

         if (left.Month > right.Month)
            return true;
         if (left.Month < right.Month)
            return false;

         return left.Day > right.Day;
      }

      public static bool operator <(Date? left, Date? right)
      {
         if (left is null && right is null)
            return true;
         if (left is null || right is null)
            return false;
         if (left.Year < right.Year)
            return true;
         if (left.Year > right.Year)
            return false;

         if (left.Month < right.Month)
            return true;
         if (left.Month > right.Month)
            return false;

         return left.Day < right.Day;
      }

      public static bool operator >=(Date left, Date right)
      {
         return left > right || left == right;
      }

      public static bool operator <=(Date left, Date right)
      {
         return left < right || left == right;
      }

      public static Date operator +(Date date, int days)
      {
         var newDate = new Date(date.Year, date.Month, date.Day);
         newDate.AddDays(days);
         return newDate;
      }

      public static Date operator -(Date date, int days)
      {
         var newDate = new Date(date.Year, date.Month, date.Day);
         newDate.AddDays(-days);
         return newDate;
      }

      public static Date operator +(Date date, (int months, int days) tuple)
      {
         var newDate = new Date(date.Year, date.Month, date.Day);
         newDate.AddMonths(tuple.months);
         newDate.AddDays(tuple.days);
         return newDate;
      }

      public static Date operator -(Date date, (int months, int days) tuple)
      {
         var newDate = new Date(date.Year, date.Month, date.Day);
         newDate.AddMonths(-tuple.months);
         newDate.AddDays(-tuple.days);
         return newDate;
      }

      public static Date operator +(Date date, (int years, int months, int days) tuple)
      {
         var newDate = new Date(date.Year, date.Month, date.Day);
         newDate.AddYears(tuple.years);
         newDate.AddMonths(tuple.months);
         newDate.AddDays(tuple.days);
         return newDate;
      }

      public static Date operator -(Date date, (int years, int months, int days) tuple)
      {
         var newDate = new Date(date.Year, date.Month, date.Day);
         newDate.AddYears(-tuple.years);
         newDate.AddMonths(-tuple.months);
         newDate.AddDays(-tuple.days);
         return newDate;
      }

      public static implicit operator string(Date date)
      {
         return date?.ToString() ?? string.Empty;
      }
   }
}