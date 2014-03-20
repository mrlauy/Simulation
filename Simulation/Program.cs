using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GUI());
            // new RunConsole();
        }

        class RunConsole : IUpdate
        {
            public RunConsole()
            {
                Console.WriteLine("Start simulation");

                Simulation simulation = new Simulation(this);

                // Create the thread object, passing in the Alpha.Beta method
                // via a ThreadStart delegate. This does not start the thread.
                Thread thread = new Thread(new ThreadStart(simulation.Run));

                // Start the thread
                thread.Start();


                thread.Join();

                Console.WriteLine("Press a key to close...");
                Console.ReadKey();
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
    }
}
