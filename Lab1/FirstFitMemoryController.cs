using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class FirstFitMemoryController : MemoryController
    {
        public FirstFitMemoryController(int memorySize) : base(memorySize)
        { }

        public override int Search(int processSize)
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
    }
}
