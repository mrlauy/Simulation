using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;

namespace Simulation
{
    public enum State { IDLE, BUSY, BLOCKED, BROKEN, WASBROKEN };
    public enum Machine { M1a, M1b, M1c, M1d, M2a, M2b, M3a, M3b, M4a, M4b, DUMMY };

    public class Simulation
    {
        private IUpdate parent;

        private static int RUN_LENGTH = 10000;
        private TreeSet<Event> eventList;
        public bool Paused { get; set; }            // is the simulation paused
        public bool Running { get; set; }   // is the simulation running
        public int Speed { get; set; }   // Simulation speed

        private Random random;

        private int CRATE_SIZE = 10;
        private int BUFFER_SIZE = 10;
        // State variables
        public long Time { get; private set; }  // Simulation time
        public Dictionary<Machine, State> MachineState { get; private set; }    // state of each individual machine
        public int BufferA { get; private set; }     // buffer between machine 1a, machine 1b and machine 2a
        public int BufferB { get; private set; }     // buffer between machine 1c, machine 1d and machine 2b


        // public int cratesReadyforInputM3 { get; private set; }       // ?
        public int dvdReadyForM3 { get; private set; }               // number of dvd in a crate ready to be prossed by machine 3
        public int cratesToBeFilledM3 { get; private set; }         // number of crates ready to be filled before machine 3 can process a crate;

        public int dvdReadyForInputM4 { get; private set; }         // number of dvd ready to be processed by machine 4, 
        //   at least one dvd ready means there is a crate, 21 dvd ready means that there are two crates 

        public int dvdCounter { get; private set; }    // counter for how many dvds the production has started to produce
        public int dvdProduced { get; private set; }    // number of DVD produced 
        public int dvdInProduction { get; private set; }    // number of DVD in production

        // state of the machine in case something has gone wrong
        private Dictionary<Machine, long> TimeM1ShouldHaveFinished;
        private Dictionary<Machine, long> TimeM1HasBrokenDown;
        private Dictionary<Machine, long> TimeM3ShouldHaveFinished;
        private Dictionary<Machine, long> TimeM3HasBrokenDown;

        public Simulation(IUpdate parent)
        {
            this.parent = parent;
            Initialize();
        }

        public void Initialize()
        {
            eventList = new TreeSet<Event>();
            random = new Random();

            // add initial events, machine 1, breakdowns 1,3,4, and end of simulation
            BufferA = BufferB = 0;
            cratesToBeFilledM3 = 2;
            dvdReadyForM3 = 0;
            dvdReadyForInputM4 = 0;

            dvdCounter = 1;
            dvdProduced = 0;
            dvdInProduction = 4;

            TimeM1ShouldHaveFinished = new Dictionary<Machine, long>();
            TimeM1HasBrokenDown = new Dictionary<Machine, long>();
            TimeM3ShouldHaveFinished = new Dictionary<Machine, long>();
            TimeM3HasBrokenDown = new Dictionary<Machine, long>();

            MachineState = new Dictionary<Machine, State>();
            MachineState[Machine.M1a] = State.BUSY;
            MachineState[Machine.M1b] = State.BUSY;
            MachineState[Machine.M1c] = State.BUSY;
            MachineState[Machine.M1d] = State.BUSY;
            MachineState[Machine.M2a] = State.IDLE;
            MachineState[Machine.M2b] = State.IDLE;
            MachineState[Machine.M3a] = State.IDLE;
            MachineState[Machine.M3b] = State.IDLE;
            MachineState[Machine.M4a] = State.IDLE;
            MachineState[Machine.M4b] = State.IDLE;

            scheduleM1(0, Machine.M1a);
            scheduleM1(0, Machine.M1b);
            scheduleM1(0, Machine.M1c);
            scheduleM1(0, Machine.M1d);

            eventList.Add(new Event(RUN_LENGTH, Type.END_OF_SIMULATION, Machine.DUMMY, 0));

            Time = 0;
            Running = false;
            Paused = false;
            Speed = 9;
        }

