using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    enum Type { MACHINE_1, MACHINE_2, MACHINE_3, ADD_TO_CRATE, MACHINE_4, BREAKDOWN_1, BREAKDOWN_3, BREAKDOWN_4, REPAIRED_1, REPAIRED_3, REPAIRED_4, END_OF_SIMULATION };

    class Event : IComparer<Event>, IComparable<Event>
    {
        public long Time { get; private set; }
        public Type Type { get; private set; }
        public Machine Machine { get; private set; } // M1A, M2B, M1C or M4A
        public int DVD { get; private set; }

        public Event(long time, Type type, Machine machine, int dvd)
        {
            Time = time;
            Type = type;
            Machine = machine;
            DVD = dvd;
        }

        // TODO maybe better to abstract such event that is not machine specific
        public Event(long time, Type type)
        {
            Time = time;
            Type = type;
        }

        public int Compare(Event x, Event y)
        {
            // Events are never equal = 0, otherwise duplicates are maybe not possible 
            // Done events are handled before init events
            
			if (x.Time < y.Time) {
                return -1;
            }
            else if (x.Time > y.Time)
            {
                return 1;
            }
            else
            {
                int equality = x.Type.CompareTo(y.Type);
                if (equality == 0)
                {
                    equality = x.Type.CompareTo(y.Type);
                    return equality == 0 ? -1 : equality;
                }
                else
                {
                    return equality;
                }
            }
        }

        public int CompareTo(Event e)
        {
            // Events are never equal = 0, otherwise duplicates are maybe not possible 
            // Done events are handled before init events

            // TODO add priority to which event is handeld first
            if (Time < e.Time)
            {
                return -1;
            }
            else if (Time > e.Time)
            {
                return 1;
            }
            else
            {
                int equality = Type.CompareTo(e.Type);
                if (equality == 0)
                {
                    equality = Type.CompareTo(e.Type);
                    return equality == 0 ? -1 : equality;
                }
                else
                {
                    return equality;
                }
            }
        }

        public override string ToString()
        {
            return "E: [time=" + Time + ", \ttype=" + Type + ", dvd=" + DVD + ", machine=" + Machine + "]";
        }
    }
}
