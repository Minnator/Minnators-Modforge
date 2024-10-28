using Editor.DataClasses;
using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Interfaces;

public abstract class ProvinceCollection<T>(string name, Color color) : ProvinceComposite(name, color) where T : ProvinceComposite 
// Area, Region, SuperRegion, TradeNode, TradeCompany, Continent, ProvinceGroup
{
   private ICollection<T> _subCollection = [];

   protected ICollection<T> SubCollection
   {
      get => _subCollection;
      set
      {
         AddRange(value);
         SetBounds();
      }
   }

   public abstract void Invoke(CProvinceCollectionType type, ProvinceComposite composite);
   public abstract void AddToEvent(CProvinceCollectionType type, EventHandler<ProvinceComposite> eventHandler);

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
         if (Globals.State == State.Running)
            Invoke(CProvinceCollectionType.Modify, composite);
      }
      else
      {
         if (Globals.State == State.Running)
            Invoke(CProvinceCollectionType.Add, composite);
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
         if (Globals.State == State.Running)
            Invoke(CProvinceCollectionType.Remove, composite);
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
}

public abstract class ProvinceComposite(string name, Color color) : Saveable// Province + IProvinceCollection
{
   public readonly string Name = name;

   public Color Color
   {
      get => _color;
      set
      {
         _color = value;
         if (Globals.State == State.Running)
            Invoke(this);
      }
   }

   public abstract void Invoke(ProvinceComposite composite);
   public abstract void AddToEvent(EventHandler<ProvinceComposite> handler);

   public List<ProvinceComposite> Parents = [];
   public Rectangle Bounds = Rectangle.Empty;
   private Color _color = color;
   public string GetTitleLocKey => Name;
   public string GetDescriptionLocKey => $"desc_{Name}";
   public abstract ICollection<Province> GetProvinces();
   public abstract ICollection<int> GetProvinceIds();
   public abstract Rectangle GetBounds();
   public abstract void SetBounds();

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
