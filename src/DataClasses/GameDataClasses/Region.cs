using System.Text;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Saving;

namespace Editor.DataClasses.GameDataClasses;
#nullable enable
public class Region : ProvinceCollection<Area>
{
   public List<Monsoon> Monsoon { get; set; } = [];
   public SuperRegion SuperRegion
   {
      get
      {
         return GetFirstParentOfType(SaveableType.SuperRegion) as SuperRegion ?? SuperRegion.Empty;
      }
   }

   public Region(string name, Color color, List<Area> areas, List<Monsoon> monsoon) : this(name, color, areas)
   {
      Monsoon = monsoon;
   }

   public Region(string name, Color color, List<Area> areas) : base(name, color)
   {
      SubCollection = areas;
   }


   public new static Region Empty => new ("", Color.Empty, []);

   public override SaveableType WhatAmI()
   {
      return SaveableType.Region;
   }

   public override string[] GetDefaultFolderPath()
   {
      return ["map"];
   }

   public override string GetFileEnding()
   {
      return ".txt";
   }

   public override string SavingComment()
   {
      return Localisation.GetLoc(Name);
   }

   public override KeyValuePair<string, bool> GetFileName()
   {
      return new("region", true);
   }

   public override string GetSaveString(int tabs)
   {
      List<string> areaNames = [];
      foreach (var area in SubCollection)
         areaNames.Add(area.Name);
      var sb = new StringBuilder();
      sb.AppendLine($"{Name} = {{");
      SavingUtil.AddFormattedStringListOnePerRow("areas", areaNames, 1, ref sb);
      sb.AppendLine("}");
      return sb.ToString();
   }

   public override string GetSavePromptString()
   {
      return $"Save regions file";
   }

   public static EventHandler<ProvinceComposite> ColorChanged = delegate { };

   public override void ColorInvoke(ProvinceComposite composite)
   {
      ColorChanged.Invoke(this, composite);
   }

   public override void AddToColorEvent(EventHandler<ProvinceComposite> handler)
   {
      ColorChanged += handler;
   }
   public static EventHandler<ProvinceCollectionEventArguments<Area>> ItemsModified = delegate { };


   public override void Invoke(ProvinceCollectionEventArguments<Area> eventArgs)
   {
      ItemsModified.Invoke(this, eventArgs);
   }

   public override void AddToEvent(EventHandler<ProvinceCollectionEventArguments<Area>> eventHandler)
   {
      ItemsModified += eventHandler;
   }

   public override void RemoveGlobal()
   {
      Globals.Regions.Remove(Name);
   }

   public override void AddGlobal()
   {
      Globals.Regions.Add(Name, this);
   }

   public override string GetHeader()
   {
      return "# random_new_world_region is used for RNW. Must be first in this list.\r\nrandom_new_world_region = {\r\n}";
   }
}

public class Monsoon(string start, string end)
{
   public string Start { get; set; } = start;
   public string End { get; set; } = end;
}