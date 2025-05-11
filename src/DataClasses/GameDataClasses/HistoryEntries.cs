using Editor.Parser;
using System.Text;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Saving;
using static Editor.Saving.SavingUtil;

namespace Editor.DataClasses.GameDataClasses
{
   public interface IHistoryProvider<T> where T : HistoryEntry
   {
      public List<T> History { get; set; }
   }

   public abstract class HistoryEntry(Date date) : IComparable<Date>, IComparable<HistoryEntry>
   {
      public List<IToken> Effects { get; set; } = [];
      public Date Date { get; } = date;
      public virtual void Activate(ITarget target)
      {
         foreach (var eff in Effects) 
            eff.Activate(target);
      }
      public int CompareTo(HistoryEntry? other)
      {
         if (ReferenceEquals(this, other))
         {
            return 0;
         }

         if (other is null)
         {
            return 1;
         }

         return Date.CompareTo(other.Date);
      }

      public int CompareTo(Date? other)
      {
         if (other is null)
            return 1;

         return Date.CompareTo(other);
      }

      public override string ToString()
      {
         var sb = new StringBuilder();
         FormatSimpleTokenBlock(Effects, 0, Date, ref sb);
         return sb.ToString();
      }

      public override bool Equals(object? obj)
      {
         if (obj is null)
            return false;
         if (ReferenceEquals(this, obj))
            return true;
         return GetHashCode() == obj.GetHashCode();
      }

      public override int GetHashCode()
      {
         return Date.GetHashCode() ^ Effects.GetHashCode() ^ 7;
      }
   }
   
   public class ProvinceHistoryEntry(Date date) : HistoryEntry(date)
   {
      public override string ToString()
      {
         var sb = new StringBuilder();
         FormatSimpleTokenBlock(Effects, 0, Date, ref sb);
         return sb.ToString();
      }
   }

   public class CountryHistoryEntry(Date date) : HistoryEntry(date)
   {

      // TODO: this is temporary, we need to implement a proper way to store the content after creating pdx langauge parser
      public string Content { get; set; } = string.Empty;

      public List<Person> Persons { get; set; } = [];
      public List<Leader> Leaders { get; set; } = [];
      public List<IElement> Effects { get; set; } = [];

      public bool IsDummy { get; set; } = false;

      public bool HasPerson => Persons.Any();
      public bool HasEffect => Effects.Any();

      public bool HasMonarch => Persons.Any(p => p.Type == PersonType.Monarch);
      public bool HasHeir => Persons.Any(p => p.Type == PersonType.Heir);
      public bool HasQueen => Persons.Any(p => p.Type == PersonType.Queen);
      public int MonarchCount => Persons.Count(p => p.Type == PersonType.Monarch);
      public int HeirCount => Persons.Count(p => p.Type == PersonType.Heir);
      public int QueenCount => Persons.Count(p => p.Type == PersonType.Queen);

      public void GetSavingString(int tabs, ref StringBuilder sb)
      {
         sb.AppendLine();
         OpenBlock(ref tabs, $"{Date}", ref sb);
         foreach (var leaders in Leaders)
            leaders.GetSavingString(tabs, ref sb);
         foreach (var person in Persons)
            person.GetSavingString(tabs, ref sb);
         foreach (var eff in Effects)
            eff.FormatElement(tabs, ref sb);
         CloseBlock(ref tabs, ref sb);
      }

      public override string ToString()
      {
         return $"{Date:yyyy.M.d}| P: {Persons.Count}| L: {Leaders.Count}| E: {Effects.Count}";
      }
   }
}