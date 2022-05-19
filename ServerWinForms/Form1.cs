using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace ServerWinForms
{
    public partial class Form1 : Form
    {
        List<ClientInfo> nodes;
        int port = 8088;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.GridLines = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            nodes = scan();

            for (int i = 0; i < nodes.Count; ++i)
            {
                if (nodes[i].Status == true)
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
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                Console.WriteLine(nodes[i].ToString());
                ListViewItem lvi = new ListViewItem();
                ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();
                listView1.Items[0].SubItems.Add(nodes[i].Address);
                listView1.Items[1].SubItems.Add(nodes[i].Name);
                listView1.Items[2].SubItems.Add(nodes[i].OS);
                if (nodes[i].Status)
                    listView1.Items[3].SubItems.Add("up");
                else
                    listView1.Items[3].SubItems.Add("down");
            }
        }

        static public List<ClientInfo> scan()
        {
            List<ClientInfo> nodes = new List<ClientInfo>();
            try
            {
                Ping ping;
                PingReply reply;

                for (int i = 1; i <= 10; ++i)
                {
                    if (i == 2) continue;
                    string ipAddress = "192.168.8." + i;

                    ping = new Ping();
                    try
                    {
                        reply = ping.Send(ipAddress, 300);
                    }
                    catch
                    {
                        break;
                    }

                    if (reply.Status == IPStatus.Success)
                    {
                        nodes.Add(new ClientInfo(ipAddress, true));
                    }
                    else
                    {
                        nodes.Add(new ClientInfo(ipAddress, false));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return nodes;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            port = Int32.Parse(textBox1.Text);
        }
    }
}
