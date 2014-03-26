using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simulation
{
    class MultiSim : IUpdate
    {
        private GUI gui;
        public MultiSim(GUI parent)
        {
            Thread thread = new Thread(new ThreadStart(Run));
            gui = parent;
            thread.Start();
        }

        public void Run()
        {
            Console.WriteLine("Start multiple Simulations");
            List<Data> data = new List<Data>();
            int runLength = 10000;
            int bufferSize = 10;
            int crateSize = 10;

            Simulation sim = new Simulation(this);
            sim.Feedback = false;

            sim.ChangeSimulation(runLength, bufferSize, crateSize);
            sim.Initialize();
            sim.Run();
            data.Add(WrapData(1, sim, runLength, bufferSize, crateSize));
            Console.WriteLine("Simulation 1 finished");

            bufferSize = 15; 
            crateSize = 15;
            sim.ChangeSimulation(runLength, bufferSize, crateSize);
            sim.Initialize();
            sim.Run();
            data.Add(WrapData(2, sim, runLength, bufferSize, crateSize));
            Console.WriteLine("Simulation 2 finished");


            bufferSize = 20;
            crateSize = 20;
            sim.ChangeSimulation(runLength, bufferSize, crateSize);
            sim.Initialize();
            sim.Run();
            data.Add(WrapData(3, sim, runLength, bufferSize, crateSize));
            Console.WriteLine("Simulation 3 finished");

            gui.Enable();

        }

        private Data WrapData(int number, Simulation sim, int runLength, int bufferSize, int crateSize)
        {
            Data data = new Data(number, runLength, bufferSize, crateSize);
            data.Produced = sim.dvdProduced;
            data.Failed = sim.dvdFailed;
            data.ProductionHour = (sim.dvdProduced.Count > 0 ? sim.dvdProduced.Count / ((runLength - sim.dvdProduced.First()) / 3600) : 0);
            data.Throughput = sim.dvdThroughput;
            data.IdleTime = sim.IdleTime;
            data.BusyTime = sim.BusyTime;
            data.BlockedTime = sim.BlockedTime;
            data.StartTimes = sim.dvdStartTimes;
            data.Statistics = sim.Statistics;
            return data;
        }

        public void UpdateSim()
        {
            //   no need for graphical updates
        }
        public void UpdateOut()
        {
            // no need to redirected console output
        }
    }

    class Data
    {
        public int Number { private set; get; }
        public int RunLength { private set; get; }
        public int BufferSize { private set; get; }
        public int CrateSize { private set; get; }

        public List<double> Produced { set; get; }
        public List<double> Failed { set; get; }
        public List<double> StartTimes { set; get; }
        public List<double> Throughput { set; get; }

        public double ProductionHour { set; get; }

        public Dictionary<Machine, double> IdleTime { set; get; }
        public Dictionary<Machine, double> BusyTime { set; get; }
        public Dictionary<Machine, double> BlockedTime { set; get; }
        public Dictionary<string, Dictionary<double, int>> Statistics { set; get; }

        public Data(int number, int runLength, int bufferSize, int crateSize)
        {
            Number = number;
            RunLength = runLength;
            BufferSize = bufferSize;
            CrateSize = crateSize;
        }
    }
}
