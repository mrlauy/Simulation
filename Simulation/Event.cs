using System; using System.Collections.Generic; using System.Linq; using System.Text; using System.Threading.Tasks;  namespace Simulation {     enum Type { MACHINE_1, MACHINE_2, MACHINE_3, ADD_TO_CRATE, MACHINE_4, BREAKDOWN_1, BREAKDOWN_3, BREAKDOWN_4, REPAIRED_1, REPAIRED_3, REPAIRED_4, END_OF_SIMULATION };

    class Event : IComparer<Event>, IComparable<Event>     {         public long Time { get; private set; }         public int DVD { get; private set; }         public Type Type { get; private set; }         public int Machine { get; private set; } // A, B, C or D          public Event(long time, int dvd, Type type, int machine)         {             Time = time;             DVD = dvd;             Type = type;             Machine = machine;         }          // TODO maybe better to abstract such event that is not machine specific         public Event(long time, Type type)         {             Time = time;             Type = type;         }

        public int Compare(Event x, Event y)
        {
            // Events are never equal = 0, otherwise duplicates are maybe not possible 
            // Done events are handled before init events

            // TODO add priority to which event is handeld first
            return x.Time.CompareTo(y.Time);
        }
        public int CompareTo(Event x)
        {
            // Events are never equal = 0, otherwise duplicates are maybe not possible 
            // Done events are handled before init events

            // TODO add priority to which event is handeld first
            return Time.CompareTo(x.Time);
        }     } } 