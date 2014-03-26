using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;




namespace Simulation
{
    enum Type { MACHINE_1, MACHINE_2, MACHINE_3, ADD_TO_CRATE, MACHINE_4, BREAKDOWN_1, REPAIRED_1, REPAIRED_4, END_OF_SIMULATION };

    public enum State { IDLE, BUSY, BLOCKED, BBROKEN, BROKEN, WASBROKEN };
    public enum Machine { M1a, M1b, M1c, M1d, M2a, M2b, M3a, M3b, M4a, M4b, DUMMY };

    public class Simulation
    {
        private IUpdate parent;

        private static int RUN_LENGTH = 86400;
        private TreeSet<Event> eventList;
        public bool Paused { get; set; }            // is the simulation paused
        public bool Running { get; set; }   // is the simulation running
        public int Speed { get; set; }   // Simulation speed
        public bool Feedback { get; set; } // If simulation gives feedback

        private Input input;

        private int CRATE_SIZE = 20;
        private int BUFFER_SIZE = 20;

        // State variables
        public double Time { get; private set; }  // Simulation time
        public Dictionary<Machine, State> MachineState { get; private set; }    // state of each individual machine

        // state of the machine in case something has gone wrong
        private Dictionary<Machine, Event> M1ShouldHaveFinished;
        private Dictionary<Machine, double> TimeM1HasBrokenDown;
        private Dictionary<Machine, int> dvdBeforeM4Service;

        public Queue<int> BufferA { get; private set; }     // buffer between machine 1a, machine 1b and machine 2a
        public Queue<int> BufferB { get; private set; }     // buffer between machine 1c, machine 1d and machine 2b

        public Queue<int> dvdReadyForM3a { get; private set; }      // dvd in crate ready to be processed by machine 3
        public Queue<int> dvdReadyForM3b { get; private set; }               // number of dvd in a crate ready to be processed by machine 3

        public Queue<int> dvdReadyForInputM4a { get; private set; }         // number of dvd ready to be processed by machine 4, 
        public Queue<int> dvdReadyForInputM4b { get; private set; }         // number of dvd ready to be processed by machine 4, 
        //   at least one dvd ready means there is a crate, 21 dvd ready means that there are two crates 

        // Statistic variables
        public int dvdCounter { get; private set; }             // counter for how many dvds the production has started to produce
        public int dvdInProduction { get; private set; }        // number of DVD in production
        public List<double> dvdProduced { get; private set; }   // time of dvd is finished
        public List<double> dvdFailed { get; private set; }     // times that a DVD failed during the process
        public List<double> dvdStartTimes { get; private set; }    // time a dvd starts production
        public List<double> dvdThroughput { get; private set; }    // throughput time of dvds

        public Dictionary<int, double> StartTimes { get; private set; }  // start time of dvd id's
        public Dictionary<Machine, double> BusyTime { get; private set; } // time a machine is busy
        public Dictionary<Machine, double> IdleTime { get; private set; } // time a machine is broken
        public Dictionary<Machine, double> BlockedTime { get; private set; } // time a machine is blocked

        public Dictionary<string, Dictionary<double, int>> Statistics { get; private set; } // for each buffer keep the volume of the buffer/batch

        public Simulation(IUpdate parent)
        {
            Feedback = true;
            this.parent = parent;
            Initialize();
        }

        public void ChangeSimulation(int runLength, int bufferSize, int crateSize)
        {
            RUN_LENGTH = runLength;
            BUFFER_SIZE = bufferSize;
            CRATE_SIZE = crateSize;
        }

