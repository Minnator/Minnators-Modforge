using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Editor.DataClasses.GameDataClasses;

public class Terrain(string name) : INotifyPropertyChanged
{
   public string Name { get; set; } = name;
   public Color Color { get; set; } = Color.DimGray;
   public string SoundType = string.Empty;
   public string Type = string.Empty;
   public bool IsWater = false;
   public bool IsInlandSea = false;
   public int DefenceBonus = 0;
   public float MovementCostMultiplier = 1.0f;
   public float NationDesignerCostMultiplier = 1.0f;

   public List<Modifier> Modifiers = [];

   public List<Province> TerrainOverrides = [];



   #region Property Changed

   public event PropertyChangedEventHandler? PropertyChanged;

   protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }

   protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
   {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
   }

   #endregion
      
}

#region Terrain Definitions

public struct TDefinition
{
   public string Name { get; set; }
   public string Type { get; set; }
   public byte ColorIndex { get; set; }

   public TDefinition(string name, string type, byte colorIndex)
   {
      Name = name;
      Type = type;
      ColorIndex = colorIndex;
   }

   public string GetSavingString() => $"\t{Name}\t\t\t = {{ type = {Type} \t\t\tcolor = {{{   ColorIndex, 3}}}";
}

public class TerrainDefinitions
{
   List<TDefinition> Definitions = [];

   public void AddDefinition(string name, string type, byte colorIndex)
   {
      Definitions.Add(new TDefinition(name, type, colorIndex));
   }

   public string GetSavingString()
   {
      StringBuilder sb = new();
      sb.AppendLine("terrain = {");
      foreach (var def in Definitions) 
         sb.AppendLine(def.GetSavingString());
      sb.AppendLine("}");
      return sb.ToString();
   }
}

#endregion

#region Tree Definition

public struct TreeDefinition
{
   public string Name { get; set; }
   public string Terrain { get; set; }
   public byte[] ColorIndex { get; set; }

   public TreeDefinition(string name, string terrain, byte[] colorIndex)
   {
      Name = name;
      Terrain = terrain;
      ColorIndex = colorIndex;
   }

   public string GetSavingString() => $"\t{Name}\t\t\t = {{ type = {Terrain} \t\t\tcolor = {{{   ColorIndex, 3}}}";
}

public class TreeDefinitions
{
   List<TreeDefinition> Definitions = [];

   public void AddDefinition(string name, string terrain, byte[] colorIndex)
   {
      Definitions.Add(new TreeDefinition(name, terrain, colorIndex));
   }

   public string GetSavingString()
   {
      StringBuilder sb = new();
      sb.AppendLine("tree = {");
      foreach (var def in Definitions) 
         sb.AppendLine(def.GetSavingString());
      sb.AppendLine("}");
      return sb.ToString();
   }
}

#endregion