        public void Run()
        {
            Running = true;
            Console.WriteLine("Simulation running");
            while (Running)
            {
                if (!Paused)
                {
                    Event e = eventList.Pop();
                    Time = e.Time;

                    Console.WriteLine("Process Event: " + e);

                    //long time = e.Time;
                    switch (e.Type)
                    {
                        case Type.MACHINE_1:
                            M1Finished(e);
                            break;
                        case Type.MACHINE_2:
                            M2Finished(e);
                            break;
                        case Type.ADD_TO_CRATE:
                            AddDVDToCrate(e);
                            break;
                        case Type.MACHINE_3:
                            M3Finished(e);
                            break;
                        case Type.MACHINE_4:
                            M4Finished(e);
                            break;
                        case Type.END_OF_SIMULATION:
                            Running = false;
                            break;
                        default:
                            Console.WriteLine("FAIL!");
                            break;
                    }

                    parent.UpdateSim();

                }
                Thread.Sleep(Speed);
            }
            Console.WriteLine("Simulation finished");
        }

        private void M1Finished(Event e)
        {
            long time = e.Time;
            Machine machine = e.Machine;

            State state = MachineState[machine];
            if (state == State.BROKEN)
            {
                // M1 is broken which cause the dvd that was supposed to be finished to be still in M1
                TimeM1ShouldHaveFinished[machine] = time;
            }
            else if (state == State.WASBROKEN)
            {
                // M1 has been broken during producing the dvd
                // the repair time of the machine is being added to the finishing time of the dvd
                MachineState[machine] = State.BUSY;

                // schedule M1Finished at time machine 1 was broken 
                eventList.Add(new Event(time + TimeM1HasBrokenDown[machine], e.Type, machine, e.DVD));
            }
            else if (machine == Machine.M1a || machine == Machine.M1b)
            {
                // keep producing dvd's, schedule new M1Finished
                BufferA++;
                scheduleM1(time, machine);

                if (MachineState[Machine.M2a] == State.IDLE)
                {
                    // Make M2a start production if M2a is available
                    MachineState[Machine.M2a] = State.BUSY;

                    // schedule the next step of the dvd production
                    scheduleM2(time, Machine.M2a);
                }
            }
            else
            {
                // M2b isn't available for input, place the DVD in buffer if there is room
                BufferB++;
                // keep producing dvd's, schedule new M1Finished
                scheduleM1(time, machine);

                if (MachineState[Machine.M2b] == State.IDLE)
                {
                    // Make M2b start production if M2b is available
                    MachineState[Machine.M2b] = State.BUSY;

                    // schedule the next step of the dvd production and restart machine to produce the next dvd
                    scheduleM2(time, Machine.M2b);
                }
            }
        }

        private void M2Finished(Event e)
        {
            // schedule AddDVDtoCrate event 
            scheduleAddDVDToCrate(e.Time);

            if (e.Machine == Machine.M2a)
            {
                // schedule M2Finished
                scheduleM2(e.Time, e.Machine);

                // check if machine was blocked and need to be scheduled again
                if (MachineState[Machine.M1a] == State.BLOCKED)
                {
                    MachineState[Machine.M1a] = State.BUSY;
                    scheduleM1(e.Time, Machine.M1a);
                }
                if (MachineState[Machine.M1b] == State.BLOCKED)
                {
                    MachineState[Machine.M1b] = State.BUSY;
                    scheduleM1(e.Time, Machine.M1b);
                }
            }
            else
            {       // schedule M2Finished
                scheduleM2(e.Time, e.Machine);

                if (MachineState[Machine.M1c] == State.BLOCKED)
                {
                    MachineState[Machine.M1c] = State.BUSY;
                    scheduleM1(e.Time, Machine.M1c);
                }
                if (MachineState[Machine.M1d] == State.BLOCKED)
                {
                    MachineState[Machine.M1d] = State.BUSY;
                    scheduleM1(e.Time, Machine.M1d);
                }
            }
        }

