using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class NextFitMemoryController : MemoryController
    {
        public NextFitMemoryController(int memorySize) : base(memorySize)
        { }

        public override int Search(int processSize)
        {
            for (int i = 0; i < memory.Count; i++)
            {
                int index = (i + lastIndex + 1) % memory.Count;
                var segment = memory[index];
                if (segment.Type != MemorySegmentType.Hole)
                    continue;
                if (segment.EndAddress - segment.StartAddress + 1 >= processSize)
                {
                    lastIndex = index;
                    return index;
                }
            }
            lastIndex = -1;
            return -1;
        }
    }
}
