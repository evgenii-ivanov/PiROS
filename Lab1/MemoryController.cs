using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class MemoryController
    {
        private readonly List<MemorySegment> memory;
        private readonly List<Process> processes;
        private int lastProcessId = 0;

        public MemoryController(int memorySize)
        {
            memory = new List<MemorySegment>();
            memory.Add(new MemorySegment(0, memorySize, MemorySegmentType.Hole));
            processes = new List<Process>();
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
            lastProcessId++;
            var process = new Process(lastProcessId, processSegment);
        }

        private void RemoveProcess(int processId)
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
        }
    }
}
