using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

namespace Simulation
{
    public partial class GUI : Form
    {
        private Simulation Sim;
        private static Dictionary<State, Color> COLOR = new Dictionary<State, Color>
        {
            {State.IDLE, Color.White},
            {State.BUSY, Color.Green},
            {State.BLOCKED, Color.Blue},
            {State.BROKEN, Color.DarkGray},
            {State.WASBROKEN, Color.Gray}
        };

        public GUI()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e) {
            Sim = new Simulation(this);
            Console.SetOut(new TextBoxWriter(txtConsole));
            Console.WriteLine("Now redirecting output to the text box");

        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (!Sim.Running)
            {
                // Create the thread object, passing in the Alpha.Beta method
                // via a ThreadStart delegate. This does not start the thread.
                Thread thread = new Thread(new ThreadStart(Sim.Run));

                // Start the thread
                thread.Start();
                buttonStart.Text = "Pause";
            }
            else if (Sim.Paused)
            {
                Sim.Paused = false;
                buttonStart.Text = "Start";
            }
            else
            {
                Sim.Paused = true;
                buttonStart.Text = "Pause";
            }
        }


        public void UpdateSim()
        {
            SetControlPropertyValue(timeLabel, "Text", Sim.Time.ToString());
            SetControlPropertyValue(labelDVDInProduction, "Text", Sim.dvdInProduction.ToString());
            SetControlPropertyValue(labelDVDProduced, "Text", Sim.dvdProduced.ToString());

            SetControlPropertyValue(labelBufferA, "Text", Sim.BufferA.ToString());
            SetControlPropertyValue(labelBufferB, "Text", Sim.BufferB.ToString());

            SetControlPropertyValue(labelCrateContent, "Text", Sim.dvdReadyForM3.ToString());
            SetControlPropertyValue(labelNumberOfCrates, "Text", Sim.cratesToBeFilledM3.ToString());
            SetControlPropertyValue(labelDVDoutput, "Text", Sim.dvdReadyForInputM4.ToString());

            SetControlPropertyValue(panelM1A, "BackColor", COLOR[Sim.MachineState[Machine.M1a]]);
            SetControlPropertyValue(panelM1B, "BackColor", COLOR[Sim.MachineState[Machine.M1b]]);
            SetControlPropertyValue(panelM1C, "BackColor", COLOR[Sim.MachineState[Machine.M1c]]);
            SetControlPropertyValue(panelM1D, "BackColor", COLOR[Sim.MachineState[Machine.M1d]]);
            SetControlPropertyValue(panelM2A, "BackColor", COLOR[Sim.MachineState[Machine.M2a]]);
            SetControlPropertyValue(panelM2B, "BackColor", COLOR[Sim.MachineState[Machine.M2b]]);
            SetControlPropertyValue(panelM3A, "BackColor", COLOR[Sim.MachineState[Machine.M3a]]);
            SetControlPropertyValue(panelM3B, "BackColor", COLOR[Sim.MachineState[Machine.M3b]]);
            SetControlPropertyValue(panelM4A, "BackColor", COLOR[Sim.MachineState[Machine.M4a]]);
            SetControlPropertyValue(panelM4B, "BackColor", COLOR[Sim.MachineState[Machine.M4b]]);
        }

        delegate void SetControlValueCallback(Control oControl, string propName, object propValue);
        private void SetControlPropertyValue(Control oControl, string propName, object propValue)
        {
            if (oControl.InvokeRequired)
            {
                SetControlValueCallback d = new SetControlValueCallback(SetControlPropertyValue);
                oControl.Invoke(d, new object[] { oControl, propName, propValue });
            }
            else
            {
                System.Type t = oControl.GetType();
                PropertyInfo[] props = t.GetProperties();
                foreach (PropertyInfo p in props)
                {
                    if (p.Name.ToUpper() == propName.ToUpper())
                    {
                        p.SetValue(oControl, propValue, null);
                    }
                }
            }
        }
    }
}
