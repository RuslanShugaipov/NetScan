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
            Console.WriteLine("Введите порт: ");
            int port = Int32.Parse(Console.ReadLine());

            IPEndPoint ipPoint = new IPEndPoint(0, port);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(5);

                while (true)
                {
                    Console.WriteLine("Ожидание.\n");
                    Socket handler = listenSocket.Accept();
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    byte[] data = new byte[256];
                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                        if (builder.ToString() == "Запрос конфигурации.")
                            break;
                    } while (true);

                    Console.WriteLine("Запрос на конфигурацию получен.");
                    string message = $"{Environment.MachineName}|{Environment.OSVersion.Platform}";
                    data = Encoding.UTF8.GetBytes(message);
                    handler.Send(data);
                    Console.WriteLine("Конфигурация отправлена.\n");

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
