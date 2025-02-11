using System.ComponentModel;

namespace Editor.Controls.EXT
{
   public class EXT_TextBox : TextBox
   {
      private const int DEFAULT_TIMER_INTERVAL = 1000;

      private bool _useTimer = true;
      private int _timerInterval = DEFAULT_TIMER_INTERVAL;
      private bool _isTimerRunning = false;
      private bool _confirmOnReturn = true;
      private bool _cancelOnEsc = true;


      [Category("MMF EXT_Behavior")]
      public bool UseTimer
      {
         get => _useTimer;
         set => _useTimer = value;
      }

      [Category("MMF EXT_Behavior")]
      public int TimerInterval
      {
         get => _timerInterval;
         set
         {
            _timerInterval = value;
            _timer.Interval = value;
         }
      }

      [Browsable(false)]
      public bool IsTimerRunning
      {
         get => _isTimerRunning;
         private set => _isTimerRunning = value;
      }

      [Category("MMF EXT_Behavior")]
      public bool ConfirmOnReturn
      {
         get => _confirmOnReturn;
         set => _confirmOnReturn = value;
      }

      [Category("MMF EXT_Behavior")]
      public bool CancelOnEsc
      {
         get => _cancelOnEsc;
         set => _cancelOnEsc = value;
      }

      public event EventHandler<string>? ConfirmInput;
      public event EventHandler<string>? CancelInput;

      protected virtual void OnConfirmInput() => ConfirmInput?.Invoke(this, Text);
      protected virtual void OnCancelInput() => CancelInput?.Invoke(this, Text);

      private readonly System.Windows.Forms.Timer _timer = new() { Interval = DEFAULT_TIMER_INTERVAL };

      public EXT_TextBox()
      {
         KeyDown += KeyDownLogic;

         _timer.Tick += TimerTick;
      }

      private void TimerTick(object? sender, EventArgs e)
      {
         ResetTimer();
         OnConfirmInput();
      }

      private void KeyDownLogic(object? sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Return && ConfirmOnReturn)
         {
            OnConfirmInput();
            return;
         }

         if (e.KeyCode == Keys.Escape && CancelOnEsc)
         {
            OnCancelInput();
            Text = string.Empty;
            return;
         }

         if (UseTimer)
         {
            _timer.Stop();  
            _timer.Start();
            IsTimerRunning = true;
         }
      }

      public void ResetTimer()
      {
         _timer.Stop();
         IsTimerRunning = false;
      }
   }
}