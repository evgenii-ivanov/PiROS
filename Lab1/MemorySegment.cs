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

        public static MemorySegment operator +(MemorySegment memorySegment1, MemorySegment memorySegment2)
        {
            int startAddress = Math.Min(memorySegment1.StartAddress, memorySegment2.StartAddress);
            int endAddress = Math.Max(memorySegment1.EndAddress, memorySegment2.EndAddress);
            return new MemorySegment(startAddress, endAddress, MemorySegmentType.Hole);
        }
    }
}
