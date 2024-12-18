using Editor.DataClasses.Misc;
using Editor.Helper;

namespace Editor.DataClasses.GameDataClasses
{
   public class Bookmark : ITitleAdjProvider
   {
      public string NameLocKey;
      public string DescLocKey;

      public Date Date;

      public List<Tag> CountryTags;

      public Bookmark(string nameLocKey, string descLocKey, Date date, List<Tag> countryTags)
      {
         NameLocKey = nameLocKey;
         DescLocKey = descLocKey;
         Date = date;
         CountryTags = countryTags;
      }

      public string TitleKey => NameLocKey;

      public string TitleLocalisation => Localisation.GetLoc(TitleKey);

      public string AdjectiveKey => DescLocKey;

      public string AdjectiveLocalisation => Localisation.GetLoc(AdjectiveKey);

      public override string ToString()
      {
         return $"{Date} ({TitleLocalisation})";
      }

      public override bool Equals(object? obj)
      {
         if (obj is Bookmark other)
            return Equals(other);
         return false;
      }

      private bool Equals(Bookmark other) => NameLocKey == other.NameLocKey && Date.Equals(other.Date);

      public override int GetHashCode() => HashCode.Combine(NameLocKey, Date);

      public static bool operator ==(Bookmark left, Bookmark right) => Equals(left, right);

      public static bool operator !=(Bookmark left, Bookmark right) => !Equals(left, right);
   }
}