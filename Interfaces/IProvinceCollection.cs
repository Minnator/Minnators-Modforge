using System.Collections.Generic;
using System.Drawing;

namespace Editor.Interfaces;

public interface IProvinceCollection
{
   public int[] GetProvinceIds();
   public IProvinceCollection? ScopeOut();
   public List<IProvinceCollection>? ScopeIn();
   public Color Color { get; set; }
}