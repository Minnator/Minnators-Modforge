using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Editor.Interfaces;

namespace Editor.DataClasses;

#nullable enable

public class Province : IProvinceCollection
{
   #region ManagementData

   // Management data
   public int Id { get; set; }
   public Color Color { get; set; }
   public int BorderPtr { get; set; }
   public int BorderCnt { get; set; }
   public int PixelPtr { get; set; }
   public int PixelCnt { get; set; }
   public Rectangle Bounds { get; set; }
   public Point Center { get; set; }


   #endregion

   #region Globals from the game

   // Globals from the Game
   public string Area { get; set; } = string.Empty;
   public string Continent { get; set; } = string.Empty;

   #endregion

   // Province data
   public List<Tag> Claims { get; set; } = [];
   public List<Tag> Cores { get; set; } = [];
   public Tag Controller { get; set; } = Tag.Empty;
   public Tag Owner { get; set; } = Tag.Empty;
   public Tag TribalOwner { get; set; } = Tag.Empty;
   public int BaseManpower { get; set; }
   public int BaseTax { get; set; }
   public int BaseProduction { get; set; }
   public int CenterOfTrade { get; set; }
   public int ExtraCost { get; set; }
   public int NativeFerocity { get; set; }
   public int NativeHostileness { get; set; }
   public int NativeSize { get; set; }
   public int RevoltRisk { get; set; }
   public List<string> DiscoveredBy { get; set; } = [];
   public string Capital { get; set; } = string.Empty;
   public string Culture { get; set; } = string.Empty;
   public string Religion { get; set; } = string.Empty;
   public bool HasFort15Th { get; set; } // TODO parse to check other buildings
   public bool IsHre { get; set; }
   public bool IsCity { get; set; }
   public bool IsSeatInParliament { get; set; }
   public TradeGood TradeGood { get; set; }


   public string GetLocalisation()
   {
      return Globals.Localisation.TryGetValue($"PROV{Id}", out var loc) ? loc : Id.ToString();
   }

   public override bool Equals(object? obj)
   {
      if (obj is Province other)
         return Id == other.Id;
      return false;
   }
   
   public override int GetHashCode()
   {
      return Id.GetHashCode();
   }

   public int[] GetProvinceIds()
   {
      return [Id];
   }

   public IProvinceCollection ScopeOut()
   {
      return Globals.Areas[Area];
   }

   public List<IProvinceCollection>? ScopeIn()
   {
      return [this];
   }
}