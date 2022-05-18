using System;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Enter port: ");
            //int port = Int32.Parse(Console.ReadLine());

            //try
            //{
            //    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //    socket.Bind(new IPEndPoint(0, port));
            //    socket.Listen(5);

            //    while (true)
            //    {
            //        var listener = socket.Accept();
            //        var buffer = new byte[256];
            //        var size = 0;
            //        var data = new StringBuilder();
            //        do
            //        {
            //            size = listener.Receive(buffer);
            //            data.Append(Encoding.UTF8.GetString(buffer, 0, size));
            //        } while (listener.Available > 0);

            //        Console.WriteLine(data);

            //        listener.Send(Encoding.UTF8.GetBytes("Success"));

            //        listener.Shutdown(SocketShutdown.Both);
            //        listener.Close();
            //    }
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

            Console.WriteLine("Enter port: ");
            int port = Int32.Parse(Console.ReadLine());
            List<ClientInfo> nodes = scan();

            for (int i = 0; i < nodes.Count; ++i)
            {
                if (nodes[i].Status == true)
                {
                    IPEndPoint ipPoint = new IPEndPoint(0, port);
                    Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    try
                    {
                        listenSocket.Bind(ipPoint);
                        listenSocket.Listen(1);

                        Socket handler = listenSocket.Accept();
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        byte[] data = new byte[256];
                        do
                        {
                            bytes = handler.Receive(data);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        } while (builder.Length == 0);

                        handler.Send(Encoding.UTF8.GetBytes("Data recevied."));

                        Console.WriteLine(builder.ToString());

                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                Console.WriteLine(nodes[i].ToString());
            }
            Console.ReadKey();
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
                        try
                        {
                            nodes.Add(new ClientInfo(ipAddress, true));
                        }
                        catch
                        {
                            nodes.Add(new ClientInfo(ipAddress, true));
                        }
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
    }
}
