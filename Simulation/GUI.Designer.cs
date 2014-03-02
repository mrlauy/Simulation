namespace Simulation
{
    partial class GUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelBufferB = new System.Windows.Forms.Label();
            this.labelBufferA = new System.Windows.Forms.Label();
            this.panelM2B = new System.Windows.Forms.Panel();
            this.labelM2B = new System.Windows.Forms.Label();
            this.panelM2A = new System.Windows.Forms.Panel();
            this.labelM2A = new System.Windows.Forms.Label();
            this.panelM1D = new System.Windows.Forms.Panel();
            this.labelM1D = new System.Windows.Forms.Label();
            this.panelM1C = new System.Windows.Forms.Panel();
            this.labelM1C = new System.Windows.Forms.Label();
            this.panelM1B = new System.Windows.Forms.Panel();
            this.labelM1B = new System.Windows.Forms.Label();
            this.panelM1A = new System.Windows.Forms.Panel();
            this.labelM1A = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.timeLabel = new System.Windows.Forms.Label();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panelM2B.SuspendLayout();
            this.panelM2A.SuspendLayout();
            this.panelM1D.SuspendLayout();
            this.panelM1C.SuspendLayout();
            this.panelM1B.SuspendLayout();
            this.panelM1A.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(585, 33);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(585, 62);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Pause";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelBufferB);
            this.panel1.Controls.Add(this.labelBufferA);
            this.panel1.Controls.Add(this.panelM2B);
            this.panel1.Controls.Add(this.panelM2A);
            this.panel1.Controls.Add(this.panelM1D);
            this.panel1.Controls.Add(this.panelM1C);
            this.panel1.Controls.Add(this.panelM1B);
            this.panel1.Controls.Add(this.panelM1A);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(563, 224);
            this.panel1.TabIndex = 2;
            // 
            // labelBufferB
            // 
            this.labelBufferB.AutoSize = true;
            this.labelBufferB.Location = new System.Drawing.Point(119, 121);
            this.labelBufferB.Name = "labelBufferB";
            this.labelBufferB.Size = new System.Drawing.Size(13, 13);
            this.labelBufferB.TabIndex = 5;
            this.labelBufferB.Text = "0";
            // 
            // labelBufferA
            // 
            this.labelBufferA.AutoSize = true;
            this.labelBufferA.Location = new System.Drawing.Point(119, 80);
            this.labelBufferA.Name = "labelBufferA";
            this.labelBufferA.Size = new System.Drawing.Size(13, 13);
            this.labelBufferA.TabIndex = 4;
            this.labelBufferA.Text = "0";
            // 
            // panelM2B
            // 
            this.panelM2B.BackColor = System.Drawing.SystemColors.Window;
            this.panelM2B.Controls.Add(this.labelM2B);
            this.panelM2B.Location = new System.Drawing.Point(166, 112);
            this.panelM2B.Name = "panelM2B";
            this.panelM2B.Size = new System.Drawing.Size(55, 37);
            this.panelM2B.TabIndex = 3;
            // 
            // labelM2B
            // 
            this.labelM2B.AutoSize = true;
            this.labelM2B.Location = new System.Drawing.Point(3, 0);
            this.labelM2B.Name = "labelM2B";
            this.labelM2B.Size = new System.Drawing.Size(28, 13);
            this.labelM2B.TabIndex = 0;
            this.labelM2B.Text = "M2b";
            // 
            // panelM2A
            // 
            this.panelM2A.BackColor = System.Drawing.SystemColors.Window;
            this.panelM2A.Controls.Add(this.labelM2A);
            this.panelM2A.Location = new System.Drawing.Point(166, 69);
            this.panelM2A.Name = "panelM2A";
            this.panelM2A.Size = new System.Drawing.Size(55, 37);
            this.panelM2A.TabIndex = 3;
            // 
            // labelM2A
            // 
            this.labelM2A.AutoSize = true;
            this.labelM2A.Location = new System.Drawing.Point(3, 0);
            this.labelM2A.Name = "labelM2A";
            this.labelM2A.Size = new System.Drawing.Size(28, 13);
            this.labelM2A.TabIndex = 0;
            this.labelM2A.Text = "M2a";
            // 
            // panelM1D
            // 
            this.panelM1D.BackColor = System.Drawing.SystemColors.Window;
            this.panelM1D.Controls.Add(this.labelM1D);
            this.panelM1D.Location = new System.Drawing.Point(27, 155);
            this.panelM1D.Name = "panelM1D";
            this.panelM1D.Size = new System.Drawing.Size(55, 37);
            this.panelM1D.TabIndex = 2;
            // 
            // labelM1D
            // 
            this.labelM1D.AutoSize = true;
            this.labelM1D.Location = new System.Drawing.Point(3, 0);
            this.labelM1D.Name = "labelM1D";
            this.labelM1D.Size = new System.Drawing.Size(28, 13);
            this.labelM1D.TabIndex = 0;
            this.labelM1D.Text = "M1d";
            // 
            // panelM1C
            // 
            this.panelM1C.BackColor = System.Drawing.SystemColors.Window;
            this.panelM1C.Controls.Add(this.labelM1C);
            this.panelM1C.Location = new System.Drawing.Point(27, 112);
            this.panelM1C.Name = "panelM1C";
            this.panelM1C.Size = new System.Drawing.Size(55, 37);
            this.panelM1C.TabIndex = 2;
            // 
            // labelM1C
            // 
            this.labelM1C.AutoSize = true;
            this.labelM1C.Location = new System.Drawing.Point(3, 0);
            this.labelM1C.Name = "labelM1C";
            this.labelM1C.Size = new System.Drawing.Size(28, 13);
            this.labelM1C.TabIndex = 0;
            this.labelM1C.Text = "M1c";
            // 
            // panelM1B
            // 
            this.panelM1B.BackColor = System.Drawing.SystemColors.Window;
            this.panelM1B.Controls.Add(this.labelM1B);
            this.panelM1B.Location = new System.Drawing.Point(27, 69);
            this.panelM1B.Name = "panelM1B";
            this.panelM1B.Size = new System.Drawing.Size(55, 37);
            this.panelM1B.TabIndex = 2;
            // 
            // labelM1B
            // 
            this.labelM1B.AutoSize = true;
            this.labelM1B.Location = new System.Drawing.Point(3, 0);
            this.labelM1B.Name = "labelM1B";
            this.labelM1B.Size = new System.Drawing.Size(28, 13);
            this.labelM1B.TabIndex = 0;
            this.labelM1B.Text = "M1b";
            // 
            // panelM1A
            // 
            this.panelM1A.BackColor = System.Drawing.SystemColors.Window;
            this.panelM1A.Controls.Add(this.labelM1A);
            this.panelM1A.Location = new System.Drawing.Point(27, 26);
            this.panelM1A.Name = "panelM1A";
            this.panelM1A.Size = new System.Drawing.Size(55, 37);
            this.panelM1A.TabIndex = 1;
            // 
            // labelM1A
            // 
            this.labelM1A.AutoSize = true;
            this.labelM1A.Location = new System.Drawing.Point(3, 0);
            this.labelM1A.Name = "labelM1A";
            this.labelM1A.Size = new System.Drawing.Size(28, 13);
            this.labelM1A.TabIndex = 0;
            this.labelM1A.Text = "M1a";
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(582, 13);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(33, 13);
            this.labelTime.TabIndex = 3;
            this.labelTime.Text = "Time:";
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Location = new System.Drawing.Point(640, 12);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(13, 13);
            this.timeLabel.TabIndex = 4;
            this.timeLabel.Text = "0";
            // 
            // txtConsole
            // 
            this.txtConsole.Location = new System.Drawing.Point(12, 242);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsole.Size = new System.Drawing.Size(820, 199);
            this.txtConsole.TabIndex = 5;
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 453);
            this.Controls.Add(this.txtConsole);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "GUI";
            this.Text = "GUI";
            this.Load += new System.EventHandler(this.Form_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelM2B.ResumeLayout(false);
            this.panelM2B.PerformLayout();
            this.panelM2A.ResumeLayout(false);
            this.panelM2A.PerformLayout();
            this.panelM1D.ResumeLayout(false);
            this.panelM1D.PerformLayout();
            this.panelM1C.ResumeLayout(false);
            this.panelM1C.PerformLayout();
            this.panelM1B.ResumeLayout(false);
            this.panelM1B.PerformLayout();
            this.panelM1A.ResumeLayout(false);
            this.panelM1A.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelM1A;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Panel panelM1D;
        private System.Windows.Forms.Label labelM1D;
        private System.Windows.Forms.Panel panelM1C;
        private System.Windows.Forms.Label labelM1C;
        private System.Windows.Forms.Panel panelM1B;
        private System.Windows.Forms.Label labelM1B;
        private System.Windows.Forms.Panel panelM1A;
        private System.Windows.Forms.Label labelBufferA;
        private System.Windows.Forms.Panel panelM2B;
        private System.Windows.Forms.Label labelM2B;
        private System.Windows.Forms.Panel panelM2A;
        private System.Windows.Forms.Label labelM2A;
        private System.Windows.Forms.Label labelBufferB;
        private System.Windows.Forms.TextBox txtConsole;
    }
}