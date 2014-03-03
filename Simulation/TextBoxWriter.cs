using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulation
{
    public class TextBoxWriter : TextWriter
    {
        TextBox _output = null;

        public TextBoxWriter(TextBox output)
        {
            _output = output;
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(value);
            // _output.AppendText(value.ToString());
         
            if (_output.InvokeRequired)
            {
                _output.Invoke((MethodInvoker)(() => _output.AppendText(value.ToString() + Environment.NewLine)));
            }
            else
            {
                _output.AppendText(value.ToString() + Environment.NewLine);
            }
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
