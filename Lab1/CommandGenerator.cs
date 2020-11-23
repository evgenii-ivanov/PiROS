using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    class CommandGenerator
    {
        public int CommandsNumber
        {
            get;
            set;
        }

        public List<Command> Commands
        {
            get;
            private set;
        }

        public Tuple<int, int> CreationTimeRange
        {
            get;
            set;
        }
        public Tuple<int, int> LiveTimeRange
        {
            get;
            set;
        }

        public Tuple<int, int> MemoryRequiredRange
        {
            get;
            set;
        }

        private int GetGaussianDistributedNumber(Random rand, Tuple<int, int> range)
        {
            int sum = 0;
            int cnt = 8;
            for (int i = 0; i < cnt; ++i)
                sum += rand.Next(range.Item1, range.Item2);
            return sum / cnt;
        }

        public void GenerateCommands()
        {
            var rand = new Random();
            for (int i = 0; i < CommandsNumber; ++i)
                Commands.Add(new Command(
                    rand.Next(CreationTimeRange.Item1, CreationTimeRange.Item2),
                    GetGaussianDistributedNumber(rand, LiveTimeRange),
                    GetGaussianDistributedNumber(rand, MemoryRequiredRange)));
            Commands.Sort();
        }

        public void SaveCommandsInFile(string fileName = "commands.txt")
        {
            try
            {
                StreamWriter sw = new StreamWriter("D:\\"+fileName);
                foreach (var command in Commands)
                    sw.WriteLine(command.ToString());
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
