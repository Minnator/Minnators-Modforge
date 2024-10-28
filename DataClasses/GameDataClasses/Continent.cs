using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Editor.DataClasses.Commands;
using Editor.Helper;
using Editor.Interfaces;
using Editor.Savers;

namespace Editor.DataClasses.GameDataClasses;
#nullable enable
public class Continent : ProvinceCollection<Province>
{
   public Continent(string name, Color color, List<Province> provinces) : base(name, color)
   {
      SubCollection = provinces;
   }

   public override ModifiedData WhatAmI()
   {
      return ModifiedData.Continent;
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
      var sb = new StringBuilder();
      SavingUtil.AddFormattedIntList(Name, GetProvinceIds(), 0, ref sb);
      return sb.ToString();
   }

   public override string GetSavePromptString()
   {
      return $"Save continents file";
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