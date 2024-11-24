using System.Collections;
using System.ComponentModel;
using Editor.DataClasses.Commands;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Saving;

namespace Editor.DataClasses.GameDataClasses;

public class ProvinceCollectionEventArguments<T>(ProvinceCollectionType type, ICollection<T> composite)
{
   public readonly ProvinceCollectionType Type = type;
   public readonly ICollection<T> Composite = composite;

}

// TODO add a flag to not edit the collection but the items in the collection
public abstract class ProvinceCollection<T> : ProvinceComposite where T : ProvinceComposite
   // Area, Region, SuperRegion, TradeNode, TradeCompany, Continent, ProvinceGroup, Country
{
   public ProvinceCollection (string name, Color color, ObjEditingStatus status = ObjEditingStatus.Modified) : base(name, color)
   {
      base.EditingStatus = status;
   }

   public ProvinceCollection(string name, Color color, ref PathObj path, ICollection<T> provinces) : this(name, color, ObjEditingStatus.Unchanged) 
   {
      SetPath(ref path);
      SubCollection = provinces;
   }

   private readonly ICollection<T> _subCollection = [];

   [Browsable(false)]
   internal ICollection<T> SubCollection
   {
      get => _subCollection;
      init
      {
         AddRange(value);
         SetBounds();
      }
   }

   public abstract void Invoke(ProvinceCollectionEventArguments<T> eventArgs);
   public abstract void AddToEvent(EventHandler<ProvinceCollectionEventArguments<T>> eventHandler);

   /// <summary>
   /// Recursively gets all provinces in the SubCollections
   /// </summary>
   /// <returns></returns>
   public override ICollection<Province> GetProvinces()
   {
      var provinces = new List<Province>();
      foreach (var subCollection in SubCollection)
         provinces.AddRange(subCollection.GetProvinces());
      return provinces;
   }

   public override ICollection<int> GetProvinceIds()
   {
      var ids = new List<int>();
      foreach (var subCollection in SubCollection)
         ids.AddRange(subCollection.GetProvinceIds());
      return ids;
   }

   public override Rectangle GetBounds()
   {
      List<Rectangle> childBounds = [];
      foreach (var subCollection in SubCollection)
         childBounds.Add(subCollection.GetBounds());
      return Geometry.GetBounds(childBounds);
   }

   public override void SetBounds() => Bounds = GetBounds();

   public void Add(T composite, bool setBounds = true)
   {
      var whatAmI = WhatAmI();
      var hadPreviousParent = false;
      var oldParent = Empty;

      foreach (var parent in composite.Parents)
      {
         if (parent.WhatAmI() == whatAmI)
         {
            oldParent = parent;
            ((ProvinceCollection<T>)parent).Remove(composite, setBounds, true);
            hadPreviousParent = true;
            break;
         }
      }
      composite.Parents.Add(this);
      _subCollection.Add(composite);
      if (setBounds)
         SetBounds();

      if (hadPreviousParent)
      {
         //if (Globals.State == State.Running)
         //   Invoke(ProvinceCollectionType.Modify, composite);
      }
      else
      {
         //if (Globals.State == State.Running)
         //   Invoke(ProvinceCollectionType.Add, composite);
      }
   }

   public void Remove(T composite, bool setBounds = true, bool toBeAdded = false)
   {
      composite.Parents.Remove(this);
      _subCollection.Remove(composite);
      if (setBounds)
         SetBounds();
      if (!toBeAdded)
      {
         //if (Globals.State == State.Running)
         //   Invoke(ProvinceCollectionType.Remove, composite);
      }
   }

   public void RemoveRange(ICollection<T> composites)
   {
      foreach (var composite in composites)
         Remove(composite, false);
      SetBounds();
   }

   public void AddRange(ICollection<T> composites)
   {
      foreach (var composite in composites)
         Add(composite, false);
      SetBounds();
   }

   public void NewAdd(T composite, bool addToGlobal = false, bool tryAddEventToHistory = true)
   {
      if (_subCollection.Contains(composite))
         return;
      var command = new CAddProvinceCollection<T>(this, addToGlobal);
      command.AddNewComposite(composite);
      ExecuteAndAdd(command, tryAddEventToHistory);
   }

   public void NewAddRange(ICollection<T> composites, bool addToGlobal = false, bool tryAddEventToHistory = true)
   {
      var command = new CAddProvinceCollection<T>(this, addToGlobal);
      foreach (var composite in composites)
      {
         if (_subCollection.Contains(composite))
            continue;
         command.AddNewComposite(composite);
      }
      ExecuteAndAdd(command, tryAddEventToHistory);
   }

   public void NewRemove(T composite, bool removeFromGlobal = false, bool tryAddEventToHistory = true)
   {
      if (!_subCollection.Contains(composite))
         return;
      var command = new CRemoveProvinceCollection<T>(this, removeFromGlobal);
      command.RemoveNewComposite(composite);
      ExecuteAndAdd(command, tryAddEventToHistory);
   }

   public void NewRemoveRange(ICollection<T> composites, bool removeFromGlobal = false, bool tryAddEventToHistory = true)
   {
      var command = new CRemoveProvinceCollection<T>(this, removeFromGlobal);
      foreach (var composite in composites)
      {
         if (!_subCollection.Contains(composite))
            continue;
         command.RemoveNewComposite(composite);
      }
      ExecuteAndAdd(command, tryAddEventToHistory);
   }

   public void NewRemoveFromGlobal()
   {
      NewRemoveRange(_subCollection, true);
   }

   public void ExecuteAndAdd(ICommand command, bool tryAddEventToHistory)
   {
      command.Execute();
      if (tryAddEventToHistory && Globals.State == State.Running)
      {
         Globals.HistoryManager.AddCommand(command);
      }
   }

   public void InternalAdd(T composite)
   {
      _subCollection.Add(composite);
   }

   public void InternalRemove(T composite)
   {
      _subCollection.Remove(composite);
   }

   public abstract void RemoveGlobal();
   public abstract void AddGlobal();

}

