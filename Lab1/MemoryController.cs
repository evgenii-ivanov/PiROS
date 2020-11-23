using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab1
{
    public abstract class MemoryController
    {
        protected readonly List<MemorySegment> memory;
        
        protected readonly Queue<Tuple<int, int>> processQueue;
        protected int _lastProcessId = 0;
        protected int lastIndex = -1;
        protected int _memorySize;

        public bool IsDefragmentationDone
        {
            get;
            set;
        }

        public List<Process> Processes
        {
            get;
            protected set;
        }

        public MemoryController(int memorySize)
        {
            memory = new List<MemorySegment>();
            memory.Add(new MemorySegment(0, memorySize - 1, MemorySegmentType.Hole));
            Processes = new List<Process>();
            _memorySize = memorySize;
            processQueue = new Queue<Tuple<int, int>>();
        }

        public abstract int Search(int processSize);

        public InsertionResult InsertProcess(int processSize)
        {
            if (processSize > _memorySize)
            {
                throw new Exception("Process size is too big");
            }
            if (!IsEnoughMemory(processSize))
            {
                _lastProcessId++;
                processQueue.Enqueue(new Tuple<int, int>(_lastProcessId, processSize));
                return new InsertionResult() { ProcessId = _lastProcessId, IsInserted = false };
            }
            var segmentIndex = Search(processSize);
            if (segmentIndex != -1)
            {
                InsertProcess(segmentIndex, processSize);
                _lastProcessId++;
                return new InsertionResult() { ProcessId = _lastProcessId, IsInserted = true };
            }
            if (segmentIndex < -1 && segmentIndex >= memory.Count)
            {
                throw new Exception("Check your segment search function");
            }
            Defragment();
            return InsertProcess(processSize);
        }

        private void InsertProcess(int segmentIndex, int size, int? id = null)
        {
            id ??= _lastProcessId + 1;
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
            var process = new Process(id.Value, processSegment);
            Processes.Add(process);
        }

        public List<int> RemoveProcess(int processId)
        {
            var process = Processes.Find(x => x.Id == processId);
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
                startAddress = memory[segmentIndex].StartAddress;
                memory.RemoveAt(segmentIndex);
            }
            var memorySegment = new MemorySegment(startAddress, endAddress, MemorySegmentType.Hole);
            memory.Insert(segmentIndex, memorySegment);
            Processes.Remove(process);
            var insertedIds = new List<int>();
            while (processQueue.Any() && PassProcess(processQueue.Peek().Item1, processQueue.Peek().Item2))
            {
                insertedIds.Add(processQueue.Peek().Item1);
                processQueue.Dequeue();
            }
            return insertedIds;
        }

        private bool PassProcess(int id, int processSize)
        {
            if (!IsEnoughMemory(processSize))
            {
                return false;
            }
            var segmentIndex = Search(processSize);
            if (segmentIndex != -1)
            {
                InsertProcess(segmentIndex, processSize, id);
                return true;
            }
            Defragment();
            segmentIndex = Search(processSize);
            InsertProcess(segmentIndex, processSize, id);
            return true;
        }

        private bool IsEnoughMemory(int processSize)
        {
            int availableMemory = _memorySize - memory.Sum(x => x.Type == MemorySegmentType.Process ? x.EndAddress - x.StartAddress + 1 : 0);
            return availableMemory >= processSize;
        }

        private void Defragment()
        {
            IsDefragmentationDone = true;
            int lastEmpty = -1;
            int removedProcessesNumber = 0;
            for (int i = 0; i < memory.Count; i++)
            {
                if (memory[i].Type == MemorySegmentType.Hole)
                {
                    continue;
                }
                var process = Processes.Find(x => x.MemorySegment.StartAddress == memory[i].StartAddress);
                var segmentSize = memory[i].EndAddress - memory[i].StartAddress + 1;
                memory.RemoveAt(i);
                var segment = new MemorySegment(lastEmpty + 1, lastEmpty + segmentSize, MemorySegmentType.Process);
                lastEmpty += segmentSize;
                memory.Insert(removedProcessesNumber++, segment);
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
