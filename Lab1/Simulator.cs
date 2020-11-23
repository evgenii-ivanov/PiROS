using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lab1
{
    public class Simulator
    {
        private MemoryController _memoryController;
        private CommandGenerator _commandGenerator;

        private SortedSet<Event> events;
        private List<string> logs;
        private int defragmentationsNumber;
        private int queuedProcessNumber;
        private int _defragmentationTime;

        public Simulator(MemoryController memoryController, CommandGenerator commandGenerator, int defragmentationTime)
        {
            _commandGenerator = commandGenerator;
            _memoryController = memoryController;
            events = new SortedSet<Event>();
            logs = new List<string>();
            defragmentationsNumber = 0;
            queuedProcessNumber = 0;
            _defragmentationTime = defragmentationTime;
        }

        public void Run()
        {
            for(int i = 0; i < _commandGenerator.Commands.Count; ++i)
            {
                events.Add(new Event()
                {
                    EventType = EventType.Insertion,
                    Time = _commandGenerator.Commands[i].CreationTime,
                    CommandId = i
                });
            }

            var processCommandMatching = new SortedList<int, int>();

            while(events.Count > 0)
            {
                var currentEvent = events.Min;
                events.Remove(currentEvent);
                _memoryController.IsDefragmentationDone = false;
                switch(currentEvent.EventType)
                {
                    case EventType.Insertion:
                        {
                            var insertionResult = _memoryController.InsertProcess(_commandGenerator.Commands[currentEvent.CommandId].MemoryRequired);
                            processCommandMatching.Add(insertionResult.ProcessId, currentEvent.CommandId);
                            if(insertionResult.IsInserted)
                            {
                                events.Add(new Event()
                                {
                                    EventType = EventType.Removing,
                                    Time = currentEvent.Time + _commandGenerator.Commands[currentEvent.CommandId].LiveTime,
                                    ProcessId = insertionResult.ProcessId
                                });
                                logs.Add("Time " + currentEvent.Time.ToString() + " Process started. ProcessId = " + insertionResult.ProcessId.ToString());
                            }
                            else
                            {
                                queuedProcessNumber++;
                                logs.Add("Time " + currentEvent.Time.ToString() + " Process added to the queue. ProcessId = " + insertionResult.ProcessId.ToString());
                            }
                            break;
                        }
                    case EventType.Removing:
                        {
                            var removedIds = _memoryController.RemoveProcess(currentEvent.ProcessId.Value);
                            logs.Add("Time " + currentEvent.Time.ToString() + " Process removed. ProcessId = " + currentEvent.ProcessId.Value.ToString());
                            foreach (var id in removedIds)
                            {
                                events.Add(new Event()
                                {
                                    EventType = EventType.Removing,
                                    Time = currentEvent.Time + _commandGenerator.Commands[processCommandMatching[currentEvent.ProcessId.Value]].LiveTime,
                                    ProcessId = id
                                });
                                logs.Add("Time " + currentEvent.Time.ToString() + " Queued process started. ProcessId = " + id.ToString());
                            }
                            break;
                        }
                    default:
                        break;
                }
                if(_memoryController.IsDefragmentationDone)
                {
                    defragmentationsNumber++;
                    logs.Add("Time " + currentEvent.Time.ToString() + " Defragmentation was performed. Current number of defrafmentation  = " + defragmentationsNumber.ToString());
                }
            }
        }

        public void SaveLogs(string logFileName = "logs.txt")
        {
            try
            {
                StreamWriter sw = new StreamWriter("D:\\" + logFileName);
                foreach (var log in logs)
                    sw.WriteLine(log);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void CountStatistics(string statisticsFileName = "logs.txt")
        {
            try
            {
                StreamWriter sw = new StreamWriter("D:\\" + statisticsFileName);
                int processesNumber = _commandGenerator.CommandsNumber;
                sw.WriteLine("Number of processes : " + processesNumber);
                sw.WriteLine("Number of defragmentations : " + defragmentationsNumber.ToString() + " (" + (defragmentationsNumber / (double)processesNumber * 100).ToString() + "%)");
                sw.WriteLine("Number of queud processes : " + queuedProcessNumber.ToString() + " (" + (queuedProcessNumber / (double)processesNumber * 100).ToString() + "%)");
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
