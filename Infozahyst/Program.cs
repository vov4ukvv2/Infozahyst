// Program.cs
using BenchmarkDotNet.Running;
using Infozahyst;
using System;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        //var summary = BenchmarkRunner.Run<BenchmarkProgram>();    uncomment this code if you want to run Benchmark
        var server = new NetSdrServer();
        Thread serverThread = new Thread(server.Start);
        serverThread.Start();
        var netSdrClient = new NetSdrClient();
        Console.WriteLine("Select an action:");
        Console.WriteLine("1) Connect");
        Console.WriteLine("2) Disconnect");
        Console.WriteLine("3) Starting IQ transmission");
        Console.WriteLine("4) Stopping IQ transmission");
        Console.WriteLine("5) Changing the receiver frequency");
        while (true)
        {
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    netSdrClient.Connect();
                    break;
                case "2":
                    netSdrClient.Disconnect();
                    break;
                case "3":
                    netSdrClient.ControlReceiverState(true);
                    break;
                case "4":
                    netSdrClient.ControlReceiverState(false);
                    break;
                case "5":
                    netSdrClient.SetReceiverFrequency();
                    break;
                default:
                    Console.WriteLine("Error. Repead");
                    break;
            }
        }
    }
}
