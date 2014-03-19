using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Event : IComparer<Event>, IComparable<Event>
    {
        public double Time { get; private set; }
        public Type Type { get; private set; }
        public Machine Machine { get; private set; } // M1A, M2B, M1C or M4A
        public int DVD { get; private set; }
        public List<int> DVDs { get; private set; }

        public Event(double time, Type type, Machine machine, int dvd)
        {
            Time = time;
            Type = type;
            Machine = machine;
            DVD = dvd;
            DVDs = null;

            if (dvd == 0)
            {
                Console.WriteLine("FAIL");
            }
        }

        public Event(double time, Type type, Machine machine, List<int> dvds)
        {
            Time = time;
            Type = type;
            Machine = machine;
            DVD = -1;
            DVDs = dvds;
        }

        public Event(double time, Type type, Machine machine)
        {
            Time = time;
            Type = type;
            Machine = machine;
            DVD = -1;
            DVDs = null;
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
                    equality = x.Machine.CompareTo(y.Machine);
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
                    equality = Machine.CompareTo(e.Machine);
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
            return "E: [time=" + Math.Round(Time, 4) + ", \ttype=" + Type + (Machine != Machine.DUMMY ? ", machine=" + Machine : "")  + (DVD > 0 ? ", dvd=" + DVD : (DVDs != null ? ", dvds="+ DVDs : "")) + "]";
        }
    }
}
