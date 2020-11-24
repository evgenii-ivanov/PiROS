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

            Console.WriteLine("Configure command generator");

            generator.CommandsNumber = config.CommandNumber;
            generator.CreationTimeRange = new Tuple<int, int>(config.MinimalCreationTime, config.MaximalCreationTime);
            generator.LiveTimeRange = new Tuple<int, int>(config.MinimalLiveTime, config.MaximalLiveTime);
            generator.BPLiveTimeRange = new Tuple<int, int>(config.MinimalBPLiveTime, config.MaximalBPLiveTime);
            generator.MemoryRequiredRange = new Tuple<int, int>(config.MinimalMemoryRequired, config.MaximalMemoryRequired);
            generator.BPMemoryRequiredRange = new Tuple<int, int>(config.MinimalBPMemoryRequired, config.MaximalBPMemoryRequired);
            generator.PercentageOfBackgroundProcess = config.PercentageOfBackgroundProcesses;

            if (config.GenerateNewCommands)
            {
                Console.WriteLine("Commands generation is started");
                generator.GenerateCommands();
                generator.SaveCommandsInFile();
                Console.WriteLine("Commands generation is finished");
            }
            else
            {
                Console.WriteLine("Commands reading");
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
                Console.WriteLine("Commands are read");
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
                Console.WriteLine($"Simulator runs {memoryController.GetType().ToString()}");
                simulator.Run();
                Console.WriteLine($"{memoryController.GetType().ToString()} simulation is finished");
                var logFileName = $"{memoryController.GetType().ToString()}Logs.txt";
                var statisticsFileName = $"{memoryController.GetType().ToString()}Statistics.txt";
                simulator.SaveLogs(logFileName);
                simulator.CountStatistics(statisticsFileName);
                Console.WriteLine($"You can find log in {logFileName}");
                Console.WriteLine($"You can find statisitics in {statisticsFileName}");
            }
        }
    }
}
