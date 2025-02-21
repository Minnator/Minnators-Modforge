namespace Editor.DataClasses.DataStructures
{
   /*
   public class RefBinding<T> : BaseBinding<T>
   {
      private T _data;

      public RefBinding(ref T data, Saveable saveable, string propertyName) : base(saveable, propertyName)
      {
         _data = data;
      }

      protected override T Data
      {
         get => _data;
         set => _data = value;
      }
   }

   public class FuncBinding<T> : BaseBinding<T>
   {
      private readonly Func<Control, T> _get;
      private readonly Action<Control, T> _set;
      private Control _control;

      public FuncBinding(Control control, Action<Control, T> set, Saveable saveable, string propertyName) : base(saveable, propertyName)
      {
         _get = get;
         _set = set;
         _control = control;
      }

      public void UpdateControl(Control control, EventHandler handler)
      {
         _control = control;
         Handler -= OnControlChange;
         Handler = handler;
         Handler += OnControlChange;
      }

      protected override T Data
      {
         get => _get.Invoke(_control);
         set => _set.Invoke(_control, value);
      }
   }

   public abstract class BaseBinding<T>
   {
      protected abstract T Data { get; set; }

      private readonly Saveable _saveable;
      private readonly PropertyInfo _propertyInfo;
      protected EventHandler Handler;
      
      private bool _isUpdating = false;

      protected BaseBinding(EventHandler handler, Saveable saveable, string propertyName)
      {
         _saveable = saveable;
         _propertyInfo = saveable.GetType().GetProperty(propertyName)!;
         Debug.Assert(_propertyInfo != null, "PropertyInfo != null");
         Debug.Assert(handler != null, "handler != null");
         Handler = handler;
         Enable();
      }

      public virtual void Disable()
      {
        _saveable.AddToPropertyChanged(OnPropertyChanged);
      }

      public virtual void Enable()
      {
         _saveable.AddToPropertyChanged(OnPropertyChanged);
      }

      public virtual void OnPropertyChanged(object? saveable, string propertyName)
      {
         if (_isUpdating || !propertyName.Equals(_propertyInfo.Name) || !Equals(saveable, _saveable))
               return;
         _isUpdating = true;
         Data = _saveable.GetProperty<T>(_propertyInfo);
         _isUpdating = false;
      }
   }
   */
}