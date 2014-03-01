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
        private TreeSet<Event> eventList;
        
     
        public Simulation()
        {
            eventList = new TreeSet<Event>();
            Initialize();
        }

        private void Initialize()
        {
            // add initial events, machine 1, breakdowns 1,3,4, and end of simulation
            eventList.Insert(new Event(0, 1, Type.MACHINE_1, 1));
            eventList.Insert(new Event(RUN_LENGTH, Type.END_OF_SIMULATION));
            eventList.Insert(new Event(10, 1, Type.MACHINE_1, 1));
            eventList.Insert(new Event(20, 1, Type.MACHINE_1, 1));
            eventList.Insert(new Event(30, 1, Type.MACHINE_1, 1));
            Console.WriteLine("EventList: " + eventList);
        }

        public void Run()
        {
            bool running = true;
            Console.WriteLine("Simulation running");
            while (running)
            {
                Event evnt = eventList.Pop();

                Console.WriteLine("Process Event: " + evnt);

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
