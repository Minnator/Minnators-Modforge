using Windows.Devices.Enumeration;

namespace Editor.DataClasses.GameDataClasses
{
   public class Define
   {
      public static Dictionary<byte, string> NameSpaces = [];
      public static string GlobalNameSpace;

      public enum DefineType
      {
         Float,
         Int,
         String,
      }

      public DefineType Type { get; }
      public byte NameSpace { get; }
      public string Name { get; }
      public object Value { get; }

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
   }
}