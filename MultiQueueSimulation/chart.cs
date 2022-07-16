using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueModels;
using MultiQueueTesting;
using System.IO;


namespace MultiQueueSimulation
{
    public partial class chart : Form
    {
        public chart()
        {
            InitializeComponent();
        }

        private void chart_Load(object sender, EventArgs e)
        {
            

        }
        public void load_chart(int server , int server_number )
        {
            this.Text = "Server number :  " + server_number;
            chart2.ChartAreas[0].AxisX.Minimum = 0;
            for (int i = 1; i < globals.sss1.SimulationTable.Count(); i++)
            {
                
                if (globals.sss1.SimulationTable[i].AssignedServer == globals.ListOfServers[server])
                {
                    for (int k = globals.sss1.SimulationTable[i].StartTime; k <= globals.sss1.SimulationTable[i].EndTime; k++)
                        chart2.Series["Busy"].Points.AddXY(k,1);
                }
                /*
                if (globals.sss1.SimulationTable[i].AssignedServer == globals.ListOfServers[server])
                {
                    chart2.Series["Busy"].Points.Add()
                }
                */
            }
            this.Show();

        }
    }
}
