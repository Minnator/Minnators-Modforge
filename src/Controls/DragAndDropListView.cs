namespace Editor.Controls
{
   public class SwappEventArgs(int from, int to) : EventArgs
   {
      public int From { get; } = from;

      public int To { get; } = to;
   }

   public sealed class DragAndDropListView : ListView
   {
      private int draggedIndex = -1;

      public event EventHandler<SwappEventArgs>? ItemMoved;

      public DragAndDropListView()
      {
         AllowDrop = true;
         View = View.List;
         FullRowSelect = true;
         MultiSelect = false;
         ItemDrag += ListView_ItemDrag;
         DragEnter += ListView_DragEnter;
         DragDrop += ListView_DragDrop;
      }

      private void ListView_ItemDrag(object? sender, ItemDragEventArgs e)
      {
         draggedIndex = ((ListView)sender!).Items.IndexOf((ListViewItem)e.Item!);
         var num = (int)DoDragDrop(e.Item!, DragDropEffects.Move);
      }

      private void ListView_DragEnter(object? sender, DragEventArgs e)
      {
         if (!e.Data!.GetDataPresent(typeof(ListViewItem)))
            return;
         e.Effect = DragDropEffects.Move;
      }

      private void ListView_DragDrop(object? sender, DragEventArgs e)
      {
         if (draggedIndex < 0)
            return;
         var client = PointToClient(new(e.X, e.Y));
         var itemAt = GetItemAt(client.X, client.Y);
         if (itemAt != null)
         {
            var index = itemAt.Index;
            var listViewItem = Items[draggedIndex];
            Items.RemoveAt(draggedIndex);
            Items.Insert(index, listViewItem);
            OnItemMoved(index, draggedIndex);
         }
         draggedIndex = -1;
      }

      private void OnItemMoved(int from, int to)
      {
         ItemMoved?.Invoke(this, new(from, to));
      }
   }
}
