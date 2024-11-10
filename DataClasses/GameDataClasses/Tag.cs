using System.Diagnostics;

namespace Editor.DataClasses.GameDataClasses;

public readonly struct Tag(string tag) : IEquatable<Tag>
{
   public string _tag { get; init; } = tag;

   public override string ToString()
   {
      return _tag;
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
      return obj is Tag other && Equals(other);
   }

   public override int GetHashCode()
   {
      return _tag.GetHashCode();
   }

   public static implicit operator string(Tag tag)
   {
      return tag._tag;
   }

   public static implicit operator Tag(string tag)
   {
      return new Tag(tag);
   }

   public static bool operator ==(Tag a, Tag b)
   {
      return a._tag == b._tag;
   }

   public static bool operator !=(Tag a, Tag b)
   {
      return a._tag != b._tag;
   }

   // Add an empty state to the Tag struct
   public static readonly Tag Empty = new("###");

   public bool IsEmpty()
   {
      return this == Empty;
   }

   public static bool TryParse(string str, out Tag outTag)
   {
      if (str.Length == 3)
      {
         outTag = new (str);
         return true;
      }
      outTag = Empty;
      return false;
   }

   public bool IsValid()
   {
      return Globals.Countries.ContainsKey(this);
   }

   public bool Equals(Tag other)
   {
      return _tag == other._tag;
   }
}
