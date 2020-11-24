using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            var sr = new StreamReader("D:\\config.json");
            var configString = sr.ReadToEnd();
            sr.Close();

            var config = JsonSerializer.Deserialize<Configuration>(configString);

            CommandGenerator generator = new CommandGenerator();

            generator.CommandsNumber = config.CommandNumber;
            generator.CreationTimeRange = new Tuple<int, int>(config.MinimalCreationTime, config.MaximalCreationTime);
            generator.LiveTimeRange = new Tuple<int, int>(config.MinimalLiveTime, config.MaximalLiveTime);
            generator.BPLiveTimeRange = new Tuple<int, int>(config.MinimalBPLiveTime, config.MaximalBPLiveTime);
            generator.MemoryRequiredRange = new Tuple<int, int>(config.MinimalMemoryRequired, config.MaximalMemoryRequired);
            generator.BPMemoryRequiredRange = new Tuple<int, int>(config.MinimalBPMemoryRequired, config.MaximalBPMemoryRequired);
            generator.PercentageOfBackgroundProcess = config.PercentageOfBackgroundProcesses;

            if (config.GenerateNewCommands)
            {
                generator.GenerateCommands();
                generator.SaveCommandsInFile();
            }
            else
            {
                generator.Commands = new List<Command>();
                var csr = new StreamReader("D:\\commands.txt");
                for(int i = 0; i < config.CommandNumber; ++i)
                {
                    var buffer = csr.ReadLine();
                    var strings = buffer.Split(' ');
                    generator.Commands.Add(new Command(
                        Int32.Parse(strings[0]),
                        Int32.Parse(strings[1]),
                        Int32.Parse(strings[2])
                        ));
                }
                csr.Close();
            }

            var memoryControllers = new List<MemoryController>()
            { 
                new FirstFitMemoryController(config.MemorySize),
                new NextFitMemoryController(config.MemorySize),
                new BestFitMemoryController(config.MemorySize),
                new WorstFitMemoryController(config.MemorySize)
            };

            foreach (var memoryController in memoryControllers)
            {
                var simulator = new Simulator(memoryController, generator, config.DefragmentationTime, config.OutOfStatisticsCommandNumber);
                simulator.Run();
                simulator.SaveLogs(memoryController.GetType().ToString() + "Logs.txt");
                simulator.CountStatistics(memoryController.GetType().ToString() + "Statistics.txt");
            }
        }
    }
}