        public void Initialize()
        {
            Time = 0.0;
            Running = false;
            Paused = false;
            Speed = 1;

            eventList = new TreeSet<Event>();
            input = new Input();

            //  initiate buffers
            BufferA = new Queue<int>();
            BufferB = new Queue<int>();
            dvdReadyForM3a = new Queue<int>();
            dvdReadyForM3b = new Queue<int>();
            dvdReadyForInputM4a = new Queue<int>();
            dvdReadyForInputM4b = new Queue<int>();

            // initiate statistic counters
            dvdCounter = 0;
            dvdInProduction = 0;
            dvdFailed = new List<double>();
            dvdStartTimes = new List<double>();
            dvdProduced = new List<double>();
            dvdThroughput = new List<double>();

            StartTimes = new Dictionary<int, double>();
            BusyTime = new Dictionary<Machine, double>();
            BusyTime[Machine.M1a] = 0;
            BusyTime[Machine.M1b] = 0;
            BusyTime[Machine.M1c] = 0;
            BusyTime[Machine.M1d] = 0;
            BusyTime[Machine.M2a] = 0;
            BusyTime[Machine.M2b] = 0;
            BusyTime[Machine.M3a] = 0;
            BusyTime[Machine.M3b] = 0;
            BusyTime[Machine.M4a] = 0;
            BusyTime[Machine.M4b] = 0;
            IdleTime = new Dictionary<Machine, double>();
            IdleTime[Machine.M1a] = 0;
            IdleTime[Machine.M1b] = 0;
            IdleTime[Machine.M1c] = 0;
            IdleTime[Machine.M1d] = 0;
            IdleTime[Machine.M2a] = 0;
            IdleTime[Machine.M2b] = 0;
            IdleTime[Machine.M3a] = 0;
            IdleTime[Machine.M3b] = 0;
            IdleTime[Machine.M4a] = 0;
            IdleTime[Machine.M4b] = 0;
            BlockedTime = new Dictionary<Machine, double>();
            BlockedTime[Machine.M1a] = 0;
            BlockedTime[Machine.M1b] = 0;
            BlockedTime[Machine.M1c] = 0;
            BlockedTime[Machine.M1d] = 0;
            BlockedTime[Machine.M2a] = 0;
            BlockedTime[Machine.M2b] = 0;
            BlockedTime[Machine.M3a] = 0;
            BlockedTime[Machine.M3b] = 0;
            BlockedTime[Machine.M4a] = 0;
            BlockedTime[Machine.M4b] = 0;

            Statistics = new Dictionary<string, Dictionary<double, int>>();
            Statistics["bufferA"] = new Dictionary<double, int>();
            Statistics["bufferB"] = new Dictionary<double, int>();
            Statistics["batchAin"] = new Dictionary<double, int>();
            Statistics["batchBin"] = new Dictionary<double, int>();
            Statistics["batchAout"] = new Dictionary<double, int>();
            Statistics["batchBout"] = new Dictionary<double, int>();

            // initiate machine states
            M1ShouldHaveFinished = new Dictionary<Machine, Event>();
            TimeM1HasBrokenDown = new Dictionary<Machine, double>();
            dvdBeforeM4Service = new Dictionary<Machine, int>();
            dvdBeforeM4Service[Machine.M4a] = input.M4Service();
            dvdBeforeM4Service[Machine.M4b] = input.M4Service();

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


            // add initial events, machine 1, breakdowns 1,3,4, and end of simulation
            scheduleM1(Time, Machine.M1a);
            scheduleM1(Time, Machine.M1b);
            scheduleM1(Time, Machine.M1c);
            scheduleM1(Time, Machine.M1d);

            scheduleM1Breakdown(Time, Machine.M1a);
            scheduleM1Breakdown(Time, Machine.M1b);
            scheduleM1Breakdown(Time, Machine.M1c);
            scheduleM1Breakdown(Time, Machine.M1d);

            // schedule end of simulation
            eventList.Add(new Event(RUN_LENGTH, Type.END_OF_SIMULATION, Machine.DUMMY));
        }

        public void Run()
        {
            Running = true;
            // SaveStatistics();
            Console.WriteLine("Simulation running");
            while (Running)
            {
                if (!Paused && eventList.Count > 0)
                {
                    Event e = eventList.Pop();
                    UpdateStatistics(e);
                    Time = e.Time;

                    if (Feedback)
                    {
                        Console.WriteLine("Process Event: " + e);
                    }

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
                        case Type.BREAKDOWN_1:
                            BreakdownM1(e);
                            break;
                        case Type.REPAIRED_1:
                            RepairM1(e);
                            break;
                        case Type.REPAIRED_4:
                            RepairM4(e);
                            break;
                        case Type.END_OF_SIMULATION:
                            Running = false;
                            break;
                        default:
                            Console.WriteLine("Forgot to do somewhing!" + e.Type);
                            break;
                    }

                    if (Feedback)
                    {
                        parent.UpdateSim();
                    }
                }

                if (!Feedback)
                {
                    Thread.Sleep(Speed);
                }
            }

            bool hadFeedback = Feedback;
            Feedback = true;
            parent.UpdateOut();
            parent.UpdateSim();

            SaveStatistics();
            Console.WriteLine("Simulation finished");
            Feedback = hadFeedback;
        }

