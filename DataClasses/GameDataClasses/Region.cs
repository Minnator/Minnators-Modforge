using System.Text;
using Editor.DataClasses.Commands;
using Editor.Helper;
using Editor.Interfaces;
using Editor.Savers;

namespace Editor.DataClasses.GameDataClasses;
#nullable enable
public class Region : ProvinceCollection<Area>
{
   public List<Monsoon> Monsoon { get; set; } = [];
   public SuperRegion SuperRegion
   {
      get
      {
         if (Parents.Count < 1)
            return SuperRegion.Empty;
         return (Parents[0] as SuperRegion)!;
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

   public override ModifiedData WhatAmI()
   {
      return ModifiedData.Region;
   }

   public override string SavingComment()
   {
      return Localisation.GetLoc(Name);
   }

   public override PathObj GetDefaultSavePath()
   {
      return new (["map"]);
   }

   public override string GetSaveString(int tabs)
   {
      List<string> areaNames = [];
      foreach (var area in SubCollection)
         areaNames.Add(area.Name);
      var sb = new StringBuilder();
      sb.AppendLine($"{Name} = {{");
      SavingUtil.AddFormattedStringList("areas", areaNames, 1, ref sb);
      sb.AppendLine("}");
      return sb.ToString();
   }

   public override string GetSavePromptString()
   {
      return $"Save regions file";
   }

   public EventHandler<ProvinceComposite> ColorChanged = delegate { };
   public EventHandler<ProvinceComposite> ItemAddedToArea = delegate { };
   public EventHandler<ProvinceComposite> ItemRemovedFromArea = delegate { };
   public EventHandler<ProvinceComposite> ItemModified = delegate { };

   public override void Invoke(ProvinceComposite composite)
   {
      ColorChanged.Invoke(this, composite);
   }

   public override void AddToEvent(EventHandler<ProvinceComposite> handler)
   {
      ColorChanged += handler;
   }

   public override void Invoke(ProvinceCollectionType type, ProvinceComposite composite)
   {
      switch (type)
      {
         case ProvinceCollectionType.Add:
            ItemAddedToArea.Invoke(this, composite);
            break;
         case ProvinceCollectionType.Remove:
            ItemRemovedFromArea.Invoke(this, composite);
            break;
         case ProvinceCollectionType.Modify:
            ItemModified.Invoke(this, composite);
            break;
      }
   }

   public override void AddToEvent(ProvinceCollectionType type, EventHandler<ProvinceComposite> eventHandler)
   {
      switch (type)
      {
         case ProvinceCollectionType.Add:
            ItemAddedToArea += eventHandler;
            break;
         case ProvinceCollectionType.Remove:
            ItemRemovedFromArea += eventHandler;
            break;
         case ProvinceCollectionType.Modify:
            ItemModified += eventHandler;
            break;
      }
   }
}

public class Monsoon(string start, string end)
{
   public string Start { get; set; } = start;
   public string End { get; set; } = end;
}