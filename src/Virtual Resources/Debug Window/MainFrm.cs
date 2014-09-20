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
        public const int ram = 32 * 1024 * 1024;
        public string TotalRAM = "/" + ram.ToString() + " Bytes"; //32 MB

        public MainFrm()
        {
            InitializeComponent();
        }

        bool Connected = false;
        int RAMValue = 0;

        private Thread aRefresh;
        bool Update = false;
        private void MainFrm_Load(object sender, EventArgs e)
        {
            RAMMeter.Maximum = ram;

            aRefresh = new Thread(delegate()
            {
                using (var pipeClient = new NamedPipeClientStream(".", pipe, PipeDirection.InOut))
                {
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

                                var Br = new BinaryReader(pipeClient);
                                switch (Br.ReadByte())
                                {
                                    //RAM
                                    case 0xFA:
                                    {
                                        RAMValue = BitConverter.ToInt32(Br.ReadBytes(4), 0);
                                        Update = true;
                                    }
                                    break;
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
                }
                else
                {
                    lbl_connection.Text = "Failed";
                    lbl_connection.ForeColor = Color.Red;
                    RAMValue = 0;
                }

                lbl_ram.Text = RAMValue + TotalRAM;
                RAMMeter.Value = RAMValue;
            }
        }
    }
}
