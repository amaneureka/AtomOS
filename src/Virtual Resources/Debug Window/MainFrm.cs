using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.IO.Pipes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Debug_Window
{
    public partial class MainFrm : Form
    {
        public const string pipe = "atomos_alpha";
        public const int ram = 0x6FFFFFFF;
        public string TotalRAM = "/" + ram.ToString() + " Bytes"; //32 MB

        public MainFrm()
        {
            InitializeComponent();
        }

        bool Connected = false;
        uint RAMValue = 0;

        private Thread aRefresh;
        bool Update = false;
        int Pings = 0;
        private void MainFrm_Load(object sender, EventArgs e)
        {
            RAMMeter.Maximum = ram;

            aRefresh = new Thread(delegate()
            {
                using (var pipeClient = new NamedPipeClientStream(".", pipe, PipeDirection.InOut))
                {
                    var Br = new BinaryReader(pipeClient);
                    while (true)
                    {
                        if (pipeClient.IsConnected)
                        {
                            try
                            {
                                if (!Connected)
                                {
                                    Connected = true;
                                    Refresh();
                                }
                                switch (Br.ReadByte())
                                {
                                    //RAM
                                    case 0xFA:
                                    {
                                        RAMValue = BitConverter.ToUInt32(Br.ReadBytes(4), 0);
                                        Update = true;
                                    }
                                    break;
                                    //Ping
                                    /*case 0xEF:
                                    {
                                        if (BitConverter.ToUInt32(Br.ReadBytes(4), 0) == 0xFFFFFFFF)
                                            Pings = 0;
                                        Update = true;
                                    }
                                    break;*/
                                }
                                
                            }
                            catch
                            {
                                //We won't exception
                            }
                        }
                        else
                        {
                            if (Connected)
                            {                                
                                Connected = false;
                                Refresh();
                            }

                            pipeClient.Connect();
                            //Thread.Sleep(100);
                        }
                        
                        if (Update)
                        {
                            Refresh();
                            Update = false;
                        }
                        Pings++;
                    }
                }
            });
            aRefresh.Start();
        }

        delegate void RefreshCallback();
        public void Refresh()
        {
            if (this.InvokeRequired)
                this.Invoke(new RefreshCallback(Refresh));
            else
            {
                if (Connected)
                {
                    lbl_connection.Text = "Active";
                    lbl_connection.ForeColor = Color.Green;

                    if (Pings < 20)
                    {
                        lbl_System.ForeColor = Color.Green;
                        lbl_System.Text = "Active";
                    }
                    else
                    {
                        lbl_System.ForeColor = Color.Red;
                        lbl_System.Text = "Died";
                    }
                }
                else
                {
                    lbl_connection.Text = "Failed";
                    lbl_connection.ForeColor = Color.Red;
                    lbl_System.ForeColor = Color.Red;
                    lbl_System.Text = "Died";
                    RAMValue = 0;
                    Pings = 0;
                }

                lbl_ram.Text = RAMValue + TotalRAM;
                //RAMMeter.Value = RAMValue;
            }
        }
    }
}
