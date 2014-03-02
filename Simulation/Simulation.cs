﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace Simulation
{
    public enum State { IDLE, BUSY, BLOCKED, BROKEN, WASBROKEN };
    public enum Machine { M1a, M1b, M1c, M1d, M2a, M2b, M3a, M3b, M4a, M4b };

    public class Simulation
    {
        private GUI parent;

        private static int RUN_LENGTH = 200;
        private TreeSet<Event> eventList;

        // State variables
        public long Time  { get; private set; }  // Simulation time
        public Dictionary<Machine, State> MachineState  { get; private set; }    // state of each individual machine
        public int BufferA { get; private set; }     // buffer between machine 1a, machine 1b and machine 2a
        public int BufferB { get; private set; }     // buffer between machine 1c, machine 1d and machine 2b


        // public int cratesReadyforInputM3 { get; private set; }       // ?
        public int dvdReadyForM3 { get; private set; }               // number of dvd in a crate ready to be prossed by machine 3
        public int cratesToBeFilledM3 { get; private set; }         // number of crates ready to be filled before machine 3 can process a crate;

        public int dvdReadyForInputM4 { get; private set; }         // number of dvd ready to be processed by machine 4, 
        //   at least one dvd ready means there is a crate, 21 dvd ready means that there are two crates 

        public int dvdCounter  { get; private set; }  // counter for how many dvds the production has started to produce

        // state of the machine in case something has gone wrong
        private Dictionary<Machine, long> TimeM1ShouldHaveFinished;
        private Dictionary<Machine, long> TimeM1HasBrokenDown;
        private Dictionary<Machine, long> TimeM3ShouldHaveFinished;
        private Dictionary<Machine, long> TimeM3HasBrokenDown;

        public Simulation()
        {
            parent = null;
            eventList = new TreeSet<Event>();
            Initialize();
        }

        public Simulation(GUI parent)
        {
            this.parent = parent;
            eventList = new TreeSet<Event>();
            Initialize();
        }

        private void Initialize()
        {
            // add initial events, machine 1, breakdowns 1,3,4, and end of simulation
            BufferA = BufferB = 0;
            cratesToBeFilledM3 = 0;
            dvdReadyForM3 = 0;
            dvdReadyForInputM4 = 0;

            dvdCounter = 1;

            TimeM1ShouldHaveFinished = new Dictionary<Machine, long>();
            TimeM1HasBrokenDown = new Dictionary<Machine, long>();
            TimeM3ShouldHaveFinished = new Dictionary<Machine, long>();
             TimeM3HasBrokenDown = new Dictionary<Machine, long>();

             MachineState = new Dictionary<Machine, State>();
             MachineState[Machine.M1a] = State.IDLE;
             MachineState[Machine.M1b] = State.IDLE;
             MachineState[Machine.M1c] = State.IDLE;
             MachineState[Machine.M1d] = State.IDLE;
             MachineState[Machine.M2a] = State.IDLE;
             MachineState[Machine.M2b] = State.IDLE;
             MachineState[Machine.M3a] = State.IDLE;
             MachineState[Machine.M3b] = State.IDLE;
             MachineState[Machine.M4a] = State.IDLE;
             MachineState[Machine.M4b] = State.IDLE;

            eventList.Add(new Event(0, Type.MACHINE_1, Machine.M1a, dvdCounter++));
            eventList.Add(new Event(0, Type.MACHINE_1, Machine.M1b, dvdCounter++));
            eventList.Add(new Event(0, Type.MACHINE_1, Machine.M1c, dvdCounter++));
            eventList.Add(new Event(0, Type.MACHINE_1, Machine.M1d, dvdCounter++));
            eventList.Add(new Event(RUN_LENGTH, Type.END_OF_SIMULATION));
            Console.WriteLine("EventList: " + eventList);
        }

        public void Run()
        {
            bool running = true;
            Console.WriteLine("Simulation running");
            while (running)
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
                        running = false;
                        break;
                    default:
                        Console.WriteLine("FAIL!");
                        break;
                }

                if (parent!=null)
                {
                    parent.UpdateSim();
                }

                Thread.Sleep(500);
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
                if (MachineState[Machine.M2a] == State.IDLE)
                {
                    // Make M2a start production if M2a is available
                    MachineState[Machine.M2a] = State.BUSY;

                    // schedule the next step of the dvd production and make the next dvd
                    scheduleM2(time, Machine.M2a);
                    scheduleM1(time, machine);
                }
                else
                {
                    // M2a isn't available for input, place the DVD in buffer if there is room
                    BufferA++;

                    if (BufferA == 20)
                    {
                        // stop production, buffer full
                        MachineState[machine] = State.IDLE;
                    }
                    else
                    {
                        // keep producing dvd's, schedule new M1Finished
                        scheduleM1(time, machine);
                    }
                }
            }
            else if (MachineState[Machine.M2b] == State.IDLE)
            {
                // Make M2b start production if M2b is available
                MachineState[Machine.M2b] = State.BUSY;

                // schedule the next step of the dvd production and restart machine to produce the next dvd
                scheduleM2(time, Machine.M2b);
                scheduleM1(time, machine);
            }
            else
            {
                // M2b isn't available for input, place the DVD in buffer if there is room
                BufferB++;

                if (BufferB == 20)
                {
                    // stop production, buffer full
                    MachineState[machine] = State.IDLE;
                }
                else
                {
                    // keep producing dvd's, schedule new M1Finished
                    scheduleM1(time, machine);
                }
            }
        }

        private void M2Finished(Event e)
        {
                if (dvdReadyForM3 < cratesToBeFilledM3 * 20)
                {
                    // schedule AddDVDtoCrate event 
                    scheduleAddDVDToCrate(e.Time);

                    if (e.Machine == Machine.M2a)
                    {
                        if (BufferA > 0)
                        {
                            BufferA--;
                            // schedule M2Finished
                            scheduleM2(e.Time, e.Machine);
                        }
                        else
                        {
                            // no input for the machine
                            MachineState[e.Machine] = State.IDLE;
                        }
                    }
                    else
                    {
                        if (BufferB > 0)
                        {
                            BufferB--;
                            // schedule M2Finished
                            scheduleM2(e.Time, e.Machine);
                        }
                        else
                        {
                            // no input for the machine
                            MachineState[e.Machine] = State.IDLE;
                        }
                    }
                }
                else
                {
                    // the machine will not be able to output the next dvd
                    MachineState[e.Machine] = State.BLOCKED;
                }
            
        }

        private void AddDVDToCrate(Event e)
        {
            dvdReadyForM3++;

            if (dvdReadyForM3 >= cratesToBeFilledM3 * 20)
            {
                // One crate is full and therefore ready to be put in machine 3
                if (dvdReadyForM3 == cratesToBeFilledM3 * 20)
                {
                    // If no other crates are available we stop M2 from producing dvd's 
                    MachineState[e.Machine] = State.BLOCKED;
                }

                // If M3 is available we start it's production
                if (MachineState[Machine.M3a] == State.IDLE)
                {
                    cratesToBeFilledM3--;
                    dvdReadyForM3 -= 20;
                    MachineState[Machine.M3a] = State.BUSY;
                    scheduleM3(e.Time, Machine.M3a);
                }
                else if (MachineState[Machine.M3b] == State.IDLE)
                {
                    cratesToBeFilledM3--;
                    dvdReadyForM3 -= 20;
                    MachineState[Machine.M3b] = State.BUSY;
                    scheduleM3(e.Time, Machine.M3b);
                }
            }
        }

    private void M3Finished(Event e){ 

        if (MachineState[e.Machine] == State.BROKEN) {
            // M3 is broken which cause the batch of dvd's that was supposed to be finished to be still in M3
            TimeM3ShouldHaveFinished[e.Machine] = e.Time;
        } else if (MachineState[e.Machine] == State.WASBROKEN) {
            // M3 has been broken during producing the dvd
            // the repair time of the machine is being added to the finishing time of the dvd's in the batch
            MachineState[e.Machine] = State.BUSY;
            // TODO schedule M3Finished : time machine 3 was broken
        } else {
                dvdReadyForInputM4 +=20;
            // M3 is finished and starts M4 if M4 is available
            if (MachineState[Machine.M4a] == State.IDLE) {
                //M3 outputs a crate with 20 dvd's 
                dvdReadyForInputM4--;
                MachineState[Machine.M4a] = State. BUSY;
                scheduleM4(e.Time, Machine.M4a);
            } else if (MachineState[Machine.M4b] == State.IDLE) {
                //M3 outputs a crate with 20 dvd's 
                dvdReadyForInputM4--;
                MachineState[Machine.M4b] = State. BUSY;
                scheduleM4(e.Time, Machine.M4b);
            } 

            // If a full crate is available for input M3, start producing this crate. Else, output the crate and go back to waiting for input. 
            if (dvdReadyForM3 >= cratesToBeFilledM3 * 20) {
                cratesToBeFilledM3--;
                dvdReadyForM3 -= 20;
                scheduleM3(e.Time, e.Machine);
            } else {
                MachineState[e.Machine] = State.IDLE;
            }
        }
    }

    private void M4Finished(Event e)
    {
        // A DVD is ready
        if (dvdReadyForInputM4 > 0)
        {
            // contentCratesFinished - 1
            scheduleM4(e.Time, e.Machine);

            //If M2 emptied a whole crate
            if (dvdReadyForInputM4 % 20 == 0)
            {
                // a crate is empty and ready to be filled again
                cratesToBeFilledM3++;

                // If M2 was blocked in it's output, lift the blockade, there are empty crates again
                if (MachineState[Machine.M2a] == State.BLOCKED)
                {
                    scheduleM2(e.Time, Machine.M2a);
                    MachineState[Machine.M2a] = State.BUSY;
                }
                else if (MachineState[Machine.M2b] == State.BLOCKED)
                {
                    scheduleM2(e.Time, Machine.M2b);
                    MachineState[Machine.M2b] = State.BUSY;
                }
            }
        }
        else
        {
            MachineState[e.Machine] = State.BUSY;
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
            long processTime = 60; // gemiddelde
            eventList.Add(new Event(time + processTime, Type.MACHINE_1, machine, 0));
        }
        private void scheduleM2(long time, Machine machine)
        {
            long processTime = 24; // 
            eventList.Add(new Event(time + processTime, Type.MACHINE_2, machine, 0));

        }
        private void scheduleAddDVDToCrate(long time)
        {
            long processTime = 5*60; // 
            eventList.Add(new Event(time + processTime, Type.ADD_TO_CRATE));
        }
        private void scheduleM3(long time, Machine machine)
        {
            long processTime = 10 + 6 +3*60; // 
            eventList.Add(new Event(time + processTime, Type.MACHINE_3, machine, 0));
        }
        private void scheduleM4(long time, Machine machine)
        {
            long processTime = 25; // gemiddelde
            eventList.Add(new Event(time + processTime, Type.MACHINE_4, machine, 0));
        }
    }
}