namespace Editor.DataClasses.ObservableObjects
{
   public interface ObservableICollection<T> : ICollection<T>
   {
      public bool AddNotifyCommand(T item);
      public bool AddRangeNotifyCommand(T item);

      public bool RemoveNotifyCommand(T item);
      public bool RemoveRangeNotifyCommand(T item);
   }
}