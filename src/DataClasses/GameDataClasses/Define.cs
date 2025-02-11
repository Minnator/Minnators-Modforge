using Editor.Helper;
using Editor.Saving;

namespace Editor.DataClasses.GameDataClasses
{
   public class Define : Saveable
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
      public override void OnPropertyChanged(string? propertyName = null) {  }
      public override SaveableType WhatAmI() => SaveableType.Define;
      public override string[] GetDefaultFolderPath() => ["common", "defines"];
      public override string GetFileEnding() => ".lua";
      public override KeyValuePair<string, bool> GetFileName() => new($"{Globals.Settings.Saving.Formatting.ModPrefix}_{Name}.lua", false);
      public override string SavingComment() => string.Empty;
      public override string GetSaveString(int tabs) => $"{new ('\t', tabs)}{GetNameSpaceString()} = {Value}";
      public override string GetSavePromptString() => $"Define {Name} in {GetNameSpaceString()}";
   }
}