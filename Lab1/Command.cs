using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class Command : IComparable
    {
        public int CreationTime
        {
            get;
            private set;
        }
        public int LiveTime
        {
            get;
            private set;
        }
        public int MemoryRequired
        {
            get;
            private set;
        }

        public Command(int creationTime, int liveTime, int memoryRequired)
        {
            CreationTime = creationTime;
            LiveTime = liveTime;
            MemoryRequired = memoryRequired;
        }

        public int CompareTo(object o)
        {
            Command c = o as Command;
            if (c != null)
                return this.CreationTime.CompareTo(c.CreationTime);
            else
                throw new ArgumentNullException();
        }

    }
}
