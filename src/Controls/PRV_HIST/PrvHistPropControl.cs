using System.Reflection;
using Editor.Controls.PROPERTY;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Saving;
using static Editor.Loading.Enhanced.PCFL.Implementation.PCFL_Scope;

namespace Editor.Controls.PRV_HIST;

/*
 * How it shall work
 * The control will display the state of a province
 * The control will have a historyLoadHandle which will be triggered on province selection, history entry or date change
 * The control has at least one TokenEffect which will be added to a history entry on data confirmation *
 */



public interface IPrvHistSingleEffectPropControl<TProperty> where TProperty : notnull
{
   public Func<TProperty, IToken> EffectToken { get; init; } 
}

public interface IPrvHisSetOptSinglePropControl<TProperty> : IPrvHistSingleEffectPropControl<TProperty> where TProperty : notnull
{
   public Func<TProperty, IToken> SetEffectToken { get; init; }

}

public interface IPrvHistDualEffectPropControl<TProperty, TItem> : IPropertyControlList<Province, TProperty, TItem>
   where TProperty : ICollection<TItem>, new() where TItem : notnull
{
   public Func<TItem, IToken> AddEffectToken { get; init; }
   public Func<TItem, IToken> RemoveEffectToken { get; init; }
}