        private void AddDVDToCrate(Event e)
        {
            dvdReadyForM3++;

            // Check if a crate is full and therefore ready to be put in machine 3
            if (dvdReadyForM3 >= CRATE_SIZE)
            {
                // If M3 is available we start it's production
                if (MachineState[Machine.M3a] == State.IDLE)
                {
                    MachineState[Machine.M3a] = State.BUSY;
                    scheduleM3(e.Time, Machine.M3a);
                }
                else if (MachineState[Machine.M3b] == State.IDLE)
                {
                    MachineState[Machine.M3b] = State.BUSY;
                    scheduleM3(e.Time, Machine.M3b);
                }
            }
        }

        private void M3Finished(Event e)
        {
            if (MachineState[e.Machine] == State.BROKEN)
            {
                // M3 is broken which cause the batch of dvd's that was supposed to be finished to be still in M3
                TimeM3ShouldHaveFinished[e.Machine] = e.Time;
            }
            else if (MachineState[e.Machine] == State.WASBROKEN)
            {
                // M3 has been broken during producing the dvd
                // the repair time of the machine is being added to the finishing time of the dvd's in the batch
                MachineState[e.Machine] = State.BUSY;
                // TODO schedule M3Finished : time machine 3 was broken
            }
            else
            {
                dvdReadyForInputM4 += CRATE_SIZE;
                // M3 is finished and starts M4 if M4 is available
                if (MachineState[Machine.M4a] == State.IDLE)
                {
                    MachineState[Machine.M4a] = State.BUSY;
                    scheduleM4(e.Time, Machine.M4a);
                }

                if (MachineState[Machine.M4b] == State.IDLE)
                {
                    MachineState[Machine.M4b] = State.BUSY;
                    scheduleM4(e.Time, Machine.M4b);
                }
                scheduleM3(e.Time, e.Machine);
            }
        }

        private void M4Finished(Event e)
        {
            // update statistics
            dvdProduced++;
            dvdInProduction--;
            int before = dvdReadyForInputM4;

            scheduleM4(e.Time, e.Machine);

            // If M4 emptied a whole crate
            if (before % CRATE_SIZE == 1 && dvdReadyForInputM4 % CRATE_SIZE == 0)
            {
                // a crate is empty and ready to be filled again
                cratesToBeFilledM3++;

                // If M2 was blocked in it's output, lift the blockade, there are empty crates again
                if (MachineState[Machine.M2a] == State.BLOCKED)
                {
                    MachineState[Machine.M2a] = State.BUSY;
                    scheduleM2(e.Time, Machine.M2a);
                }

                if (MachineState[Machine.M2b] == State.BLOCKED)
                {
                    MachineState[Machine.M2b] = State.BUSY;
                    scheduleM2(e.Time, Machine.M2b);
                }
            }
        }

        /*
	
        BreakdownM1:
            machine 1 = BROKEN
            brokenMachineTime = time
            schedule repair machine 1: 2 hours exp distr.
            time machine 1 is was broken = : 2 hours exp distr. 

        RepairM1:
            schedule breakdown machine 1: 8 hours
            if (TimeM1ShouldHaveFinished is set) {
                machine 1 = BUSY
                //repair time 2 hours exp. 
                schedule machine 1 finished: time now (de tijd dat hij gerepareerd is) + (time product should have finished - time broken down)
                time machine should have finished = null // not set
            } else {
                // het is nog niet bekend hoe lang de dvd in de machine heeft gezeten als die kapot is geweest
                machine 1 = WASBROKEN
            }

        BreakdownM3:
            machine 3 = BROKEN
            brokenMachine3Time = time
            schedule repair machine 3
            time machine 3 is was broken = that time

        RepairM3:
            schedule breakdown machine 3 : happens to 3% of the dvd's
            if (machine 3 should have finished is set) {
                machine 3 = BUSY
                //repair time 5 minutes exp. distribution
                schedule machine 3 finished: time now (de tijd dat hij gerepareerd is) + (time product should have finished - time broken down)
                time machine should have finished = null // not set
            } else {
                // het is nog niet bekend hoe lang de dvd in de machine heeft gezeten als die kapot is geweest
                machine 3 = WASBROKEN
            }
	
        BreakdownM4:
            machine 4 = BROKEN
            brokenMachine4Time = time
            schedule repair machine 4
            time machine 4 is was broken = that time

        RepairM4:
            schedule breakdown machine 4 : after 200 dvd's following the given distribution
            if (machine 4 should have finished is set) {
                machine 4 = BUSY
                //repair time 15 minutes sd 1 minute
                schedule machine 4 finished: time now (de tijd dat hij gerepareerd is) + (time product should have finished - time broken down)
                time machine should have finished = null // not set
            } else {
                // het is nog niet bekend hoe lang de dvd in de machine heeft gezeten als die kapot is geweest
                machine 4 = WASBROKEN
            }
        
            */

