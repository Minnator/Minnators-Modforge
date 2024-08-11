using System.Diagnostics;

namespace Editor.DataClasses.GameDataClasses;

public readonly struct Tag(string tag)
{
   public string GetTag => tag.ToUpper();

   public override string ToString()
   {
      return GetTag;
   }

   public static Tag FromString(string tag)
   {
      if (tag.Length == 3 || tag.Equals(string.Empty))
         return new(tag);
      Debug.WriteLine($"False tag {tag}");
      throw new ArgumentException("Tag must be 3 characters long");
   }

   public override bool Equals(object? obj)
   {
      if (obj is Tag other)
         return GetTag == other.GetTag;
      return false;
   }

   public override int GetHashCode()
   {
      return GetTag.GetHashCode();
   }

   public static implicit operator string(Tag tag)
   {
      return tag.GetTag;
   }

   public static implicit operator Tag(string tag)
   {
      return new Tag(tag);
   }

   public static bool operator ==(Tag a, Tag b)
   {
      return a.GetTag == b.GetTag;
   }

   public static bool operator !=(Tag a, Tag b)
   {
      return a.GetTag != b.GetTag;
   }

   // Add an empty state to the Tag struct
   public static readonly Tag Empty = new("###");
}
