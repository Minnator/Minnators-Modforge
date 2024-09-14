namespace Editor
{
   public class ObservableList<T>
   {
      private readonly List<T> _list = [];

      public event Action<T>? ItemAdded;

      public void Add(T item)
      {
         _list.Add(item);
         OnItemAdded(item); 
      }

      protected virtual void OnItemAdded(T item)
      {
         ItemAdded?.Invoke(item);
      }

   }
}