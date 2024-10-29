﻿using System.Text;
using Editor.Helper;
using Editor.Savers;

namespace Editor.DataClasses.GameDataClasses;
#nullable enable
public class Continent : ProvinceCollection<Province>
{
   public Continent(string name, Color color, List<Province> provinces) : base(name, color)
   {
      SubCollection = provinces;
   }

   public override SaveableType WhatAmI()
   {
      return SaveableType.Continent;
   }

   public override string SavingComment()
   {
      return Localisation.GetLoc(Name);
   }

   public override PathObj GetDefaultSavePath()
   {
      return new (["map", "continent.txt"]);
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

   public static EventHandler<ProvinceComposite> ColorChanged = delegate { };

   public override void ColorInvoke(ProvinceComposite composite)
   {
      ColorChanged.Invoke(this, composite);
   }

   public override void AddToColorEvent(EventHandler<ProvinceComposite> handler)
   {
      ColorChanged += handler;
   }
   public static EventHandler<ProvinceCollectionEventArguments<Province>> ItemsModified = delegate { };

   public override void Invoke(ProvinceCollectionEventArguments<Province> eventArgs)
   {
      ItemsModified.Invoke(this, eventArgs);
   }

   public override void AddToEvent(EventHandler<ProvinceCollectionEventArguments<Province>> eventHandler)
   {
      ItemsModified += eventHandler;
   }

   public override void RemoveGlobal()
   {
      Globals.Continents.Remove(Name);
   }

   public override void AddGlobal()
   {
      Globals.Continents.Add(Name, this);
   }

   public new static Continent Empty => new("", Color.Empty, []);
}