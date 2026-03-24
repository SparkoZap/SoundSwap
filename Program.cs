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
        Console.WriteLine("\nSelect a Main Audio Source");
        Console.WriteLine("---------------------------");

        List<Process> processes = new();

        var deviceEnumerator = new MMDeviceEnumerator();
        var defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        var audioSession = defaultDevice.AudioSessionManager;
        for (int i = 0; i < audioSession.Sessions.Count; i++)
        {
            var process = Process.GetProcessById((int)audioSession.Sessions[i].GetProcessID);
            if (audioSession.Sessions[i].State == NAudio.CoreAudioApi.Interfaces.AudioSessionState.AudioSessionStateActive)
            {
                Console.WriteLine($"{i}. {process.ProcessName} is currently playing audio");
            }
        }
        Console.Write("\nChoose From The List [1, 2, 3...]: ");
        var answer = Console.ReadLine();
    }
}