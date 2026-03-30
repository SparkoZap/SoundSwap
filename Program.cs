using System.Diagnostics;
using NAudio.CoreAudioApi;

/*
    Mute all other apps when the selected app hits a certain threshold audio, to maximize focus.
*/

namespace AudioManager;

public class Program
{
    public static void Main(string[] args)
    {
        // Capture Processes Outputting Sound.

        Dictionary<int, AudioSessionControl> processes = new();

        var deviceEnumerator = new MMDeviceEnumerator();
        var defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

        Console.WriteLine("\nSelect a Priority Audio Source");
        Console.WriteLine("---------------------------");

        var audioSession = defaultDevice.AudioSessionManager;
        for (int i = 0; i < audioSession.Sessions.Count; i++)
        {
            var session = audioSession.Sessions[i];

            var process = Process.GetProcessById((int)session.GetProcessID);
            
            string name = string.IsNullOrWhiteSpace(session.DisplayName) ? 
            process.ProcessName : session.DisplayName;

            Console.WriteLine($"{i}. {name}");
            processes.Add(i, session);
        }

        Console.Write("\nChoose From The List [1, 2, 3...]: ");
        
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out var selection) || !processes.ContainsKey(selection))
        {
            Console.WriteLine("Not a valid selection.");
            return;
        }

        var priority = processes[selection];
        Console.WriteLine($"You've selected {priority.DisplayName}");

        Dictionary<AudioSessionControl, float> originalVolumes = new();
        foreach (var proc in processes.Values)
        {
            if (proc != priority)
                originalVolumes[proc] = proc.SimpleAudioVolume.Volume;
        }

        while (true)
        {
            bool isPlaying = priority.AudioMeterInformation.MasterPeakValue > 0.0001f;

            foreach (var proc in processes.Values)
            {
                if (proc == priority) continue;

                proc.SimpleAudioVolume.Volume = isPlaying ? 0.30f : originalVolumes[proc];
            }

            Thread.Sleep(100);
        }
    }
}