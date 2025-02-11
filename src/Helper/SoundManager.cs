using System.Media;
using System.Runtime.InteropServices;
using Editor.ErrorHandling;

namespace Editor.Helper
{
   public static class SoundManager
   {
      private static readonly HashSet<string> ValidExtensions = [".wav", ".mp3"];
      private static readonly Dictionary<DefaultSounds, MemoryStream> Sounds = [];

      public enum DefaultSounds
      {
         G_OKClick,
         G_CancelClick,
         CloseWindow,
         TabChanged,
         PopUp,
      }

      public static void PlaySound(DefaultSounds sound)
      {
         if (Sounds.TryGetValue(sound, out var memoryStream))
         {
            memoryStream.Position = 0; // Reset stream position
            using var player = new SoundPlayer(memoryStream);
            player.Play();
         }
         else
            AddToDictionary(sound);
      }

      public static void PlaySound(DefaultSounds sound, float volume)
      {
         if (Sounds.TryGetValue(sound, out var memoryStream))
         {
            memoryStream.Position = 0; // Reset stream position
            using var player = new SoundPlayer(memoryStream);
            player.Play();
            SetVolume(1, 1);
         }
         else
            AddToDictionary(sound);
      }

      private static void AddToDictionary(DefaultSounds sound)
      {
         switch (sound)
         {
            case DefaultSounds.G_OKClick:
               if (!Sounds.ContainsKey(sound) && FilesHelper.GetVanillaPath(out var path, "sound", "general_ok_button_click.wav"))
                  Sounds.Add(sound, new(File.ReadAllBytes(path)));
               break;
            case DefaultSounds.G_CancelClick:
               if (!Sounds.ContainsKey(sound) && FilesHelper.GetVanillaPath(out path, "sound", "general_back_button_click.wav"))
                  Sounds.Add(sound, new(File.ReadAllBytes(path)));
               break;
            case DefaultSounds.CloseWindow:
               if (!Sounds.ContainsKey(sound) && FilesHelper.GetVanillaPath(out path, "sound", "close_window.wav"))
                  Sounds.Add(sound, new(File.ReadAllBytes(path)));
               break;
            case DefaultSounds.TabChanged:
               if (!Sounds.ContainsKey(sound) && FilesHelper.GetVanillaPath(out path, "sound", "old", "unused", "switch_tab.wav"))
                  Sounds.Add(sound, new(File.ReadAllBytes(path)));
               break;
            case DefaultSounds.PopUp:
               if (!Sounds.ContainsKey(sound) && FilesHelper.GetVanillaPath(out path, "sound", "event_pop_up.wav"))
                  Sounds.Add(sound, new(File.ReadAllBytes(path)));
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(sound), sound, null);
         }

         PlaySound(sound);
      }

      public static void PlayAllSoundsOnce()
      {
         foreach (var sound in Enum.GetValues<DefaultSounds>())
            PlaySound(sound, 1);
      }

      public static IErrorHandle PlaySound(string str)
      {
         if (!File.Exists(str))
            return new ErrorObject(ErrorType.FileNotFound, $"The file {str} does not exist!");
         if (!ValidExtensions.Contains(Path.GetExtension(str)))
            return new ErrorObject(ErrorType.NotSupportedAudioExtension, $"The file {str} has an unsupported extension!");

         using var player = new System.Media.SoundPlayer(str);
         player.Play();

         return ErrorHandle.Success;
      }

      // Core Playback methods
      [DllImport("winmm.dll", EntryPoint = "waveOutSetVolume")]
      private static extern int WaveOutSetVolume(IntPtr hwo, uint dwVolume);

      private static void SetVolume(float leftVolume, float rightVolume)
      {
         // Convert volumes (0.0f to 1.0f) to uint (0x0000 - 0xFFFF)
         uint left = (uint)(leftVolume * 0xFFFF);
         uint right = (uint)(rightVolume * 0xFFFF);

         // Combine left and right volumes
         uint combinedVolume = (right << 16) | left;

         // Set volume
         WaveOutSetVolume(IntPtr.Zero, combinedVolume);
      }

      // Testing

      private static Thread panningThread;
      private static bool stopPanning = false;

      public static void StartPanning(string filePath)
      {
         stopPanning = false;
         PlaySound(filePath); // Play the sound in a loop or just once
         panningThread = new Thread(() =>
         {
            float leftVolume = 0.0f;
            float rightVolume = 1.0f;
            int steps = 100; // Number of steps for panning
            int delay = 50;  // Delay in milliseconds between steps

            for (int i = 0; i <= steps && !stopPanning; i++)
            {
               // Gradually adjust volumes
               leftVolume = i / (float)steps;
               rightVolume = 1.0f - leftVolume;
               SetVolume(leftVolume, rightVolume);

               Thread.Sleep(delay); // Small delay to make the panning smooth
            }
         });
         panningThread.Start();
      }

      private static void StopPanning()
      {
         stopPanning = true;
         panningThread?.Join(); // Wait for the thread to complete
         SetVolume(1.0f, 1.0f); // Reset to normal volume
      }
   }
}