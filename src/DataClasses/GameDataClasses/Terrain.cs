using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Saving;
using static Editor.Saving.SavingUtil;

namespace Editor.DataClasses.GameDataClasses;

public class Terrain : ProvinceCollection<Province>, INotifyPropertyChanged
{

   public Terrain(string name, Color color, ObjEditingStatus status = ObjEditingStatus.Modified) : base(name, color, status)
   {
      base.EditingStatus = status;
      SubCollection = [];
   }

   public Terrain(string name, Color color, ref PathObj path) : this(name, color, ObjEditingStatus.Unchanged)
   {
      SetPath(ref path);
   }

   public static EventHandler<Province> OnTerrainChanged = delegate { };

   private EventHandler<ProvinceComposite> ColorEvent = delegate { };
   public override void ColorInvoke(ProvinceComposite composite)
   {
      ColorEvent.Invoke(this, composite);
   }

   public override void AddToColorEvent(EventHandler<ProvinceComposite> handler)
   {
      ColorEvent += handler;
   }

   public string SoundType = string.Empty;
   public string Type = string.Empty;
   public bool IsWater = false;
   public bool IsInlandSea = false;
   public int DefenceBonus = 0;
   public float MovementCostMultiplier = 1.0f;
   public float NationDesignerCostMultiplier = 1.0f;

   public List<ISaveModifier> Modifiers = [];

   public override bool GetSaveStringIndividually => false;

   public override string ToString()
   {
      return Name;
   }

   #region Property Changed

   public event PropertyChangedEventHandler? PropertyChanged;

   public override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
   {
      PropertyChanged?.Invoke(this, new (propertyName));
   }


   public override SaveableType WhatAmI()
   {
      return SaveableType.Terrain;
   }

   public override string[] GetDefaultFolderPath()
   {
      return ["map"];
   }

   public override string GetFileEnding()
   {
      return ".txt";
   }

   public override KeyValuePair<string, bool> GetFileName()
   {
      return new($"terrain", true);
   }

   public override string SavingComment()
   {
      return $"# {Localisation.GetLoc(Name)}";
   }

   public override string GetSaveString(int tabs)
   {
      throw new EvilActions("NO, don't save like dis");
   }

   public void AddCategoryString(StringBuilder sb, int tabs)
   {
      OpenBlock(ref tabs, Name, ref sb);
      AddColor(tabs, Color, ref sb);
      AddString(tabs, Type, "sound_type", ref sb);
      AddBoolIfYes(tabs, IsWater, "is_water", ref sb);
      AddBoolIfYes(tabs, IsInlandSea, "inland_sea", ref sb);
      AddInt(tabs, DefenceBonus, "defence", ref sb);
      AddFloat(tabs, MovementCostMultiplier,"movement_cost", ref sb);
      AddModifiers(tabs, Modifiers, ref sb);
      AddFloat(tabs, NationDesignerCostMultiplier, "nation_designer_cost_multiplier", ref sb);
      AddFormattedProvinceList(tabs, SubCollection, "terrain_override", ref sb);
      CloseBlock(ref tabs, ref sb);
   }

   public override string GetFullFileString(List<Saveable> changed, List<Saveable> unchanged)
   {
      var sb = new StringBuilder();

      sb.AppendLine(GetHeader());
      var tabs = 0;
      OpenBlock(ref tabs, "categories", ref sb);

      foreach (var obj in unchanged)
      {
         if (obj is not Terrain terrain)
            continue;
         terrain.AddCategoryString(sb, tabs);
      }
      //TODO move to method in savehelper
      sb.AppendLine($"\t### Modified {DateTime.Now} ###");
      foreach (var obj in changed)
      {
         if (obj is not Terrain terrain)
            continue;
         terrain.AddCategoryString(sb, tabs);
      }
      CloseBlock(ref tabs, ref sb);

      sb.AppendLine("##################################################################\r\n### Graphical terrain\r\n###\t\ttype\t=\trefers to the terrain defined above, \"terrain category\"'s \r\n### \tcolor \t= \tindex in bitmap color table (see terrain.bmp)\r\n###\r\n\r\nterrain = {");
      foreach (var def in Globals.TerrainDefinitions.Definitions)
      {
         sb.AppendLine(def.GetSavingString());
      }

      sb.AppendLine("}\r\n\r\n##################################################################\r\n### Tree terrain\r\n###\t\tterrain\t=\trefers to the terrain tag defined above\r\n### \tcolor \t= \tindex in bitmap color table (see tree.bmp)\r\n###\r\n\r\ntree = {");
      foreach (var def in Globals.TreeDefinitions.Definitions)
      {
         sb.AppendLine(def.GetSavingString());
      }
      sb.AppendLine("}");
      return sb.ToString(); 
   }

   public override string GetSavePromptString()
   {
      return $"terrain_type: \"{Name}\""; ;
   }

   public override string GetHeader()
   {
      return "##################################################################\r\n### Terrain Categories\r\n###\r\n### Terrain types: plains, mountains, hills, desert, artic, forest, jungle, marsh, pti\r\n### Types are used by the game to apply certain bonuses/maluses on movement/combat etc.\r\n###\r\n### Sound types: plains, forest, desert, sea, jungle, mountains\r\n";
   }

