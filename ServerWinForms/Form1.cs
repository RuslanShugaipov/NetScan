using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerWinForms
{
    public partial class Form1 : Form
    {
        static List<ClientInfo> nodes = new List<ClientInfo>();
        static int port = 8091;
        static ListView lv;
        static ProgressBar pb;
        static TextBox tb3;
        static TextBox tb2;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.GridLines = true;
            lv = listView1;
            pb = progressBar1;
            tb3 = textBox3;
            tb2 = textBox2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread scanThread = new Thread(new ThreadStart(scan));
            scanThread.Start();
        }

        static public void scan()
        {
            try
            {
                IPAddress.Parse(tb2.Text);
            }
            catch
            {
                return;
            }
            string[] nums = localIP.Split('.');
            if (nums.Length != 4)
                return;

            int index = localIP.LastIndexOf(".");
            string localNetAddress = localIP.Substring(0, index + 1);
            int localPCNumber = Int32.Parse(localIP.Split(new char[] { '.' })[3]);
            nodes.Clear();
            try
            {
                Ping ping;
                PingReply reply;

                tb3.Text = "Сканирование...";
                pb.Maximum = 8;
                pb.Value = 0;

                for (int i = 1; i < 10; ++i)
                {
                    if (i == localPCNumber) continue;
                    string ipAddress = localNetAddress + i;
                    ping = new Ping();

                    reply = ping.Send(ipAddress, 100);
                    if (reply.Status == IPStatus.Success)
                    {
                        nodes.Add(new ClientInfo(ipAddress, true));
                        getConfiguration(nodes.Count - 1);
                    }
                    else
                    {
                        nodes.Add(new ClientInfo(ipAddress, false));
                    }
                    pb.Value++;
                }
                tb3.Text = "Сканирование завершено";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static public void getConfiguration(int i)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(nodes[i].Address), port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);
                string message = "Запрос конфигурации.";
                byte[] data = Encoding.UTF8.GetBytes(message);
                socket.Send(data);

                data = new byte[256];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;

                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                }
                while (builder.Length == 0);

                string[] configuration = builder.ToString().Split(new char[] { '|' });

                nodes[i].Name = configuration[0];
                nodes[i].OS = configuration[1];

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch
            {
                return;
            }
        }

        static public void printToLV()
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                if (nodes[i].Status)
                {
                    ListViewItem lvi = new ListViewItem(nodes[i].Address);
                    lvi.SubItems.Add(nodes[i].Name);
                    lvi.SubItems.Add(nodes[i].OS);
                    lvi.SubItems.Add("up");
                    lv.Items.Add(lvi);
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            localIP = textBox2.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lv.Items.Clear();
            Thread printThread = new Thread(new ThreadStart(printToLV));
            printThread.Start();
        }
    }
}
