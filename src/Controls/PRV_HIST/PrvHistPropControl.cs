using System;
using System.Diagnostics;
using System.Reflection;
using Editor.Controls.PROPERTY;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;
using Editor.Saving;

namespace Editor.Controls.PRV_HIST;

/*
 * How it shall work
 * The control will display the state of a province
 * The control will have a historyLoadHandle which will be triggered on province selection, history entry or date change
 * The control has at least one TokenEffect which will be added to a history entry on data confirmation
 *
 */


public abstract class IPrvHistSimpleEffectPropControl<TProperty> : IPropertyControl<Province, TProperty> where TProperty : notnull
{
   public PropertyInfo PropertyInfo { get; init; }
   public SimpleEffect<TProperty> Effect { get; init; } // and empty constructed SimpleEffect<TProperty> object is provided via constructor

   protected IPrvHistSimpleEffectPropControl(PropertyInfo propertyInfo, SimpleEffect<TProperty> effect)
   {
      PropertyInfo = propertyInfo;
      Effect = effect;
      // Register the load action for the property control so we can trigger it on province selection, history entry or date change
      LoadGuiEvents.ProvHistoryLoadAction += ((IPropertyControl<Province, TProperty>)this).LoadToGui;
   }

   internal void ParseEffectValue(string value) => Effect.Parse(new(Effect.GetTokenName(), value, -1), PathObj.Empty, ParsingContext.ProvinceEmpty);

   public abstract IErrorHandle GetFromGui(out TProperty value);
   public abstract void SetValue(TProperty value);
   public abstract void SetFromGui();
   public abstract void SetDefault();
}

public abstract class PrvHistDualEffectPropControl<TProperty> : IPropertyControl<Province, TProperty> where TProperty : notnull
{
   protected PrvHistDualEffectPropControl(PropertyInfo propertyInfo, SimpleEffect<TProperty> addEffect, SimpleEffect<TProperty> removeEffect)
   {
      PropertyInfo = propertyInfo;
      AddEffect = addEffect;
      RemoveEffect = removeEffect;
      // Register the load action for the property control so we can trigger it on province selection, history entry or date change
      LoadGuiEvents.ProvHistoryLoadAction += ((IPropertyControl<Province, TProperty>)this).LoadToGui;
   }

   public PropertyInfo PropertyInfo { get; init; }
   public SimpleEffect<TProperty> AddEffect { get; init; }
   public SimpleEffect<TProperty> RemoveEffect { get; init; }

   internal void ParseAddEffectValue(string value, bool add)
   {
      if (add)
         AddEffect.Parse(new(AddEffect.GetTokenName(), value, -1), PathObj.Empty, ParsingContext.ProvinceEmpty);
      else
         RemoveEffect.Parse(new(RemoveEffect.GetTokenName(), value, -1), PathObj.Empty, ParsingContext.ProvinceEmpty);
      HistoryEntry
   }

   internal ICollection<Saveable> GetSaveables() => Selection.GetSelectedProvincesAsSaveable();

   public abstract void SetFromGui();
   public abstract void SetDefault();
   public abstract IErrorHandle GetFromGui(out TProperty value);
   public abstract void SetValue(TProperty value);
}