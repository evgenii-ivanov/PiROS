using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class WorstFitMemoryController : MemoryController
    {
        public WorstFitMemoryController(int memorySize) : base(memorySize)
        { }

        public override int Search(int processSize)
        {
            int worstSegmentIndex = -1;
            int worstSegmentSize = 0;
            for (int i = 0; i < memory.Count; i++)
            {
                var segment = memory[i];
                if (segment.Type != MemorySegmentType.Hole)
                    continue;
                int size = segment.EndAddress - segment.StartAddress + 1;
                if (size >= processSize && size > worstSegmentSize)
                {
                    worstSegmentIndex = i;
                    worstSegmentSize = size;
                }
            }
            return worstSegmentIndex;
        }
    }
}
