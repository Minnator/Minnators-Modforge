namespace Editor.Helper;

class ThumbButtonFilter : IMessageFilter
{
   const int WM_XBUTTONDOWN = 0x020B;

   public event Action<bool>? OnXButton1;
   public event Action<bool>? OnXButton2;

   public bool PreFilterMessage(ref Message m)
   {
      if (m.Msg != WM_XBUTTONDOWN) return false;

      var button = (m.WParam.ToInt32() >> 16) & 0xFFFF;
      var shift = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

      if (button == 1) 
         OnXButton1?.Invoke(shift);
      else if (button == 2) 
         OnXButton2?.Invoke(shift);

      return false; // don't block message
   }
}