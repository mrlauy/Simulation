using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Simulation
{
    class Simulation 
    {
        private static int RUN_LENGTH = 100;
        private SortedSet<Event> eventList;
     
        public Simulation()
        {
            eventList = new SortedSet<Event>();
            Initialize();
        }

        private void Initialize()
        {
            // add initial events, machine 1, breakdowns 1,3,4, and end of simulation
            eventList.Add(new Event(0, 1, Type.MACHINE_1, 1));
            eventList.Add(new Event(RUN_LENGTH, Type.END_OF_SIMULATION)); 
        }

        public void Run()
        {
            bool running = true;
            Console.WriteLine("Simulation running");
            while (running)
            {
                Event evnt = eventList.First();
                eventList.Remove(evnt);

                long time  = evnt.Time;
                switch (evnt.Type)
                {
                    case Type.END_OF_SIMULATION:
                        running = false;
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine("Simulation finished");
        }
    }
    
}
