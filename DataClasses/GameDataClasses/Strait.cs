using Editor.Helper;

namespace Editor.DataClasses.GameDataClasses
{
   public enum StraitType
   {
      Land,
      Sea,
      Canal,
      Lake,
      River
   }

   public class Strait
   {
      public Province From { get; set; }
      public Province To { get; set; }
      public Province Through { get; set; }
      public StraitType Type { get; set; }
      public Point Start { get; set; }
      public Point End { get; set; }
      public string Comment { get; set; }
      public Rectangle Bounds { get; set; }

      public Strait(Province from, Province to, Province through, StraitType type)
      {
         From = from;
         To = to;
         Through = through;
         Type = type;
         CalculateBounds();
      }

      public Strait(Province from, Province to, Province through, StraitType type, Point start, Point end, string comment) : this(from, to, through, type)
      {
         Start = start;
         End = end;
         Comment = comment;
      }

      public void CalculateBounds()
      {
         Bounds = Geometry.GetBounds([From, To, Through]);
      }

      public override bool Equals(object? obj)
      {
         if (obj is not Strait strait)
            return false;

         return strait.From == From && strait.To == To && strait.Through == Through;
      }

      public override int GetHashCode()
      {
         return From.GetHashCode() ^ To.GetHashCode() ^ Through.GetHashCode();
      }

      public override string ToString()
      {
         return $"{From} -> {To} through {Through}";
      }

      public static bool operator ==(Strait left, Strait right)
      {
         return left.Equals(right);
      }

      public static bool operator !=(Strait left, Strait right)
      {
         return !left.Equals(right);
      }

      public static Strait Empty => new(Province.Empty, Province.Empty, Province.Empty, StraitType.Land);
   }
}