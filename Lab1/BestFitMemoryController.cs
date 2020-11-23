using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class BestFitMemoryController : MemoryController
    {
        public BestFitMemoryController(int memorySize) : base(memorySize)
        { }

        public override int Search(int processSize)
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
    }
}
