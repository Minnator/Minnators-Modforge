using System.Text;
using Editor.DataClasses.Commands;
using Editor.Helper;
using Editor.Savers;

namespace Editor.DataClasses.GameDataClasses;
public class SuperRegion : ProvinceCollection<Region>
{
   public SuperRegion(string name, Color color, List<Region> regions) : base(name, color)
   {
      SubCollection = regions;
   }

   public override SaveableType WhatAmI()
   {
      return SaveableType.SuperRegion;
   }

   public override string SavingComment()
   {
      return Localisation.GetLoc(Name);
   }

   public override PathObj GetDefaultSavePath()
   {
      return new (["map","superregion.txt"]);
   }

   public override string GetSaveString(int tabs)
   {
      List<string> regionNames = [];
      foreach (var region in SubCollection)
         regionNames.Add(region.Name);

      var sb = new StringBuilder();
      SavingUtil.AddFormattedStringListOnePerRow(Name, regionNames, 0, ref sb);
      return sb.ToString();
   }

   public override string GetSavePromptString()
   {
      return $"Save super regions file";
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
   public static EventHandler<ProvinceCollectionEventArguments<Region>> ItemsModified = delegate { };

   public override void Invoke(ProvinceCollectionEventArguments<Region> eventArgs)
   {
      ItemsModified.Invoke(this, eventArgs);
   }

   public override void AddToEvent(EventHandler<ProvinceCollectionEventArguments<Region>> eventHandler)
   {
      ItemsModified += eventHandler;
   }

   public override string GetHeader()
   {
      return "#\r\n#  ⎛⎝(•ⱅ•)⎠⎞\r\n";
   }

   public override void RemoveGlobal()
   {
      Globals.SuperRegions.Remove(Name);
   }

   public override void AddGlobal()
   {
      Globals.SuperRegions.Add(Name, this);
   }

   public new static SuperRegion Empty => new ("", Color.Empty, []);

}