using System.Diagnostics;
using System.Globalization;
using Editor.Helper;
using Editor.Saving;

namespace Editor.DataClasses.GameDataClasses
{
   public class Define : Saveable
   {
      public static readonly Dictionary<byte, string> NameSpaces = [];
      public static string GlobalNameSpace = string.Empty;
      private object _value;

      public enum DefineType
      {
         Float,
         Int,
         String,
      }

      public DefineType Type { get; }
      public byte NameSpace { get; }
      public string Name { get; }

      public object Value
      {
         get => _value;
         private set => SetField(ref _value, value);
      }

      public static Define Empty { get; } = new (DefineType.Int, 0, "unDefined", -1);

      public Define(DefineType type, byte nameSpace, string name, object value)
      {
         Type = type;
         NameSpace = nameSpace;
         Name = name;
         Value = value;
      }
      internal string GetNameSpaceString() => $"{GlobalNameSpace}.{NameSpaces[NameSpace]}.{Name}";

      public override string ToString() => $"{GetNameSpaceString()} = {Value}";
      public override void OnPropertyChanged(string? propertyName = null) {  }
      public override SaveableType WhatAmI() => SaveableType.Define;
      public override string[] GetDefaultFolderPath() => ["common", "defines"];
      public override string GetFileEnding() => ".lua";
      public override KeyValuePair<string, bool> GetFileName() => new($"{Globals.Settings.Saving.Formatting.ModPrefix}_{Name}.lua", false);
      public override string SavingComment() => string.Empty;
      public override string GetSaveString(int tabs) => $"{new ('\t', tabs)}{GetNameSpaceString()} = {Value}";
      public override string GetSavePromptString() => $"Define {Name} in {GetNameSpaceString()}";

      public string GetValueAsText
      {
         get
         {
            if (Type == DefineType.Float)
               return ((float)Value).ToString(CultureInfo.InvariantCulture);
            return Value?.ToString() ?? string.Empty;
         }
      }

      public bool SetValue(string value)
      {
         switch (Type)
         {
            case DefineType.Float:
               if (float.TryParse(value, out var floatValue))
               {
                  Value = floatValue;
                  return true;
               }
               break;
            case DefineType.Int:
               if (int.TryParse(value, out var intValue))
               {
                  Value = intValue;
                  return true;
               }
               break;
            case DefineType.String:
               Value = value;
               return true;
         }
         return false;
      }

      public static T GetValue<T>(string defineName)
      {
         if (Globals.Defines.TryGetValue(defineName, out var define))
         {
            Debug.Assert(define.Value is T, "The requested type does not match the type of the define!");
            return (T)define.Value;
         }
         Debug.Assert(false, "This should never be the case! Requesting a non existent Define");
         return default!;
      }

      public override string ToPropertyString() => GetNameSpaceString();

      public override int GetHashCode()
      {
         var hashCode = new HashCode();
         hashCode.Add(Type);
         hashCode.Add(NameSpace);
         hashCode.Add(Name);
         hashCode.Add(Value);
         return hashCode.ToHashCode();
      }

      public override bool Equals(object? obj)
      {
         if (obj is Define other)
            return Type == other.Type && NameSpace == other.NameSpace && Name.Equals(other.Name) && Value == other.Value;
         return false;
      }

      public static bool operator ==(Define left, Define right) => left.Equals(right);
      public static bool operator !=(Define left, Define right) => !left.Equals(right);
   }
}