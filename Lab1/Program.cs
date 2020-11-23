using System;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandGenerator generator = new CommandGenerator();
            generator.CommandsNumber = 10000;
            generator.CreationTimeRange = new Tuple<int, int>(1, 1000000);
            generator.LiveTimeRange = new Tuple<int, int>(1, 1000);
            generator.MemoryRequiredRange = new Tuple<int, int>(10, 2000000);


        }
    }
}
