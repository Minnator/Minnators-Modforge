using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.ErrorHandling;
using Editor.Saving;

namespace Editor.DataClasses.Misc
{
   public partial class Date : IComparable<Date>
   {
      private static readonly int[] StartDateOfMonth = [0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334];
      private int _timeStamp;
      public EventHandler<Date> OnDateChanged = delegate { };

      [GeneratedRegex(@"(?<year>-?\d{1,4})\.(?<month>\d{1,2})\.(?<day>\d{1,2})")]
      private static partial Regex DateRegexGeneration();
      private static readonly Regex DateRegex = DateRegexGeneration();

      public int Year => TimeStamp / 365;
      public int Month => GetGregorian().month;
      public int Day => GetGregorian().day;

      [Browsable(false)]
      public static Date MinValue => new(int.MinValue);

      [Browsable(false)]
      public static Date MaxValue => new(int.MaxValue);

      [Browsable(false)]
      public static Date Empty => new(1, 1, 1);

      public string GetNameOfMonth(int month)
      {
         return month switch
         {
            1 => "January",
            2 => "February",
            3 => "March",
            4 => "April",
            5 => "May",
            6 => "June",
            7 => "July",
            8 => "August",
            9 => "September",
            10 => "October",
            11 => "November",
            12 => "December",
            _ => "Oink"
         };
      }

      /// <summary>
      /// Each day is one tick
      /// Every year is 365 days
      /// The valid range is from -10.000 to 10.000
      /// 0 is the year 0 and 0.0.0 is the first day of the year 0: 1.1.0
      /// Months are 30, 31 or 28 days
      /// </summary>
      public int TimeStamp
      {
         get => _timeStamp;
         private set
         {
            _timeStamp = value;
            OnDateChanged.Invoke(this, this);
         }
      }

      public Date(int year, int month, int day)
      {
         SetDate(year, month, day);
      }

      public Date(int timeStamp)
      {
         TimeStamp = timeStamp;
      }

      public (int year, int month, int day) GetGregorian()
      {
         var (year, remainder) = Math.DivRem(_timeStamp, 365);
         var month = 12;
         var day = remainder;
         for (var i = 1; i < StartDateOfMonth.Length; i++)
         {
            var temp = remainder - StartDateOfMonth[i];
            if (temp < 0){
               month = i;
               break;
            }
            day = temp;
         }
         return (year, month, day + 1);
      }

      public void AddDays(int days)
      {
         TimeStamp += days;
      }

      public void AddMonths(int months)
      {
         var (year, month, day) = GetGregorian();
         var newMonth = month + months;

         while (newMonth > 12)
         {
            newMonth -= 12;
            year++;
         }

         while (newMonth < 1)
         {
            newMonth += 12;
            year--;
         }

         SetDate(year, newMonth, day);
      }

      public void AddYears(int years)
      {
         TimeStamp += years * 365;
      }

      public void SetDate(Date Date) => TimeStamp = Date.TimeStamp;
      public void SetDateSilent(Date Date) => _timeStamp = Date.TimeStamp;
      public void SetDate(int year, int month, int day)
      {
         Debug.Assert(month >= 1 && month <= 12, $"The month {month} is not a valid month");
         Debug.Assert(day >= 1 && day <= DaysInMonth(month), $"The day {day} is not a valid day in month {month}");

         TimeStamp = year * 365 + StartDateOfMonth[month - 1] + day - 1;
      }

      public Date Copy() => new(Year, Month, Day);
      public Date Copy(Date date) => new(date.Year, date.Month, date.Day);
      public int DaysBetween(Date date) => date.TimeStamp - TimeStamp;

      public static IErrorHandle TryParse(string str, out Date Date)
      {
         Date = MinValue;
         if (string.IsNullOrWhiteSpace(str))
         {
            return new ErrorObject(ErrorType.IllegalDateFormat, $"An empty string \"[{str}]\" can not be parsed to a Date", addToManager: false);
         }
         var match = DateRegex.Match(str);
         if (!match.Success)
            return new ErrorObject(ErrorType.IllegalDateFormat, $"The string \"{str}\" does not match the Date format <yyyy.mm.dd>", addToManager: false);

         if (!short.TryParse(match.Groups["year"].Value, out var year) ||
             !byte.TryParse(match.Groups["month"].Value, out var month) ||
             !byte.TryParse(match.Groups["day"].Value, out var day))
            return new ErrorObject(ErrorType.IllegalDateFormat, $"The Date {match} is not a valid Date.", addToManager: false);

         if (month < 1 || month > 12 || day < 1 || day > DaysInMonth(month))
            return new ErrorObject(ErrorType.IllegalDateFormat, $"The Date {year}.{month}.{day} is not a valid Date.", addToManager: false);

         Date = new(year, month, day);
         return ErrorHandle.Success;
      }
      public static int DaysInMonth(int month)
      {
         return month switch
         {
            2 => 28,
            4 or 6 or 9 or 11 => 30,
            _ => 31
         };
      }

      public override string ToString()
      {
         var (year, month, day) = GetGregorian();
         return $"{year}.{month}.{day}";
      }

      public override bool Equals(object? obj) => obj is Date Date && TimeStamp == Date.TimeStamp;

      protected bool Equals(Date other)
      {
         return TimeStamp == other.TimeStamp;
      }

      public override int GetHashCode()
      {
         return TimeStamp;
      }

      public int CompareTo(Date? other)
      {
         return TimeStamp.CompareTo(other?.TimeStamp);
      }

      public static bool operator ==(Date left, Date right)
      {
         return Equals(left, right);
      }

      public static bool operator !=(Date left, Date right)
      {
         return !Equals(left, right);
      }

      public static bool operator >(Date left, Date right)
      {
         return left.TimeStamp > right.TimeStamp;
      }

      public static bool operator <(Date left, Date right)
      {
         return left.TimeStamp < right.TimeStamp;
      }

      public static bool operator >=(Date left, Date right)
      {
         return left.TimeStamp >= right.TimeStamp;
      }

      public static bool operator <=(Date left, Date right)
      {
         return left.TimeStamp <= right.TimeStamp;
      }

      public static Date operator +(Date Date, int days)
      {
         var newDate = new Date(Date.TimeStamp);
         newDate.AddDays(days);
         return newDate;
      }

      public static Date operator -(Date Date, int days)
      {
         var newDate = new Date(Date.TimeStamp);
         newDate.AddDays(-days);
         return newDate;
      }

      public static int operator +(Date Date, Date other)
      {
         return Date.TimeStamp + other.TimeStamp;
      }

      public static int operator -(Date Date, Date other)
      {
         return Date.TimeStamp - other.TimeStamp; ;
      }

      public static Date operator ++(Date Date)
      {
         Date.AddDays(1);
         return Date;
      }

      public static Date operator --(Date Date)
      {
         Date.AddDays(-1);
         return Date;
      }

      public static implicit operator string(Date Date) => Date.ToString();
   }
}