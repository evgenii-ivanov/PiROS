using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class Process
    {
        public int Id
        {
            get;
            private set;
        }

        public MemorySegment MemorySegment
        {
            get;
            private set;
        }

        public Process(int id, MemorySegment memorySegment)
        {
            Id = id;
            MemorySegment = memorySegment;
        }
    }
}