        private void UpdateStatistics(Event e)
        {
            double elapsedTime = e.Time - Time;
            UpdateStatistic(Machine.M1a, elapsedTime);
            UpdateStatistic(Machine.M1b, elapsedTime);
            UpdateStatistic(Machine.M1c, elapsedTime);
            UpdateStatistic(Machine.M1d, elapsedTime);
            UpdateStatistic(Machine.M2a, elapsedTime);
            UpdateStatistic(Machine.M2b, elapsedTime);
            UpdateStatistic(Machine.M3a, elapsedTime);
            UpdateStatistic(Machine.M3b, elapsedTime);
            UpdateStatistic(Machine.M4a, elapsedTime);
            UpdateStatistic(Machine.M4b, elapsedTime);

            Statistics["bufferA"].Add(e.Time, BufferA.Count);
            Statistics["bufferB"].Add(e.Time, BufferB.Count);
            Statistics["batchAin"].Add(e.Time, dvdReadyForM3a.Count);
            Statistics["batchBin"].Add(e.Time, dvdReadyForM3b.Count);
            Statistics["batchAout"].Add(e.Time, dvdReadyForInputM4a.Count);
            Statistics["batchBout"].Add(e.Time, dvdReadyForInputM4b.Count);
        }

        private void UpdateStatistic(Machine machine, double elapsedTime)
        {
            if (MachineState[machine] == State.BUSY)
            {
                BusyTime[machine] += elapsedTime;
            }
            else if (MachineState[machine] == State.BLOCKED)
            {
                BlockedTime[machine] += elapsedTime;
            }
            else if (MachineState[machine] == State.IDLE)
            {
                IdleTime[machine] += elapsedTime;
            }
        }

        private void SaveStatistics()
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            Console.WriteLine("Simulation Saving Statistics");

