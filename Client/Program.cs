using System;
using System.Net.Sockets;
using System.Text;
using System.Net;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Enter port: ");
            //int port = Int32.Parse(Console.ReadLine());
            //Console.WriteLine("Enter ip: ");
            //string ip = Console.ReadLine();
            ////const string ip = "169.254.204.28";

            //var ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            //var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Console.WriteLine("Enter message:");
            //var message = Console.ReadLine();

            //var data = Encoding.UTF8.GetBytes(message);

            //socket.Connect(ipEndPoint);
            //socket.Send(data);

            //var buffer = new byte[256];
            //var size = 0;
            //var answer = new StringBuilder();

            //do
            //{
            //    size = socket.Receive(buffer);
            //    answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
            //} while (socket.Available > 0);

            //Console.WriteLine(answer.ToString());

            //socket.Shutdown(SocketShutdown.Both);
            //socket.Close();

            //Console.ReadLine();



            Console.WriteLine("Enter port: ");
            int port = Int32.Parse(Console.ReadLine());
            //Console.WriteLine("Enter ip: ");
            //string ip = Console.ReadLine();
            string ip = "192.168.8.3";

            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);
                string msg = $"{Environment.MachineName} {Environment.OSVersion.Platform}";
                byte[] data = Encoding.Unicode.GetBytes(msg);
                Console.WriteLine(msg);
                socket.Send(data);

                var buffer = new byte[256];
                var size = 0;
                var answer = new StringBuilder();
                do
                {
                    size = socket.Receive(buffer);
                    answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
                } while (socket.Available > 0);

                Console.WriteLine(answer.ToString());

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
