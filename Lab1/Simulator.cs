using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    class Simulator
    {
        private MemoryController MemoryController
        {
            get;
            set;
        }
        private CommandGenerator CommandGenerator
        {
            get;
            set;
        }

        Simulator(MemoryController memoryController, CommandGenerator commandGenerator)
        {
            CommandGenerator = commandGenerator;
            MemoryController = memoryController;
        }

        public void Run()
        {

        }

        public void SaveLogs(string logFileName = "logs.txt")
        {

        }
    }
}
