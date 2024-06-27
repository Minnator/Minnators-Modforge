using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Editor.Interfaces;

namespace Editor.DataClasses;

#nullable enable

public class Province : IProvinceCollection
{
   // Management data
   public int Id { get; set; }
   public Color Color { get; set; }
   public int BorderPtr { get; set; }
   public int BorderCnt { get; set; }
   public int PixelPtr { get; set; }
   public int PixelCnt { get; set; }
   public Rectangle Bounds { get; set; }
   public Point Center { get; set; }

   // Globals form the Game
   public string Area { get; set; } = string.Empty;
   public string Continent { get; set; } = string.Empty;


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