   public new static Terrain Empty  { get; } = new("undefined", Color.Empty, ObjEditingStatus.Immutable){Color = Color.DimGray};

   public void SetOverride(ICollection<Province> p)
   {
      NewAddRange(p);
      OnPropertyChanged(nameof(SubCollection));
      foreach (var province in p)
         OnTerrainChanged.Invoke(this, province);
   }
   
   public override bool Equals(object? obj)
   {
      if (obj is Terrain terrain)
         return Name == terrain.Name;

      return false;
   }

   public override int GetHashCode()
   {
      return Name.GetHashCode();
   }

   public static bool operator ==(Terrain a, Terrain b)
   {
      return a.Equals(b);
   }

   public static bool operator !=(Terrain a, Terrain b)
   {
      return !a.Equals(b);
   }

   #endregion

   public override void Invoke(ProvinceCollectionEventArguments<Province> eventArgs)
   {
      ItemsModified.Invoke(this, eventArgs);
   }

   public static EventHandler<ProvinceCollectionEventArguments<Province>> ItemsModified = delegate { };
   public override void AddToEvent(EventHandler<ProvinceCollectionEventArguments<Province>> eventHandler)
   {
      ItemsModified += eventHandler;
   }

   public override void RemoveGlobal()
   {
      Globals.Terrains.Remove(this.Name);
   }

   public override void AddGlobal()
   {
      Globals.Terrains.Add(this.Name, this);   
   }

   public static IErrorHandle GeneralParse(string? str, out object result)
   {
      var handle = TryParse(str, out var terrain);
      result = terrain;
      return handle;
   }

   public static IErrorHandle TryParse(string input, out Terrain terrain)
   {
      if (string.IsNullOrEmpty(input))
      {
         terrain = Empty;
         return new ErrorObject(ErrorType.TypeConversionError, "Could not parse Terrain!", addToManager: false);
      }

      if (!Globals.Terrains.TryGetValue(input, out terrain!))
      {
         terrain = Empty;
         return new ErrorObject(ErrorType.TODO_ERROR, $"Terrain \"{input}\" was not defined!",
            addToManager: false);
      }

      return ErrorHandle.Sucess;
   }
}

#region Terrain Definitions

public struct TDefinition
{
   public string Name { get; set; }
   public Terrain Type { get; set; }
   public byte ColorIndex { get; set; }

   public TDefinition(string name, Terrain type, byte colorIndex)
   {
      Name = name;
      Type = type;
      ColorIndex = colorIndex;
   }

   public string GetSavingString() => $"\t{Name}\t\t\t = {{ type = {Type} \t\t\tcolor = {{{   ColorIndex, 3}}}}}";
}

public class TerrainDefinitions
{
   public List<TDefinition> Definitions = [];

   public void AddDefinition(string name, Terrain type, byte colorIndex)
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

   public Terrain GetTerrainForIndex(int index)
   {
      foreach (var def in Definitions)
      {
         if (def.ColorIndex == index)
            return def.Type;
      }
      return Terrain.Empty;
   }

   public List<KeyValuePair<int, Terrain>> GetColorToTerrain()
   {
      List<KeyValuePair<int, Terrain>> kvps = [];
      for (var i = 0; i < Definitions.Count; i++)
      {
         var terrain = GetTerrainForIndex(Definitions[i].ColorIndex);
         kvps.Add(new(Definitions[i].ColorIndex, terrain));
      }
      return kvps;
   }
}

#endregion

#region Tree Definition

public struct TreeDefinition
{
   public string Name { get; set; }
   public Terrain Terrain { get; set; }
   public byte[] ColorIndex { get; set; }

   public TreeDefinition(string name, Terrain terrain, byte[] colorIndex)
   {
      Name = name;
      Terrain = terrain;
      ColorIndex = colorIndex;
   }

   public string GetSavingString() => $"\t{Name}\t\t\t = {{ type = {Terrain} \t\t\tcolor = {{{GetColorIndexString(ColorIndex), 3}}}}}";
   private static string GetColorIndexString(Byte[] bytes)
   {
      var sb = new StringBuilder();
      foreach (var b in bytes)
         sb.Append($"{b, 3}");
      return sb.ToString();
   }


}

public class TreeDefinitions
{
   public List<TreeDefinition> Definitions = [];

   public void AddDefinition(string name, Terrain terrain, byte[] colorIndex)
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

   public Terrain GetTerrainForIndex(int index)
   {
      foreach (var def in Definitions)
      {
         if (def.ColorIndex.Contains((byte)index))
            return def.Terrain;
      }
      return Terrain.Empty;
   }

   public List<KeyValuePair<int, Terrain>> GetColorToIndexKvps()
   {
      List<KeyValuePair<int, Terrain>> kvps = [];
      for (var i = 0; i < Definitions.Count; i++)
      {
         for (var j = 0; j < Definitions[i].ColorIndex.Length; j++)
         {
            var terrain = GetTerrainForIndex(Definitions[i].ColorIndex[j]);
            kvps.Add(new(Definitions[i].ColorIndex[j], terrain));
         }
      }
      return kvps;
   }
}

#endregion