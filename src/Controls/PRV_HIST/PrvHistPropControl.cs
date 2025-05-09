using System.Reflection;
using Editor.Controls.PROPERTY;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Helper;
using Editor.Saving;
using static Editor.Loading.Enhanced.PCFL.Implementation.PCFL_Scope;

namespace Editor.Controls.PRV_HIST;

/*
 * How it shall work
 * The control will display the state of a province
 * The control will have a historyLoadHandle which will be triggered on province selection, history entry or date change
 * The control has at least one TokenEffect which will be added to a history entry on data confirmation *
 */


public interface IPrvHistSingleEffectPropControl<TProperty> : IPropertyControl<Province, TProperty> where TProperty : notnull
{
   public PCFL_TokenParseDelegate EffectDelegate { get; init; } 
}

public interface IPrvHisSetOptSinglePropControl<TProperty> : IPrvHistSingleEffectPropControl<TProperty> where TProperty : notnull
{
   public PCFL_TokenParseDelegate SetEffectDelegate { get; init; }
}

public interface IPrvHistDualEffectPropControl<TProperty, TItem> : IPropertyControlList<Province, TProperty, TItem>
   where TProperty : ICollection<TItem>, new() where TItem : notnull
{
   public PCFL_TokenParseDelegate AddEffectDelegate { get; init; }
   public PCFL_TokenParseDelegate RemoveEffectDelegate { get; init; }
   public new void LoadToGui(List<Province> list, PropertyInfo propInfo, bool force)
   {
      if (force || propInfo.Equals(PropertyInfo))
         if (AttributeHelper.GetSharedAttributeList<Province, TProperty, TItem>(PropertyInfo, out var value, list))
            SetValue(value);
         else
            SetDefault();
   }
}

public interface IPrvHisSetOptDualEffPropControl<TProperty, TItem> : IPrvHistDualEffectPropControl<TProperty, TItem>
   where TProperty : ICollection<TItem>, new() where TItem : notnull
{
   public PCFL_TokenParseDelegate SetEffectDelegate { get; init; } 
}
