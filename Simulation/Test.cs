using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;


namespace Simulation
{
    class Test
    {
        public Test()
        {
            Console.WriteLine("Testing with processing times.");
            Input pt = new Input();
            int n = 500;

            double[] values = new double[n];
            double[] values1 = new double[n];
            double[] values2 = new double[n];
            double[] values3 = new double[n];

            for (int i = 0; i < n; i++)
            {
                values[i] = pt.M2();
            }
            Array.Sort(values);

            for (int i = 0; i < n; i++)
            {
                values1[i] = pt.M2();
            }
            Array.Sort(values1);
            for (int i = 0; i < n; i++)
            {
                values2[i] = pt.M2();
            }
            Array.Sort(values2);


            double[] obs = pt.M2Observations();
            Array.Sort(obs);

            double avg1 = values1.Average();
            double avg1_ = 12.90103617 * 1.895859505;
            double avg2 = values2.Average();
            double avg2_ = 12.90103617 * 1.895859505;
            double avgO = obs.Average();
            double avgM_ = values.Average();

            Console.WriteLine("avg: {0:G} - {1:G}", avg1, avg1_);
            Console.WriteLine("avg: {0:G} - {1:G}", avg2, avg2_);
            Console.WriteLine("avg: {0:G} - {1:G}", avgO, avgM_);

            Chart chart = new Chart();
            chart.Size = new Size(800, 600);

            ChartArea area = new ChartArea("F");
            chart.ChartAreas.Add(area);
            chart.Legends.Add(new Legend());


            //Create a series using the data
            Series series = new Series();
            series.Points.DataBindY(values);

            //Set the chart type, Bar; horizontal bars
            series.ChartType = SeriesChartType.Line;
            //Assign it to the required area
            series.ChartArea = "F";

            //Create a series using the data


            Series serieO = new Series("M2 Observaties");
            serieO.LegendText = "M2 Observaties";
            serieO.IsVisibleInLegend = true;
            serieO.Points.DataBindY(obs);
            serieO.ChartType = SeriesChartType.Line;
            serieO.BorderDashStyle = ChartDashStyle.Dot;
            serieO.BorderWidth = 2;
            serieO.ChartArea = "F";
            serieO.Color = Color.Red;
            chart.Series.Add(serieO);

            //Add the series to the chart
            // chart.Series.Add(series);
            chart.Series.Add(Serie("M2 values 1", values1, Color.Blue));
            chart.Series.Add(Serie("M2 values 2", values2, Color.Green));
            // chart.Series.Add(Serie("M2 Observaties", obs, Color.Red));



            // Save the shit
            chart.SaveImage("graph.png", ChartImageFormat.Png);

            Console.WriteLine("Saved test graph.");
        }


        private Series Serie(string name, IEnumerable values, Color c)
        {
            Series series = new Series(name);
            series.LegendText = name;
            series.IsVisibleInLegend = true;
            series.Points.DataBindY(values);
            series.ChartType = SeriesChartType.Line;
            series.ChartArea = "F";
            series.Color = c;
            return series;
        }

        private void PlotGraph(Series series, string filename)
        {
            FileStream outputStream = new FileStream(filename + ".png", FileMode.OpenOrCreate);

            using (var ch = new Chart())
            {
                ch.ChartAreas.Add(new ChartArea());
                ch.Series.Add(series);
                ch.SaveImage(outputStream, ChartImageFormat.Png);
            }
            outputStream.Flush();
            outputStream.Close();
        }

    }
}
