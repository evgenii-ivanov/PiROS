using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab1
{
    public class MemoryController
    {
        private readonly List<MemorySegment> memory;
        private readonly List<Process> processes;
        private readonly Queue<int> processQueue;
        private int _lastProcessId = 0;
        private int lastIndex = -1;
        private int _memorySize;
        private Func<int, int> defaultSegmentSearchFunc;

        public MemoryController(int memorySize, Func<int, int> segmentSearchFunc)
        {
            memory = new List<MemorySegment>();
            memory.Add(new MemorySegment(0, memorySize, MemorySegmentType.Hole));
            processes = new List<Process>();
            _memorySize = memorySize;
            defaultSegmentSearchFunc = segmentSearchFunc;
        }

        public int FirstFitSearch(int processSize)
        {
            for (int i = 0; i < memory.Count; i++)
            {
                var segment = memory[i];
                if (segment.Type != MemorySegmentType.Hole)
                    continue;
                if (segment.EndAddress - segment.StartAddress + 1 >= processSize)
                {
                    return i;
                }
            }
            return -1;
        }

        public int NextFitSearch(int processSize)
        {
            for (int i = 0; i < memory.Count; i++)
            {
                int index = (i + lastIndex + 1) % memory.Count;
                var segment = memory[index];
                if (segment.Type != MemorySegmentType.Hole)
                    continue;
                if (segment.EndAddress - segment.StartAddress + 1 >= processSize)
                {
                    lastIndex = i;
                    return i;
                }
            }
            lastIndex = -1;
            return -1;
        }

        public int BestFitSearch(int processSize)
        {
            int bestSegmentIndex = -1;
            int bestSegmentSize = _memorySize + 2;
            for (int i = 0; i < memory.Count; i++)
            {
                var segment = memory[i];
                if (segment.Type != MemorySegmentType.Hole)
                    continue;
                int size = segment.EndAddress - segment.StartAddress + 1;
                if (size >= processSize && size < bestSegmentSize)
                {
                    bestSegmentIndex = i;
                    bestSegmentSize = size;
                }
            }
            return bestSegmentIndex;
        }

        public int WorstFitSearch(int processSize)
        {
            int bestSegmentIndex = -1;
            int bestSegmentSize = 0;
            for (int i = 0; i < memory.Count; i++)
            {
                var segment = memory[i];
                if (segment.Type != MemorySegmentType.Hole)
                    continue;
                int size = segment.EndAddress - segment.StartAddress + 1;
                if (size >= processSize && size > bestSegmentSize)
                {
                    bestSegmentIndex = i;
                    bestSegmentSize = size;
                }
            }
            return bestSegmentIndex;
        }

        public bool InsertProcess(int processSize)
        {
            if (processSize > _memorySize)
            {
                throw new Exception("Process size is too big");
            }
            if (!IsEnoughMemory(processSize))
            {
                processQueue.Append(processSize);
                return false;
            }
            var segmentIndex = defaultSegmentSearchFunc(processSize);
            if (segmentIndex != -1)
            {
                InsertProcess(segmentIndex, processSize);
                return true;
            }
            if (segmentIndex < 0 && segmentIndex >= memory.Count)
            {
                throw new Exception("Check your segment search function");
            }
            Defragment(processSize);
            return InsertProcess(processSize);
        }

        private void InsertProcess(int segmentIndex, int size)
        {
            if (memory[segmentIndex].Type != MemorySegmentType.Hole)
            {
                throw new Exception("Proccess can be inserted only into hole");
            }
            var startAddress = memory[segmentIndex].StartAddress;
            var endAddress = memory[segmentIndex].EndAddress;
            int holeSize = endAddress - startAddress + 1;
            if (holeSize < size)
            {
                throw new Exception("No enough memory");
            }
            var processSegment = new MemorySegment(startAddress, startAddress + size - 1, MemorySegmentType.Process);
            memory.RemoveAt(segmentIndex);
            memory.Insert(segmentIndex, processSegment);
            if (holeSize != size)
            {
                var holeSegment = new MemorySegment(processSegment.EndAddress + 1, endAddress, MemorySegmentType.Hole);
                memory.Insert(segmentIndex + 1, holeSegment);
            }
            _lastProcessId++;
            var process = new Process(_lastProcessId, processSegment);
            processes.Add(process);
        }

        public void RemoveProcess(int processId)
        {
            var process = processes.Find(x => x.Id == processId);
            int startAddress = process.MemorySegment.StartAddress;
            int endAddress = process.MemorySegment.EndAddress;
            var segmentIndex = memory.FindIndex(x => x.StartAddress == startAddress && x.EndAddress == endAddress);
            memory.RemoveAt(segmentIndex);
            if (memory.Count > segmentIndex && memory[segmentIndex].Type == MemorySegmentType.Hole)
            {
                endAddress = memory[segmentIndex].EndAddress;
                memory.RemoveAt(segmentIndex);
            }
            if (segmentIndex >= 1 && memory[segmentIndex - 1].Type == MemorySegmentType.Hole)
            {
                segmentIndex--;
                startAddress = memory[segmentIndex ].StartAddress;
                memory.RemoveAt(segmentIndex);
            }
            var memorySegment = new MemorySegment(startAddress, endAddress, MemorySegmentType.Hole);
            memory.Insert(segmentIndex, memorySegment);
            processes.Remove(process);
            while (processQueue.Any() && InsertProcess(processQueue.Peek()))
            {
                processQueue.Dequeue();
            }
        }

        private bool IsEnoughMemory(int processSize)
        {
            int availableMemory = _memorySize - memory.Sum(x => x.Type == MemorySegmentType.Process ? x.EndAddress - x.StartAddress + 1 : 0);
            return availableMemory >= processSize;
        }

        private void Defragment(int processSize)
        {
            int lastEmpty = -1;
            int removedProcessesNumber = 0;
            for (int i = 0; i < memory.Count; i++)
            {
                if (memory[i].Type == MemorySegmentType.Hole)
                {
                    continue;
                }
                var process = processes.Find(x => x.MemorySegment.StartAddress == memory[i].StartAddress);
                var segmentSize = memory[i].EndAddress - memory[i].StartAddress + 1;
                memory.RemoveAt(i);
                var segment = new MemorySegment(lastEmpty + 1, lastEmpty + segmentSize, MemorySegmentType.Process);
                lastEmpty += segmentSize;
                memory.Insert(removedProcessesNumber, segment);
                process.Relocate(segment.StartAddress, segment.EndAddress);
            }
            if (lastEmpty != _memorySize)
            {
                memory.RemoveAll(x => x.Type == MemorySegmentType.Hole);
                var holeSegment = new MemorySegment(lastEmpty + 1, _memorySize, MemorySegmentType.Hole);
                memory.Add(holeSegment);
            }
        }
    }
}
