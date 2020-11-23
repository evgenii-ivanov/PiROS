using System;
using System.Collections.Generic;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandGenerator generator = new CommandGenerator();

            generator.CommandsNumber = 10000;
            generator.CreationTimeRange = new Tuple<int, int>(1, 1000000);
            generator.LiveTimeRange = new Tuple<int, int>(1, 10000);
            generator.BPLiveTimeRange = new Tuple<int, int>(1, 100000);
            generator.MemoryRequiredRange = new Tuple<int, int>(10, 2000000);
            generator.BPMemoryRequiredRange = new Tuple<int, int>(10, 5000);
            generator.PercentageOfBackgroundProcess = 93;

            generator.GenerateCommands();
            generator.SaveCommandsInFile();

            int memorySize = 8000000;
            int defragmentatinTime = 5;

            var memoryControllers = new List<MemoryController>()
            { 
                new FirstFitMemoryController(memorySize),
                new NextFitMemoryController(memorySize),
                new BestFitMemoryController(memorySize),
                new WorstFitMemoryController(memorySize)
            };

            foreach (var memoryController in memoryControllers)
            {
                var simulator = new Simulator(memoryController, generator, defragmentatinTime);
                simulator.Run();
                simulator.SaveLogs(memoryController.GetType().ToString() + "Logs.txt");
                simulator.CountStatistics(memoryController.GetType().ToString() + "Statistics.txt");
            }
        }
    }
}
