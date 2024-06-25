using System.Collections.Generic;

namespace Editor.Interfaces;

public interface IProvinceCollection
{
   public int[] GetProvinceIds();
   public IProvinceCollection? ScopeOut();
   public List<IProvinceCollection>? ScopeIn();
}