public abstract class ProvinceComposite(string name, Color color) : Saveable// Province + IProvinceCollection
{
   public readonly string Name = name;

   public virtual Color Color
   {
      get => _color;
      set
      {
         _color = value;
         if (Globals.State == State.Running)
            ColorInvoke(this);
      }
   }

   public abstract void ColorInvoke(ProvinceComposite composite);
   public abstract void AddToColorEvent(EventHandler<ProvinceComposite> handler);

   public List<ProvinceComposite> Parents = [];
   public Rectangle Bounds = Rectangle.Empty;
   private Color _color = color;
   [Browsable(false)]
   public string GetTitleLocKey => Name;
   [Browsable(false)]
   public string GetDescriptionLocKey => $"desc_{Name}";
   public abstract ICollection<Province> GetProvinces();
   public abstract ICollection<int> GetProvinceIds();
   public abstract Rectangle GetBounds();
   public abstract void SetBounds();

   public virtual ProvinceComposite GetFirstParentOfType(SaveableType type)
   {
      foreach (var parent in Parents)
      {
         if (parent.WhatAmI() == type)
            return parent;

         var recursiveParent = parent.GetFirstParentOfType(type);
         if (recursiveParent != Empty)
            return recursiveParent;
      }
      return Empty;
   }


   public override bool Equals(object? obj)
   {
      if (obj is ProvinceComposite other)
         return Name == other.Name && WhatAmI() == other.WhatAmI();
      return false;
   }

   public override int GetHashCode()
   {
      return HashCode.Combine(Name.GetHashCode(), WhatAmI());
   }

   public override string ToString()
   {
      return Name;
   }

   public static bool operator ==(ProvinceComposite left, ProvinceComposite right)
   {
      if (left is null)
         return right is null;
      return left.Equals(right);
   }

   public static bool operator !=(ProvinceComposite left, ProvinceComposite right)
   {
      if (left is null)
         return !(right is null);
      return !left.Equals(right);
   }

   public static ProvinceComposite Empty => Province.Empty;

}


// Province > Area > Region > SuperRegion 
// Province > TradeNode
// Province > TradeCompany
// Province > Continent
// Province > ProvinceGroup
