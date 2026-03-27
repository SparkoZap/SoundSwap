using System.Diagnostics;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

/*
    Mute all other apps when the selected app hits a certain threshold audio, to maximize focus.
*/

namespace AudioManager;

public class Program
{
    public static void Main(string[] args)
    {
        // Capture Processes Outputting Sound.

        bool isPlaying = false;

        Dictionary<int, AudioSessionControl> processes = new();

        var deviceEnumerator = new MMDeviceEnumerator();
        var defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

        Console.WriteLine("\nSelect a Priority Audio Source");
        Console.WriteLine("---------------------------");

        var audioSession = defaultDevice.AudioSessionManager;
        for (int i = 0; i < audioSession.Sessions.Count; i++)
        {
            var process = Process.GetProcessById((int)audioSession.Sessions[i].GetProcessID);
            Console.WriteLine($"{i}. {process.ProcessName}");
            processes.Add(i, audioSession.Sessions[i]);
        }

        Console.Write("\nChoose From The List [1, 2, 3...]: ");

        var priority = processes[Convert.ToInt32(Console.ReadLine())];
        Console.WriteLine($"You've selected {priority.DisplayName}");

        while(true) 
        {
            if (priority.State.Equals(AudioSessionState.AudioSessionStateActive))
            {
                isPlaying = true;
            } 
            else
            {
                isPlaying = false;
            }

            foreach (var proc in processes.Values)
            {
                if (proc != priority)
                {
                    proc.SimpleAudioVolume.Volume = isPlaying ? 0 : 1;
                }
            }
        }
    }
}