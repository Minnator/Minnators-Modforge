namespace Editor.DataClasses.ObservableObjects
{
   public interface ObservableICollection<T> : ICollection<T>
   {
      public new void Clear();
      public new bool Add(T item);
      public bool AddRange(T item);
      public new bool Remove(T item);
      public bool RemoveRange(T item);
   }
}