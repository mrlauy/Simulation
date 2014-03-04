using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Simulation
{
    class ReadTimes
    {
        public void Read()
        {
            string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string exeDir = System.IO.Path.GetDirectoryName(exeLocation);
            string dataFile = Path.Combine(exeDir, @"data\im.txt");

            try
            {
                Debug.WriteLine("READ" + dataFile);
                if (File.Exists(dataFile))
                {
                    Debug.WriteLine("READING");
                    string[] lines = System.IO.File.ReadAllLines(dataFile);

                    // Display the file contents by using a foreach loop.
                    Console.WriteLine("Contents of WriteLines2.txt = " + lines);
                    foreach (string line in lines)
                    {
                        // Use a tab to indent each line of the file.
                        Console.WriteLine("\t" + line);
                    }
                }
                else
                {
                    Debug.WriteLine("ANFDKJAHFKADF");
                }
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