        private void scheduleM1(long time, Machine machine)
        {
            int limit, buffer = BUFFER_SIZE;

            // the limit of the buffer is 19 when the other machine will produce the 20th dvd.
            if (machine == Machine.M1a || machine == Machine.M1b)
            {
                buffer = BufferA;
                limit = (MachineState[Machine.M1a] == State.BUSY && MachineState[Machine.M1b] == State.BUSY ? BUFFER_SIZE - 1 : BUFFER_SIZE);
            }
            else
            {
                buffer = BufferB;
                limit = (MachineState[Machine.M1c] == State.BUSY && MachineState[Machine.M1d] == State.BUSY ? BUFFER_SIZE - 1 : BUFFER_SIZE);
            }


            if (buffer >= limit)
            {
                // stop production, buffer full
                MachineState[machine] = State.BLOCKED;
            }
            else
            {
                // keep producing dvd's, schedule new M1Finished
                dvdInProduction++;
                long processTime = 60; // gemiddelde
                eventList.Add(new Event(time + processTime, Type.MACHINE_1, machine, 0));
            }
        }
        private void scheduleM2(long time, Machine machine)
        {
            int limit = cratesToBeFilledM3 * CRATE_SIZE - (MachineState[Machine.M2a] == State.BUSY && MachineState[Machine.M2b] == State.BUSY ? 0 : 1);
            if (dvdReadyForM3 <= limit && cratesToBeFilledM3 > 0)
            {
                if (machine == Machine.M2a)
                {
                    if (BufferA > 0)
                    {
                        BufferA--;

                        long processTime = 24; // 
                        eventList.Add(new Event(time + processTime, Type.MACHINE_2, machine, 0));
                    }
                    else
                    {
                        // no input for the machine
                        MachineState[machine] = State.IDLE;
                    }
                }
                else
                {
                    if (BufferB > 0)
                    {
                        BufferB--;

                        long processTime = 24; // 
                        eventList.Add(new Event(time + processTime, Type.MACHINE_2, machine, 0));
                    }
                    else
                    {
                        // no input for the machine
                        MachineState[machine] = State.IDLE;
                    }
                }
            }
            else
            {
                // the machine will not be able to output the next dvd
                MachineState[machine] = State.BLOCKED;
            }
        }
        private void scheduleAddDVDToCrate(long time)
        {
            long processTime = 5 * 60; // 
            eventList.Add(new Event(time + processTime, Type.ADD_TO_CRATE, Machine.DUMMY, 0));
        }
        private void scheduleM3(long time, Machine machine)
        {
            // If a full crate is available for input M3, start producing this crate. Else, output the crate and go back to waiting for input. 
            if (dvdReadyForM3 >= CRATE_SIZE && cratesToBeFilledM3 > 0 )
            {
                cratesToBeFilledM3--;
                dvdReadyForM3 -= CRATE_SIZE;

                long processTime = 10 + 6 + 3 * 60; // 
                eventList.Add(new Event(time + processTime, Type.MACHINE_3, machine, 0));
            }
            else
            {
                MachineState[machine] = State.IDLE;
            }
        }
        private void scheduleM4(long time, Machine machine)
        {
            if (dvdReadyForInputM4 > 0)
            {
                dvdReadyForInputM4--;
                long processTime = 25; // gemiddelde
                eventList.Add(new Event(time + processTime, Type.MACHINE_4, machine, 0));
            }
            else
            {
                MachineState[machine] = State.IDLE;
            }
        }

        private double getRandomTime()
        {
            double u = random.NextDouble();
            return Math.Log(1 - u) / (0 - 0.1);
        }
    }
}
