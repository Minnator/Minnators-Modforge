using System.Collections.Generic;
using System.Security.AccessControl;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.ParadoxLanguage.Trigger;
using Editor.Parser;
using Editor.Saving;
using static Editor.Saving.SavingUtil;

namespace Editor.DataClasses.GameDataClasses
{
   public class TradeCompany : ProvinceCollection<Province>
   {
      public TradeCompany(List<TriggeredName> names, string name, Color color, ObjEditingStatus status = ObjEditingStatus.Modified) : base(name, color, status)
      {
         Names = names;
      }

      public TradeCompany(List<TriggeredName> names, string name, Color color, ref PathObj path, ICollection<Province> provinces) : base(name, color, ref path, provinces)
      {
         Names = names;
      }

      public override void OnPropertyChanged(string? propertyName = null) { }
      

      public new static TradeCompany Empty => new ([], string.Empty, Color.Empty, ObjEditingStatus.Immutable);
      
      List<TriggeredName> Names { get; set; } = [];
      public string GetSpecificName()
      {
         foreach (var name  in Names)
         {
            if (name.Name.Contains("Root_Culture"))
               return name.Name;
         }
         return string.Empty;
      }

      public string GetLocalisation()
      {
         return Localisation.GetLoc(Name);
      }

      public override string ToString()
      {
         return Name;
      }

      public override SaveableType WhatAmI()
      {
         return SaveableType.TradeCompany;
      }

      public override string[] GetDefaultFolderPath()
      {
         return ["common", "trade_companies"];
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
         return new("00_trade_companies", true);
      }

      public override string GetSaveString(int tabs)
      {
         var sb = new System.Text.StringBuilder();
         OpenBlock(ref tabs, Name, ref sb);
         AddColor(tabs, Color, ref sb);
         AddFormattedProvinceList(tabs, GetProvinces(),"provinces", ref sb);
         AddNames(tabs, Names, ref sb);
         CloseBlock(ref tabs, ref sb);
         return sb.ToString();
      }

      public override string GetSavePromptString()
      {
         return $"Save trade companies file";
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
         Globals.TradeCompanies.Remove(Name);
      }

      public override void AddGlobal()
      {
         if (!Globals.TradeCompanies.TryAdd(Name, this))
            MessageBox.Show($"The TradeCompany {Name} does already exist and can not be created.", $"TradeCompany {Name} already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
   }
}