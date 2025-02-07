using System.Reflection;
using Editor.Helper;

namespace Editor.Forms.Feature.AdvancedSelections;

public enum Operations
{
   Equal,
   NotEqual,
   LessThan,
   GreaterThan,
   GreaterThanOrEqual,
   LessThanOrEqual
}

public enum ProvinceSource
{
   Selection,
   AreasFromSelection,
   RegionsFromSelection,
   SuperRegionsFromSelection,
   ContinentsFromSelection,
   TradeNodesFromSelection,
   TradeCompaniesFromSelection,
   ColonialRegionsFromSelection,
   AllProvinces,
   LandProvinces,
   SeaProvinces
}

public interface ISelectionModifier
{
   public string Name { get; set; }
   public void Execute(ProvinceSource source, Operations operation, PropertyInfo attr, object value);
}

