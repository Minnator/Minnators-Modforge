using System.Text;
using Editor.DataClasses.Commands;
using Editor.Helper;
using Editor.Interfaces;
using Editor.Savers;

namespace Editor.DataClasses.GameDataClasses;
#nullable enable
public class Area : ProvinceCollection<Province>
{

   public Area(string name, ICollection<Province> provinces, Color color) : base (name, color)
   {
      SubCollection = provinces;
   }

   // Contains the provinces in the area will be editable as the array is only a few elements long
   
   public string Edict { get; set; } = string.Empty;
   public float Prosperity { get; set; } = 0;
   public bool IsStated { get; set; } = false;

   public Region Region
   {
      get
      {
         if (Parents.Count < 1)
            return Region.Empty;
         return (Parents[0] as Region) ?? Region.Empty;
      }
   }

   public override bool Equals(object? obj)
   {
      if (obj is Area other)
         return Name == other.Name;
      return false;
   }

   public override int GetHashCode()
   {
      return Name.GetHashCode();
   }
   
   public override ModifiedData WhatAmI()
   {
      return ModifiedData.Areas;
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
      return $"Save areas file";
   }

   public new static Area Empty => new("", [], Color.Empty);

   public EventHandler<ProvinceComposite> AreaColorChanged = delegate { };
   public EventHandler<ProvinceComposite> ProvinceAddedToArea = delegate { };
   public EventHandler<ProvinceComposite> ProvinceRemovedFromArea = delegate { };
   public EventHandler<ProvinceComposite> ModifyProvinceInArea = delegate { };


   public override void Invoke(CProvinceCollectionType type, ProvinceComposite composite)
   {
      switch (type)
      {
         case CProvinceCollectionType.Add:
            ProvinceAddedToArea.Invoke(this, composite);
            break;
         case CProvinceCollectionType.Remove:
            ProvinceRemovedFromArea.Invoke(this, composite);
            break;
         case CProvinceCollectionType.Modify:
            ModifyProvinceInArea.Invoke(this, composite);
            break;
      }
   }

   public override void AddToEvent(CProvinceCollectionType type, EventHandler<ProvinceComposite> handler)
   {
      switch (type)
      {
         case CProvinceCollectionType.Add:
            ProvinceAddedToArea += handler;
            break;
         case CProvinceCollectionType.Remove:
            ProvinceRemovedFromArea += handler;
            break;
         case CProvinceCollectionType.Modify:
            ModifyProvinceInArea += handler;
            break;
      }
   }
   
   public override void Invoke(ProvinceComposite composite)
   {
      AreaColorChanged.Invoke(this, composite);
   }

   public override void AddToEvent(EventHandler<ProvinceComposite> handler)
   {
      AreaColorChanged += handler;
   }

}