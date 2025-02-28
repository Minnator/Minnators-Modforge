using System.Diagnostics;
using System.Runtime.InteropServices;
using Editor.DataClasses.Achievements;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Forms.PopUps;


public class AchievementPopup : Form
{
   private readonly Timer _showTimer;
   private readonly Timer _closeTimer;
   private readonly Timer _delayTimer;
   private readonly int _targetY;
   private const int SPEED = 3;
   private const int PADDING = 10;


   private static bool _isShowing;
   private static Queue<Achievement> _queue = new();
   private static Timer _queueTimer = new() { Interval = 5000 }; // we can show an achievement every 5 seconds

   static AchievementPopup()
   {
      _queueTimer.Tick += (_, _) => ShowNext();
   }

   public static void Show(Achievement achievement)
   {
      _queue.Enqueue(achievement);

      if (!_isShowing)
      {
         _isShowing = true;
         ShowNext();
      }
   }

   private static void ShowNext()
   {
      if (_queue.Count == 0)
      {
         _queueTimer.Stop();
         _isShowing = false;
         return;
      }

      var achievement = _queue.Dequeue();
      var popup = new AchievementPopup(achievement.Name, achievement.Icon);
      popup.Show();
      _queueTimer.Start();
   }

   private AchievementPopup(string text, Image icon)
   {
      FormBorderStyle = FormBorderStyle.None;
      StartPosition = FormStartPosition.Manual;
      base.BackColor = Color.Black;
      Opacity = 0; // Start invisible
      Size = new(250, 60);
      TopMost = false;

      Location = new(Screen.PrimaryScreen!.WorkingArea.Width - Width - PADDING, Screen.PrimaryScreen.WorkingArea.Height);
      _targetY = Screen.PrimaryScreen.WorkingArea.Height - Height - PADDING;

      var iconBox = new PictureBox
      {
         Image = icon,
         SizeMode = PictureBoxSizeMode.Zoom,
         Size = new(40, 40),
         Location = new(PADDING, PADDING)
      };

      var textLabel = new Label
      {
         Text = text,
         ForeColor = Color.White,
         Font = new("Arial", 12, FontStyle.Bold),
         AutoSize = false,
         Size = new(180, 40),
         Location = new(60, PADDING),
         TextAlign = ContentAlignment.MiddleLeft
      };

      Controls.Add(iconBox);
      Controls.Add(textLabel);

      // Timers for animation
      _showTimer = new() { Interval = 15 };
      _showTimer.Tick += ShowTimer_Tick;
      _showTimer.Start();

      _closeTimer = new() { Interval = 15 };
      _closeTimer.Tick += CloseTimer_Tick;

      _delayTimer = new() { Interval = 3000 };

      _delayTimer.Tick += (_, _) =>
      {
         _delayTimer.Stop();
         _closeTimer.Start();
      };
   }

   private void ShowTimer_Tick(object? sender, EventArgs e)
   {
      if (Opacity < 1)
         Opacity += 0.05;
      if (Top > _targetY)
         Top -= SPEED;

      if (Top <= _targetY && Opacity >= 1)
      {
         _showTimer.Stop();
         TopMost = true;
         _delayTimer.Start();
      }
      TopMost = true;
   }

   private void CloseTimer_Tick(object? sender, EventArgs e)
   {
      if (Opacity > 0)
         Opacity -= 0.05;
      else
      {
         _closeTimer.Stop();
         Close();
      }
   }
}