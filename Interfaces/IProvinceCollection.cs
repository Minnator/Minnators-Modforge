using System.Collections.Generic;
using System.Drawing;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Interfaces;
#nullable enable
public interface IProvinceCollection
{
   public string Name { get; }
   public int[] GetProvinceIds();
   public ICollection<Province> GetProvinces();
   public IProvinceCollection? ScopeOut();
   public List<IProvinceCollection>? ScopeIn();
   public Color Color { get; set; }
}