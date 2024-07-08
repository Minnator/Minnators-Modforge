﻿using System;

namespace Editor.DataClasses;

public unsafe class ModifiableStack<T>
{

   private T[] _items = new T[1];

   public int Capacity
   {
      get => _items.Length;
      private set => Array.Resize(ref _items, value);
   }

   public int Count { get; private set; }

   public void Push(T item)
   {
      if (Count >= Capacity)
         Capacity *= 2;
      _items[Count++] = item;
   }

   public T Pop()
   {
      if (Count == 0) 
         throw new InvalidOperationException("The stack is empty");
      return _items[--Count];
   }

   public T Peek()
   {
      if (Count == 0) 
         throw new InvalidOperationException("The stack is empty");
      return _items[Count - 1];
   }

   public T* PeekRef()
   {
      if (Count == 0) 
         throw new InvalidOperationException("The stack is empty");
      fixed (T* ptr = &_items[Count - 1])
      {
         return ptr;
      }
   }

   public void Clear()
   {
      Count = 0;
   }

   public bool Contains(T item)
   {
      for (var i = 0; i < Count; i++)
      {
         if (_items == null)
            continue;
         if (_items[i]!.Equals(item)) 
            return true;
      }
      return false;
   }

   public T[] ToArray()
   {
      return _items;
   }

   public void TrimExcess()
   {
      if (Count < Capacity)
         Array.Resize(ref _items, Count);
   }


}