            string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory(), "run*");
            string directory = Directory.GetCurrentDirectory() + "\\run" + dirs.Count();

            Directory.CreateDirectory(directory);

            StoreFile(directory, "start", dvdStartTimes);
            StoreFile(directory, "failed", dvdFailed);
            StoreFile(directory, "finished", dvdProduced);

            StoreFile(directory, "throughput", dvdThroughput);


            StoreFile(directory, "bufferA", Statistics["bufferA"]);
            StoreFile(directory, "bufferB", Statistics["bufferB"]);
            StoreFile(directory, "batchAin", Statistics["batchAin"]);
            StoreFile(directory, "batchBin", Statistics["batchBin"]);
            StoreFile(directory, "batchAout", Statistics["batchAout"]);
            StoreFile(directory, "batchBout", Statistics["batchBout"]);

            StoreStatistics(directory);

        }

        private void StoreStatistics(string dir)
        {
            using (StreamWriter file = new StreamWriter(dir + "\\statistics.txt"))
            {
                file.WriteLine("runTime:\t{0:D}", RUN_LENGTH);
                file.WriteLine("buffer:\t{0:D}", BUFFER_SIZE);
                file.WriteLine("batch:\t{0:D}", CRATE_SIZE);
                file.WriteLine();
                file.WriteLine("dvdProduced:\t{0:D}", dvdProduced.Count);
                file.WriteLine("dvdProduced:\t{0:F}", dvdProduced.Count / (RUN_LENGTH / 3600.0));
                file.WriteLine("dvdProduced:\t{0:F}", (dvdProduced.Count > 0 ? dvdProduced.Count / ((RUN_LENGTH - dvdProduced.First()) / 3600) : 0));

                if (dvdThroughput.Count > 0)
                {
                    file.WriteLine("avgThroughput:\t{0:F}", dvdThroughput.Average());
                    file.WriteLine("minThroughput:\t{0:F}", dvdThroughput.Min());
                    file.WriteLine("maxThroughput:\t{0:F}", dvdThroughput.Max());
                }
                else
                {
                    file.WriteLine("avgThroughput:\r\nminThroughput\r\nmaxThroughput");
                }

                file.WriteLine();
                file.WriteLine("Machine: Idle, Busy, Blocked, Broken time");
                file.WriteLine(MachineStatistic(Machine.M1a));
                file.WriteLine(MachineStatistic(Machine.M1b));
                file.WriteLine(MachineStatistic(Machine.M1c));
                file.WriteLine(MachineStatistic(Machine.M1d));
                file.WriteLine(MachineStatistic(Machine.M2a));
                file.WriteLine(MachineStatistic(Machine.M2b));
                file.WriteLine(MachineStatistic(Machine.M3a));
                file.WriteLine(MachineStatistic(Machine.M3b));
                file.WriteLine(MachineStatistic(Machine.M4a));
                file.WriteLine(MachineStatistic(Machine.M4b));
            }
        }

        private string MachineStatistic(Machine machine)
        {
            return String.Format("{0}\t{1:F}\t{2:F}\t{3:F}\t{4:F}", machine, IdleTime[machine], BusyTime[machine], BlockedTime[machine], Time - IdleTime[machine] - BusyTime[machine] - BlockedTime[machine]);
        }

        private void StoreFile(string dir, string filename, IList list)
        {
            using (StreamWriter file = new StreamWriter(dir + "\\" + filename + ".txt"))
            {
                foreach (var item in list)
                {
                    file.WriteLine(item);
                }
            }
        }
        private void StoreFile(string dir, string filename, Dictionary<double, int> dictionary)
        {
            using (StreamWriter file = new StreamWriter(dir + "\\" + filename + ".txt"))
            {
                var list = dictionary.ToList();
                if (list.Count > 0)
                {
                    int last = list[0].Value;
                    file.Write("{0:F}\t{1:D}", list[0].Key, list[0].Value);
                    for (int i = 1; i < list.Count; i++)
                    {
                        if (last != list[i].Value)
                        {
                            last = list[i].Value;
                            file.Write("\r\n{0:F}\t{1:D}", list[i].Key, list[i].Value);
                        }
                    }
                }
            }
        }

        private void M1Finished(Event e)
        {
            double time = e.Time;
            Machine machine = e.Machine;

            State state = MachineState[machine];
            if (state == State.BROKEN)
            {
                // M1 is broken which cause the dvd that was supposed to be finished to be still in M1
                M1ShouldHaveFinished[machine] = e;
            }
            else if (state == State.WASBROKEN)
            {
                // M1 has been broken during producing the dvd
                // the repair time of the machine is being added to the finishing time of the dvd
                MachineState[machine] = State.BUSY;

                // schedule M1Finished at time machine 1 was broken 
                eventList.Add(new Event(time + (time - TimeM1HasBrokenDown[machine]), e.Type, machine, e.DVD));

                TimeM1HasBrokenDown.Remove(machine);
                M1ShouldHaveFinished.Remove(machine);
            }
            else if (machine == Machine.M1a || machine == Machine.M1b)
            {
                // add dvd to buffer
                BufferA.Enqueue(e.DVD);

                // keep producing dvd's, schedule new M1Finished
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
                // add dvd to buffer
                BufferB.Enqueue(e.DVD);

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
            scheduleAddDVDToCrate(e.Time, e.Machine, e.DVD);

            // schedule M2Finished
            scheduleM2(e.Time, e.Machine);

            // check if a machine 1 was blocked and need to be scheduled again
            if (e.Machine == Machine.M2a)
            {
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
            {
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
            if (e.Machine == Machine.M2a)
            {
                dvdReadyForM3a.Enqueue(e.DVD);

                // Check if a crate is full and therefore ready to be put in machine 3
                if (dvdReadyForM3a.Count >= CRATE_SIZE)
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
            else // add to different crate
            {
                dvdReadyForM3b.Enqueue(e.DVD);

                // Check if a crate is full and therefore ready to be put in machine 3
                if (dvdReadyForM3b.Count >= CRATE_SIZE)
                {
                    // If M3 is available we start it's production
                    if (MachineState[Machine.M3b] == State.IDLE)
                    {
                        MachineState[Machine.M3b] = State.BUSY;
                        scheduleM3(e.Time, Machine.M3b);
                    }
                    else if (MachineState[Machine.M3a] == State.IDLE)
                    {
                        MachineState[Machine.M3a] = State.BUSY;
                        scheduleM3(e.Time, Machine.M3a);
                    }
                }
            }
        }

        private void M3Finished(Event e)
        {
            scheduleM3(e.Time, e.Machine);

            // add crate to the an empty buffer
            bool toBufferA = dvdReadyForInputM4a.Count > dvdReadyForInputM4b.Count ? false : true;
            foreach (int dvd in e.DVDs)
            {
                if (toBufferA)
                {
                    dvdReadyForInputM4a.Enqueue(dvd);
                }
                else
                {
                    dvdReadyForInputM4b.Enqueue(dvd);
                }
            }

            if (toBufferA)
            {
                // M3 is finished and starts M4 if M4 is available
                if (MachineState[Machine.M4a] == State.IDLE)
                {
                    MachineState[Machine.M4a] = State.BUSY;
                    scheduleM4(e.Time, Machine.M4a);
                }
            }
            else if (MachineState[Machine.M4b] == State.IDLE)
            {
                MachineState[Machine.M4b] = State.BUSY;
                scheduleM4(e.Time, Machine.M4b);
            }
        }

        private void M4Finished(Event e)
        {
            // update statistics
            // The chance the dvd has failed during the production
            if (input.DVDFails())
            {
                dvdFailed.Add(e.Time);
            }
            else
            {
                dvdProduced.Add(e.Time);
            }
            dvdInProduction--;
            dvdBeforeM4Service[e.Machine]--;
            dvdThroughput.Add(e.Time - StartTimes[e.DVD]);

            StartTimes.Remove(e.DVD);


            int before = (e.Machine == Machine.M4a ? dvdReadyForInputM4a.Count : dvdReadyForInputM4b.Count);

            scheduleM4(e.Time, e.Machine);

            // If M4 emptied a whole crate
            if (e.Machine == Machine.M4a)
            {
                if (before % CRATE_SIZE == 1 && dvdReadyForInputM4a.Count % CRATE_SIZE == 0)
                {
                    // a crate is empty and ready to be filled again
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
            else if (before % CRATE_SIZE == 1 && dvdReadyForInputM4b.Count % CRATE_SIZE == 0)
            {
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

        private void BreakdownM1(Event e)
        {
            MachineState[e.Machine] = (MachineState[e.Machine] == State.BLOCKED ? State.BBROKEN : State.BROKEN);
            TimeM1HasBrokenDown[e.Machine] = e.Time;

            // schedule repair machine 1: 2 hours exp distr.
            scheduleM1Repair(e.Time, e.Machine);
        }

        private void RepairM1(Event e)
        {
            // schedule breakdown machine 1: 
            scheduleM1Breakdown(e.Time, e.Machine);

            // check if the machine was in the meantime finished
            if (M1ShouldHaveFinished.ContainsKey(e.Machine))
            {
                // schedule machine 1 finished: time now (= de tijd dat hij gerepareerd is) + time product should have finished - time broken down
                double delay = M1ShouldHaveFinished[e.Machine].Time - TimeM1HasBrokenDown[e.Machine];

                //repair time 2 hours exp. 
                MachineState[e.Machine] = State.BUSY;
                eventList.Add(new Event(e.Time + delay, Type.MACHINE_1, e.Machine, M1ShouldHaveFinished[e.Machine].DVD));

                // reset variables
                TimeM1HasBrokenDown.Remove(e.Machine);
                M1ShouldHaveFinished.Remove(e.Machine);
            }
            else if (MachineState[e.Machine] == State.BBROKEN)
            {
                MachineState[e.Machine] = State.BUSY;
                scheduleM1(e.Time, e.Machine);
            }
            else
            {
                // het is nog niet bekend hoe lang de dvd in de machine heeft gezeten als die kapot is geweest
                MachineState[e.Machine] = State.WASBROKEN;
            }
        }

        private void RepairM4(Event e)
        {
            dvdBeforeM4Service[e.Machine] = input.M4Service();
            MachineState[e.Machine] = State.BUSY;
            scheduleM4(e.Time, e.Machine);
        }

        private void scheduleM1(double time, Machine machine)
        {
            int limit, buffer = BUFFER_SIZE;

            // the limit of the buffer is 19 when the other machine will produce the 20th dvd.
            if (machine == Machine.M1a || machine == Machine.M1b)
            {
                buffer = BufferA.Count;
                limit = (MachineState[Machine.M1a] == State.BUSY && MachineState[Machine.M1b] == State.BUSY ? BUFFER_SIZE - 1 : BUFFER_SIZE);
            }
            else
            {
                buffer = BufferB.Count;
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
                dvdCounter++;

                // keep track which dvd starts production
                StartTimes[dvdCounter] = time;
                dvdStartTimes.Add(time);

                double processTime = input.M1();

                eventList.Add(new Event(time + processTime, Type.MACHINE_1, machine, dvdCounter));
            }
        }

        private void scheduleM2(double time, Machine machine)
        {
            // check the limit and the dvds that are ready depending on the machine to be scheduled
            int limit = CRATE_SIZE - (MachineState[Machine.M2a] == State.BUSY && MachineState[Machine.M2b] == State.BUSY ? 0 : 1);
            int dvdOutput = machine == Machine.M2a ? dvdReadyForM3a.Count : dvdReadyForM3b.Count;

            if (dvdOutput <= limit)
            {
                if (machine == Machine.M2a)
                {
                    if (BufferA.Count > 0)
                    {
                        int dvd = BufferA.Dequeue();
                        double processTime = input.M2();
                        eventList.Add(new Event(time + processTime, Type.MACHINE_2, machine, dvd));
                    }
                    else
                    {
                        // no input for the machine
                        MachineState[machine] = State.IDLE;
                    }
                }
                else if (machine == Machine.M2b)
                {
                    if (BufferB.Count > 0)
                    {
                        int dvd = BufferB.Dequeue();
                        double processTime = input.M2();
                        eventList.Add(new Event(time + processTime, Type.MACHINE_2, machine, dvd));
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

        private void scheduleAddDVDToCrate(double time, Machine machine, int dvd)
        {
            double processTime = input.ToCrate();
            eventList.Add(new Event(time + processTime, Type.ADD_TO_CRATE, machine, dvd));
        }

        private void scheduleM3(double time, Machine machine)
        {
            // If a full crate is available for input M3, start producing this crate. Else, output the crate and go back to waiting for input. 
            if (dvdReadyForM3a.Count >= CRATE_SIZE)
            {
                // calculate the processing time
                List<int> crate = new List<int>();

                for (int i = 0; i < CRATE_SIZE; i++)
                {
                    crate.Add(dvdReadyForM3a.Dequeue());
                }

                double processTime = input.M3(CRATE_SIZE);
                eventList.Add(new Event(time + processTime, Type.MACHINE_3, machine, crate));
            }
            else if (dvdReadyForM3b.Count >= CRATE_SIZE)
            {
                List<int> crate = new List<int>();
                for (int i = 0; i < CRATE_SIZE; i++)
                {
                    crate.Add(dvdReadyForM3b.Dequeue());
                }

                double processTime = input.M3(CRATE_SIZE);
                eventList.Add(new Event(time + processTime, Type.MACHINE_3, machine, crate));
            }
            else
            {
                // no crates available
                MachineState[machine] = State.IDLE;
            }
        }

        private void scheduleM4(double time, Machine machine)
        {
            if (dvdBeforeM4Service[machine] == 0)
            {
                MachineState[machine] = State.BROKEN;
                scheduleM4Repair(time, machine);
            }
            else if (machine == Machine.M4a && dvdReadyForInputM4a.Count > 0)
            {
                int dvd = dvdReadyForInputM4a.Dequeue();

                double processTime = input.M4();
                eventList.Add(new Event(time + processTime, Type.MACHINE_4, machine, dvd));
            }
            else if (machine == Machine.M4b && dvdReadyForInputM4b.Count > 0)
            {
                int dvd = dvdReadyForInputM4b.Dequeue();
                double processTime = input.M4();
                eventList.Add(new Event(time + processTime, Type.MACHINE_4, machine, dvd));
            }
            else if (machine == Machine.M4a && dvdReadyForInputM4b.Count == CRATE_SIZE && MachineState[Machine.M4b] == State.BROKEN)
            {
                Console.WriteLine("Crate switch");

                //the crate switch costs no time
                for (int i = 0; i < CRATE_SIZE; i++)
                {
                    int dvd = dvdReadyForInputM4b.Dequeue();
                    dvdReadyForInputM4a.Enqueue(dvd);
                }
                scheduleM4(time, machine);
            }
            else if (machine == Machine.M4b && dvdReadyForInputM4a.Count >= CRATE_SIZE && MachineState[Machine.M4a] == State.BROKEN)
            {
                Console.WriteLine("Crate switch");

                //the crate switch costs no time
                for (int i = 0; i < CRATE_SIZE; i++)
                {
                    int dvd = dvdReadyForInputM4a.Dequeue();
                    dvdReadyForInputM4b.Enqueue(dvd);
                }
                scheduleM4(time, machine);
            }
            else
            {
                MachineState[machine] = State.IDLE;
            }
        }

        private void scheduleM1Breakdown(double time, Machine machine)
        {
            double breakTime = input.M1Breakdown();
            eventList.Add(new Event(time + breakTime, Type.BREAKDOWN_1, machine));
        }
        private void scheduleM1Repair(double time, Machine machine)
        {
            double breakTime = input.M1Repair();
            eventList.Add(new Event(time + breakTime, Type.REPAIRED_1, machine));
        }
        private void scheduleM4Repair(double time, Machine machine)
        {
            double breakTime = input.M4Repair();
            eventList.Add(new Event(time + breakTime, Type.REPAIRED_4, machine));
        }
    }
}
