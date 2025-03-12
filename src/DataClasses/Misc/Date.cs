using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.ErrorHandling;
using Editor.Saving;

namespace Editor.DataClasses.Misc
{
   public partial class Date : IComparable<Date>
   {
      // Errorcodes: 
      private static readonly Regex DateRegex = DateRegexGeneration();
      private int _timeStamp;

      [GeneratedRegex(@"(?<year>-?\d{1,4})\.(?<month>\d{1,2})\.(?<day>\d{1,2})")]
      private static partial Regex DateRegexGeneration();

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

      public EventHandler<Date> OnDateChanged = delegate { };

      public Date(int year, int month, int day)
      {
         TimeStamp = year * 365 + SumDaysToMonth(month) + day - 1;
      }
      public Date(int timeStamp)
      {
         TimeStamp = timeStamp;
      }

      public int Year => TimeStamp / 365;
      public int Month => GetMonth(TimeStamp, out _);
      public int Day => GetDay(GetMonth(TimeStamp, out var remainder), remainder);

      [Browsable(false)]
      public static Date MinValue { get; } = new(short.MinValue, 1, 1);

      [Browsable(false)]
      public static Date MaxValue { get; } = new(short.MaxValue, 12, 31);
      [Browsable(false)]
      public static Date Empty { get; } = new(0, 0, 0);

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
            _ => "Invalid month"
         };
      }

      public void SetDate(Date date) => TimeStamp = date.TimeStamp;
      public Date Copy() => new(Year, Month, Day);
      public Date Copy(Date d) => new(d.TimeStamp);

      public Date AddYears(int years)
      {
         TimeStamp += years * 365;
         return this;
      }

      public Date AddMonths(int months)
      {
         var curMonth = GetMonth(TimeStamp, out var remainder);
         var curDay = GetDay(curMonth, remainder);

         while (months > 0)
         {
            if (curMonth + months <= 12)
            {
               curMonth += months;
               TimeStamp = Year * 365 + SumDaysToMonth(curMonth) + curDay;
               return this;
            }

            months -= 12 - curMonth + 1;
            curMonth = 1;
            TimeStamp = (Year + 1) * 365 + SumDaysToMonth(curMonth) + curDay;
         }
         while (months < 0)
         {
            if (curMonth + months > 0)
            {
               curMonth += months;
               TimeStamp = Year * 365 + SumDaysToMonth(curMonth) + curDay;
               return this;
            }

            months += curMonth;
            curMonth = 12;
            TimeStamp = Year * 365 + SumDaysToMonth(curMonth) + curDay;
            AddYears(-1);
         }

         return this;
      }

      public Date AddDays(int days)
      {
         var curYear = Year;
         var curMonth = GetMonth(TimeStamp, out var remainder);
         var curDay = GetDay(curMonth, remainder);

         while (days > 0)
         {
            var daysInMonth = DaysInMonth(curMonth);
            if (curDay + days <= daysInMonth)
            {
               curDay += days;
               TimeStamp = Year * 365 + SumDaysToMonth(curMonth) + curDay;
               return this;
            }

            days -= daysInMonth - curDay + 1;
            curDay = 1;
            if (curMonth == 12)
            {
               curMonth = 1;
               curYear++;
            }
            else
               curMonth++;
            TimeStamp = curYear * 365 + SumDaysToMonth(curMonth) + curDay;
         }
         while (days < 0)
         {
            if (curDay + days > 0)
            {
               curDay += days;
               TimeStamp = Year * 365 + SumDaysToMonth(curMonth) + curDay;
               return this;
            }

            days += curDay;
            curMonth--;
            if (curMonth == 0)
            {
               curMonth = 12;
               curYear--;
            }
            curDay = DaysInMonth(curMonth);
            TimeStamp = curYear * 365 + SumDaysToMonth(curMonth) + curDay;
         }
         return this;
      }

      public int DaysBetween(Date date) => date.TimeStamp - TimeStamp;
      
      private static int SumDaysToMonth(int month)
      {
         var sum = 0;
         for (var i = 1; i < month; i++)
            sum += DaysInMonth(i);
         return sum;
      }

      private static int GetDay(int month, int remainder)
      {
         Debug.Assert(remainder <= DaysInMonth(month), "Bigger remainder than days in month");
         return remainder + 1;
      }

      private static int GetMonth(int timeStamp, out int remainder)
      {
         var curMonth = 1;  
         remainder = timeStamp % 365; 

         Debug.Assert(remainder <= 365, "Why are there more days in a year left than there are in a full year");

         while (remainder > DaysInMonth(curMonth)) 
         {
            remainder -= DaysInMonth(curMonth);  
            curMonth++; 
            if (curMonth > 12) 
               break; 
         }
         return curMonth;  
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

      public int CurrentDaysInYear()
      {
         return TimeStamp % 365;
      }

      public static IErrorHandle TryParse(string str, out Date date)
      {
         date = MinValue;
         if (string.IsNullOrWhiteSpace(str))
         {
            return new ErrorObject(ErrorType.IllegalDateFormat, $"An empty string \"[{str}]\" can not be parsed to a date", addToManager: false);
         }
         var match = DateRegex.Match(str);
         if (!match.Success)
            return new ErrorObject(ErrorType.IllegalDateFormat, $"The string \"{str}\" does not match the date format <yyyy.mm.dd>", addToManager: false);

         if (!short.TryParse(match.Groups["year"].Value, out var year) ||
             !byte.TryParse(match.Groups["month"].Value, out var month) ||
             !byte.TryParse(match.Groups["day"].Value, out var day))
            return new ErrorObject(ErrorType.IllegalDate, $"The date {match} is not a valid date.", addToManager: false);

         if (month < 1 || month > 12 || day < 1 || day > DaysInMonth(month))
            return new ErrorObject(ErrorType.IllegalDate, $"The date {year}.{month}.{day} is not a valid date.", addToManager: false);

         date = new(year, month, day);
         return ErrorHandle.Success;
      }

      public override string ToString() => $"{Year}.{Month}.{Day}";

      public override bool Equals(object? obj) => obj is Date date && TimeStamp == date.TimeStamp;

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

      public static Date operator +(Date date, int days)
      {
         var newDate = new Date(date.TimeStamp);
         newDate.AddDays(days);
         return newDate;
      }

      public static Date operator -(Date date, int days)
      {
         var newDate = new Date(date.TimeStamp);
         newDate.AddDays(-days);
         return newDate;
      }

      public static int operator +(Date date, Date other)
      {
         return date.TimeStamp + other.TimeStamp;
      }

      public static int operator -(Date date, Date other)
      {
         return date.TimeStamp - other.TimeStamp; ;
      }

      public static Date operator ++(Date date)
      {
         date.AddDays(1);
         return date;
      }

      public static Date operator --(Date date)
      {
         date.AddDays(-1);
         return date;
      }

      public static implicit operator string(Date date) => date.ToString();
   }
}