using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class Configuration
    {
        public int CommandNumber
        {
            set;
            get;
        }
        public int OutOfStatisticsCommandNumber
        {
            set;
            get;
        }
        public int MinimalCreationTime
        {
            set;
            get;
        }
        public int MaximalCreationTime
        {
            set;
            get;
        }
        public int MinimalLiveTime
        {
            set;
            get;
        }
        public int MaximalLiveTime
        {
            set;
            get;
        }
        public int MinimalBPLiveTime
        {
            set;
            get;
        }
        public int MaximalBPLiveTime
        {
            set;
            get;
        }
        public int MinimalMemoryRequired
        {
            set;
            get;
        }
        public int MaximalMemoryRequired
        {
            set;
            get;
        }
        public int MinimalBPMemoryRequired
        {
            set;
            get;
        }
        public int MaximalBPMemoryRequired
        {
            set;
            get;
        }
        public int PercentageOfBackgroundProcesses
        {
            set;
            get;
        }
        public int MemorySize
        {
            set;
            get;
        }
        public int DefragmentationTime
        {
            set;
            get;
        }
        public bool GenerateNewCommands
        {
            set;
            get;
        }
    }
}
/* Json script
{
  "CommandNumber": 10000,
  "OutOfStatisticsCommandNumber": 1000,
  "MinimalCreationTime": 1,
  "MaximalCreationTime": 1000000,
  "MinimalLiveTime": 1,
  "MaximalLiveTime": 10000,
  "MinimalBPLiveTime": 1,
  "MaximalBPLiveTime": 100000,
  "MinimalMemoryRequired": 10,
  "MaximalMemoryRequired": 2000000,
  "MinimalBPMemoryRequired": 10,
  "MaximalBPMemoryRequired": 5000,
  "PercentageOfBackgroundProcesses": 93,
  "MemorySize": 8000000,
  "DefragmentationTime": 5,
  "GenerateNewCommands": false
}

 */
