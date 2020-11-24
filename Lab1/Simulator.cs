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
        private List<int> delayTime;
        private int defragmentationsNumber;
        private int queuedProcessNumber;
        private int _defragmentationTime;
        private int _outOfStatisticsCommandNumber;

        public Simulator(MemoryController memoryController, CommandGenerator commandGenerator, int defragmentationTime, int outOfStatisticsCommandNumber)
        {
            _commandGenerator = commandGenerator;
            _memoryController = memoryController;
            events = new SortedSet<Event>();
            logs = new List<string>();
            delayTime = new List<int>();
            defragmentationsNumber = 0;
            queuedProcessNumber = 0;
            _defragmentationTime = defragmentationTime;
            _outOfStatisticsCommandNumber = outOfStatisticsCommandNumber;

            for (int i = 0; i < _commandGenerator.CommandsNumber; ++i)
                delayTime.Add(0);
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
            int commandProcessedNumber = 0;


            while(events.Count > 0)
            {
                var currentEvent = events.Min;
                events.Remove(currentEvent);
                _memoryController.IsDefragmentationDone = false;
                switch(currentEvent.EventType)
                {
                    case EventType.Insertion:
                        {
                            commandProcessedNumber++;
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
                                if(commandProcessedNumber > _outOfStatisticsCommandNumber)
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
                                int commandId = processCommandMatching[currentEvent.ProcessId.Value];
                                events.Add(new Event()
                                {
                                    EventType = EventType.Removing,
                                    Time = currentEvent.Time + _commandGenerator.Commands[commandId].LiveTime,
                                    ProcessId = id
                                });
                                delayTime[commandId] += currentEvent.Time - _commandGenerator.Commands[commandId].CreationTime;
                                logs.Add("Time " + currentEvent.Time.ToString() + " Queued process started. ProcessId = " + id.ToString());
                            }
                            break;
                        }
                    default:
                        break;
                }
                if(_memoryController.IsDefragmentationDone && commandProcessedNumber > _outOfStatisticsCommandNumber)
                {
                    foreach(var cevent in events)
                    {
                        if (cevent.ProcessId.HasValue)
                            delayTime[processCommandMatching[cevent.ProcessId.Value]] += _defragmentationTime;
                        else
                            delayTime[cevent.CommandId] += _defragmentationTime;
                    }
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

                int processesNumber = _commandGenerator.CommandsNumber - _outOfStatisticsCommandNumber;
                sw.WriteLine("Number of processes : " + processesNumber.ToString());

                Int64 summaryLiveTime = 0;
                Int64 summaryDelayTime = 0;
                for (int i = _outOfStatisticsCommandNumber; i < _commandGenerator.CommandsNumber; ++i)
                {
                    summaryDelayTime += delayTime[i];
                    summaryLiveTime += _commandGenerator.Commands[i].LiveTime;
                }
                sw.WriteLine("Average runtime : " + ((summaryLiveTime + summaryDelayTime) / (double)processesNumber).ToString());

                sw.WriteLine("Number of defragmentations : " + defragmentationsNumber.ToString() + 
                    " (" + (defragmentationsNumber / (double)processesNumber * 100).ToString() + "%)");

                sw.WriteLine("Number of queud processes : " + queuedProcessNumber.ToString() + 
                    " (" + (queuedProcessNumber / (double)processesNumber * 100).ToString() + "%)");

                sw.WriteLine("Average delay : " + (summaryDelayTime / (double)processesNumber).ToString());

                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
