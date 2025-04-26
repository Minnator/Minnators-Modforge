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

public abstract class PrvHistDualEffectPropControl<TProperty> : IPropertyControl<Province, TProperty> where TProperty : notnull
{
   protected PrvHistDualEffectPropControl(PropertyInfo propertyInfo, PCFL_TokenParseDelegate addEffectDelegate, PCFL_TokenParseDelegate removeEffectDelegate)
   {
      PropertyInfo = propertyInfo;
      AddEffectDelegate = addEffectDelegate;
      RemoveEffectDelegate = removeEffectDelegate;
      // Register the load action for the property control so we can trigger it on province selection, history entry or date change
      LoadGuiEvents.ProvHistoryLoadAction += ((IPropertyControl<Province, TProperty>)this).LoadToGui;
   }

   public PropertyInfo PropertyInfo { get; init; }
   public PCFL_TokenParseDelegate AddEffectDelegate { get; init; }
   public PCFL_TokenParseDelegate RemoveEffectDelegate { get; init; }

   internal ICollection<Saveable> GetSaveables() => Selection.GetSelectedProvincesAsSaveable();

   public virtual void SetFromGui()
   {
      if (Globals.State != State.Running || !GetFromGui(out var value).Log())
         return;
      // TODO implement a history command
   }
   public abstract void SetDefault();
   public abstract IErrorHandle GetFromGui(out TProperty value);
   public abstract void SetValue(TProperty value);
}

public abstract class PrvHisSetOptDualEffPropControl<TProperty> : PrvHistDualEffectPropControl<TProperty> where TProperty : notnull
{
   public PCFL_TokenParseDelegate SetEffectDelegate { get; init; } 
   public PrvHisSetOptDualEffPropControl(PropertyInfo propertyInfo,
                                         PCFL_TokenParseDelegate setEffectDelegate,
                                         PCFL_TokenParseDelegate addEffectDelegate,
                                         PCFL_TokenParseDelegate removeEffectDelegate) : base(propertyInfo, addEffectDelegate, removeEffectDelegate)
   {
      SetEffectDelegate = setEffectDelegate;
   }
}
