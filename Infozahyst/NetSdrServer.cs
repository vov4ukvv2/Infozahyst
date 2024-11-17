using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infozahyst
{
    public class NetSdrServer
    {
        private TcpListener? _tcpListener;
        public void Start()
        {
            _tcpListener = new TcpListener(IPAddress.Any, 50000);
            _tcpListener.Start();
            Console.WriteLine("Server is listening on port 50000...");
            while (true)
            {
                try
                {
                    var tcpClient = _tcpListener.AcceptTcpClient();
                    Console.WriteLine("Client connected!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        public void Stop()
        {
            _tcpListener?.Stop();
        }
    }
}
