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
  
    public partial class Form1 : Form
    {
      
        SimulationSystem system = new SimulationSystem();
        public Form1()
        {
            InitializeComponent();
        }
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
        public decimal get_avgwaitt(decimal sumwait,SimulationSystem system)
        {
            decimal avg = ((sumwait) / (Convert.ToDecimal(system.StoppingNumber)));
            return avg;
        }
        public decimal get_wait_prob(decimal custwait, SimulationSystem system)
        {
            decimal wp = custwait  / (Convert.ToDecimal(system.StoppingNumber));
            return wp;
        }

        public int get_maxq_len(SimulationSystem system)
        {
            int mxq = 0;
            for (int i = 0; i < system.StoppingNumber; i++)
            {
                int mxq1 = 0;
                if (system.SimulationTable[i].TimeInQueue > 0)
                {
                    mxq1++;
                    for (int j = i + 1; j < system.StoppingNumber; j++)
                    {
                        if (system.SimulationTable[i].StartTime > system.SimulationTable[j].ArrivalTime)
                        {
                            mxq1++;
                        }
                        else
                            break;
                    }
                    if (mxq1 > mxq)
                        mxq = mxq1;
                }
            }
            return mxq;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int stop;
            int selection;
            string Path = "C:/Users/abdul/source/repos/ConsoleApp1/ConsoleApp1/TestCase3.txt";
           
            string numofs = File.ReadLines(Path).Skip(1).Take(1).First();

             system.NumberOfServers = int.Parse(numofs);
             numofs = File.ReadLines(Path).Skip(4).Take(1).First();
            system.StoppingNumber = int.Parse(numofs);
            numofs = File.ReadLines(Path).Skip(7).Take(1).First();
            stop = int.Parse(numofs);
            numofs = File.ReadLines(Path).Skip(10).Take(1).First();
            selection = int.Parse(numofs);
            // ---------------- ------- Read InterArrivalDistribution -------------------- ------

            FileStream fs = new FileStream(Path, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            int intervalsnumber = 0;
            for (int i = 0; i < 13; i++)
            {
                sr.ReadLine();
            }

            while (true)
            {

                string str;
                str = sr.ReadLine();
                if (str == "")
                {
                    break;
                }
                string[] fields = str.Split(',');
                TimeDistribution v1 = new TimeDistribution();
                intervalsnumber++;
                v1.Time = int.Parse(fields[0]);
                v1.Probability = decimal.Parse(fields[1]);
                system.InterarrivalDistribution.Add(v1);
            }
           for(int i = 0; i < intervalsnumber; i++)
            {
                if(i == 0)
                {
                    system.InterarrivalDistribution[i].CummProbability = system.InterarrivalDistribution[i].Probability;
                    system.InterarrivalDistribution[i].MinRange = 1;
                    decimal temp = system.InterarrivalDistribution[i].CummProbability * 100;
                    system.InterarrivalDistribution[i].MaxRange = (int)temp;

                }
                else
                {
                    system.InterarrivalDistribution[i].CummProbability = system.InterarrivalDistribution[i - 1].CummProbability + system.InterarrivalDistribution[i].Probability;
                    system.InterarrivalDistribution[i].MinRange = system.InterarrivalDistribution[i - 1].MaxRange + 1;
                    decimal temp = system.InterarrivalDistribution[i].CummProbability * 100;
                    system.InterarrivalDistribution[i].MaxRange = (int)temp;

                }
            }
           //-------------------------------------------------------------------------
          

            // --------------------------- Read Server Distribution -------------------------------------
            sr.ReadLine(); // On Line 19 
                           //textBox1.Text = Convert.ToString(system.NumberOfServers);
            int serverDistributionNumber = 0;
            List<Server> s1 = new List<Server>();
           
            for (int i = 1; i <= system.NumberOfServers; i++)
            {
                Server s2 = new Server();
                if (i != 1)
                sr.ReadLine();
                while (true) // start line 20 
                {
                    TimeDistribution t1 = new TimeDistribution();
                    string str1;
                    str1 = sr.ReadLine();
                    if (str1 == "")
                    {
                        break;
                    }
                    else if (str1 == null)
                        break;
                    string[] ff = str1.Split(',');
                    t1.Time = int.Parse(ff[0]);
                    t1.Probability = decimal.Parse(ff[1]);
                    if(i == 1)
                    {
                        serverDistributionNumber++;
                    }
                    s2.TimeDistribution.Add(t1);
                   
                }
                system.Servers.Add(s2);
                s1.Add(s2);
            }
           

           // ----------------------Calculate CummProp and Range for Servers-------------- -

            for (int i = 0; i < system.NumberOfServers; i++)
                {
                

                    for (int j = 0; j < serverDistributionNumber; j++)
                    {

                        if (j == 0)
                        {
                            s1[i].TimeDistribution[j].CummProbability = s1[i].TimeDistribution[j].Probability;
                            s1[i].TimeDistribution[j].MinRange = 1;
                            decimal temp = s1[i].TimeDistribution[j].CummProbability * 100;
                            s1[i].TimeDistribution[j].MaxRange = (int)temp;
                        //
                        system.Servers[i].TimeDistribution[j].CummProbability = s1[i].TimeDistribution[j].CummProbability;
                        system.Servers[i].TimeDistribution[j].MinRange = s1[i].TimeDistribution[j].MinRange;
                        system.Servers[i].TimeDistribution[j].MaxRange = s1[i].TimeDistribution[j].MaxRange;


                    }
                        else
                        {
                            s1[i].TimeDistribution[j].CummProbability = s1[i].TimeDistribution[j - 1].CummProbability + s1[i].TimeDistribution[j].Probability;
                            s1[i].TimeDistribution[j].MinRange = s1[i].TimeDistribution[j - 1].MaxRange + 1;
                            decimal temp = s1[i].TimeDistribution[j].CummProbability * 100;
                            s1[i].TimeDistribution[j].MaxRange = (int)temp;
                        //
                        system.Servers[i].TimeDistribution[j].CummProbability = s1[i].TimeDistribution[j].CummProbability;
                        system.Servers[i].TimeDistribution[j].MinRange = s1[i].TimeDistribution[j].MinRange;
                        system.Servers[i].TimeDistribution[j].MaxRange = s1[i].TimeDistribution[j].MaxRange;

                    }

                    }

                }
            for(int i=0;i<system.NumberOfServers;i++)
            {
                
                s1[i].ID = i + 1;
                system.Servers[i].ID = i + 1;
            }

          
           // ---------------------------Output---------------------------------- -
            int custnum = 1;
            int clock = 0;
            int interarrival = 0;
            int servicetime = 0;
            decimal sumwait = 0;
            decimal custwait = 0;
            int start = 0;
            int end = 0;
            
            int servernum = 0;
            int ran = 0;
            int ranservice = 0;
            int maxq = 0;
            int max = -100;

            int[] ch = new int[system.NumberOfServers];  // end bta3 kol server b3ml update 3leh
            for(int k=0;k<system.NumberOfServers;k++)
            {
                ch[k] = 0;
            }
            decimal[] sv = new decimal[system.NumberOfServers];
            for (int k = 0; k < system.NumberOfServers; k++)
            {
                sv[k] = 0;
            }
            decimal[] sv1 = new decimal[system.NumberOfServers];
            for (int k = 0; k < system.NumberOfServers; k++)
            {
                sv1[k] = 0;
            }
            decimal[] idle = new decimal[system.NumberOfServers];
            for (int k = 0; k < system.NumberOfServers; k++)
            {
                idle[k] = 0;
            }

            if (selection == 1 || selection == 3)
            {
                for (int i = 0; i < system.StoppingNumber; i++)
                {
                    int time = 0;
                    SimulationCase cs = new SimulationCase();
                    bool flag = false; // lw el servers kolha mlyana
                    ran = RandomNumber(1, 101);


                    for (int j = 0; j < intervalsnumber; j++)
                    {
                        if (ran >= system.InterarrivalDistribution[j].MinRange && ran <= system.InterarrivalDistribution[j].MaxRange)
                        {
                            interarrival = system.InterarrivalDistribution[j].Time;
                        }
                    }
                    if (i != 0)
                    {
                        clock = clock + interarrival;
                        start = start + interarrival;
                    }



                    for (int j = 0; j < system.NumberOfServers; j++)
                    {
                        ranservice = RandomNumber(1, 101);

                        if ((ch[j] == 0) || (clock - ch[j] >= 0))
                        {
                            flag = true;
                            for (int k = 0; k < serverDistributionNumber; k++)
                            {
                                if (ranservice >= s1[j].TimeDistribution[k].MinRange && ranservice <= s1[j].TimeDistribution[k].MaxRange)
                                {
                                    servicetime = s1[j].TimeDistribution[k].Time;

                                }
                            }
                            start = clock;
                            servernum = j;
                            ch[j] = start + servicetime;
                            break;
                        }



                    }
                    if (flag == false)
                    {

                        custwait++;
                        int min = 10000;
                        int tmp22 = 0;
                        for (int k = 0; k < system.NumberOfServers; k++)
                        {
                            if (ch[k] - clock < min)
                            {
                                min = ch[k] - clock;
                                tmp22 = k;
                            }
                        }
                        servernum = tmp22;
                        for (int k = 0; k < serverDistributionNumber; k++)
                        {
                            if (ranservice >= s1[servernum].TimeDistribution[k].MinRange && ranservice <= s1[servernum].TimeDistribution[k].MaxRange)
                            {
                                servicetime = s1[servernum].TimeDistribution[k].Time;

                            }
                        }
                        start = ch[servernum];
                        time = min;
                        sumwait += time;
                        ch[servernum] = start + servicetime;
                    }



                    end = start + servicetime;

                    cs.CustomerNumber = custnum;
                    cs.RandomInterArrival = ran;
                    cs.InterArrival = interarrival;
                    cs.ArrivalTime = clock;
                    cs.RandomService = ranservice;
                    cs.ServiceTime = servicetime;
                    cs.AssignedServer = s1[servernum];
                    cs.StartTime = start;
                    cs.EndTime = end;
                    cs.TimeInQueue = time;
                    system.SimulationTable.Add(cs);


                    custnum++;


                }
            }
            else if(selection==2)
            {
                for (int i = 0; i < system.StoppingNumber; i++)
                {
                    int time = 0;
                    SimulationCase cs = new SimulationCase();
                    ran = RandomNumber(1, 101);


                    for (int j = 0; j < intervalsnumber; j++)
                    {
                        if (ran >= system.InterarrivalDistribution[j].MinRange && ran <= system.InterarrivalDistribution[j].MaxRange)
                        {
                            interarrival = system.InterarrivalDistribution[j].Time;
                        }
                    }
                    if (i != 0)
                    {
                        clock = clock + interarrival;
                        start = start + interarrival;
                    }
                    ranservice = RandomNumber(1, 101);
                    List<int> free_servers = new List<int>();
                    free_servers.Clear();
                    for (int j = 0; j < system.NumberOfServers; j++)
                    {
                        if ((ch[j] == 0) || (clock - ch[j] >= 0))
                        {
                            free_servers.Add(j);
                        }
                    }
                    if(free_servers.Count>0)
                    {
                        int indx = RandomNumber(0, free_servers.Count);
                        int ranserver = free_servers[indx];

                        for (int k = 0; k < serverDistributionNumber; k++)
                        {
                            if (ranservice >= s1[ranserver].TimeDistribution[k].MinRange && ranservice <= s1[ranserver].TimeDistribution[k].MaxRange)
                            {
                                servicetime = s1[ranserver].TimeDistribution[k].Time;

                            }
                        }
                        start = clock;
                        servernum = ranserver;
                        ch[ranserver] = start + servicetime;
                    }
                    else
                    {
                        custwait++;
                        int min = 10000;
                        int tmp22 = 0;
                        for (int k = 0; k < system.NumberOfServers; k++)
                        {
                            if (ch[k] - clock < min)
                            {
                                min = ch[k] - clock;
                                tmp22 = k;
                            }
                        }
                        servernum = tmp22;
                        for (int k = 0; k < serverDistributionNumber; k++)
                        {
                            if (ranservice >= s1[servernum].TimeDistribution[k].MinRange && ranservice <= s1[servernum].TimeDistribution[k].MaxRange)
                            {
                                servicetime = s1[servernum].TimeDistribution[k].Time;

                            }
                        }
                        start = ch[servernum];
                        time = min;
                        sumwait += time;
                        ch[servernum] = start + servicetime;
                    }
                    end = start + servicetime;

                    cs.CustomerNumber = custnum;
                    cs.RandomInterArrival = ran;
                    cs.InterArrival = interarrival;
                    cs.ArrivalTime = clock;
                    cs.RandomService = ranservice;
                    cs.ServiceTime = servicetime;
                    cs.AssignedServer = s1[servernum];
                    cs.StartTime = start;
                    cs.EndTime = end;
                    cs.TimeInQueue = time;
                    system.SimulationTable.Add(cs);


                    custnum++;

                }

                }

            for (int i = 0; i < system.StoppingNumber; i++)
            {
            
                dataGridView1.Rows.Add(system.SimulationTable[i].CustomerNumber, system.SimulationTable[i].RandomInterArrival, system.SimulationTable[i].InterArrival,
                    system.SimulationTable[i].ArrivalTime, system.SimulationTable[i].RandomService, system.SimulationTable[i].StartTime,
                    system.SimulationTable[i].ServiceTime,
                      system.SimulationTable[i].EndTime, system.SimulationTable[i].TimeInQueue,  system.SimulationTable[i].AssignedServer.ID);
                
            }
            int mxq = get_maxq_len(system);
            decimal avg= get_avgwaitt(sumwait,system);
            decimal wp = get_wait_prob(custwait, system);
            PerformanceMeasures pf = new PerformanceMeasures();
            pf.AverageWaitingTime = avg;
            pf.WaitingProbability = Convert.ToDecimal(wp);
            pf.MaxQueueLength = mxq;
            system.PerformanceMeasures = pf;
            for (int i = 0; i < system.StoppingNumber; i++)
            {
                sv[system.SimulationTable[i].AssignedServer.ID - 1] += system.SimulationTable[i].ServiceTime; //calculate service of each server
                sv1[system.SimulationTable[i].AssignedServer.ID - 1] += 1; // calculate number of customer in each server
            }

            int maxrun = -1000;
            for (int i = 0; i < system.StoppingNumber; i++)
            {
                if (system.SimulationTable[i].EndTime > maxrun)
                {
                    maxrun = system.SimulationTable[i].EndTime;    // getting total runtime of simulation
                }
            }

            for (int i = 0; i < system.NumberOfServers; i++)
            {
                if (sv1[i] == 0)
                {
                    system.Servers[i].AverageServiceTime = 0;
                }
                else
                {
                    system.Servers[i].AverageServiceTime = sv[i] / sv1[i]; 
                    system.Servers[i].Utilization = sv[i] / Convert.ToDecimal(maxrun);
                }
            }
            decimal[] fi = new decimal[system.NumberOfServers];  // calculating end time of each server
            for (int k = 0; k < system.NumberOfServers; k++)
            {
                fi[k] = 0;
            }
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                for (int j = system.StoppingNumber - 1; j >= 0; j--)
                {
                    if (i == system.SimulationTable[j].AssignedServer.ID - 1)
                    {
                        fi[i] = system.SimulationTable[j].EndTime;
                        break;
                    }
                }
            }
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                int end1 = 0;
                bool flag1 = false;
                for (int j = 0; j < system.StoppingNumber; j++)
                {
                    if (flag1 == false && i == system.SimulationTable[j].AssignedServer.ID - 1)
                    {
                        flag1 = true;
                        end1 = system.SimulationTable[j].EndTime;
                        idle[i] += system.SimulationTable[j].StartTime;
                    }
                    else if (flag1 == true && i == system.SimulationTable[j].AssignedServer.ID - 1)
                    {
                        idle[i] += system.SimulationTable[j].StartTime - end1;
                        end1 = system.SimulationTable[j].EndTime;
                    }
                }
                idle[i] += maxrun - fi[i];
                system.Servers[i].IdleProbability = idle[i] / Convert.ToDecimal(maxrun);
            }


            string result = TestingManager.Test(system, Constants.FileNames.TestCase3);
            MessageBox.Show(result);
            globals.sss1 = system;
            globals.ListOfServers = s1;
            
            for(int l = 0; l < system.NumberOfServers; l++)
            {
                chart c1 = new chart();
                c1.load_chart(l , l+1);
            }

            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
    public static class globals
    {
        
        public static SimulationSystem sss1 = new SimulationSystem();
        public static List<Server> ListOfServers = new List<Server>();
           
    }
}
