using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class MemorySegment
    {
        public int StartAddress
        {
            get;
            private set;
        }

        public int EndAddress
        {
            get;
            private set;
        }

        public MemorySegmentType Type
        {
            get;
            private set;
        }

        public MemorySegment(int startAddress, int endAddress, MemorySegmentType type)
        {
            StartAddress = startAddress;
            EndAddress = endAddress;
            Type = type;
        }
    }
}
