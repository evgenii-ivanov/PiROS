using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class Event : IComparable
    {
        public EventType EventType
        {
            get;
            set;
        }

        public int Time
        {
            get;
            set;
        }

        public int? ProcessId
        {
            get;
            set;
        }
        public int CommandId
        {
            get;
            set;
        }

        public int CompareTo(object o)
        {
            var c = o as Event;
            if (c != null)
            {
                if(this.Time!=c.Time)
                    return this.Time.CompareTo(c.Time);
                if(this.EventType!=c.EventType)
                    return this.EventType.CompareTo(c.EventType);
                return this.CommandId.CompareTo(c.CommandId);
            }
            else
                throw new ArgumentNullException();
        }
    }
}
