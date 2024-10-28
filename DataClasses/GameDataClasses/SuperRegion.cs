using System.Text;
using Editor.DataClasses.Commands;
using Editor.Helper;
using Editor.Interfaces;
using Editor.Savers;

namespace Editor.DataClasses.GameDataClasses;
public class SuperRegion : ProvinceCollection<Region>
{
   public SuperRegion(string name, Color color, List<Region> regions) : base(name, color)
   {
      SubCollection = regions;
   }

   public override ModifiedData WhatAmI()
   {
      return ModifiedData.SuperRegions;
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
      List<string> regionNames = [];
      foreach (var region in SubCollection)
         regionNames.Add(region.Name);

      var sb = new StringBuilder();
      SavingUtil.AddFormattedStringList(Name, regionNames, 0, ref sb);
      return sb.ToString();
   }

   public override string GetSavePromptString()
   {
      return $"Save super regions file";
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

   public override void Invoke(CProvinceCollectionType type, ProvinceComposite composite)
   {
      switch (type)
      {
         case CProvinceCollectionType.Add:
            ItemAddedToArea.Invoke(this, composite);
            break;
         case CProvinceCollectionType.Remove:
            ItemRemovedFromArea.Invoke(this, composite);
            break;
         case CProvinceCollectionType.Modify:
            ItemModified.Invoke(this, composite);
            break;
      }
   }

   public override void AddToEvent(CProvinceCollectionType type, EventHandler<ProvinceComposite> eventHandler)
   {
      switch (type)
      {
         case CProvinceCollectionType.Add:
            ItemAddedToArea += eventHandler;
            break;
         case CProvinceCollectionType.Remove:
            ItemRemovedFromArea += eventHandler;
            break;
         case CProvinceCollectionType.Modify:
            ItemModified += eventHandler;
            break;
      }
   }
   public static SuperRegion Empty => new ("", Color.Empty, []);
}