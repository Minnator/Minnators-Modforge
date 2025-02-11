using System.Runtime.CompilerServices;
using Editor.ErrorHandling;

namespace Editor.DataClasses.GameDataClasses;

public readonly struct Tag : IEquatable<Tag>, IComparable
{
   public Tag(string tagValue)
   {
      TagValue = tagValue;
   }
   
   public string TagValue { get;}


   public override string ToString()
   {
      return TagValue;
   }

   public int CompareTo(object? obj)
   {
      if (obj is Tag other)
         return string.Compare(TagValue, other.TagValue, StringComparison.Ordinal);
      throw new ArgumentException("Object is not a Tag");
   }

   public static Tag FromString(string tag)
   {
      if (tag.Length == 3 || tag.Equals(string.Empty))
         return new(tag);
      System.Diagnostics.Debug.WriteLine($"False tag {tag}");
      throw new ArgumentException("Tag must be 3 characters long");
   }


   public override bool Equals(object? obj)
   {
      return obj is Tag other && Equals(other);
   }

   public override int GetHashCode()
   {
      return TagValue.GetHashCode();
   }

   public static implicit operator string(Tag tag)
   {
      return tag.TagValue;
   }

   public static implicit operator Tag(string tag)
   {
      return new Tag(tag);
   }
   
   public static bool operator ==(Tag a, Tag b)
   {
      return a.TagValue == b.TagValue;
   }

   public static bool operator !=(Tag a, Tag b)
   {
      return a.TagValue != b.TagValue;
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

   public static IErrorHandle GeneralParse(string value, out object result)
   {
      if (TryParse(value, out var outTag))
      {
         result = outTag;
         return ErrorHandle.Success;
      }
      result = Empty;
      return new ErrorObject(ErrorType.TypeConversionError, "Could not parse Tag!", addToManager: false);
   }

   public bool IsValid()
   {
      return Globals.Countries.ContainsKey(this);
   }

   public bool Equals(Tag other)
   {
      return TagValue == other.TagValue;
   }
}
