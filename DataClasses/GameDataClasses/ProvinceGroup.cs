using System.Text;
using Editor.DataClasses.Commands;
using Editor.Helper;
using Editor.Interfaces;
using Editor.Savers;

namespace Editor.DataClasses.GameDataClasses
{
   public class ProvinceGroup(string name, Color color) : ProvinceCollection<Province>(name, color)
   {
      public override ModifiedData WhatAmI()
      {
         return ModifiedData.ProvinceGroups;
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
         SavingUtil.AddFormattedIntList(name, GetProvinceIds(), 0, ref sb);
         return sb.ToString();
      }

      public override string GetSavePromptString()
      {
         return $"Save province groups file";
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
   }
}