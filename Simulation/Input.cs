using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Input
    {
        private Random random;
        private double[] M1Times;
        private double[] M2Times;
        private double[] M4Times;

        public Input()
        {
            random = new Random();
            M1Times = M1Observations();
            M2Times = M2Observations();
            M4Times = M4Observations();
            Array.Sort(M1Times);
            Array.Sort(M2Times);
            Array.Sort(M4Times);
        }

        /// <summary>
        /// Processing time of machine 1
        /// </summary>
        /// <returns>processing time</returns>
        public double M1()
        {
            return Exp(59.9395426);
        }

        /// <summary>
        /// Processing time of machine 1, based on observations
        /// </summary>
        /// <returns>processing time</returns>
        public double M1_()
        {
            //we have an array with our sorted observed procution times
            //these times variate between 0-541 seconds.

            //We let a random number decide which observed proc. time to use. 
            double u = random.NextDouble();
            double randompickD = u * M1Times.Length;
            int randompick = (int)randompickD;
            return M1Times[randompick];
        }

        /// <summary>
        /// Processing time of machine 2
        /// </summary>
        /// <returns>processing time</returns>
        public double M2()
        {
            return Gamma(12.90103617, 1.895859505); 
        }

        /// <summary>
        /// Time the dvd is spending on the conveyor belt
        /// </summary>
        /// <returns></returns>
        public double ToCrate()
        {
            return Exp(5 * 60);   // 5 min exp dist
        }

        /// <summary>
        /// Processing time of machine 3
        /// </summary>
        /// <returns>processing time</returns>
        public double M3(int dvds)
        {
            double sputteringTime = 0;
            double lacquerTime = 0;

            for (int i = 0; i < dvds; i++)
            {
                sputteringTime += Exp(10);
                lacquerTime += Exp(6);
            }

            double processTime = sputteringTime + lacquerTime + 3 * 60; // exp(10 sec) + exp(6 sec) + 3 min

            // with 3 percent per dvd, the batch is delayed with average 5 min to clean
            double chance = random.NextDouble();
            if (chance < 0.03 * dvds)
            {
                processTime += Exp(5 * 60);
            }

            return processTime;
        }

        /// <summary>
        /// Processing time of machine 4
        /// </summary>
        /// <returns>processing time</returns>
        public double M4()
        {
            //We let a random number decide which observed proc. time to use. 
            double u = random.NextDouble();
            double randompickD = u * M4Times.Length;
            int randompick = (int)randompickD;
            return M4Times[randompick];
        }

        /// <summary>
        /// return true if the DVD has failed during production
        /// </summary>
        /// <returns>failed</returns>
        public bool DVDFails()
        {
            double chance = random.NextDouble();
            return chance < 0.02; // 2% van de dvds
        }

        /// <summary>
        /// Time until machine will breakdown, an exponential distribution with a mean of 8 hour
        /// </summary>
        /// <returns>breakdown time</returns>
        public double M1Breakdown()
        {
            return Exp(8 * 60 * 60);   // 8 hours exp distr
        }

        /// <summary>
        /// Time until machine will repaired after broken down. 
        /// </summary>
        /// <returns>breakdown time</returns>
        public double M1Repair()
        {
            return Exp(2 * 60 * 60);   // 2 hours exp distr
        }

        /// <summary>
        /// The amount of DVDs that machine 4 can process before needing service
        /// </summary>
        /// <returns>DVD before service</returns>
        public int M4Service()
        {
            double u = random.NextDouble();
            if (u > 0.8)
            {
                return 202;
            }
            else if (u > 0.6)
            {
                return 201;
            }
            else if (u > 0.4)
            {
                return 200;
            }
            else if (u > 0.2)
            {
                return 199;
            }
            else
            {
                return 198;
            }
        }

        /// <summary>
        /// Time machine 4 is down during service
        /// </summary>
        /// <returns>service time</returns>
        public double M4Repair()
        {
            return Exp(15 * 60);   // 15 min exp distr
        }

        /// <summary>
        /// Exponential distribution
        /// </summary>
        /// <param name="mean">mean</param>
        /// <returns>random value</returns>
        private double Exp(double mean)
        {
            double u = random.NextDouble();
            return -mean * Math.Log(1 - u);
        }

        public double Gamma(double a, double b)
        {
            // assert( b > 0. && c > 0. );
            double A = 1.0 / Math.Sqrt(2 * b - 1);
            double B = b - Math.Log(4);
            double Q = b + 1 / A;
            double T = 4.5;
            double D = 1 + Math.Log(T);
            double C = 1 + b / Math.E;

            if (b < 1.0)
            {
                while (true)
                {
                    double u = random.NextDouble();
                    double p = C * u;

                    if (p > 1)
                    {
                        double y = -Math.Log((C - p) / b);
                        double u2 = random.NextDouble();
                        if (u2 <= Math.Pow(y, b - 1))
                        {
                            return a * y;
                        }
                    }
                    else
                    {
                        double y = Math.Pow(p, 1 / b);
                        double u2 = random.NextDouble();
                        if (u2 <= Math.Exp(-y))
                        {
                            return a * y;
                        }
                    }
                }
            }
            else if (b == 1.0)
            {
                return Exp(a);
            }
            else
            {
                while (true)
                {
                    double u1 = random.NextDouble();
                    double u2 = random.NextDouble();
                    double v = A * Math.Log(u1 / (1 - u1));
                    double y = b * Math.Exp(v);
                    double z = u1 * u1 * u2;
                    double w = B + Q * v - y;
                    if (w + D - T * z >= 0 || w >= Math.Log(z))
                    {
                        return a * y;
                    }
                }
            }

        }

        public double Weibull(double A, double B)
        {
            // F−1(u) = [−αln(1−u)]1/β      0 < u < 1.
            double u = random.NextDouble();
            return Math.Pow(-A * Math.Log(1 - u), 1 / B);
        }

        /// <summary>
        /// read and return the observated processing times from machine 1
        /// </summary>
        /// <returns>list of processing times</returns>
        public double[] M1Observations()
        {
            Console.WriteLine("Reading Machine 1 observations.");
            string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string exeDir = System.IO.Path.GetDirectoryName(exeLocation);
            string file = Path.Combine(exeDir, @"data\M1Observations.txt");

            //return de array met processing tijden 
            return ReadObservations(file);
        }

        public double[] M2Observations()
        {
            Console.WriteLine("Reading Machine 2 observations.");
            string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string exeDir = System.IO.Path.GetDirectoryName(exeLocation);
            string file = Path.Combine(exeDir, @"data\M2Observations.txt");

            //return de array met processing tijden 
            return ReadObservations(file);
        }

        public double[] M4Observations()
        {
            Console.WriteLine("Reading Machine 4 observations.");
            string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string exeDir = System.IO.Path.GetDirectoryName(exeLocation);
            string file = Path.Combine(exeDir, @"data\M4Observations.txt");

            //return de array met processing tijden 
            return ReadObservations(file);
        }

        /// <summary>
        /// Read observation from data file
        /// </summary>
        /// <param name="file">the file with the values</param>
        /// <returns>list of process times</returns>
        private double[] ReadObservations(string file)
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(file);
                double[] values = new double[lines.Length];

                // Use the file contents to fill an array of doubles by using a foreach loop.
                for (int i = 0; i < lines.Length; i++)
                {
                    values[i] = double.Parse(lines[i]);
                }

                return values;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found: " + file);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Reading file exception: " + e.Message);
            }
            return new double[0];
        }
    }
}
